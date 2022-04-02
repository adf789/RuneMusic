using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using System;
using UnityEngine.Events;
namespace MPTKDemoCatchMusic
{
    public class RuneMusicView : MonoBehaviour
    {
        public static float noteStayTime = 1.0f;
        public MidiFilePlayer midiFilePlayer;
        public MidiStreamPlayer midiStreamPlayer;
        public RuneNoteView NoteDisplay;
        public ObjectPulling<RuneNoteView> notePulling;
        public GameObject baseNoteRangeObj;
        public float RuneRangeMinY, RuneRangeMaxY, RuneRangeMinX, RuneRangeMaxX, noteStartTime;
        public struct RuneRange
        {
            public float minY, maxY, minX, maxX, originIndex;
        }
        private List<RuneRange> allRanges;
        private Transform mainCameraT;
        private bool isEnd;
        private float blockFrequency = 0.3f;

        public void EndLoadingSF()
        {
            Debug.Log("End loading SF, MPTK is ready to play");
            Debug.Log("   Time To Load SoundFont: " + Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadSoundFont.TotalSeconds, 3).ToString() + " second");
            Debug.Log("   Time To Load Samples: " + Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadWave.TotalSeconds, 3).ToString() + " second");
            Debug.Log("   Presets Loaded: " + MidiPlayerGlobal.MPTK_CountPresetLoaded);
            Debug.Log("   Samples Loaded: " + MidiPlayerGlobal.MPTK_CountWaveLoaded);
        }

        void Start()
        {
            mainCameraT = Camera.main.transform;

            allRanges = new List<RuneRange>();

            notePulling = new ObjectPulling<RuneNoteView>(NoteDisplay, 20);

            // Default size of a Unity Plan

            RuneRangeMinY = -35;
            RuneRangeMaxY = 68;

            RuneRangeMinX = -54;
            RuneRangeMaxX = 54;

            // 노트가 생성 가능한 범위 설정
            int rangeIndex = 0;
            float sizeY = (RuneRangeMaxY - RuneRangeMinY) / 7.0f, sizeX = (RuneRangeMaxX - RuneRangeMinX) / 7.0f;
            int row = 0, col = 0;
            for(float tempRangeY = RuneRangeMinY; tempRangeY < RuneRangeMaxY; tempRangeY += sizeY)
            {
                if (col++ % 2 == 1) continue;
                for (float tempRangeX = RuneRangeMinX; tempRangeX < RuneRangeMaxX; tempRangeX += sizeX)
                {
                    if (row++ % 2 == 0) continue;

                    RuneRange tempRange = new RuneRange();
                    tempRange.minX = tempRangeX;
                    tempRange.maxX = tempRangeX + sizeX;
                    tempRange.minY = tempRangeY;
                    tempRange.maxY = tempRangeY + sizeY;
                    tempRange.originIndex = rangeIndex++;
                    allRanges.Add(tempRange);
                }
            }

            if (midiFilePlayer != null)
            {
                midiFilePlayer.MPTK_MidiName = MidiPlayerGlobal.CurrentMidiSet.MidiFiles[ItemPref.instance.curMainRune.musicIndex];

                // If call is already set from the inspector there is no need to set another listeneer
                if (!midiFilePlayer.OnEventNotesMidi.HasEvent())
                {
                    // No listener defined, set now by script. NotesToPlay will be called for each new notes read from Midi file
                    Debug.Log("MusicView: no OnEventNotesMidi defined, set by script");
                    midiFilePlayer.OnEventNotesMidi.AddListener(NotesToPlay);
                }
            }
            else
                Debug.Log("MusicView: no MidiFilePlayer detected");

        }

        /// <summary>
        /// Call when a group of midi events is ready to plays from the the midi reader.
        /// Playing the events are delayed until they "fall out"
        /// </summary>
        /// <param name="notes"></param>
        public void NotesToPlay(List<MPTKEvent> notes)
        {
            bool isFirstNote = false;

            //Debug.Log(midiFilePlayer.MPTK_PlayTime.ToString() + " count:" + notes.Count);
            foreach (MPTKEvent note in notes)
            {
                switch (note.Command)
                {
                    case MPTKCommand.NoteOn:
                        if (note.Value > 40 && note.Value < 100)// && note.Channel==1)
                        {
                            // 출력되는 노트
                            RuneNoteView n;
                            Vector3 position;
                            float time = UnityEngine.Random.Range(-0.3f, 0.1f);
                            // 노트의 채널이 0이면서,
                            // 노트들의 첫 노트이면서,
                            // 노트간의 간격이 0.5s ~ 0.7s 이면서,
                            // 현재 생성 가능한 공간이 있을 경우
                            if (note.Channel == 0 && !isFirstNote && noteStartTime > (0.6f + time) && allRanges.Count != 0)
                            {
                                noteStartTime = 0;
                                isFirstNote = true;

                                int rangeIndex = UnityEngine.Random.Range(0, allRanges.Count - 1);
                                RuneRange range = allRanges[rangeIndex];
                                allRanges.RemoveAt(rangeIndex);

                                float x = UnityEngine.Random.Range(range.minX, range.maxX);
                                float y = UnityEngine.Random.Range(range.minY, range.maxY);

                                position = new Vector3(x, y, -210);
                                //n = GameObject.Instantiate<RuneNoteView>(NoteDisplay, position, Quaternion.identity);
                                n = notePulling.GetObject(position);

                                n.Init();
                                n.judgeWidth = 2.5f;
                                n.view = true;
                                n.block = UnityEngine.Random.Range(0f, 1f) < blockFrequency;
                                if (isEnd) n.nonTouch = true;
                                n.setRangeIndex(range);
                            }
                            // 재생용 노트
                            else
                            {
                                position = mainCameraT.position;
                                position.z -= 10f;

                                //n = GameObject.Instantiate<RuneNoteView>(NoteDisplay, position, Quaternion.identity);
                                n = notePulling.GetObject(position);
                            }

                            n.Init();
                            n.gameObject.SetActive(true);
                            n.hideFlags = HideFlags.HideInHierarchy;
                            n.midiStreamPlayer = midiStreamPlayer;
                            n.note = note;
                        }
                        break;
                        // 시작되는 노트
                    case MPTKCommand.PatchChange:
                        {
                            //Debug.Log($"PatchChange Channel:{note.Channel}  Preset index:{note.Value}");
                            // See noteview.cs: update() move the note along the plan until they fall out, then they are played
                            Vector3 position = mainCameraT.position;
                            position.z -= 10f;
                            //RuneNoteView n = GameObject.Instantiate<RuneNoteView>(NoteDisplay, position, Quaternion.identity);
                            RuneNoteView n = notePulling.GetObject(position);

                            n.Init();
                            n.gameObject.SetActive(true);
                            n.hideFlags = HideFlags.HideInHierarchy;
                            n.midiStreamPlayer = midiStreamPlayer;
                            n.note = note;
                        }
                        break;
                }
            }
        }

        private void PlaySound()
        {
            // Some sound for waiting the notes ...
            if (!NoteView.FirstNotePlayed)
                //! [Example PlayNote]
                midiStreamPlayer.MPTK_PlayEvent
                (
                    new MPTKEvent()
                    {
                        Channel = 9,
                        Duration = 999999,
                        Value = 48,
                        Velocity = 100
                    }
                );
            //! [Example PlayNote]
        }
        
        public void Clear()
        {
            NoteView[] components = GameObject.FindObjectsOfType<NoteView>();
            foreach (NoteView noteview in components)
            {
                if (noteview.enabled)
                    //Debug.Log("destroy " + ut.name);
                    DestroyImmediate(noteview.gameObject);
            }
        }

        public void musicStop(float endTime)
        {
            isEnd = true;
            StartCoroutine("End", endTime);
        }

        IEnumerator End(float endTime)
        {
            yield return new WaitForSeconds(endTime);
            midiFilePlayer.MPTK_Stop();
        }

        public void addRange(RuneRange range)
        {
            allRanges.Add(range);
        }

        public void adjustBlockNoteFrequency(float percent, bool multi)
        {
            if (multi) blockFrequency *= percent;
            else blockFrequency /= percent;
        }

        private void Update()
        {
            noteStartTime += Time.deltaTime;
        }
    }
}