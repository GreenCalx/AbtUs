using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleBundle : MonoBehaviour
{
    public UnityEvent OnAllSolved;
    [Tooltip("Puzzles to solve must be children")]
    public List<Puzzle> PuzzlesToSolve;
    public bool OnAllSolvedTriggered = false;

    void Start()
    {
        PuzzlesToSolve = new List<Puzzle>(GetComponentsInChildren<Puzzle>());
        foreach(Puzzle p in PuzzlesToSolve)
        {
            p.parentPuzzleBundle = this;
        }
    }

    public void NotifySolved(Puzzle iPuzzle)
    {
        if (OnAllSolvedTriggered)
            return;

        foreach(Puzzle p in PuzzlesToSolve)
        {
            if (!p.puzzleSolved)
                return;
        }

        OnAllSolvedTriggered = true;
        OnAllSolved.Invoke();
    }
}
