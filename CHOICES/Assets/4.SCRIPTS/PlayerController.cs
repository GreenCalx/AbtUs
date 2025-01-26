using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody self_rb;
    public bool freeze_inputs = false;
    public GameCamera FPSCamera;
    public float hMove, vMove;
    public float hCam, vCam;
    public float speed = 10f;
    public float maxSpeed = 2f;
    public float runningMaxSpeed = 4f;
    public float turnSpeed = 5f;
    public bool playerDoAction;
    public bool playerDoRun;
    public bool freezeToggle;
    private bool isMoving = false;
    private bool isRunning = false;

    private float cameraVRot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (self_rb==null)
            self_rb = GetComponentInChildren<Rigidbody>();
        hMove = 0f;
        vMove = 0f;
        
        self_rb.maxLinearVelocity = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        FetchInputs();
    }

    void FixedUpdate()
    {
        ProcessInputs();
    }

    private void FetchInputs()
    {
        hMove = Input.GetAxis("Horizontal");
        vMove = Input.GetAxis("Vertical");

        hCam = Input.GetAxis("Mouse X");
        vCam = Input.GetAxis("Mouse Y");

        playerDoRun = Input.GetButton("Run");
        playerDoAction = Input.GetButton("DoAction");

        freezeToggle  = Input.GetKeyUp("f");


        isMoving = (hMove!=0f)||(vMove!=0f);
    }

    private void ProcessInputs()
    {
        if (freezeToggle)
        { freeze_inputs = !freeze_inputs; }
        if (freeze_inputs)
        { return; }

        if (!!self_rb && !isMoving)
        {
            // self_rb.linearVelocity = new Vector3(0f, self_rb.linearVelocity.y, 0f);
            // self_rb.angularVelocity = Vector3.zero;
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

    }
}
