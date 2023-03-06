using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class CharacterMovementV2 : MonoBehaviour
{
    //------Variables------\\
    #region Variables

    //Change height by adding an offset to these values
    //Camera position
    [Header("Set Camera Positions")]
    private float proneCamPos = -0.5f;
    private float standingCamPos = 0.9f;
    private float crouchCamPos = 0.15f;

    //Collider posistion
    [Header("Set Collider Positions")]
    private float ColliderStandingCenter = 0.01625f;
    private float ColliderCrouchCenter = -0.2453436f;
    private float ColliderProneCenter = -0.4178125f;
    //Collider Height
    [Header("Set Collider Heights")]
    private float colliderStandingHeight = 2.008125f;
    private float colliderCrouchHeight = 1.484938f;
    private float colliderProneHeight = 1.14f;

    [Header("Animation")]
    private Animator AnimCon;
    private float LastxyMove;

    [Header("Velocity")]
    [SerializeField] private float rbVelocity_Y;
    private float lastYPos;
    [SerializeField] private float rbSpeed;

    [Header("Ability limits")]
    [SerializeField] private float maxWallJumps = 3f;
    [SerializeField] private float no_wallJumps;
    [SerializeField] private float slideCoolDown = 5f;
    [SerializeField] private float slideCoolDownTimer;
    [SerializeField] private bool slideCoolDownBool = false;
    [SerializeField] private float maxGrapples = 3f;
    [SerializeField] private float no_Grapples;
    [SerializeField] private float grappleHookCoolDown = 6f;
    [SerializeField] private float grappleHookCoolDownTimer;
    private bool StartGrappleTimer = false;
    private bool inGrapple = false;
    public bool grappleCheck = false;
    public bool canGrapple = true;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private float walkSpeed = 8f;
    private float crouchSpeed = 5f;
    private float proneSpeed = 2.5f;
    private float sprintSpeed = 18f;
    private float overSpeed = 20f;
    private float airSpeed = 0.2f;
    [SerializeField] private float acceleration = 16f;
    private float moveMultiplier = 6.81818465f;
    private Vector2 xyMove;
    private Vector2 xyMoveJoystick;
    private bool useJoystick = false;
    [SerializeField] private bool inputSprint = false;
    private Vector3 moveDirection;
    private Vector3 slopeMoveDirection;
    private Vector3 slopeGravityDirection;

    [Header("CounterMovement")]
    [SerializeField] private float CM = 1f;
    private bool inOverSpeed = false;
    private float overSpeedMod = 2.5f;

    [Header("WallRunning")]
    [SerializeField] public bool inWallRun = false;
    [SerializeField] private bool allowWallRun = true;
    private float wallRunningDistance = 0.6f;
    private float minimumJumpHeight = 1.5f;
    private float wallRunGravity = 6.81f;
    private float wallForce = 6f;
    private float wallRunTime = 1.5f;
    private float wallRunFallTime = 0.75f;
    private float lastPos;
    [SerializeField] private float wallRunTimer;
    [SerializeField] private float wallRunFallTimer;
    [SerializeField] private bool velocityReset, wallCheck, touchingWall;
    [SerializeField] public bool wallLeft, wallRight;
    private RaycastHit wallLeftHit, wallRightHit;
    private Transform wallObject;
    private float positionY;
    [SerializeField] private Transform lastWallObject;

    [Header("Grounded")]
    [SerializeField] public bool isGrounded;
    [SerializeField] private bool isGroundedRC;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 RCVector;
    private float groundDistance = 0.52f;
    //private float playerHeight = 2f;
    [SerializeField] private float maxSlopeAngle = 35f;
    [SerializeField] private float wallAngle = 75f;
    [SerializeField] private float angle;
    [SerializeField] private float angleCP;
    private bool useSlopeHit = false;
    [SerializeField] private float lastAngle;
    private Vector3 lastMoveDirection;
    [SerializeField] private bool directionHitSlope;
    [SerializeField] private bool groundToSlopeRC;
    //[SerializeField] private float angleRC;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 20f;
    //[SerializeField] private float jumpDownForce = 12f;
    [SerializeField] private bool isGroundedJump = false;
    [SerializeField] private bool isGroundedLatch = false;
    private float jumpGroundDistance = 0.52f;
    [SerializeField] private bool inputJump, wallJump, grappleWallRun;
    [SerializeField] private bool inJump = false;
    [SerializeField] private bool doubleJump = false;
    private Vector3 jumpVelocity;
    [SerializeField] private float jumpSpeed;

    [Header("Drag")]
    private float groundDrag = 6f;
    private float airDrag = 0.15f;

    [Header("Gravity")]
    [SerializeField] private float downForce = 16f;
    private bool useDownForce = true;
    [SerializeField] private float dir = 1f;

    [Header("Crouch")]
    [SerializeField] Transform cameraPosition;
    private bool inputCrouch = false;
    [SerializeField] private bool inCrouch = false;
    [SerializeField] private bool inProne = false;
    [SerializeField] private Vector2 crouchLatch = new Vector2(0,0);
    [SerializeField] private bool changePos, canStand;
    private float moveCamPos;
    private float crouchToJumpTime = 0.05f;

    [Header("Sliding")]
    [SerializeField] private float slideForce = 75f;
    [SerializeField] private float slideTime = 0.75f;
    [SerializeField] private float slideTimer = 0f;
    [SerializeField] private float slideAngle = 20f;
    [SerializeField] private float slideHoldTime = 0.025f;
    private Vector3 slideMove;
    [SerializeField] public bool inSlide = false;
    private bool slideJump = false;
    [SerializeField] private bool fallSlide = false;

    [Header("AntiGravity")]
    [SerializeField] private float antiGravityCeilingLimit = 100f;
    [SerializeField] private float antiGravityFlipTime = 0.75f;
    [SerializeField] private float antiGravityCoolDown = 2f;
    [SerializeField] private float antiGravityCoolDownTimer;
    [SerializeField] private bool inAntigravity = false;
    [SerializeField] private bool AGcoolDownBool = false;
    private float AG = 1f;
    private float antiGravityRotate;
    private bool flippingGravity = false;

    [Header("Camera")]
    [SerializeField] Transform CameraRig;
    [SerializeField] Transform orientation;
    [SerializeField] public Vector2 xySensitivity, xyRotation;
    [SerializeField] private Vector2 xyLook;
    private float multiplier = 0.01f;

    //other variables
    public InputManager Controls;
    PlayerInput playerInput;
    Rigidbody rb;
    RaycastHit slopeHit;
    RaycastHit SphereHit;
    ContactPoint contactPoint;
    Collision contact;
    [SerializeField] private float slopeThreshold = 0.5f;
    public bool hideCursor = false;
    string lastScheme;

    //scripts
    WeaponAndItemScript GGScript;
    GameDataFile GDFile;

    [Header("Collider")]
    [SerializeField] private Transform CharacterModel;
    [SerializeField] private PhysicMaterial PlayerMaterial;
    [SerializeField] private PhysicMaterial SlopeMaterial;
    private CapsuleCollider PlayerCollider;
    [SerializeField] private float ColliderCenter;
    [SerializeField] private float Radius = 0.58f; //Radius of the collider

    //Cursor lock
    private bool isLocked = false;
    int SkipFrames = 1000;
    int CurrentFrame = 0;


    #endregion

    //------Functions------\\
    #region StandardFunctions
    private void Start()
    {   //Performs functions before the first frame

        //Fixes cursor in place and hides it
        //Cursor.lockState = CursorLockMode.Locked;
        //isLocked = true;

        //Set variables for start of game
        lastPos = transform.position.y;
        moveSpeed = walkSpeed;
        xySensitivity = new Vector2(12, 6);
        moveCamPos = standingCamPos; //sets camera position at start of game
        grappleHookCoolDownTimer = grappleHookCoolDown;
        slideCoolDownTimer = slideCoolDown;
        no_Grapples = 0f;
        no_wallJumps = 0f;
    }
    private void Awake()
    {   //Pefroms functions on the first frame


        //Rigidbody
        rb = GetComponent<Rigidbody>();
        GDFile = FindObjectOfType<GameDataFile>();
        GGScript = GetComponent<WeaponAndItemScript>();
        PlayerCollider = CharacterModel.GetComponent<CapsuleCollider>();
        PlayerCollider.height = colliderStandingHeight;
        PlayerCollider.center = new Vector3(0, ColliderStandingCenter, 0);
        rb.freezeRotation = true;
        //Input
        //InputSystem.pollingFrequency = 120;
        Controls = new InputManager();
        playerInput = GetComponent<PlayerInput>();
        lastScheme = playerInput.currentControlScheme;
        DefaultInput();
        //Animator
        AnimCon = GetComponent<Animator>();

        //Set variables from GameDataFile
        //playerHeight = GDFile.player_Height;

        //Movement
        walkSpeed = GDFile.Player_walkSpeed;
        crouchSpeed = GDFile.Player_crouchSpeed;
        proneSpeed = GDFile.Player_proneSpeed;
        sprintSpeed = GDFile.Player_sprintSpeed;
        overSpeed = GDFile.Player_overSpeed;
        airSpeed = GDFile.Player_airSpeed;
        acceleration = GDFile.Player_acceleration;
        maxSlopeAngle = GDFile.Player_maxSlopeAngle;
        //Jump
        jumpForce = GDFile.Player_jumpForce;
        //Drag
        groundDrag = GDFile.Player_groundDrag;
        airDrag = GDFile.Player_airDrag;
        //gravity
        downForce = GDFile.Player_downForce;

        Vector3 contactPoint = orientation.up;

        //Abilities
        //AnitiGravity
        antiGravityCeilingLimit = GDFile.Player_antiGravityCeilingLimit;
        antiGravityFlipTime = GDFile.Player_antiGravityFlipTime;
        antiGravityCoolDown = GDFile.Player_antiGravityCoolDown;
        //Wallrun
        maxWallJumps = GDFile.Player_maxWallJumps;
        //Slide
        slideCoolDown = GDFile.Player_slideCoolDown;
        slideForce = GDFile.Player_slideForce;
        slideTime = GDFile.Player_slideTime;
        slideAngle = GDFile.Player_slideAngle;
        slideHoldTime = GDFile.Player_slideHoldTime;
        //GrapplingHook
        maxGrapples = GDFile.Player_maxGrapples;
        grappleHookCoolDown = GDFile.Player_grappleHookCoolDown;
    }
    private void Update()
    {   //performs updates at a frame dependent rate
        //if (SkipFrames == CurrentFrame)
        //{
            if (playerInput.currentControlScheme == "Keyboard & Mouse") { hideCursor = false; }
            else { hideCursor = true; }
            CursorLockAndUnlockState();
            //CurrentFrame = SkipFrames;
        //}
        //else CurrentFrame--;

        if (GDFile.isPaused) return;

        AnimationManager();
        AntiGravityUpdate();
        AbilityLimiter();
        onSlope();
        angleDetection();

        if (GGScript.AscendGrapple) { useDownForce = false; } //turn off downforce while using grapple
        else if (!useDownForce) useDownForce = true;

        TimerUpdate();
        GroundDetection();
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);//find moveDirection when player is on a slope 
       
        MyInput(); //Get player input
        ControlDrag(); //set rb drag
        ColliderUpdate(); //set Collider size when crouch/prone/standing
        Crouch(); //move player
        SpeedControl(); //changes player speed
        SprintUpdate(); //Corrects sprint speed when holding sprint between crouch/prone/standing
        if ((inputSprint && xyMoveJoystick.y < 0.35f && xyMoveJoystick.y > 0.01f)) { inputSprint = false; } //stop sprint if character slows down to near stop
        if(!inSlide) Countermovement();
        wallRunManager();
        SlopeJump();
    }
    private void FixedUpdate()
    {   //Performs updates at a time dependent rate

        //stick to the ground when going over a slope to prevent flying over the slope(((FIX ISSUE))) causes issue when on ramp and when u jump it pushes u down quicker
        if (!onSlope() && isGrounded && !inWallRun && !inGrapple && !inJump && !inputJump && rbSpeed >= sprintSpeed)
        {
            //Debug.Log("going off");
            //float speed = Mathf.MoveTowards(rb.velocity.y, -15f, 50f * Time.deltaTime);
            //rb.velocity = new Vector3(rb.velocity.x, speed, rb.velocity.z);
            //rb.AddForce(-orientation.up * 2f, ForceMode.VelocityChange);
        }

        //Y Velocity using formula
        if (inAntigravity) rbVelocity_Y = (lastYPos - transform.position.y) / Time.deltaTime;
        else rbVelocity_Y = (transform.position.y - lastYPos) / Time.deltaTime;
        lastYPos = transform.position.y;

        //AntiGravity
        if (AG == -1f)
        {
            Vector3 Antigravity = -Physics.gravity;
            rb.AddForce(Antigravity);
        }
        Movement();
        if (isGroundedJump && inputJump && !inCrouch && !inProne && !inSlide && !inWallRun && !wallJump) { inputJump = false; Jump(); }
        if (inSlide && isGrounded) SlideMovement();
        if (inWallRun) WallRunMovement();

        //Additional Gravity
        if (!isGroundedJump && useDownForce)
        {
            Vector3 gravity = new Vector3(0, -downForce * AG, 0);
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
        //else if (isGrounded) useDownForce = true;
    }
    private void LateUpdate()
    {
        Camera(); //player and camera rotations 
    }
    #endregion

    #region Input
    private void OnEnable() { Controls.Menu.Enable(); Controls.Default.Disable(); } //Enables user input 
    private void OnDisable() { Controls.Default.Disable(); Controls.Menu.Disable(); } //Disables user input

    public void PauseCharacterInput()
    {
        if (GDFile.isPaused) { Controls.Default.Disable(); Controls.Menu.Enable(); playerInput.SwitchCurrentActionMap("Menu"); }
        else { Controls.Menu.Disable(); Controls.Default.Enable(); playerInput.SwitchCurrentActionMap("Default"); }
        //Debug.Log(playerInput.currentActionMap);
    }
    void DefaultInput()
    {   //Set default input at start of game

        //Player (Default) Controls
        Controls.Default.Move.performed += ctx => { useJoystick = false; xyMove = ctx.ReadValue<Vector2>(); };
        //Controls.Default.MoveJoystick.performed += ctx => xyMoveJoystick = ctx.ReadValue<Vector2>();
        Controls.Default.Move.canceled += ctx => xyMove = Vector2.zero;
        Controls.Default.Look.performed += ctx => { xyLook = ctx.ReadValue<Vector2>(); if (playerInput.currentControlScheme != "Keyboard & Mouse") { xyLook *= Time.unscaledDeltaTime * 100f; } };
        Controls.Default.Jump.performed += ctx => inputJump = true;
        Controls.Default.Jump.canceled += ctx => { inputJump = false; /*AllowCrouchJump = true;*/ };
        //Controls.Default.JumpHeld.performed += ctx => inputJump = false; //Stops continuous jumping
        Controls.Default.Sprint.performed += ctx => { if (canStand) { moveCamPos = standingCamPos; inputSprint = true; } };
        Controls.Default.SprintToggle.performed += ctx => { if (canStand) { inputSprint = !inputSprint; if (inputSprint) { moveCamPos = standingCamPos; } } }; //toggle sprint on/off for joysticks
        Controls.Default.Sprint.canceled += ctx => inputSprint = false;
        Controls.Default.Crouch.performed += ctx => { inputCrouch = true; inCrouch = true; crouchLatch.x = 1f; };
        Controls.Default.Crouch.canceled += ctx => { inputCrouch = false; crouchLatch.y = crouchLatch.x; crouchLatch.x = 0f; };
        Controls.Default.Prone.performed += ctx => { inputCrouch = false; ProneSlideSwitch(); };
        Controls.Default.InstantProne.performed += ctx => { if (inputSprint && inJump) { DolphinDive(); } else if (!inSlide && !inputSprint) Prone(); }; //Allows remapping crouch on keyboard
        Controls.Default.Slide.performed += ctx => { inputCrouch = false; if (isGroundedJump && !inCrouch && !inProne && !slideCoolDownBool && (angle < slideAngle || rb.velocity.y <= 0f)) { Slide(); } }; //allows remapping slide on keyboard
        Controls.Default.DoubleJump.performed += ctx => doubleJump = true;
        Controls.Default.Item4.performed += ctx => { if (!AGcoolDownBool) { AntiGravity(); } };
        Controls.Default.Select.performed += ctx => { CursorLockAndUnlockState(); isLocked = !isLocked; };
        Controls.Default.Start.performed += ctx => { if (GDFile.MenuContainer.GetComponent<MainMenuScript>().inGame) GDFile.PauseGame(); };
        Controls.Default.Item1.performed += ctx => ScreenSize();
        Controls.Default.Message.performed += ctx => { GDFile.MenuContainer.GetComponent<MainMenuScript>().ShowMesssageGUI(); ; };

        //Menu Controls
        Controls.Menu.Resume.performed += ctx => { GDFile.MenuContainer.GetComponent<MainMenuScript>().ResumeGame(); };
        Controls.Menu.Cancel.canceled += ctx => { GDFile.MenuContainer.GetComponent<MainMenuScript>().CancelBackSwitch(); };
        Controls.Menu.LeftTab.performed += ctx => { GDFile.MenuContainer.GetComponent<MainMenuScript>().LeftTab(); };
        Controls.Menu.RightTab.performed += ctx => { GDFile.MenuContainer.GetComponent<MainMenuScript>().RightTab(); };
        Controls.Menu.Message.performed += ctx => { GDFile.MenuContainer.GetComponent<MainMenuScript>().ShowMesssageGUI(); };
        Controls.Menu.Submit.performed += ctx => { if (GDFile.MenuContainer.GetComponent<MainMenuScript>().MessageContainer.activeSelf) { GDFile.MenuContainer.GetComponent<MainMenuScript>().EnterMessage(); } };
    }
    #endregion

    #region Animation
    public class animData
    {
        public bool jump;
        public bool doubleJump;
        public bool crouch;
        public bool prone;
        public bool sStop;
        public float x;
        public float y;
        public float LastxyMove;
    }
    private void AnimationManager()
    {   //Handles any animation

        AnimCon.SetBool("DoubleJump", doubleJump);
        AnimCon.SetBool("Jump", inJump);
        if (xyMove.y > 0f)
        { AnimCon.SetFloat("X", (moveSpeed / sprintSpeed)); }
        else if (xyMove.y < 0f)
        { AnimCon.SetFloat("X", (moveSpeed / sprintSpeed) * -1f); }
        else AnimCon.SetFloat("X", Mathf.MoveTowards(moveSpeed, 0f, 250000f));
        AnimCon.SetBool("Crouch", inCrouch);// if (inCrouch) else AnimCon.SetBool("Crouch", false);
        AnimCon.SetBool("Prone", inProne);//if (inProne)  else AnimCon.SetBool("Prone", false);
        if (Mathf.Abs(LastxyMove) > 0f && xyMove.y == 0f && inputSprint) AnimCon.SetTrigger("SuddenStop");

        LastxyMove = xyMove.y;
        AnimCon.SetFloat("LastxyMove", LastxyMove);
    }
    public List<bool> GetAnimDataBoolList()
    {
        List<bool> boolList = new List<bool>();
        boolList.Add(inJump);
        boolList.Add(doubleJump);
        boolList.Add(inCrouch);
        boolList.Add(inProne);

        bool sStop;
        if (Mathf.Abs(LastxyMove) > 0f && xyMove.y == 0f && inputSprint) sStop = true;
        else sStop = false;
        boolList.Add(sStop);

        return boolList;
    }
    public List<float> GetAnimDataFloatList()
    {
        List<float> floatList = new List<float>();

        if (xyMove.y > 0f)
        { floatList.Add(moveSpeed / sprintSpeed); }
        else if (xyMove.y < 0f)
        { floatList.Add((moveSpeed / sprintSpeed) * -1f); }
        else floatList.Add(0f);

       LastxyMove = xyMove.y;
        floatList.Add(LastxyMove);

        return floatList;
    }
    #endregion

    #region Ground/Floor/slope/anlge detection
    private void OnCollisionStay(Collision collision)
    {   //returns the objects the player collides with (used to detect the floor)
        //contactPoint = collision.contacts[0];
        contact = collision;
    }
    private void OnCollisionExit(Collision collision)
    { contact = null; }
    private bool onSlope()
    {   //Checks if the player is stood on a slope or not

        findFloor();
        if (slopeHit.normal != Vector3.up) { return true; }
        else { return false; }
    }
    private bool angleDetection()
    {   //Checks if the floor angle is less than the max floor angle 

        findFloor();
        angle = Mathf.Abs(Vector3.Angle(orientation.up, slopeHit.normal)); //finds angle based on raycasts
        if (angle <= maxSlopeAngle + 0.5f) { return true; } //returns true if the floor is walkable
        else { return false; } // returns false if the floor is not walkable
    }
    private void findFloor()
    {   //Finds the surface the player is stood on

        if(Physics.SphereCast(transform.position, 0.56f, -orientation.up, out SphereHit, 1.5f, groundLayer)) //Detect the point the player is touching
        { Physics.SphereCast(SphereHit.point + new Vector3(0, 2f, 0), 0.3f, -orientation.up, out slopeHit, 5f, groundLayer); } //perform a raycast from above the hitpoint so the normal is flat and not angled
    }
    private void GroundDetection()
    {   //Ground Detection

        groundToSlopeRC = Physics.Raycast(transform.position - new Vector3(0, 0.6f, 0), -orientation.up, jumpGroundDistance, groundLayer);//sets ground when at the edge of a steep slope to prevent sliding down constantly while walking into it
        if (angleDetection())
        {
            isGrounded = Physics.CheckSphere(transform.position - (new Vector3(0, 0.5f, 0) * AG), groundDistance, groundLayer); //Check if player is on Ground. Give a larger distance to prevent falling while going over small bumps
            if (isGrounded && !isGroundedLatch) isGroundedJump = isGrounded;
            else isGroundedJump = false;
            isGroundedRC = Physics.Raycast(transform.position - (new Vector3(0, 0.5f, 0) * AG), -orientation.up, jumpGroundDistance + 0.2f, groundLayer); //extra ground detection using a raycast. This is not needed with a collider larger than the player
            if (Physics.CheckSphere(transform.position - (new Vector3(0, 0.5f, 0) * AG), jumpGroundDistance, groundLayer)) isGroundedLatch = false;
        }
        else if (angle > maxSlopeAngle && angle <= wallAngle) 
        {
            StartCoroutine(SlopeSlideDelay()); 
        }
        else
        {
            isGrounded = false;
            isGroundedJump = false;
            isGroundedRC = false;
        }
    }
    #endregion

    #region Ability controls
    void AbilityLimiter()
    {   //sets the limits for player abilities e.g max wall runs

        if(AGcoolDownBool) antiGravityCoolDownTimer -= Time.deltaTime;
        if (AGcoolDownBool && antiGravityCoolDownTimer <= 0) AGcoolDownBool = false;
        if (no_wallJumps == maxWallJumps) { inputJump = false; StopWallRun(); } //inputJump = false (fixes glitch of jumping up after a wall jump)
        if (isGroundedRC || GGScript.isGrappling) no_wallJumps = 0f;

        if (slideCoolDownBool) slideCoolDownTimer -= Time.deltaTime;
        if (slideCoolDownTimer <= 0f) { slideCoolDownBool = false; slideCoolDownTimer = slideCoolDown; }

        if (GGScript.isGrappling && !inGrapple) { inGrapple = true; StartGrappleTimer = true; canGrapple = false; } //set variables for grapple and start timer
        if (!GGScript.isGrappling && inGrapple) { inGrapple = false; no_Grapples += 1f; } //reset variables when stop grappling
        if (StartGrappleTimer) grappleHookCoolDownTimer -= Time.deltaTime; //Timer
        if (grappleHookCoolDownTimer <= 0f) { StartGrappleTimer = false; grappleHookCoolDownTimer = grappleHookCoolDown; canGrapple = true; } //reset timer and allow next grapple
        if (no_Grapples >= maxGrapples) { canGrapple = false; /*Debug.Log("stopGrapple");*/ } // if used max number of grapples stop grapple working
        if (isGroundedRC && !GGScript.isGrappling) { no_Grapples = 0f; canGrapple = true; } //reset grapple when grounded and not in use
        //if (inWallRun) no_Grapples = 0f;
    }
    private void TimerUpdate()
    {   //Updates any timers

        //slide Timer
        if (inSlide && !fallSlide) slideTimer -= Time.deltaTime; //decrease timer if not on a slope
        //wallRun Timer
        if (inWallRun)
        {
            if (wallRunTimer > 0f) wallRunTimer -= Time.deltaTime; //decrease first timer if it has not ended
            else wallRunFallTimer -= Time.deltaTime; //decrease second (fall) timer 
        }
    }
    #endregion

    #region PlayerMovement/speedcontrol
    void MyInput()
    {   //PlayerMovement input
        
        //if (Mathf.Abs(xyMoveJoystick.magnitude) > 0.05f || useJoystick) { useJoystick = true; xyMove = xyMoveJoystick; }
        moveDirection = (orientation.transform.forward * xyMove.y) + (orientation.transform.right * xyMove.x) ;
        //CameraMovement Input
        xyRotation.x -= xyLook.y * xySensitivity.y * multiplier; //Left/Right
        xyRotation.y += xyLook.x * xySensitivity.x * multiplier; //Up/Down
    }
    void Movement()
    {   //moves the player **may need to add time.deltatime to addforce**

        if (isGrounded && !onSlope())
        {
            rb.AddForce(-orientation.up * 10f * Time.deltaTime);  //additional down force
            rb.AddForce(moveDirection.normalized * moveSpeed * moveMultiplier * CM, ForceMode.Acceleration); //Standard movement
        }
        else if (isGrounded && onSlope())
        {
            rb.AddForce(slopeGravityDirection * 10f * Time.deltaTime);  //additional down force
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * moveMultiplier * CM, ForceMode.Acceleration); //Slope movement
        }
        else rb.AddForce(slopeMoveDirection.normalized * moveSpeed * moveMultiplier * airSpeed * (CM + 0.0025f), ForceMode.Acceleration); //Air movement
    }
    void Countermovement()
    {   //changes the speed multiplyer to prevent going too far over maxspeed

        Vector3 rbVelocity = rb.velocity;
        rbVelocity.y = 0f; rbSpeed = rbVelocity.magnitude;

        //If rigidbody speed is larger than movespeed, cancel out the input so you don't go over max speed
        if ((!inSlide || !slideJump) && xyMove != Vector2.zero)
        {
            if (rbSpeed > moveSpeed + 0.5f && rbSpeed < overSpeed) { CM = 0.25f; inOverSpeed = true; } //Slow down countermovement at high speeds to allow faster movement
            else if (rbSpeed > moveSpeed) { CM = 0f; inOverSpeed = false;  } //limit speed if not yet above sprin+t speed
            else { CM = 1f; inOverSpeed = false; }
        }
        else { CM = 1f; inOverSpeed = false; }
    }
    void SpeedControl()
    {   //controls the player speed

        if (rbSpeed <= sprintSpeed)
        {
            if (inputSprint && !inCrouch && !inProne && !inOverSpeed && !inSlide) moveSpeed = Mathf.MoveTowards(moveSpeed, sprintSpeed, acceleration * Time.deltaTime); //change to sprint speed
            else if (inSlide) moveSpeed = Mathf.MoveTowards(moveSpeed, sprintSpeed, acceleration * Time.deltaTime); //change to sprint
            else if (inOverSpeed) { moveSpeed = Mathf.MoveTowards(moveSpeed, sprintSpeed, (acceleration / overSpeedMod) * Time.deltaTime); } //change to overspeed
            else if (inCrouch) moveSpeed = Mathf.MoveTowards(moveSpeed, xyMove.magnitude * crouchSpeed, acceleration * Time.deltaTime); //change to crouch speed
            else if (inProne) moveSpeed = Mathf.MoveTowards(moveSpeed, xyMove.magnitude * proneSpeed, acceleration * Time.deltaTime); // change to prone speed
            else moveSpeed = xyMove.magnitude * walkSpeed; //set default speed 
        }
    }
    void SprintUpdate()
    {   //Fixes issues with holding sprint while toggling between different speeds e.g going from crouch to standing while holding sprint

        if (inputSprint && !inProne && !inCrouch) moveSpeed = sprintSpeed;
    }
    #endregion

    #region Crouch/prone/dolphindive
    void ProneSlideSwitch()
    {   //Switch between Slide, prone, and dolphindive functions

        crouchLatch = Vector2.zero;
        if (inputSprint && isGroundedJump && !inCrouch && !inProne && !inSlide && !slideCoolDownBool && (angle < slideAngle || rb.velocity.y <= 0f)) { Slide(); }
        else if (inputSprint && inJump) DolphinDive();
        else if (!inSlide) { inProne = true; crouchLatch.x = 2f; Prone(); }
    }
    void Crouch()
    {   //Moves player between standing, crouch and prone

        if (inputJump && (moveCamPos != standingCamPos) && !inSlide && changePos ) //if press jump cancel crouch or prone and go to standing position
        {
            if (canStand) { moveCamPos = standingCamPos; }
            else { moveCamPos = crouchCamPos; }
            inputJump = false;
        }
        //Add !inJump to prvent crouching while jumping
        else if (crouchLatch.y == 1f && !inSlide /* && !inJump */) //check if pressed crouch (and released) **must have released or player wii stand before going prone
        {
            if (inProne && changePos) //if in prone and press crouch, go to crouch position
            { moveCamPos = crouchCamPos;
                /*Debug.Log("prone to crouch");*/
            }
            else if (moveCamPos == standingCamPos) //if standing and press crouch, go into crouch
            { moveCamPos = crouchCamPos;
                /*Debug.Log("standing to crouch");*/
            }
            else if (changePos) //if crouching and press crouch, go to standing position
            { moveCamPos = standingCamPos;
                /*Debug.Log("crouch to standing");*/
            }
        }
        crouchLatch.y = 0f; //reset input latch to prevent repeating the loop

        //Check if standing, and prevent jump if crouched or in prone
        if (standingCamPos <= (Mathf.Round(cameraPosition.transform.localPosition.y * 10) / 10) + crouchToJumpTime) { inCrouch = false; inProne = false; } //standing
        else if (proneCamPos >= (Mathf.Round(cameraPosition.transform.localPosition.y * 10) / 10) - crouchToJumpTime) { inProne = true; inCrouch = false; } // prone
        else { inCrouch = true; inProne = false; } //crouch    
    }
    void Prone()
    {   //sets the camera to prone position

        moveCamPos = proneCamPos;
    }
    void DolphinDive()
    {   //Performs dolphindive

        moveCamPos = proneCamPos;
        rb.AddForce(orientation.forward * jumpForce, ForceMode.Force); //apply forward force
    }
    void ColliderUpdate()
    {   //Moves the player collider when in crouch or prone, and changes the collider material

        //Change physics materials when not moving so that you don't slide down slopes
        if (isGroundedJump && (Mathf.Abs(xyMove.x) < 0.1f && Mathf.Abs(xyMove.y) < 0.1f)) PlayerCollider.material = SlopeMaterial; //change this material to custom if wanting to prevent sliding further
        else PlayerCollider.material = PlayerMaterial; //default player material for movement and air

        //Change Collider height and check if you can stand
        float checkHeight;
        if (inProne) { checkHeight = colliderCrouchHeight; }
        else { checkHeight = colliderStandingHeight; }
        Vector3 checkSpherePos = transform.position; //set player position
        checkSpherePos.y -= transform.localScale.y; //Remove player height to get bottom of player

        checkSpherePos.y += checkHeight - Radius; //Add collider height and remove radius of checkSphere
        changePos = !Physics.CheckSphere(checkSpherePos * dir, Radius, groundLayer); //Check if can go to crouch or standing

        checkSpherePos.y += colliderStandingHeight - checkHeight; //Remove the old collider height and add standing height
        canStand = !Physics.CheckSphere(checkSpherePos * dir, Radius, groundLayer); //Check if can stand
        //ColliderCenter = checkSpherePos.y; //Debug for position of checkBox

        if (moveCamPos == standingCamPos)
        {
            PlayerCollider.height = colliderStandingHeight;
            PlayerCollider.center = new Vector3(0, ColliderStandingCenter, 0);
        }
        else if (moveCamPos == crouchCamPos)
        {
            PlayerCollider.height = colliderCrouchHeight;
            PlayerCollider.center = new Vector3(0, ColliderCrouchCenter, 0);
        }
        else if (moveCamPos == proneCamPos)
        {
            PlayerCollider.height = colliderProneHeight;
            PlayerCollider.center = new Vector3(0, ColliderProneCenter, 0);
        }
    }
    #endregion

    #region Sliding and slideMovement
    private void SlopeJump()
    {   //if on a slope and jumping while not moving freeze thr rigidbody xz movement to prevent sliding

        if (onSlope() && !fallSlide && inJump && isGroundedJump && (xyMove == Vector2.zero || (Mathf.Abs(xyMoveJoystick.magnitude) <= 0.7f) && (Mathf.Abs(xyMoveJoystick.magnitude) > 0f)))
        {
            //Debug.Log("jumpy");
            // rb.velocity = new Vector3(0, 0, 0);
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

            // Set to Freeze All but Z position:
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationZ;
            // Or
            rb.constraints = RigidbodyConstraints.FreezeAll | ~RigidbodyConstraints.FreezePositionX | ~RigidbodyConstraints.FreezePositionZ;

            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
    IEnumerator SlopeSlideDelay()
    {
        inputJump = false;
        isGroundedJump = false;
        isGrounded = false;
        isGroundedRC = false;

        if (angle < 65f)
        {
            yield return new WaitForSeconds(slideHoldTime);
            StartCoroutine(SlopeSlide());
        }
        else rb.AddForce(-orientation.up * 4f);
    }
    IEnumerator SlopeSlide()
    {
        rb.AddForce(0, -10f, 0);
        fallSlide = true;
        yield return new WaitUntil(() => (rb.velocity.y < -0.001f || isGrounded || angle > maxSlopeAngle));
        inJump = false;
        FallSlide();
    }
    void Slide()
    {   //triggers the slide bool to start slide movement in update as well as sets some parameters for it

        doubleJump = false;
        slideMove = moveDirection;
        slideTimer = slideTime;
        inSlide = true;
        moveCamPos = crouchCamPos;
    }
    void FallSlide()
    {   //triggers a slide when the player is on a surface greater than the max angle

        rb.AddForce(orientation.up * -20, ForceMode.Force);
        if ((rb.velocity.y > 0f || rbSpeed < 1.75f || isGrounded) || !onSlope()) //detection for ending fallSlides
        {
            inSlide = false;
            fallSlide = false;
        }
    }
    void SlideMovement()
    {   //performs slide movement and ends sliding when conditions are met

        rb.AddForce(-orientation.up * 20, ForceMode.Force);
        Vector3 slideDirection = slideMove.normalized;
        slideDirection.y = 0f;
        slopeMoveDirection = Vector3.ProjectOnPlane(slideDirection, slopeHit.normal); //find moveDirection when player is on a slope 
        slopeGravityDirection = Vector3.ProjectOnPlane(-orientation.up, slopeHit.normal); //find moveDirection when player is on a slope 
        rb.AddForce(slideDirection * slideForce, ForceMode.Force); //Add slide force

        bool hitwall = Physics.CapsuleCast(CharacterModel.position, CharacterModel.position + CharacterModel.localScale, Radius, moveDirection, 1f); ;
        if (inputJump) { slideJump = true; } //if jump, slideJump when slide finishes
        if (doubleJump) { slideCoolDownBool = true; inSlide = false; moveCamPos = standingCamPos; Jump(); } //if double jump, jump instantly and cancel slide
        if (slideTimer <= 0f || !isGrounded || hitwall || rbSpeed < 1.75f || ((angle > slideAngle || angleCP > slideAngle) && rb.velocity.y >= -0.2f)) //slide end detection
        {
            slideCoolDownBool = true; //only start cooldown on user slide not a falling slide
            if (slideJump) { moveCamPos = standingCamPos; Jump(); } //perform jump at end of slide
            inSlide = false;
        }
    }
    #endregion

    #region Antigravity
    private void AntiGravityUpdate()
    {   //updates paramteres when in AntiGravity

        if (inAntigravity) dir = -1f;
        else dir = 1f;
        if (AG == -1f && rb.useGravity == true) rb.useGravity = false;
        if (AG == 1f && rb.useGravity == false) { rb.useGravity = true; }
        else if (AG == 0F && rb.useGravity == true) { rb.useGravity = false; }
    }
    private void AntiGravity()
    {   //Performs antigravity

        //Raycast to check if anti-Gravity flip is possible
        RaycastHit hit;
        if (Physics.Raycast(transform.position, orientation.up, out hit, antiGravityCeilingLimit, groundLayer))
        {
            AGcoolDownBool = true;
            antiGravityCoolDownTimer = antiGravityCoolDown;

            flippingGravity = true;
            Vector3 antiGravityPoint = hit.point;
            float antiGravityDistance = Vector3.Distance(transform.position, antiGravityPoint);
            //Changes variables depending on if character is on ceiling or ground
            if (antiGravityDistance < antiGravityCeilingLimit)
            {
                if (inAntigravity == true)
                {
                    //Debug.Log("normal gravity");
                    AG = 1f;
                    //rb.useGravity = true;
                    inAntigravity = false;
                    antiGravityRotate = -180;
                }
                else
                {
                    //Debug.Log("antigravity");
                    AG = -1f;
                    //rb.useGravity = false;
                    inAntigravity = true;
                    antiGravityRotate = 180;
                }
                //Transform position
                rb.AddForce(transform.up * dir *(antiGravityDistance / antiGravityFlipTime));
                //Rotate character
                StartCoroutine(Rotate(cameraPosition.right, antiGravityRotate, antiGravityFlipTime));
            }
        }
    }
    IEnumerator Rotate(Vector3 axis, float angle, float duration)
    {   //Rotates the character when antigravity is triggered

        transform.rotation = orientation.transform.rotation;
        Quaternion from = orientation.transform.rotation;
        Quaternion to = from;
        to *= Quaternion.Euler(angle * 0f, angle * 0f, angle * 1f);

        float elapsed = 0.0f;
        while (elapsed <= duration)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
        flippingGravity = false;
    }
    #endregion

    #region WallRunning
    private void wallRunManager()
    {   //deals with wallrun update functions

        if (GGScript.isGrappling)
        {
            if (inWallRun) StopWallRun(); //stop wall run if using grapplinghook
            grappleWallRun = true; //allow wall run after grappling
        }

        CheckWall();//check for walls
        if ((wallJump || grappleWallRun) && wallCheck && !GGScript.isGrappling) { wallJump = false; grappleWallRun = false; StartWallRun(); } //allow wall run when jumping to next wall or from grapling
        else if (isGroundedRC) StopWallRun();
        if (canWallRun() && wallCheck && (xyMove.x != 0f) && (xyMove.y > 0f) && !isGroundedJump && !inWallRun && allowWallRun) //start wall run 
        { allowWallRun = false; StartWallRun(); }
    }
    private void CheckWall()
    {   //Checks if the player can start a wall run, which side it should be on, and cancels it if a wall isnt detected

        Vector3 offset = new Vector3(0, 0.5f, 0);
        wallLeft = Physics.Raycast(transform.position - offset, -orientation.right, out wallLeftHit, wallRunningDistance * 2f);
        wallRight = Physics.Raycast(transform.position - offset, orientation.right, out wallRightHit, wallRunningDistance * 2f);
        if (wallRight) wallObject = wallRightHit.transform;
        if (wallLeft) wallObject = wallLeftHit.transform;
        touchingWall = Physics.CheckBox(transform.position - offset, new Vector3(wallRunningDistance, 0.25f, wallRunningDistance), transform.rotation, groundLayer);

        if (touchingWall && (wallLeft || wallRight)) wallCheck = true;
        else 
        { 
            wallCheck = false; 
            inWallRun = false;
            allowWallRun = true;
            if (AG == 0 && !inAntigravity) AG = 1;
            else if (AG == 0 && inAntigravity) AG = -1;
        }
    }
    private bool canWallRun() 
    {   //Checks if the player is far enough off the floor to start wallrunning

        return !Physics.Raycast(transform.position, -orientation.up, minimumJumpHeight); 
    }
    private void StartWallRun()
    {   //triggers the wallrun bool to start wallrun movement

        if (lastWallObject == null || lastWallObject != wallLeftHit.transform && lastWallObject != wallRightHit.transform)
        {
            wallRunTimer = wallRunTime;
            wallRunFallTimer = wallRunFallTime;
            wallJump = false;
            velocityReset = true;
            inputJump = false;
            inWallRun = true;
        }
        else { StopWallRun(); }
    }
    private void WallRunMovement()
    {   //performs wallrun movement

        if (rbVelocity_Y <= 0f && inWallRun == true && touchingWall) //Wait till at top of jump
        {
            if (velocityReset == true) { rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.25f, rb.velocity.z); } //reset velocity only 1 time
            useDownForce = false; /*rb.useGravity = false;*/ AG = 0f; velocityReset = false; //set wall run variables
            if (wallRunTimer <= 0f) { rb.AddForce(orientation.up * -wallRunGravity, ForceMode.Acceleration); } //apply downward force
            if (inputJump && touchingWall) { inWallRun = false; WallRunJump(); }
            if ( isGroundedRC || !wallCheck || Mathf.Abs(rbSpeed) <= 0.05f || xyMove.y <= 0f || wallRunFallTimer <= 0f) //stop wall run
            { StopWallRun();}
        }
        
        if (wallRight) rb.AddForce(orientation.right * wallForce, ForceMode.Force); //apply force toward wall to keep player against it
        if (wallLeft) rb.AddForce(-orientation.right * wallForce, ForceMode.Force);
        if(isGroundedRC) { StopWallRun(); /*Debug.Log("y u end?");*/ }

    }
    private void StopWallRun()
    {   //stops wallrun movement

        StartCoroutine(WallRunReset());
        inWallRun = false;
        wallJump = false;
        //Debug.Log("StopWallRun");
        //rb.useGravity = true;
        if (inAntigravity) AG = -1f;
        else AG = 1f;
        useDownForce = true;
    }
    private void WallRunJump()
    {   //performs jump off the wall when wallrunning

        //set last wall object
        no_wallJumps += 1f;
        if (wallLeft) lastWallObject = wallLeftHit.transform;
        else lastWallObject = wallRightHit.transform;

        //Debug.Log("wallJump");
        /*rb.useGravity = true;*/
        if (inAntigravity) { AG = -1f; }
        else { AG = 1f; }
        useDownForce = true;  //reset variables
        if (wallLeft) rb.AddForce(orientation.right * 10f, ForceMode.VelocityChange); //apply a force to push off the wall
        else rb.AddForce(-orientation.right * 10f, ForceMode.VelocityChange); 

        //Jump
        Vector3 jumpVector = new Vector3(0, jumpForce * AG, 0); //jump direction
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); //reset velocity
        rb.AddForce(jumpVector, ForceMode.VelocityChange); //apply jump force
        if(rbSpeed < sprintSpeed + 6f) rb.AddForce(orientation.forward * (jumpForce / 10f), ForceMode.VelocityChange); //apply forward force
        StartCoroutine(WallRunJumpCoolDown());
    }
    IEnumerator WallRunJumpCoolDown()
    {   //timer to prevent immediately jumping off a wall

        yield return new WaitForSeconds(0.25f); wallJump = true; 
    }
    IEnumerator WallRunReset()
    {   //allows wallrun again once player is on the ground

        yield return new WaitUntil(() => isGroundedJump); //wait untill landed
        allowWallRun = true;
        lastWallObject = null;
    }
    #endregion

    #region Jump
    void Jump()
    {   //performs jump

        inJump = true;
        isGroundedLatch = true;
        Vector3 jumpVector = new Vector3(0, jumpForce * dir, 0);
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(jumpVector, ForceMode.VelocityChange);
        StartCoroutine(InJumpCheck());
    }
    IEnumerator InJumpCheck()
    {   //Prevents jumping until player has landed

        yield return new WaitForFixedUpdate(); //wait untill falling
        yield return new WaitUntil(() => isGroundedJump); //wait untill landed
        //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        inJump = false;
        doubleJump = false;
        slideJump = false;
    }
    #endregion

    #region Other Functions
    void Camera()
    {   //moves the camera and applies its rotation

        //Lerp to standing/crouch/prone Camera position
        cameraPosition.localPosition = Vector3.MoveTowards(cameraPosition.localPosition, new Vector3(0, moveCamPos, 0), 7.5f * Time.deltaTime);

        //clamp Camera X rotation
        xyRotation.x = Mathf.Clamp(xyRotation.x, -90f, 90f);
        //Apply rotations
        CameraRig.transform.localRotation = Quaternion.Euler(xyRotation.x, 0f, 0f);
        orientation.transform.localRotation = Quaternion.Euler(0f, xyRotation.y, 0f);
        CharacterModel.transform.localRotation = Quaternion.Euler(0f, xyRotation.y, 0f);
    }
    void ControlDrag()
    {   //sets the player drag

        if (contact != null && isGroundedJump)
        {
            rb.drag = groundDrag;
            //if (contact.gameObject.layer == 8 && (angleCP <= maxSlopeAngle)) rb.drag = groundDrag;//if grounded and not jumping
            //else rb.drag = airDrag; //while jumping or falling
        }
        else { rb.drag = airDrag; }//while jumping or falling
    }
    public void CursorLockAndUnlockState()
    {   //Locks and hides the cursor if in game or not using keyboard & mouse 
        if (hideCursor || (!GDFile.isPaused && GDFile.MenuContainer.GetComponent<MainMenuScript>().inGame)) 
        { if (!isLocked) { isLocked = true;  Cursor.visible = false; } }

        //Frees cursor
        else if(isLocked) { isLocked = false; Cursor.visible = true; }
    }
    void ScreenSize()
    {   //TestFunction to change the screen resolution 

        if (!Screen.fullScreen)
        {
            // Switch to 1920 x 1080 full-screen
            Screen.SetResolution(1980, 1080, true);
            Debug.Log("FULLSCREEN");
        }
        else
        {
            // Switch to 640 x 480 Windowed
            Screen.SetResolution(640, 480, false);
            Debug.Log("NOTFULLSCREEN");
        }
    }
    void Dash()
    {   //performs a dash (not used)

        //Debug.Log("yes");
        rb.AddForce(CameraRig.forward * 125f, ForceMode.Impulse);
    }
    #endregion
}