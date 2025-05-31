 using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody self_rb;
    public GameCamera FPSCamera;

    [Header("Tweaks")]
    public float speed = 10f;
    public float runSpeed = 4f;
    public float turnSpeed = 5f;
    public float actionDistance = 5f;
    public float lookingDistance = 100f;
    public float actionTimeLatch = 0.2f;
    [Header("Internals")]
    public bool freeze_inputs = false;
    public bool freeze_WASD = false;
    public bool freeze_CAM = false;

    public float hMove, vMove;
    public float hCam, vCam;
    private Quaternion targetRot;
    private Vector3 targetMove;

    public bool playerDoAction;
    public bool playerInAction = false;
    public bool playerDoRun;
    public bool playerDoCancel = false;
    public bool freezeToggle;
    private bool isMoving = false;
    private bool isRunning = false;


    [Header("Internals")]
    public InteractibleObject targetedInteractibleObject;

    private TargetFeedback targetedFeedbackObject;

    private float elapsedActionTimeLatch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (self_rb==null)
            self_rb = GetComponentInChildren<Rigidbody>();
        hMove = 0f;
        vMove = 0f;
        elapsedActionTimeLatch = 0f;
        Managers.Instance.Sound.InitBGMSources(FPSCamera.transform);
    }

    void UpdateTimers()
    {
        if (elapsedActionTimeLatch<=actionTimeLatch) 
        { elapsedActionTimeLatch += Time.deltaTime; } 
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
        FetchInputs();
    }

    void FixedUpdate()
    {
        CheckInteractibleObjects();
        ProcessInputs();
        CheckTargetObjects();
    }

    private void FetchInputs()
    {
        hMove = Input.GetAxis("Horizontal");
        vMove = Input.GetAxis("Vertical");

        hCam = Input.GetAxis("Mouse X");
        vCam = Input.GetAxis("Mouse Y");

        playerDoRun = Input.GetButton("Run");
        playerDoAction = Input.GetButton("DoAction");
        playerDoCancel = Input.GetButton("Cancel");
        freezeToggle  = Input.GetButton("Freeze");

        isMoving = (hMove!=0f)||(vMove!=0f);
    }

    private void ProcessInputs()
    {
        // player action
        if (elapsedActionTimeLatch >= actionTimeLatch)
        {
            if (playerDoAction)
            {
                if (targetedInteractibleObject!=null)
                {
                    if (!playerInAction)
                    {
                        playerInAction = true;
                        targetedInteractibleObject.OnInteract(this);
                    }
                    else
                    {
                        targetedInteractibleObject.OnContinueInteract(this);
                        playerInAction = targetedInteractibleObject ? targetedInteractibleObject.IsInAction() : false;
                        
                    }
                    elapsedActionTimeLatch = 0f;
                }
            }

            if (playerDoCancel && playerInAction)
            {
                playerInAction = false;
                targetedInteractibleObject.OnCancelInteract(this);
                elapsedActionTimeLatch = 0f;
            }

            // freeze cam / movement
            if (freezeToggle && (elapsedActionTimeLatch >= actionTimeLatch))
            {
                freeze_inputs = !freeze_inputs;
                elapsedActionTimeLatch = 0f;
            }
        }


        if (freeze_inputs)
        { return; }

        // player movez
        if (playerDoRun && !isRunning)
        {
            isRunning = true;
        } else if (!playerDoRun && isRunning) {
            isRunning = false;
        }

        if (!freeze_CAM)
        {
            FPSCamera.VClampedRotation(new Vector3(-vCam, hCam), -90f, 90f);
        }

        if (!freeze_WASD)
        {
            isMoving = ((vMove != 0f) || (hMove != 0f));
            if (isMoving)
            {
                targetMove = new Vector3(hMove, 0f, vMove);
                targetMove = Vector3.ClampMagnitude(targetMove, 1f);
                targetMove = FPSCamera.transform.rotation * targetMove;
                if (!isRunning)
                    self_rb.MovePosition(transform.position + (targetMove * speed));
                else
                    self_rb.MovePosition(transform.position + (targetMove * runSpeed));
            }
        }
    }

    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.1f);
    }

    private void CheckInteractibleObjects()
    {
        if (playerInAction) // already in action with its current object
            return;

        RaycastHit objectRayHit;
        if (FPSCamera.TryRCFromScreenCenter(out objectRayHit, actionDistance))
        {
            // did hit
            InteractibleObject iobj = objectRayHit.collider.gameObject.GetComponentInParent<InteractibleObject>();
            if (iobj == null)
                iobj = objectRayHit.collider.gameObject.GetComponent<InteractibleObject>();

            if (iobj != null)
            {
                // exit if same
                if (iobj == targetedInteractibleObject)
                    return;

                // Deselect previous selection if exists
                if (targetedInteractibleObject != null)
                {
                    targetedInteractibleObject.Deselect();
                }
                // Select new target
                targetedInteractibleObject = iobj;
                targetedInteractibleObject.Select();
                UIGame.Instance.UpdateCursorFromPlayerAction(targetedInteractibleObject.GetSelectedAction());
                return;
            }
            else
                UIGame.Instance.SetCursorToDefault();
        }
        
        if (targetedInteractibleObject!=null)
        {
            targetedInteractibleObject.Deselect();
            targetedInteractibleObject = null;
            UIGame.Instance.UpdateCursorFromPlayerAction(PLAYER_ACTIONS.DEFAULT);
        }

    }

    private void CheckTargetObjects()
    {
        RaycastHit objectRayHit;
        if (FPSCamera.TryRCFromScreenCenter(out objectRayHit, lookingDistance))
        {
            // did hit
            TargetFeedback iobj = objectRayHit.collider.gameObject.GetComponent<TargetFeedback>();
            if (iobj != null)
            {

                if (iobj == targetedFeedbackObject)
                    return;

                targetedFeedbackObject = iobj;
                targetedFeedbackObject.player_looking(true);
                return;
            }

        }

        if (targetedFeedbackObject != null)
        {
            targetedFeedbackObject.player_looking(false);
            targetedFeedbackObject = null;
        }

    }
}
