using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UITitle : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public string gameScene = "LEVEL0";

    [Header("SOUND")]
    public AudioMixer MasterMixer;
    public AudioMixerGroup BGMMixerGroup;
    public GameObject prefabBGMAudioSource;
    public AudioClip titleBGM;

    [Header("GFX")]
    public Image TitleGFXImage;
    private Material TitleGFXMat;
    public string mouseParmName = "_MousePosition";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TitleGFXMat = TitleGFXImage.material;

        GameObject newAS = Instantiate(prefabBGMAudioSource);
        newAS.transform.name = "BGM " + MasterMixer.ToString();
        AudioSource asSource = newAS.GetComponent<AudioSource>();
        asSource.clip = titleBGM;
        asSource.outputAudioMixerGroup = BGMMixerGroup;
        asSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        TitleGFXMat.SetVector(mouseParmName, Input.mousePosition);
        if (Input.anyKey)
        {
            sceneLoader.loadScene(gameScene);
            Destroy(gameObject);
        }
    }
}
