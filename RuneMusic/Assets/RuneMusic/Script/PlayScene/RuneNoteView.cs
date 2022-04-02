using MidiPlayerTK;
using UnityEngine;

/// <summary>
/// Demo CatchMusic
/// </summary>
namespace MPTKDemoCatchMusic
{

    /// <summary>
    /// Defined behavior of a note
    /// </summary>
    public class RuneNoteView : MonoBehaviour
    {
        public MPTKEvent note;
        public MidiStreamPlayer midiStreamPlayer;
        public bool played = false, view = false, touched = false, nonTouch = false, block = false;
        public Material MatReady, MatGo, MatBlock;
        public RuneMusicView musicView;
        public MainStage mainEvent;
        public Transform judgeDrawT;
        public float judgeWidth = 2.5f;
        private RuneMusicView.RuneRange range;
        private float miss, bad, good, great, perfect, sizeDownSpeed;
        private Vector3 sizeScale = new Vector3(1.0f, 1.0f, 1.0f);
        private Material originMat;

        public void Init() 
        {
            sizeScale.Set(1.0f, 1.0f, 1.0f);
            judgeDrawT.localScale = sizeScale *= judgeWidth;
            if (originMat == null) originMat = judgeDrawT.GetComponent<MeshRenderer>().material;
            played = false;
            touched = false;

            if (view)
            {
                judgeDrawT.GetComponent<MeshRenderer>().material = originMat;
                gameObject.GetComponent<MeshRenderer>().enabled = true;
                // 입력 불가능하게
                if (!nonTouch) gameObject.GetComponent<MeshCollider>().enabled = true;
                judgeDrawT.gameObject.SetActive(true);
                if (block) GetComponent<MeshRenderer>().material = MatBlock;
            }
        }

        void Start()
        {
            // 노트 판정 기준
            miss = judgeWidth * 0.28f;
            bad = judgeWidth * 0.8f;
            good = judgeWidth * 0.72f;
            great = judgeWidth * 0.6f;
            perfect = judgeWidth * 0.48f;

            // 이펙트 사이즈 다운 속도
            sizeDownSpeed = (judgeDrawT.localScale.x - miss) / RuneMusicView.noteStayTime;
        }
        void Update()
        {
            // 올바른 판정에 가까우면서 material 변화
            float size = judgeDrawT.localScale.x;
            if (size <= bad && size > great) judgeDrawT.GetComponent<MeshRenderer>().material = MatReady;
            else if (size <= great) judgeDrawT.GetComponent<MeshRenderer>().material = MatGo;

            // perfect 기준일 경우 노트음 재생
            if (!played && judgeDrawT.localScale.x < perfect) PlayNote();
            // miss 판정기준을 넘을 경우
            else if (judgeDrawT.localScale.x < miss) EndNote();

        }

        void FixedUpdate()
        {
            // Move the note along the X axis
            float sizeTemp = Time.fixedDeltaTime * sizeDownSpeed;
            sizeScale.Set(1.0f, 1.0f, 1.0f);
            sizeScale *= -sizeTemp;
            judgeDrawT.localScale += sizeScale;
        }

        public void setRangeIndex(RuneMusicView.RuneRange _range)
        {
            this.range = _range;
        }

        public EnumList.NOTE_JUDGE touchNote()
        {
            float size = judgeDrawT.localScale.x;
            touched = true;

            gameObject.GetComponent<MeshRenderer>().enabled = false;
            if(!nonTouch) gameObject.GetComponent<MeshCollider>().enabled = false;
            judgeDrawT.gameObject.SetActive(false);

            if (size > good) return EnumList.NOTE_JUDGE.BAD;
            else if (size > great) return EnumList.NOTE_JUDGE.GOOD;
            else if (size > perfect) return EnumList.NOTE_JUDGE.GREAT;
            else return EnumList.NOTE_JUDGE.PERFECT;
        }

        private void PlayNote()
        {
            played = true;
            // Now play the note
            midiStreamPlayer.MPTK_PlayEvent(note);
        }

        private void EndNote()
        {
            if (view)
            {
                // 할당된 구역 반환
                musicView.addRange(range);

                // 터치가 되지 않았을 경우 miss 판정
                if (!touched)
                {
                    mainEvent.checkJudgement(transform.position, EnumList.NOTE_JUDGE.MISS, false);

                    // attack from monster to player
                    if (block) mainEvent.targetReadyGetDamage(EnumList.NOTE_JUDGE.MISS, transform.position, mainEvent.playerPatern.hitT.position, true);
                    // attack from player to monster
                    else mainEvent.targetReadyGetDamage(EnumList.NOTE_JUDGE.MISS, transform.position, mainEvent.monsterPatern.hitT.position, false);
                }
            }

            view = false;
            nonTouch = false;
            block = false;
            gameObject.SetActive(false);
        }
    }
}