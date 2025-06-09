using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PowerPlantPuzzle : Puzzle
{
    [Header("Power Plant Puzzle")]
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
    [Header("GFX")]
    public Material defaultLightningMat;
    public Material solvedLightningMat;
    public List<MeshRenderer> renderersToUpdate;
    [Header("Internals")]
    private float rotateCW_startTime = 0f;
    private float rotateCCW_startTime = 0f;
    private short rotDir = 0;

    private Quaternion initRot = Quaternion.identity;


    void Start()
    {
        foreach(MeshRenderer mr in renderersToUpdate)
        {
            if (!puzzleSolved)
                mr.material = defaultLightningMat;
            else
                mr.material = solvedLightningMat;
        }
        initRot = rotatingPartTransform.rotation;
    }

    public override void StartPuzzle(PlayerController iPC)
    {
        if (puzzleSolved)
            return;

        playerInPuzzle = iPC;

        Managers.Instance.Camera.RegisterPlayerCamTransform();
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

        // rotate
        if (playerInPuzzle.hMove < 0f) 
        {
            // Rotate CW
            rotateCW_startTime += Time.fixedDeltaTime;
            rotateCCW_startTime = 0f;
            rotatingPartTransform.Rotate(Vector3.forward * rotSpeed * rotSpeedIncrease.Evaluate(rotateCW_startTime), Space.Self);
            SetAsRotatingCCW();
        }
        else if (playerInPuzzle.hMove > 0f)
        {
            rotateCCW_startTime += Time.fixedDeltaTime;
            rotateCW_startTime = 0f;
            rotatingPartTransform.Rotate(-Vector3.forward * rotSpeed * rotSpeedIncrease.Evaluate(rotateCCW_startTime), Space.Self);
            SetAsRotatingCW();
        } else {
            rotateCCW_startTime = 0f;
            rotateCW_startTime = 0f;
            SetAsNotRotating();
        }

        // slide gems
        if (playerInPuzzle.vMove < 0f) 
        {
            // further from arrow
            foreach (PowerPlantPuzzleGem gem in gemsToAlign)
            {
                int roundedGemAngle = (int)gem.pathWalker.angle;
                if ((roundedGemAngle >= 0)&&(roundedGemAngle <= 180))
                {
                    if (!gem.pathWalker.IsBlockedCCW)
                    {
                        gem.pathWalker.angle += slideSpeed;
                        //gem.SetAsSlidingCCW();
                        gem.pathWalker.isManualSlidingCCW = true;
                    }
                    
                } else if ((roundedGemAngle>=180) && (roundedGemAngle<=360))
                {
                    if (!gem.pathWalker.IsBlockedCW)
                    {
                        gem.pathWalker.angle -= slideSpeed;
                        //gem.SetAsSlidingCW();
                        gem.pathWalker.isManualSlidingCW = true;
                    }
                }
                
            }
        }
        else if (playerInPuzzle.vMove > 0f)
        {
            // closer to arrow
            foreach (PowerPlantPuzzleGem gem in gemsToAlign)
            {
                int roundedGemAngle = (int)gem.pathWalker.angle;
                if ((roundedGemAngle >= 0)&&(roundedGemAngle <= 180))
                {
                    if (!gem.pathWalker.IsBlockedCW)
                    {
                        gem.pathWalker.angle -= slideSpeed;
                        gem.pathWalker.isManualSlidingCW = true;
                    }
                    
                } else if ((roundedGemAngle>=180) && (roundedGemAngle<=360))
                {
                    if (!gem.pathWalker.IsBlockedCCW)
                    {
                        gem.pathWalker.angle += slideSpeed;
                        gem.pathWalker.isManualSlidingCCW = true;
                    }
                }
            }
        } else {
            // nothing
            foreach(PowerPlantPuzzleGem gem in gemsToAlign) 
            { 
                gem.pathWalker.isManualSlidingCW = false; 
                gem.pathWalker.isManualSlidingCCW = false;
            }
        }
        
        // Player actions
        // if (playerInPuzzle.playerDoAction)
        // {
        //     if (TryValidatePuzzle())
        //     {
        //         OnPuzzleSolved();
        //     } else {
        //         // not solved
        //     }
        // } else if (playerInPuzzle.playerDoCancel)
        // {
        //     StopPuzzle();
        // }
    }

    public override void OnPuzzleSolved() 
    {
        puzzleSolved = true;
        StopPuzzle();
        PushBlock();

        foreach(MeshRenderer mr in renderersToUpdate)
        {
            mr.material = solvedLightningMat;
        }

        parentPuzzleBundle.NotifySolved(this);
    }

    public override void StopPuzzle()
    {
        if (!puzzleStarted)
            return;
            
        Managers.Instance.Camera.SetCamToRegisteredTransform( 1f);
        playerInPuzzle.freeze_WASD = false;
        playerInPuzzle.freeze_CAM = false;
        foreach ( PowerPlantPuzzleGem gem in gemsToAlign ) { gem.GemIsActive = false; }

        puzzleStarted = false;
        UIGame.Instance.cursorMode = false;
        playerInPuzzle = null;
        elapsedPuzzleEntryTime = 0f;

        if (!puzzleSolved)
            ResetPuzzle();
    }

    public override void ResetPuzzle()
    {
        rotatingPartTransform.rotation = initRot;
        foreach ( PowerPlantPuzzleGem gem in gemsToAlign )
        {
            gem.pathWalker.angle = gem.pathWalker.initAngle;
        }
    }

    #region LOCAL
    public void PushBlock()
    {
        StartCoroutine(BlockLerpCo());
    }

    IEnumerator BlockLerpCo()
    {
        Vector3 init = self_puzzleBlock.transform.localPosition;
        Vector3 target = new Vector3( init.x, init.y, -(init.z/2f));
        float startTime = Time.time;
        while ( (Time.time - startTime) < pushLerpTime)
        {
            float frac = (Time.time - startTime) / pushLerpTime;
            self_puzzleBlock.transform.localPosition = Vector3.Lerp(init, target, frac);
            yield return null;
        }

    }

    public bool IsRotatingCW() { return rotDir > 0; }
    public bool IsRotatingCCW() { return rotDir < 0; }
    public void SetAsRotatingCW() 
    { 
        if (rotDir == 1)
            return;
        
        rotDir = 1;
        foreach(PowerPlantPuzzleGem gem in gemsToAlign)
        {
            gem.pathWalker.CWPathMotion = true;
            gem.pathWalker.CCWPathMotion = false;
        }
    }
    public void SetAsRotatingCCW() 
    { 
        if (rotDir == -1)
            return;

        rotDir = -1; 
        foreach(PowerPlantPuzzleGem gem in gemsToAlign)
        {
            gem.pathWalker.CWPathMotion = false;
            gem.pathWalker.CCWPathMotion = true;
        }
    }
    public void SetAsNotRotating() 
    { 
        if (rotDir == 0)
            return;

        rotDir = 0;
        foreach(PowerPlantPuzzleGem gem in gemsToAlign)
        {
            gem.pathWalker.CWPathMotion = false;
            gem.pathWalker.CCWPathMotion = false;
        }
    }
    #endregion
}
