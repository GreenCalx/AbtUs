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
            DontDestroyOnLoad(this.gameObject);
        }
    }
    [Header("Debug")]
    public bool LogEvents = true;

    [Header("MetaGameData")]
    public float mouseSensivity = 1f;

    [Header("FeedbackData")]
    public float DuplicationMulFactor = 0.02f;

    [Header("Approximations")]
    public float AlignementCheckDotProdThreshold = 0.7f;
    public float MisalignementPenaltyFactor = 0.5f;
    public float TriangleMatchingAngleEps = 5f;
    public float TriangleAngleDivergencePenaltyFactor = 0.05f;
    public float SquareMatchingAngleEps = 10f;
    public float SquareAngleDivergencePenaltyFactor = 5f;
}
