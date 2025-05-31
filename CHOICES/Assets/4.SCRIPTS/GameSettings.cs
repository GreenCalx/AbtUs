using UnityEngine;

/*
    TODO : Should be const static data editable via ScriptableObject
*/
public class GameSettings : MonoBehaviour
{
    private static GameSettings instance = null;
    public static GameSettings Instance => instance;

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

    [Header("MetaGameData")]
    public float mouseSensivity = 1f;

    [Header("FeedbackData")]
    public float DuplicationMulFactor = 0.02f;
    
}
