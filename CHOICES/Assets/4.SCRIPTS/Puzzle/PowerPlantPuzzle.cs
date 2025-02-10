using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PowerPlantPuzzle : Puzzle
{
    public Transform self_puzzleBlock;
    public List<PowerPlantPuzzleGem> gemsToAlign;
    [Header("Module Rotation")]
    public Transform rotatingPartTransform;
    public float rotSpeed = 10f;
    public AnimationCurve rotSpeedIncrease;
    [Header("Gem Slide")]
    public float slideSpeed = 1f;
    [Header("On solve tweaks")]
    public float pushLerpTime = 1f;
    [Header("Internals")]
    private float rotateCW_startTime = 0f;
    private float rotateCCW_startTime = 0f;
    public override void StartPuzzle(PlayerController iPC)
    {
        if (puzzleSolved)
            return;

        playerInPuzzle = iPC;

        CameraManager.Instance.LerpCamToRef(puzzleCam, 1f);
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
        }
        return true;
    }

    public override void PuzzleInputs() 
    {
        // rotate
        if (playerInPuzzle.hMove < 0f) 
        {
            // Rotate CW
            rotateCW_startTime += Time.fixedDeltaTime;
            rotateCCW_startTime = 0f;
            rotatingPartTransform.Rotate(Vector3.forward * rotSpeed * rotSpeedIncrease.Evaluate(rotateCW_startTime), Space.Self);
        }
        else if (playerInPuzzle.hMove > 0f)
        {
            rotateCCW_startTime += Time.fixedDeltaTime;
            rotateCW_startTime = 0f;
            rotatingPartTransform.Rotate(-Vector3.forward * rotSpeed * rotSpeedIncrease.Evaluate(rotateCCW_startTime), Space.Self);
        } else {
            rotateCCW_startTime = 0f;
            rotateCW_startTime = 0f;
        }

        // slide gems
        if (playerInPuzzle.vMove < 0f) 
        {
            // further from arrow
            //rotatingPartTransform.Rotate(Vector3.forward * rotSpeed * rotSpeedIncrease.Evaluate(rotateCW_startTime), Space.Self);
            foreach (PowerPlantPuzzleGem gem in gemsToAlign)
            {
                int roundedGemAngle = (int)gem.pathWalker.angle;
                if ((roundedGemAngle > 180)||(roundedGemAngle < 180))
                {
                    gem.pathWalker.angle += slideSpeed;
                }
            }
        }
        else if (playerInPuzzle.vMove > 0f)
        {
            // closer to arrow
            foreach (PowerPlantPuzzleGem gem in gemsToAlign)
            {
                int roundedGemAngle = (int)gem.pathWalker.angle;
                if ((roundedGemAngle > 0)&&(roundedGemAngle < 360))
                {
                    gem.pathWalker.angle -= slideSpeed;
                }
            }
        } else {
            // nothing
        }
        
        // push
        if (playerInPuzzle.playerDoAction)
        {
            if (TryValidatePuzzle())
            {
                Debug.Log("Puzle solver");
                OnPuzzleSolved();
            } else {
                // not solved
            }
        }
    }

    public override void OnPuzzleSolved() 
    {
        puzzleSolved = true;
        StopPuzzle();
        PushBlock();
    }

    public override void StopPuzzle()
    {
        CameraManager.Instance.ResetPlayerCam( 1f);
        playerInPuzzle.freeze_WASD = false;
        playerInPuzzle.freeze_CAM = false;
        foreach ( PowerPlantPuzzleGem gem in gemsToAlign ) { gem.GemIsActive = false; }

        puzzleStarted = false;
        UIGame.Instance.cursorMode = false;
        playerInPuzzle = null;
    }

    #region LOCAL
    public void PushBlock()
    {
        StartCoroutine(BlockLerpCo());
    }

    IEnumerator BlockLerpCo()
    {
        Vector3 init = self_puzzleBlock.transform.localPosition;
        Vector3 target = new Vector3( init.x, init.y, -init.z);
        float startTime = Time.time;
        while ( (Time.time - startTime) < pushLerpTime)
        {
            float frac = (Time.time - startTime) / pushLerpTime;
            self_puzzleBlock.transform.localPosition = Vector3.Lerp(init, target, frac);
            yield return null;
        }

    }
    #endregion
}
