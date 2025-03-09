using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [Header("Puzzle Generics")]
    public float puzzleEntryInputLatch = 1f;
    [Header("Puzzle Optionals")]
    public PuzzleBundle parentPuzzleBundle;
    [Header("Puzzle Internal View")]
    public bool puzzleStarted = false;
    public bool puzzleSolved = false;
    public PlayerController playerInPuzzle;
    public GameCamera puzzleCam;
    protected float elapsedPuzzleEntryTime = 0f;
    void FixedUpdate() { if (playerInPuzzle) {PuzzleInputs();} }
    public virtual void StartPuzzle(PlayerController iPC) {}
    public virtual void StopPuzzle() {}
    public virtual void PuzzleInputs() {}
    public virtual bool TryValidatePuzzle() { return true; }
    public virtual void OnPuzzleSolved() {}
}
