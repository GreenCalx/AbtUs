using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public bool puzzleStarted = false;
    public bool puzzleSolved = false;
    public PlayerController playerInPuzzle;
    public GameCamera puzzleCam;
    void FixedUpdate() { if (playerInPuzzle) {PuzzleInputs();} }
    public virtual void StartPuzzle(PlayerController iPC) {}
    public virtual void StopPuzzle() {}
    public virtual void PuzzleInputs() {}
    public virtual bool TryValidatePuzzle() { return true; }
    public virtual void OnPuzzleSolved() {}
}
