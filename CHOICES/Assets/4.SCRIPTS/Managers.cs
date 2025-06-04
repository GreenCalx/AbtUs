using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

public class Managers : MonoBehaviour
{
    public SoundManager Sound;
    public CameraManager Camera;
    public ObjectChainManager ObjectChains;
    public ObjectPoolManager ObjectPools;

    private static Managers instance = null;
    public static Managers Instance => instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        Sound = GetComponent<SoundManager>();
        Camera = GetComponent<CameraManager>();
        ObjectChains = GetComponent<ObjectChainManager>();
        ObjectPools = GetComponent<ObjectPoolManager>();

        if (GameSettings.Instance.LogEvents)
        {
            EventLog.Init();
            StartCoroutine(LogCo());
        }
    }

    void OnDestroy()
    {
        if (GameSettings.Instance.LogEvents)
        {
            EventLog.Close();
        }
    }

    IEnumerator LogCo()
    {
        while(Application.isPlaying)
        {
            if (EventLog.pendingLogs.Count > 0)
            {
                EventLog.Write();
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
