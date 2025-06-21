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
    public Bounds bounds;
    public Transform player;
    private bool enabled = false;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.1f, 0.5f, 0.25f, 0.5f);
        Gizmos.DrawCube(
                bounds.center, bounds.size
             );
    }
    public void Start()
    {
        pool = pool.Where(e => e != null).ToList();
        if (player == null)
        {
            FAIL("No player ref in ObjectPool. Deactivating.");
            enabled = false;
        }
    }

    void Update()
    {
        if (bounds.Contains(player.position))
        {
            OnEnable();
        }
        else
        {
            OnDisable();
        }
    }

    public void Init()
    {
        foreach (Transform c in transform)
        {
            IPoolable as_poolable = c.GetComponent<IPoolable>();
            if (as_poolable != null)
            {
                Add(as_poolable);
            }
        }
    }

    public void OnEnable()
    {
        if (enabled)
            return;
            
        foreach (IPoolable obj in pool)
        {
            obj.OnPoolAwake();
        }
        INFO("Pool ID " + id + " Enabled");
        enabled = true;
    }

    public void OnDisable()
    {
        if (!enabled)
            return;
            
        foreach (IPoolable obj in pool)
        {
            obj.OnPoolSleep();
        }
        INFO("Pool ID " + id + " Disabled");
        enabled = false;
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
