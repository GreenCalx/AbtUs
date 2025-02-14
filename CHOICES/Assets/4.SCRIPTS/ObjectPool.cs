using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public List<GameObject> pool = new List<GameObject>();

    public void Start()
    {
        foreach(GameObject obj in pool)
        {
            if(obj == null) { pool.Remove(obj); }
        }
    }

    public void Enable(bool bol)
    {
        foreach(GameObject obj in pool)
        {
            obj.SetActive(bol);
        }
    }


    public void Add(GameObject iObject)
    {
        if (pool.Contains(iObject))
            return;
        pool.Add(iObject);
        Debug.Log(iObject.name + " has been added to pool " + name);
    }

    public virtual void Remove(GameObject iObject)
    {
        if (!pool.Contains(iObject))
            return;
        pool.Remove(iObject);   
        Debug.Log(iObject.name + " has been removed from pool " + name);
    }


}
