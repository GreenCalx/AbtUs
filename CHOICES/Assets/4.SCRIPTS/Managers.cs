using UnityEngine;

public class Managers : MonoBehaviour
{

    public SoundManager Sound;
    public CameraManager Camera;
    public ObjectChainManager ObjectChains;

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
    }
}
