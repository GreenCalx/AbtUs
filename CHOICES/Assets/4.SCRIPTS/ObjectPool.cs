using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventLog;

public class ObjectPool : MonoBehaviour
{
    public int id;
    public List<IPoolable> pool = new List<IPoolable>();

    public void Start()
    {
        pool = pool.Where(e => e != null).ToList();
    }

    public void OnEnable()
    {
        foreach (IPoolable obj in pool)
        {
            obj.OnPoolAwake();
        }
    }

    public void OnDisable()
    {
        foreach (IPoolable obj in pool)
        {
            obj.OnPoolSleep();
        }
    }


    public void Add(IPoolable iObject)
    {
        if (pool.Contains(iObject))
        {
            FAIL(" ADD " + iObject.GetName() + " in object pool " + gameObject.name + " (id:" + id + ")");
            return;
        }
            
        pool.Add(iObject);
        OK(" ADD " + iObject.GetName() + " in object pool " + gameObject.name + " (id:" + id + ")");
    }

    public virtual void Remove(IPoolable iObject)
    {
        if (!pool.Contains(iObject))
        {
            FAIL(" ADD " + iObject.GetName() + " in object pool " + gameObject.name + " (id:" + id + ")");
            return;
        }
            
        pool.Remove(iObject);
        OK(" RM " + iObject.GetName() + " from object pool" + gameObject.name + " (id:" + id + ")");
    }


}
