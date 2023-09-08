using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private bool inWallRun = false;
    [SerializeField] private bool wallJump = false;
    [SerializeField] private bool wallLeft, wallRight;
    [SerializeField] private float fallTimer = 0;
    [SerializeField] private float wallRunTimer = 0;

    [Header("Wall Run Settings")]
    [SerializeField] private LayerMask wallRunLayer; //layers that will allow wall running
    [SerializeField] private float minimumJumpHeight = 1.5f; //minimum height player must jump to wall run
    [SerializeField] private float wallForce = 6f; //force to keep player on wall
    [SerializeField] private float wallRunGravity = 6f; //down force when slipping off wall
    [SerializeField] private float wallCastDistance = 2f; // distance of ray for wall detection (from one side of player to other)
    [SerializeField] private float wallJumpForce = 20f; //Force applied when jumping off wall
    [SerializeField] private float wallRunTime = 1.5f; //Time player wallruns normally for
    [SerializeField] private float fallTime = 0.75f; //Time player will slip on wall before falling off


    private Transform lastWallObject;
    private Transform orientation;
    private Rigidbody rb;

    GameDataFile GDFile;
    InputController IC;
    EventManager EM;

    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        IC = GDFile.GetComponent<InputController>();
        EM = GDFile.GetComponent<EventManager>();
        SubEvents();

        rb = GetComponent<Rigidbody>();
        orientation = GDFile.orientation;
    }
    private void SubEvents() //subscribe to required events
    {
        //input controller
        IC.jump += OnInputJump;
    }
    private void OnInputJump() //trigger when jump is pressed
    {
        //wall jump
        if (inWallRun && !wallJump) { WallJump(); } //before wallrun to prevent immediatly jumping off wall
        //start wall run 
        if (CanWallRun() && (currentInput.xyMove.x != 0f) && (currentInput.xyMove.y > 0f) && CharacterState.inSprint && !CharacterState.grounded && !inWallRun) { StartWallRun(); }
    }
    private void Update()
    {
        Timer();
        if (wallJump && CanWallRun()) StartWallRun(); //restarts wall run when walljumping
        if (CharacterState.grounded && lastWallObject != null) { StopWallRun(); } //stops wall run
        if (inWallRun) { WallRunMovement(); CheckWallRun(); } //performs wallrun movement and checks wall run is still valid
    }
    private void Timer()
    {
        //wallRun Timer
        if (inWallRun)
        {
            if (wallRunTimer > 0f) wallRunTimer -= Time.deltaTime; //decrease first timer if it has not ended
            else fallTimer -= Time.deltaTime; //decrease second (fall) timer 
        }
    }
    private bool CanWallRun() //Checks if can start a wall run and which side it should be on
    {
        //Checks if the player is far enough off the floor to start wallrunning
        bool heightCheck = !Physics.Raycast(transform.position, -orientation.up, minimumJumpHeight);

        //checks if the player is near a wall
        Vector3 startPos = transform.position - (orientation.right * wallCastDistance); //player pos - offset, to get left and right with 1 raycast
        bool wallCheck = Physics.Raycast(startPos, orientation.right, out RaycastHit Hit, wallCastDistance * 2f, wallRunLayer);

        //determine which side the wall is on
        if (Hit.distance <= wallCastDistance) { wallLeft = true; wallRight = false; }
        else { wallLeft = false; wallRight = true; }

        //checks if player is trying to run on the same wall twice
        bool sameWallCheck = lastWallObject != Hit.transform;

        //Debug.Log("height: " + heightCheck + ", wall: " + wallCheck + ", same: " + sameWallCheck);

        //return if can wall run
        if (heightCheck && wallCheck && sameWallCheck) { lastWallObject = Hit.transform; return true; }
        else return false;
    }


    private void StartWallRun() //triggers wall run movement and sets values
    {
        //set bools
        CharacterState.inWallRun = true;
        inWallRun = true; wallJump = false;

        //set timers
        fallTimer = fallTime;
        wallRunTimer = wallRunTime;
    }
    private void StopWallRun() //stops wall run movement and resets values
    {
        lastWallObject = null;
        wallJump = false;
        inWallRun = false;
        CharacterState.inWallRun = false;
    }
    private void WallJump() //performs wall jump
    {
        wallJump = true;
        inWallRun = false;
        CharacterState.inWallRun = false;

        //Debug.Log("wallJump");
        Vector3 jumpDir = -orientation.right * wallJumpForce;
        if (wallLeft) jumpDir = orientation.right * wallJumpForce; //apply a force to push off the wall

        //Jump
        Vector3 jumpVector = new Vector3(0, wallJumpForce * CharacterState.gravityDir, 0) + jumpDir; //jump direction
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); //reset velocity
        rb.AddForce(jumpVector, ForceMode.VelocityChange); //apply jump force
        rb.AddForce(orientation.forward * (wallJumpForce), ForceMode.VelocityChange); //apply forward force
    }

    private void CheckWallRun() //checks if should stop wall run
    {
        //stop wall run if on ground
        if (CharacterState.grounded) { /*Debug.Log("1");*/ StopWallRun(); }

        //stop wall run if not moving forward or timer runs out
        else if (currentInput.xyMove.y <= 0f || fallTimer <= 0f) { /*Debug.Log("2");*/ StopWallRun(); }

        //stop wall run if not touching wall
        else if (!Physics.CheckCapsule(this.transform.position, this.transform.position, 2, wallRunLayer)) { /*Debug.Log("3");*/ StopWallRun(); }
    }
    private void WallRunMovement() //performs wallrun movement
    {
        if (rb.velocity.y <= 0f) //Wait till at top of jump
        {
            rb.useGravity = false;
            if (wallRunTimer <= 0f ) { rb.AddForce(-orientation.up * wallRunGravity, ForceMode.Acceleration); } //apply downward force
        }
        //apply force toward wall to keep player against it
        if (wallRight) rb.AddForce(orientation.right * wallForce, ForceMode.Force);
        else if (wallLeft) rb.AddForce(-orientation.right * wallForce, ForceMode.Force);
    }
}
