using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PortalChoicePuzzle : Puzzle
{
    public List<PowerPlantPuzzleGem> gemsToAlign;
    public BeamCaster beamCaster;
    [Header("Module Rotation")]
    public Transform rotatingPartTransform;
    public float rotSpeed = 10f;
    
    [Header("Internals")]
    private float rotateCW_startTime = 0f;
    private float rotateCCW_startTime = 0f;
    private short rotDir = 0;

    public override void StartPuzzle(PlayerController iPC)
    {
        if (puzzleSolved)
            return;

        playerInPuzzle = iPC;

        Managers.Instance.Camera.LerpCamToRef(puzzleCam, puzzleEntryInputLatch);
        playerInPuzzle.freeze_WASD = true;
        playerInPuzzle.freeze_CAM = true;

        foreach ( PowerPlantPuzzleGem gem in gemsToAlign ) { gem.GemIsActive = true; }
        
        UIGame.Instance.cursorMode = true;
        puzzleStarted = true;
    }

    public override bool TryValidatePuzzle()
    {
        foreach(PowerPlantPuzzleGem gem in gemsToAlign)
        {
            if (!gem.GemIsAligned)
                return false;
            if (gem.GemIsMisalgined)
                return false;
        }
        return true;
    }

    public override void PuzzleInputs() 
    {
        if (elapsedPuzzleEntryTime < puzzleEntryInputLatch)
        {
            elapsedPuzzleEntryTime += Time.deltaTime;
            return;
        }

        // module rotation
        if (playerInPuzzle.hMove > 0f) 
        {
            // Rotate CW
            rotateCW_startTime += Time.fixedDeltaTime;
            rotateCCW_startTime = 0f;
            rotatingPartTransform.Rotate(Vector3.forward * rotSpeed, Space.Self);

            beamCaster.CastBeam();
        }
        else if (playerInPuzzle.hMove < 0f)
        {
            rotateCCW_startTime += Time.fixedDeltaTime;
            rotateCW_startTime = 0f;
            rotatingPartTransform.Rotate(-Vector3.forward * rotSpeed, Space.Self);

            beamCaster.CastBeam();
        } else {
            rotateCCW_startTime = 0f;
            rotateCW_startTime = 0f;
        }

        // Player actions
        if (playerInPuzzle.playerDoAction)
        {
            if (TryValidatePuzzle())
            {
                OnPuzzleSolved();
            } else {
                // not solved
            }
        } else if (playerInPuzzle.playerDoCancel)
        {
            StopPuzzle();
        }
    }

    public override void OnPuzzleSolved() 
    {
        puzzleSolved = true;
        StopPuzzle();
    }

    public override void StopPuzzle()
    {
        Managers.Instance.Camera.ResetPlayerCam(1f);
        playerInPuzzle.freeze_WASD = false;
        playerInPuzzle.freeze_CAM = false;
        foreach ( PowerPlantPuzzleGem gem in gemsToAlign ) { gem.GemIsActive = false; }

        puzzleStarted = false;
        UIGame.Instance.cursorMode = false;
        playerInPuzzle = null;        
        elapsedPuzzleEntryTime = 0f;
    }
}