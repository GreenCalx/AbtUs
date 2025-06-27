using System.Collections.Generic;
using System.Collections; // IEnumerator
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static EventLog;


public class SceneLoader : MonoBehaviour
{
    [Header("Transition tweaks")]
    public GameObject transitionAnimator_Ref;
    private GameObject transitionAnimator_Inst;

    // transition coroutines will wait this to be true before executing
    public bool asyncTransitionLock = true;
    private readonly string transitionInTrigg = "TransitionIn";
    private readonly string transitionOutTrigg = "TransitionOut";

    /// Scene management

    public bool activeSceneIsReady { get; private set; }
    public bool operationFinished { get; private set; }
    public float operationProgress { get; private set; }

    private Queue<IEnumerator> q;
    private int n_tasks;
    private bool scene_lock = false;
    private Scene preWarmScene;
    IEnumerator runningCoroutine = null;
    public string targetScene { get; private set; }
    public string loadingScene;
    public UnityEvent beforeLoadScene;
    public UnityEvent<Scene> beforeEnableScene;
    public UnityEvent afterLoadScene;

    void Start()
    {
        operationFinished = false;
        operationProgress = 0f;
        n_tasks = 1;
        q = new Queue<IEnumerator>();
        runningCoroutine = null;
        DontDestroyOnLoad(this.gameObject);
    }

    private void updateProgress(float iTaskProgress)
    {
        if (n_tasks == 0)
        { INFO("No tasks running to update progress."); return; }
        int nCompletedTasks = n_tasks - q.Count;
        operationProgress = (nCompletedTasks * (1f / n_tasks)) + iTaskProgress / n_tasks;
    }


    public void loadScene(string iSceneName, bool transitionUIEffect = true)
    {
        INFO("SceneLoader::LoadScene : " + iSceneName);
        targetScene = iSceneName;
        Scene currentScene = SceneManager.GetActiveScene();
        ///
        q.Enqueue(PreSceneLoad());
        if (transitionAnimator_Ref!=null)
            q.Enqueue(TransitionOutOfScene());

        var transitionScene = loadingScene;

        q.Enqueue(loading(transitionScene));
        q.Enqueue(enableScene(transitionScene));

        q.Enqueue(unloading(currentScene.name));
        q.Enqueue(loading(iSceneName));

        q.Enqueue(enableScene(iSceneName));
        q.Enqueue(unloading(transitionScene));

        if (transitionAnimator_Ref!=null)
            q.Enqueue(TransitionInAScene());
        n_tasks = q.Count;

        beforeLoadScene.Invoke();
        q.Enqueue(PostSceneLoad());

        n_tasks = q.Count;

        // remove any car states or else
        operationFinished = false;
        operationProgress = 0f;
        StartCoroutine(coordinator());
        afterLoadScene.Invoke();
    }



    public void asyncPreWarm(string iSceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        q.Enqueue(preWarm(iSceneName));
        q.Enqueue(loadWarmScene());
        q.Enqueue(unloading(currentScene.name));
        beforeLoadScene.Invoke();

        operationFinished = false;
        operationProgress = 0f;
        StartCoroutine(coordinator());
        afterLoadScene.Invoke();
    }

    IEnumerator coordinator()
    {
        activeSceneIsReady = false;
        asyncTransitionLock = true;
        while (q.Count > 0)
        {
            if (runningCoroutine != null)
            {
                yield return null;
            }

            runningCoroutine = q.Dequeue();
            yield return StartCoroutine(runningCoroutine);
        }

        if (transitionAnimator_Inst!=null)
            Destroy(transitionAnimator_Inst.gameObject);
        INFO("operation finished");
        operationFinished = true;
        operationProgress = 1f;
        activeSceneIsReady = true;
    }

    IEnumerator PreSceneLoad()
    {
        beforeLoadScene.Invoke();
        yield return null;
    }

    IEnumerator PostSceneLoad()
    {
        afterLoadScene.Invoke();
        yield return null;
    }

    IEnumerator unloading(string iSceneName)
    {
        INFO("Unloading Scene " + iSceneName);
        AsyncOperation old_scene = SceneManager.UnloadSceneAsync(iSceneName);
        while (!old_scene.isDone)
        {
            updateProgress(old_scene.progress);
            yield return null;
        }

        INFO("Unloading Scene " + iSceneName + " Complete");
        runningCoroutine = null;
    }

    void OnComplete(AsyncOperation op)
    {
        INFO("Loading Complete Callback");

    }

    void OnDestroy()
    {
        
    }

    IEnumerator loading(string iSceneName)
    {
        INFO("Loading scene " + iSceneName);
        AsyncOperation scene = SceneManager.LoadSceneAsync(iSceneName, LoadSceneMode.Additive);
        if (scene == null)
        {
            FAIL(">>>> Missing scene in Build Settings <<<<");
        }

        scene.completed += OnComplete;
        scene.allowSceneActivation = false;

        while ( !scene.isDone )
        {
            INFO("scene progress " + scene.progress);
            updateProgress(scene.progress);
            // allowSceneActivation = false makes progress stop at 0.9
            if (scene.progress >= 0.88f)
            {
                if ( iSceneName != loadingScene )
                {
                    while(scene_lock)
                    {
                        INFO("Waiting in a lock...");
                        yield return new WaitForSeconds (0.2f);
                    }
                    scene.allowSceneActivation = true;
                    //break;

                } else {
                    scene.allowSceneActivation = true;
                }
            }
            yield return null;
        }

        // Finished loading new scene
        INFO("Loading scene " + iSceneName + " Complete");
        //scene.allowSceneActivation = true;
        //enableScene(iSceneName);
        runningCoroutine = null;
    }



    IEnumerator preWarm(string iSceneName)
    {
        INFO("preWarm scene " + iSceneName);
        AsyncOperation scene = SceneManager.LoadSceneAsync(iSceneName, LoadSceneMode.Additive);
        if (scene == null)
        {
            FAIL(">>>> Missing scene in Build Settings <<<<");
        }
        //scene.completed += OnComplete;
        scene.allowSceneActivation = false;
        while (!scene.isDone)
        {
            updateProgress(scene.progress);
            if (scene.progress >= 0.9f)
            {
                if ( iSceneName != loadingScene )
                {
                    while(scene_lock)
                    {
                        INFO("Waiting in a lock...");
                        yield return new WaitForSeconds (0.2f);
                    }
                    scene.allowSceneActivation = true;
                } else {
                    scene.allowSceneActivation = true;
                }
            }
            yield return null;
        }

        // Finished loading new scene
        preWarmScene  = SceneManager.GetSceneByName(iSceneName);
        INFO("preWarm scene " + iSceneName + " Complete");
        runningCoroutine = null;        
    }



    IEnumerator loadWarmScene()
    {
        if (preWarmScene.IsValid())
        {
            beforeEnableScene.Invoke(preWarmScene);
            bool activeSceneChanged = SceneManager.SetActiveScene(preWarmScene);
            if (!activeSceneChanged)
                FAIL("Failed to changed active scene for : " + preWarmScene.name);
            else
                INFO("Scene " + preWarmScene.name + " activated");
        }        
        yield return null;
    }



    IEnumerator TransitionOutOfScene()
    {
        INFO("TransitionOutOfScene IN");
        // if (asyncTransitionLock)
        //     yield return null;
        transitionAnimator_Inst = Instantiate(transitionAnimator_Ref);
        Animator a = transitionAnimator_Inst.GetComponentInChildren<Animator>();
        if (a==null)
            yield break;

        a.SetTrigger(transitionOutTrigg);
        while (a.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        a.ResetTrigger(transitionOutTrigg);
        asyncTransitionLock = true;
        INFO("TransitionOutOfScene OUT");
    }


    IEnumerator TransitionInAScene()
    {
        INFO("TransitionInAScene IN");
        if (asyncTransitionLock)
        {
            INFO("lock");
            yield return null;
        }
        INFO("unlock");
        transitionAnimator_Inst = Instantiate(transitionAnimator_Ref);
        Animator a = transitionAnimator_Inst.GetComponentInChildren<Animator>();
        if (a==null)
            yield break;
        a.SetTrigger(transitionInTrigg);
        while (a.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        a.ResetTrigger(transitionInTrigg);
        asyncTransitionLock = true;
        INFO("TransitionInAScene OUT");
    }

    ///////////////////////////////////////////////////////////

    IEnumerator enableScene(string iSceneName)
    {
        //Activate the Scene
        Scene sceneToLoad = SceneManager.GetSceneByName(iSceneName);
        if (sceneToLoad.IsValid())
        {
            beforeEnableScene.Invoke(sceneToLoad);
            bool activeSceneChanged = SceneManager.SetActiveScene(sceneToLoad);
            if (!activeSceneChanged)
                FAIL("Failed to changed active scene for : " + sceneToLoad.name);
            else
                INFO("Scene " + iSceneName + " activated");
        }
        yield return null;
    }

    //////////////////////////////////////////////////////////////////

    public void lockScene()
    {
        scene_lock = true;
    }



    public void unlockScene()
    {
        scene_lock = false;
    }
}