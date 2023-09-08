using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

static class CharacterState
{
    public static bool grounded = false;
    public static bool inSprint = false;
    public static bool inWallRun = false;
    public static bool inAntigravity = false;
    public static bool inGrapple = false;
    public static bool useDrag = true;
    public static float gravityDir = 1;
}

public class CharacterMovementV2 : MonoBehaviour
{
    //------Variables------\\
    #region Variables

    #region Area
    [Header("Area")]
    [SerializeField] private int currentArea = 0;
    [SerializeField] private GameObject respawnPoint;
    #endregion

    #region Animation
    [Header("Animation")]
    private Animator AnimCon;
    private float LastxyMove;
    #endregion

    #region Ability Limits
    //[Header("Ability limits")]
    //[SerializeField] private float maxWallJumps = 3f;
    //[SerializeField] private float no_wallJumps;
    #endregion

    #region Movement
    [Header("Current Speed")]
    [SerializeField] private float moveSpeed;
    [SerializeField] public float rbSpeed;
    [SerializeField] private float rbVelocity_Y;
    private float lastYPos;
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float crouchSpeed = 6.5f;
    [SerializeField] private float proneSpeed = 3f;
    [SerializeField] private float sprintSpeed = 12f;
    [SerializeField] private float overSpeed = 50f;
    [SerializeField] private float airSpeed = 0.2f;
    [SerializeField] private float acceleration = 24f;
    [HideInInspector] public bool inSprint = false, sprintToggle = false;
    private Vector3 moveDirection, slopeMoveDirection;
    #endregion

    #region CounterMovement
    [Header("CounterMovement")]
    [SerializeField] private float CM = 1f;
    private bool inOverSpeed = false;
    private float overSpeedMod = 25f;
    #endregion

    #region Grounded
    [Header("Grounded")]
    [SerializeField] public bool isGrounded;
    //[SerializeField] private bool isGroundedRC;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxSlopeAngle = 35f;
    [SerializeField] public float angle;
    [SerializeField] public float sideAngle;
    [SerializeField] private float sphereHits;
    [SerializeField] private List<float> angleList = new List<float>(6);
    #endregion

    #region Step offset
    [Header("Step Offset")]
    [SerializeField] private float upperStepHeight = 0.5f;
    [SerializeField] private float lowerStepHeight = 0.1f;
    [SerializeField] private float stepSpeed = 0.1f;
    [SerializeField] private float stepAngle = 15f;

    [Header("Step up Offset")]
    [SerializeField] private float distFromPlayer_StepUp = 0.1f;
    [SerializeField] private float dist = 0f;
    [SerializeField] private bool stepUp = false;
    private bool moveUp = false;

    [Header("Step down Offset")]
    [SerializeField] private float distFromPlayer_StepDown = 0.1f;
    //[SerializeField] private float snapDistance = 2f;
    [SerializeField] private float downDist = 0f;
    [SerializeField] private bool stepDown = false;

    [Header("Edge correction")]
    [SerializeField] private float distFromPlayer_Edge = 0.1f;
    [SerializeField] private float edgeCheckDist = 2f;
    [SerializeField] private float checkAngle = 0;
    [SerializeField] private bool correctEdge = false;
    #endregion

    #region Crouch
    [Header("Crouch")]
    [SerializeField] private bool inCrouch = false;
    [SerializeField] private bool inProne = false;
    [SerializeField] private bool canCrouch, canStand;
    private float moveCamPos;
    #endregion

    #region Jump
    [Header("Jump")]
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private bool inJump = false;
    [SerializeField] private bool doubleJump = false;
    #endregion

    #region Sliding
    [Header("Sliding")]
    [SerializeField] private bool inSlide = false;
    [SerializeField] private bool fallSlide = false;
    [SerializeField] private bool slideJump = false;
    [SerializeField] private float slideForce = 75f;
    [SerializeField] private float fallSlideForce = 50f;
    [SerializeField] private float slideTime = 0.75f;
    [SerializeField] private float slideTimer = 0f;
    [SerializeField] private float slideCoolDown = 5f;
    [SerializeField] private float slideCoolDownTimer;
    [HideInInspector] public bool slideCoolDownBool = false;
    private float slideAngle = 20f;
    private Vector3 slideMove;
    #endregion

    #region Drag
    [Header("Drag")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 0.15f;
    #endregion

    #region Gravity
    [Header("Gravity")]
    [SerializeField] private float addedDownForce = 18f;
    #endregion

    #region Camera
    [Header("Camera")]
    [SerializeField] Transform cameraPosition;
    [SerializeField] Transform CameraRig;
    [SerializeField] Transform orientation;
    [HideInInspector] public Vector2 xySensitivity, xyRotation;
    //[SerializeField] public Vector2 xyLook;

    [Header("Set Camera Positions")]
    [SerializeField] private float proneCamPos = -0.5f;
    [SerializeField] private float standingCamPos = 0.8f;
    [SerializeField] private float crouchCamPos = 0.15f;
    #endregion

    #region Camera/Collider Position
    //Change height by adding an offset to these values
    //Camera position

    //Collider Height

    #endregion

    #region Collider
    [Header("Collider")]
    [SerializeField] private Transform CharacterModel;
    [SerializeField] private PhysicMaterial PlayerMaterial;
    [SerializeField] private PhysicMaterial SlopeMaterial;
    private CapsuleCollider PlayerCollider;

    [Header("Set Collider Heights")]
    private float colliderRadius = 0.35f;
    private float colliderStandingHeight = 1.8f; //.008125f;
    private float colliderCrouchHeight = 1.0f; //.484938f;
    private float colliderProneHeight = 0.6f; //.14f;
    #endregion

    #region Scripts
    GameDataFile GDFile;
    InputController IC;
    EventManager EM;
    #endregion

    //other variables
    [HideInInspector] public Rigidbody rb;
    RaycastHit slopeHit;
    RaycastHit SphereHit;
    RaycastHit snapHit;

    #endregion

    //------Functions------\\
    #region StandardFunctions

    private void Start() //Performs functions before the first frame
    {
        //Set variables for start of game
        moveSpeed = walkSpeed;
        xySensitivity = new Vector2(12, 6);
        moveCamPos = standingCamPos; //sets camera position at start of game
        //no_wallJumps = 0f;
    }
    private void subEvents() //subscribe to required events
    {
        //input controller
        IC.jump += OnInputJump;
        IC.doubleJump += DoubleJump;
        IC.sprint += Sprint;
        IC.sprintToggle += SprintToggle;
        IC.sprintCancelled += SprintCancelled;
        IC.changePosition += changePosition;
        IC.slide += SlideCheck;

        //Event manager
        EM.respawn += Respawn;
        EM.area += UpdateArea;
    }
    private void Awake()
    {   //Pefroms functions on the first frame
        GDFile = FindObjectOfType<GameDataFile>();
        IC = GDFile.GetComponent<InputController>();
        EM = GDFile.GetComponent<EventManager>();
        subEvents();

        rb = GetComponent<Rigidbody>();
        PlayerCollider = CharacterModel.GetComponent<CapsuleCollider>();
        PlayerCollider.height = colliderStandingHeight;
        PlayerCollider.center = new Vector3(0, 0.05f, 0);
        rb.freezeRotation = true;
        rb.useGravity = false;

        //Animator
        AnimCon = GetComponent<Animator>();
    }
    private void Update()
    {   //performs updates at a frame dependent rate
        if (gameState.paused) return;

        AnimationManager(); //updates animations
        TimerUpdate(); //update any active timers   
        MyInput(); //Get player input
        //ControlDrag(); //set rb drag
        ColliderUpdateV2(); //set Collider size when crouch/prone/standing
        SpeedControl(); //changes player speed

        if ((sprintToggle && Mathf.Abs(currentInput.xyMove.y) < 0.35f)) { sprintToggle = false; } //stop sprint if character slows down to near stop
        if (!inSlide) Countermovement(); //update countermovement
    }
    private void FixedUpdate() //Performs updates at a time dependent rate
    {
        if (gameState.paused) return;

        StepUpOffset();
        StepDownOffset();
        EdgeCorrrection();
        GroundDetection(); //check for ground    

        //Check for death barriers
        if (Physics.Raycast(transform.position - new Vector3(0, 0.6f, 0), -orientation.up, 5f, GDFile.deathBarrierLayer)) TriggerRespawn();

        //Y Velocity using formula
        rbVelocity_Y = (transform.position.y - lastYPos) / Time.deltaTime;
        lastYPos = transform.position.y;

        //Gravity
        Gravity();
        //Movement
        if (!fallSlide) Movement();
        //slide
        if (inSlide) SlideMovement();
    }
    private void LateUpdate()
    {
        if (gameState.paused) return;
        Camera(); //player and camera rotations 
    }
    private void UpdateArea() //update the current area for enemy spawns
    {
        Debug.Log("now in area: " + gameState.area);
        currentArea = gameState.area;
    }
    #endregion

    #region Gravity
    private void Gravity()
    {
        ControlDrag();
        if (CharacterState.inWallRun) { return; }

        //turn off rb gravity
        if (CharacterState.inAntigravity) rb.useGravity = false;
        else rb.useGravity = true;
        //Additional Gravity
        //if (!isGrounded) rb.AddForce(new Vector3(0, -18f * CharacterState.gravityDir, 0), ForceMode.Acceleration);
    }
    void ControlDrag() //sets the player drag
    {
        if (!CharacterState.useDrag) return;

        if (isGrounded) { rb.drag = groundDrag; }
        else if (CharacterState.inWallRun) { rb.drag = groundDrag - 4; }
        else { rb.drag = airDrag; }//while jumping or falling
    }
    #endregion

    #region Animation
    private void AnimationManager()
    {   //Handles any animation

        AnimCon.SetBool("DoubleJump", doubleJump);
        AnimCon.SetBool("Jump", inJump);
        if (currentInput.xyMove.y > 0f)
        { AnimCon.SetFloat("X", (moveSpeed / sprintSpeed)); }
        else if (currentInput.xyMove.y < 0f)
        { AnimCon.SetFloat("X", (moveSpeed / sprintSpeed) * -1f); }
        else AnimCon.SetFloat("X", Mathf.MoveTowards(moveSpeed, 0f, 250000f));
        AnimCon.SetBool("Crouch", inCrouch);// if (inCrouch) else AnimCon.SetBool("Crouch", false);
        AnimCon.SetBool("Prone", inProne);//if (inProne)  else AnimCon.SetBool("Prone", false);
        if (Mathf.Abs(LastxyMove) > 0f && currentInput.xyMove.y == 0f && inSprint) AnimCon.SetTrigger("SuddenStop");

        LastxyMove = currentInput.xyMove.y;
        AnimCon.SetFloat("LastxyMove", LastxyMove);
    }
    public List<bool> GetAnimDataBoolList() //gives current player animation bools for multiplayer
    {
        List<bool> boolList = new List<bool>();
        boolList.Add(inJump);
        boolList.Add(doubleJump);
        boolList.Add(inCrouch);
        boolList.Add(inProne);

        bool sStop;
        if (Mathf.Abs(LastxyMove) > 0f && currentInput.xyMove.y == 0f && inSprint) sStop = true;
        else sStop = false;
        boolList.Add(sStop);

        return boolList;
    }
    public List<float> GetAnimDataFloatList() //gives current player animation floats for multiplayer
    {
        List<float> floatList = new List<float>();

        if (currentInput.xyMove.y > 0f)
        { floatList.Add(moveSpeed / sprintSpeed); }
        else if (currentInput.xyMove.y < 0f)
        { floatList.Add((moveSpeed / sprintSpeed) * -1f); }
        else floatList.Add(0f);

        LastxyMove = currentInput.xyMove.y;
        floatList.Add(LastxyMove);

        return floatList;
    }
    #endregion

    #region Ground/Floor/slope/anlge detection
    private void TriggerRespawn()
    {
        EM.RespawnEvent();
        Respawn();
    }
    public void Respawn()
    {
        inSlide = false;
        CharacterState.inAntigravity = false;
        CharacterState.gravityDir = 1;
        rb.Sleep();

        if (GDFile.currentCheckPoint) { GDFile.MoveToPoint(GDFile.currentCheckPoint); }
        else { transform.position = Vector3.zero; transform.rotation = Quaternion.Euler(Vector3.zero); }
    }
    //private void wallDetection()
    //{
    //    if (Physics.Raycast(transform.position, moveDirection, out RaycastHit wallAngleHit, 5f, groundLayer)) wallAngle = Vector3.Angle(wallAngleHit.normal, orientation.up);
    //    else wallAngle = 0;
    //}
    private void GroundDetection()
    {
        if (Physics.SphereCast(transform.position, colliderRadius * 0.9f, -orientation.up, out SphereHit, (1f - colliderRadius) * 1f, groundLayer))
        {
            angleDetection();
            if (angle <= maxSlopeAngle) isGrounded = true;
            else { isGrounded = false; if (!fallSlide && !stepUp) FallSlide(); }
        }        
        else isGrounded = false;
        CharacterState.grounded = isGrounded;
    }

    private RaycastHit angleHit;
    private List<RaycastHit> rayList = new List<RaycastHit>(6);
    private bool RayCastPoint2(Vector3 point, Vector3 offset)
    { return Physics.Raycast(point + offset, -orientation.up, out angleHit, 1.2f, groundLayer); }
    void angleDetection()
    {
        //clear lists
        angleList.Clear(); rayList.Clear(); sideAngle = 0;

        //get contact points from spherecast  //0.425 is distance to ground (0.43) minus the collider center offset (0.05)
        RaycastHit[] rays = Physics.SphereCastAll(transform.position, colliderRadius * 0.9f, -orientation.up, (1.05f - colliderRadius) * 0.9f, groundLayer);
        float lastAngle = 90;
        int i = 0, index = 0;
        foreach (RaycastHit ray in rays)
        {
            float angle = Vector3.Angle(orientation.up, ray.normal);
            if (angle < lastAngle) { lastAngle = angle; index = i; }
            i++;
        }
        sphereHits = i;
        if (sphereHits == 0) return;
        Vector3 point = rays[index].point + new Vector3(0, 1f * CharacterState.gravityDir, 0);

        //check points around contact area
        float Radius = 0.2f;
        if (RayCastPoint2(point, new Vector3(0, 0, 0) * Radius)) { rayList.Add(angleHit); } //Debug.DrawRay(point, -orientation.up * 1.2f, Color.red); 
        if (RayCastPoint2(point, new Vector3(0, 0, 1) * Radius)) { rayList.Add(angleHit);} //Debug.DrawRay(point + (new Vector3(0, 0, 1) * colliderRadius), -orientation.up * 1.2f, Color.red); 
        if (RayCastPoint2(point, new Vector3(0, 0, -1) * Radius)) { rayList.Add(angleHit); } //Debug.DrawRay(point + (new Vector3(0, 0, -1) * colliderRadius), -orientation.up * 1.2f, Color.red); 
        if (RayCastPoint2(point, new Vector3(0.866f, 0, 0.5f) * Radius)) { rayList.Add(angleHit); } //Debug.DrawRay(point + (new Vector3(0.866f, 0, 0.5f) * colliderRadius), -orientation.up * 1.2f, Color.red); 
        if (RayCastPoint2(point, new Vector3(0.866f, 0, -0.5f) * Radius)) { rayList.Add(angleHit); } //Debug.DrawRay(point + (new Vector3(0.866f, 0, -0.5f) * colliderRadius), -orientation.up * 1.2f, Color.red); 
        if (RayCastPoint2(point, new Vector3(-0.866f, 0, 0.5f) * Radius)) { rayList.Add(angleHit); } //Debug.DrawRay(point + (new Vector3(-0.866f, 0, 0.5f) * colliderRadius), -orientation.up * 1.2f, Color.red); 
        if (RayCastPoint2(point, new Vector3(-0.866f, 0, -0.5f) * Radius)) { rayList.Add(angleHit); } //Debug.DrawRay(point + (new Vector3(-0.866f, 0, -0.5f) * colliderRadius), -orientation.up * 1.2f, Color.red); 
        if (rayList.Count == 0) { angle = -1; return; }

        //get angles from rays
        foreach (RaycastHit ray in rayList)
        {
            float a = Mathf.Abs(Mathf.Round(Vector3.Angle(orientation.up, ray.normal)));
            angleList.Add(a);
        }
        //calculate median
        List<float> tempList = angleList;
        angleList.Sort();
        float median = angleList[Mathf.RoundToInt((angleList.Count - 1) / 2)];
        i = tempList.IndexOf(median);
        slopeHit = rayList[i];

        if (median != 0) //side cast to detect other sides of slopes when at the top 
        {
            Vector3 point2 = transform.position - new Vector3(0, 0.925f, 0);
            Physics.Raycast(point2, new Vector3(point.x, transform.position.y, point.z) - transform.position, out RaycastHit hit2, 1.2f, groundLayer);
            //Debug.DrawRay(point2, (new Vector3(point.x, transform.position.y, point.z) - transform.position), Color.blue);
            sideAngle = MathF.Round(Vector3.Angle(orientation.up, hit2.normal));
            if (sideAngle > maxSlopeAngle && sideAngle < 90) { angle = sideAngle; } // Debug.Log("yea its goping off"); }
            else angle = median;
        }
        else angle = median;
    }
    private bool onSlope()
    {
        if (angle > 0) return true;
        else return false;
    }
    private void AddDownForce()
    {
        Vector3 gravity = Physics.gravity * (addedDownForce * CharacterState.gravityDir);
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void StepUpOffset()
    {
        Vector3 pos = (transform.position + (moveDirection.normalized * distFromPlayer_StepUp) + new Vector3(0, 1.5f, 0));
        if (Physics.Raycast(pos, -orientation.up, out RaycastHit stairHit, 10f, groundLayer))
        {
            dist = 1 - (stairHit.distance - 1.5f);
            if (dist >= lowerStepHeight && dist <= upperStepHeight && angle <= stepAngle && Vector3.Angle(orientation.up, stairHit.normal) <= stepAngle)
            {
                stepUp = true;
                if(transform.position.y - 1f <= stairHit.point.y) { transform.position += new Vector3(0, dist * stepSpeed, 0); moveUp = true; }
                else if(moveUp) { transform.position = stairHit.point + new Vector3(0,0.5f,0); moveUp = false; }
            }
            else stepUp = false;
        }
        else stepUp = false;
    }
    private bool allowStepDown = true;
    private void StepDownOffset()
    {
        if (inJump || CharacterState.inWallRun) return;

        Vector3 pos = (transform.position + (moveDirection.normalized * distFromPlayer_StepDown) + new Vector3(0, 1.5f, 0));
        if (Physics.Raycast(pos, -orientation.up, out RaycastHit stairHit, 10f, groundLayer) && allowStepDown)
        {
            downDist = stairHit.distance - 1.5f - 1f;
            if (downDist >= lowerStepHeight && downDist <= upperStepHeight && Vector3.Angle(orientation.up, stairHit.normal) <= stepAngle)
            {
                stepDown = true;
                transform.position -= new Vector3(0, downDist * stepSpeed, 0); //= stairHit.point + new Vector3(0, 1f, 0);
            }
            else { allowStepDown = false; stepDown = false; }
        }
        else { stepDown = false; }

        if (isGrounded) allowStepDown = true;
    }
    private void EdgeCorrrection()
    {
        if (currentInput.xyMove == Vector2.zero || inJump || stepUp || !isGrounded) { correctEdge = false; return; }

        Vector3 pos = (transform.position + (moveDirection.normalized * distFromPlayer_Edge) + new Vector3(0, 1.5f, 0));
        if (Physics.Raycast(pos, -orientation.up, out RaycastHit edgeHit, 10f, groundLayer))
        {
            checkAngle = Mathf.Round(Vector3.Angle(orientation.up, edgeHit.normal));
            if (checkAngle != angle && checkAngle <= maxSlopeAngle) 
            {
                if(edgeCheckDist >= edgeHit.distance - 1.5f - 1f)
                {
                    //transform.position = edgeHit.point + new Vector3(0, 0.9f, 0);
                    rb.velocity = Vector3.ProjectOnPlane(rb.velocity, edgeHit.normal);
                    correctEdge = true;
                }
                else correctEdge = false;
            }
            else correctEdge = false;
        }
        else correctEdge = false;
    }
    #endregion

    #region Timers
    private void TimerUpdate()
    {   //Updates any timers

        //slide Timer
        if (inSlide && !fallSlide) slideTimer -= Time.deltaTime; //decrease timer if not on a slope
        //slide cooldown timer
        if (slideCoolDownBool) slideCoolDownTimer -= Time.deltaTime; //slide cooldown
        if (slideCoolDownTimer <= 0f) slideCoolDownBool = false; //turn off slide cooldown
    }
    #endregion

    #region PlayerMovement/speedcontrol
    void MyInput()
    {   
        //PlayerMovement input        
        moveDirection = (orientation.transform.forward * currentInput.xyMove.y) + (orientation.transform.right * currentInput.xyMove.x) ;

        //CameraMovement Input
        float multiplier = 0.005f; //movement scaler
        if (IC.GetControlScheme() != "Keyboard & Mouse") multiplier *= Time.unscaledDeltaTime;
        xyRotation.x -= currentInput.xyLook.y * xySensitivity.y * multiplier; //Left/Right
        xyRotation.y += currentInput.xyLook.x * xySensitivity.x * multiplier; //Up/Down
    }
    void Movement() //moves the player **may need to add time.deltatime to addforce**
    {   
        float moveMultiplier = 6.81818465f; float airVal = 1f;
        if (!isGrounded && !CharacterState.inWallRun) airVal = airSpeed;

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        float force = moveSpeed * moveMultiplier * airVal * CM; //get move force
        if (CM != 0 && !float.IsNaN(force))
        { 
            if (fallSlide) { slopeMoveDirection.y = 0; }
            rb.AddForce(slopeMoveDirection.normalized * force, ForceMode.Acceleration); //Apply movement
        }
    }
    void Countermovement() //changes the speed multiplyer to prevent going too far over maxspeed
    {
        //only add countermovement every other fram to allow changing direction when at max speed
        //if (framSkip == 1) { CM = 1; framSkip = 0; return; }
        //else framSkip++;

        Vector3 rbVelocity = rb.velocity;
        rbVelocity.y = 0f; rbSpeed = rbVelocity.magnitude;

        //If rigidbody speed is larger than movespeed, cancel out the input so you don't go over max speed
        if (currentInput.xyMove != Vector2.zero || !isGrounded)
        {
            if (rbSpeed < moveSpeed) CM = 1; //if not at max speed apply no countermovement
            else if (rbSpeed > overSpeed) CM = 0; // max countermovement if over the max overspeed
            else if (rbSpeed > moveSpeed) CM = 1 - ((rbSpeed / moveSpeed)); //slow countermovement when in overspeed
            else CM = Mathf.MoveTowards(CM, 0, Time.deltaTime); //slow when not giving input
        }
        else { CM = 1; inOverSpeed = false; } //resets countermovement if at a deadstop (No input)
    }
    void SpeedControl()
    {   //controls the player speed
        float speed = walkSpeed, OverSpd = 1; //set default speed 
        CharacterState.inSprint = inSprint || sprintToggle;

        if (inOverSpeed) { speed = sprintSpeed; OverSpd = overSpeedMod; } //change to overspeed
        else if (inCrouch && !inSlide) speed = crouchSpeed; //change to crouch speed
        else if (inProne) speed = proneSpeed; // change to prone speed
        else if (inSprint || sprintToggle) speed = sprintSpeed; // change to sprint speed

        moveSpeed = Mathf.MoveTowards(moveSpeed, currentInput.xyMove.normalized.magnitude * (speed), (acceleration / OverSpd) * Time.deltaTime);
    }
    void Sprint()
    {
        if (!canStand) { inSprint = false; return; }
        else if (currentInput.standingPos != standingPosition.standing) stand();
        inSprint = true;
    }
    void SprintToggle()
    {
        if (!canStand) { sprintToggle = false; return; }
        sprintToggle = !sprintToggle;
        if (currentInput.standingPos != standingPosition.standing && sprintToggle) stand();
    }
    void SprintCancelled()
    {
        inSprint = false;
        sprintToggle = false;
    }
    void SprintUpdate() //Fixes issues with holding sprint while toggling between different speeds e.g going from crouch to standing while holding sprint
    {   if ((inSprint || sprintToggle) && !inProne && !inCrouch) { moveSpeed = sprintSpeed; } }
    #endregion

    #region Crouch/prone/dolphindive
    public void ProneSlideSwitch() //Switch between Slide, p  rone, and dolphindive functions
    {   
        if (inSprint)
        {
            if (isGrounded && !inCrouch && !inProne && !inSlide && !slideCoolDownBool && rb.velocity.y <= 0f) { Slide(); }
            else if (inJump) DolphinDive();
        }        
        else if (!inSlide) { Prone(); }
    }
    private void changePosition() //moves player between standing/crouch/prone
    {
        if (inSlide) return;
        switch (currentInput.standingPos)
        {
            case standingPosition.standing: //Jump pressed
                if (canStand) { stand(); break; }
                if (canCrouch) { crouch(); break; }
                break;
            case standingPosition.crouch: //crouch pressed 
                if (inProne && canCrouch) { crouch(); break; }
                if (!inProne && !inCrouch) { crouch(); break; }
                if (inCrouch && canStand) { stand(); break; }
                break;
            case standingPosition.prone: //prone pressed
                ProneSlideSwitch();
                break;
        }
        SprintUpdate(); //updates movespeed to sprint if needed
    }
    void stand()
    {
        currentInput.standingPos = standingPosition.standing;
        inProne = false; inCrouch = false;
        moveCamPos = standingCamPos;
    }
    void crouch()
    {
        currentInput.standingPos = standingPosition.crouch;
        inProne = false; inCrouch = true;
        moveCamPos = crouchCamPos;
    }
    private void Prone()
    {
        currentInput.standingPos = standingPosition.prone;
        inProne = true; inCrouch = false;
        moveCamPos = proneCamPos;
    }
    private void DolphinDive()
    {   
        Prone();
        rb.AddForce(orientation.forward * jumpForce, ForceMode.Force); //apply forward force
    }
    void ColliderUpdateV2()
    {
        //Change physics materials when not moving so that you don't slide down slopes
        if (isGrounded && !inJump && currentInput.xyMove == Vector2.zero) PlayerCollider.material = SlopeMaterial; //change this material to custom if wanting to prevent sliding further
        else PlayerCollider.material = PlayerMaterial; //default player material for movement and air
                
        if (Physics.SphereCast(transform.position + new Vector3(0, -1.2f + colliderRadius, 0), colliderRadius, transform.up, out RaycastHit hit, (transform.localScale.y * 2f) + 1f, groundLayer))
        {
            //Debug.Log(hit.distance);
            canStand = hit.distance >= colliderStandingHeight;
            canCrouch = hit.distance >= colliderCrouchHeight;
        }
        else { canCrouch = true; canStand = true; }

        //Can swap for switch statement but will require adding constants instead of standingCamPos etc..
        float offset = 0.00f; //moves the collider slightly to match the character model better
        if (moveCamPos == standingCamPos)
        {
            PlayerCollider.height = colliderStandingHeight;
            PlayerCollider.center = new Vector3(0, offset, 0);
        }
        else if (moveCamPos == crouchCamPos)
        {
            float pos = (colliderCrouchHeight - colliderStandingHeight) / 2f;
            PlayerCollider.height = colliderCrouchHeight;
            PlayerCollider.center = new Vector3(0, pos + offset, 0);
        }
        else if (moveCamPos == proneCamPos)
        {
            float pos = (colliderProneHeight - colliderStandingHeight) / 2f;
            PlayerCollider.height = colliderProneHeight;
            PlayerCollider.center = new Vector3(0, pos + offset + 0.07f, 0); //add extra 0.07f since collider shifts for some reason
        }
    }
    #endregion

    #region Sliding and slideMovement    
    void SlideCheck()
    {
        if (isGrounded && !inCrouch && !inProne && !slideCoolDownBool && rb.velocity.y <= 0.5f)
        { Slide(); }
    }
    public void Slide() //triggers the slide bool to start slide movement in update as well as sets some parameters for it
    {
        doubleJump = false;
        slideMove = moveDirection;
        slideTimer = slideTime;
        inSlide = true;
        fallSlide = false;
        moveCamPos = crouchCamPos;
    }
    void FallSlide() //triggers a slide when the player is on a surface greater than the max angle
    {
        slideMove = slopeHit.normal;
        fallSlide = true;
        inSlide = true;
        rb.AddForce(-orientation.up * 10f, ForceMode.Force);
    }
    void SlideMovement() //performs slide movement and ends sliding when conditions are met
    {   
        slideMove = slideMove.normalized; slideMove.y = 0f;
        if (fallSlide) slideMove += (moveDirection.normalized * 0.1f); slideMove.y = 0f;
        rb.AddForce(slideMove * slideForce, ForceMode.Acceleration); //Add slide forward force
        rb.AddForce((-orientation.up - slopeHit.normal).normalized * fallSlideForce, ForceMode.Acceleration); //Add slide down force

        if (!fallSlide) //conditions to end normal slide
        {
            if (doubleJump) { slideJump = true; StopSlide(); } //if double jump, jump instantly and cancel slide
            if (slideTimer <= 0f || !isGrounded || rbSpeed < 1.75f || ((angle > slideAngle) && rb.velocity.y >= -0.2f)) //slide end detection
            { StopSlide(); }
        }
        else //conditions to end fall slide (steep slopes)
        {
            if ((rb.velocity.y > 0f || rbSpeed < 1.75f || isGrounded || !onSlope())) //detection for ending fallSlides
            { StopSlide(); }
        }
    }
    void StopSlide() //stops slide movement
    {
        if (!fallSlide)
        {
            slideCoolDownTimer = slideCoolDown;
            slideCoolDownBool = true;
            if (slideJump) { moveCamPos = standingCamPos; Jump(); }
            else crouch();
        }
        inSlide = false;
        fallSlide = false;
        slideJump = false;
    }
    #endregion
        
    #region Jump
    public void OnInputJump() //triggers checks using Jump
    {
        if (inSlide) { slideJump = true; return; }
        //stand if in crouch or prone
        if (moveCamPos != standingCamPos) { changePosition(); return; }
        //start jump
        if (isGrounded && !inCrouch && !inProne && !CharacterState.inWallRun) { Jump(); }
    }
    void Jump()
    {   //performs jump
        Vector3 jumpVector = new Vector3(0, jumpForce * CharacterState.gravityDir, 0);
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(jumpVector, ForceMode.VelocityChange);
        inJump = true;
        StartCoroutine(ResetJump());
    }
    void DoubleJump()
    { doubleJump = true; }
    IEnumerator ResetJump() //Prevents jumping until player has landed
    {   
        //Debug.Log("jump");
        yield return new WaitForSeconds(0.04f); //wait 2 frames
        yield return new WaitUntil(() => isGrounded);

        //Debug.Log("landed");
        inJump = false;
        doubleJump = false;
        slideJump = false;
    }
    #endregion

    #region Other Functions
    void BroadcastLocation() //broadcasts the player location for other scripts;
    { BroadcastMessage("PlayerLocation", transform.position); }
    void Camera() //moves the camera and applies its rotation
    {
        //Lerp to standing/crouch/prone Camera position
        cameraPosition.localPosition = Vector3.MoveTowards(cameraPosition.localPosition, new Vector3(0, moveCamPos, 0), 7.5f * Time.deltaTime);

        //clamp Camera X rotation
        xyRotation.x = Mathf.Clamp(xyRotation.x, -90f, 90f);
        //Apply rotations
        CameraRig.transform.localRotation = Quaternion.Euler(xyRotation.x, 0f, 0f);
        orientation.transform.localRotation = Quaternion.Euler(0f, xyRotation.y, 0f);
        CharacterModel.transform.localRotation = Quaternion.Euler(0f, xyRotation.y, 0f);
    }
    #endregion
}