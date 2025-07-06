using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
    public Scene managedScene;
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
            managedScene = gameObject.scene;
        }
    }

    void Start()
    {
        StartCoroutine(InitChain());

        if (GameSettings.Instance.LogEvents)
        {
            EventLog.Init();
            StartCoroutine(LogCo());
        }
    }

    IEnumerator InitChain()
    {
        while( SceneManager.GetActiveScene() != managedScene )
        { yield return null;  }

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
