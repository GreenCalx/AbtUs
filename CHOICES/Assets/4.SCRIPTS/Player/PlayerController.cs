using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody self_rb;
    public GameCamera FPSCamera;

    [Header("Tweaks")]
    public float speed = 10f;
    public float maxSpeed = 2f;
    public float runningMaxSpeed = 4f;
    public float turnSpeed = 5f;
    public float actionDistance = 5f;
    public float lookingDistance = 100f;
    public float actionTimeLatch = 0.2f;
    [Header("Internals")]
    public bool freeze_inputs = false;
    public float hMove, vMove;
    public float hCam, vCam;
    public bool playerDoAction;
    public bool playerInAction = false;
    public bool playerDoRun;
    public bool freezeToggle;
    private bool isMoving = false;
    private bool isRunning = false;
    private float cameraVRot;


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
        
        self_rb.maxLinearVelocity = maxSpeed;
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

        freezeToggle  = Input.GetButton("Freeze");

        isMoving = (hMove!=0f)||(vMove!=0f);
    }

    private void ProcessInputs()
    {
        if (freezeToggle && (elapsedActionTimeLatch >= actionTimeLatch))
        {   
            freeze_inputs = !freeze_inputs; 
            elapsedActionTimeLatch = 0f; 
        }
        if (freeze_inputs)
        { return; }

        if (!!self_rb && !isMoving && isGrounded())
        {
            self_rb.linearVelocity = new Vector3(0f, 0f, 0f);
            self_rb.angularVelocity = Vector3.zero;
        }

        if (playerDoRun && !isRunning)
        {
            self_rb.maxLinearVelocity = runningMaxSpeed;
            isRunning = true;
        } else if (!playerDoRun && isRunning) {
            self_rb.maxLinearVelocity = maxSpeed;
            isRunning = false;
        }

        // side step
        if (hMove < 0f)
        {
            self_rb.AddForce( -1 *  transform.right * speed, ForceMode.VelocityChange);
            isMoving = true;
        }
        else if (hMove > 0f)
        {
            self_rb.AddForce(transform.right * speed, ForceMode.VelocityChange);
            isMoving = true;
        }
        
        // forward/backward
        if (vMove > 0f)
        {
            // move forward
            // Vector3 translation = new Vector3(0f, 0f, speed * Time.fixedDeltaTime);
            // transform.Translate(translation);
            self_rb.AddForce( transform.forward * speed, ForceMode.VelocityChange);
            isMoving = true;
        } else if ( vMove < 0f )
        {
            // move backward
            // Vector3 translation = new Vector3(0f, 0f, (speed/2f) * Time.fixedDeltaTime * -1);
            // transform.Translate(translation);
            self_rb.AddForce( transform.forward * (speed/2f) * -1, ForceMode.VelocityChange);
            isMoving = true;
        }

        // cam horizontal
        transform.Rotate(Vector3.up * hCam);

        // cam vertical
        cameraVRot -= vCam;
        cameraVRot = Mathf.Clamp(cameraVRot, -90f, 90f);
        FPSCamera.transform.localEulerAngles = Vector3.right * cameraVRot;

        // player action
        if (elapsedActionTimeLatch < actionTimeLatch)
            return;

        if (playerDoAction)
        {
            if (targetedInteractibleObject!=null)
            {
                if (!playerInAction)
                {
                    playerInAction = true;
                    targetedInteractibleObject.OnInteract(this);
                    
                } else {
                    playerInAction = false;
                    targetedInteractibleObject.OnStopInteract(this);
                }
                elapsedActionTimeLatch = 0f;
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
            InteractibleObject iobj = objectRayHit.collider.gameObject.GetComponent<InteractibleObject>();
            if (iobj!=null)
            {
                if (iobj==targetedInteractibleObject)
                    return;

                targetedInteractibleObject = iobj;
                //UIGame.Instance.TryChangeCrosshairColor(Color.green);
                UIGame.Instance.UpdateCursorFromPlayerAction(targetedInteractibleObject.GetSelectedAction());
                return;
            }
        }
        
        if (targetedInteractibleObject!=null)
        {
            //UIGame.Instance.TryChangeCrosshairColor(Color.white);
            targetedInteractibleObject = null;
            UIGame.Instance.UpdateCursorFromPlayerAction(PLAYER_ACTIONS.NONE);
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
