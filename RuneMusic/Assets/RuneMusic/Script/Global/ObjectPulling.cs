using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPulling<T> where T : Component
{
    private T obj;
    private List<T> objList;
    private int capacity, index;
    public List<T> Items
    {
        get
        {
            return objList;
        }
    }

    public ObjectPulling(T _obj, int _capacity = 5)
    {
        this.obj = _obj;
        this.capacity = _capacity;
        objList = new List<T>(capacity);
        index = 0;
    }

    public T GetObject(Transform transform)
    {
        // 리스트에 풀링할 객체가 존재하지 않을 경우
        if (objList.Count == 0)
        {
            for (int i = 0; i < capacity; i++)
            {
                T tempObj = GameObject.Instantiate<T>(obj, Vector3.zero, Quaternion.identity);
                tempObj.gameObject.SetActive(false);
                objList.Add(tempObj);
            }
        }
        // 현재 사용되어야 할 객체가 아직 사용 중인 경우
        else if (objList[index].gameObject.activeSelf)
        {
            // 사용할 수 있는 인덱스를 찾음
            int i;
            for (i = 1; i < objList.Count; i++)
            {
                if (i + index >= objList.Count) index = -i;

                if (!(objList[i + index].gameObject.activeSelf))
                {
                    index = i + index;
                    break;
                }
            }

            // 사용할 수 있는 객체가 없을 경우 새로 만듦
            if (i == objList.Count)
            {
                T tempObj = GameObject.Instantiate<T>(obj, Vector3.zero, Quaternion.identity);
                tempObj.gameObject.SetActive(false);
                objList.Add(GameObject.Instantiate<T>(obj, Vector3.zero, Quaternion.identity));
                index = objList.Count - 1;
            }
        }

        InitObject(index);
        T _obj = objList[index++];
        _obj.transform.SetParent(transform);
        _obj.gameObject.SetActive(true);
        if (index == objList.Count) index = 0;

        return _obj;

    }

    public T GetObject(Vector3 position)
    {
        // 리스트에 풀링할 객체가 존재하지 않을 경우
        if (objList.Count == 0)
        {
            for (int i = 0; i < capacity; i++)
            {
                T tempObj = GameObject.Instantiate<T>(obj, Vector3.zero, Quaternion.identity);
                tempObj.gameObject.SetActive(false);
                objList.Add(tempObj);
            }
        }
        // 현재 사용되어야 할 객체가 아직 사용 중인 경우
        else if (objList[index].gameObject.activeSelf)
        {
            // 사용할 수 있는 인덱스를 찾음
            int i;
            for (i = 1; i < objList.Count; i++)
            {
                if (i + index >= objList.Count) index = -i;

                if (!(objList[i + index].gameObject.activeSelf))
                {
                    index = i + index;
                    break;
                }
            }

            // 사용할 수 있는 객체가 없을 경우 새로 만듦
            if (i == objList.Count)
            {
                T tempObj = GameObject.Instantiate<T>(obj, Vector3.zero, Quaternion.identity);
                tempObj.gameObject.SetActive(false);
                objList.Add(GameObject.Instantiate<T>(obj, Vector3.zero, Quaternion.identity));
                index = objList.Count - 1;
            }
        }

        InitObject(index);
        T _obj = objList[index++];
        _obj.transform.SetParent(null);
        _obj.transform.Translate(position);
        _obj.gameObject.SetActive(true);
        if (index == objList.Count) index = 0;

        return _obj;
    }

    public void Clear()
    {
        objList.Clear();
    }

    public void UnActive()
    {
        foreach(T obj in objList)
        {
            obj.gameObject.SetActive(false);
        }
    }

    private void InitObject(int _index)
    {
        objList[_index].transform.parent = null;
        objList[_index].transform.position = Vector3.zero;
        objList[_index].transform.rotation = Quaternion.identity;
        objList[_index].gameObject.SetActive(false);
    }

    public void AllBringBack()
    {
        for(int i = 0; i < objList.Count; i++)
        {
            InitObject(i);
        }
    }
}
