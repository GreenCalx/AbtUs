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
    public FeedbackManager FBM;
    public RenderingManager Rendering;

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
        StartCoroutine(InitChain());

#if UNITY_EDITOR

        if (GameSettings.Instance.LogEvents)
        {
            EventLog.Init();
            StartCoroutine(LogCo());
        }
#endif
    }

    IEnumerator InitChain()
    {
        Sound = GetComponent<SoundManager>();
        Sound.Init();

        Camera = GetComponent<CameraManager>();
        Camera.Init();

        // NEED OWC
        while (OverWorldControl.Instance == null) { yield return null; }

        FBM = GetComponent<FeedbackManager>();
        FBM.Init(3);

        ObjectChains = GetComponent<ObjectChainManager>();
        ObjectChains.Init();

        ObjectPools = GetComponent<ObjectPoolManager>();
        ObjectPools.Init();

        Rendering = GetComponent<RenderingManager>();
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
