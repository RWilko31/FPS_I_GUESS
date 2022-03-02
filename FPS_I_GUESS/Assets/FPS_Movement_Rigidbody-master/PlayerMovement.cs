// Some stupid rigidbody based movement by Dani
// Additions by Robert!

using System;
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    //Assingables
    public Transform playerCam;
    public Transform orientation;
    public Transform player;

    
    //Other
    private Rigidbody rb;
    private LineRenderer lr;

    //Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;
    public LayerMask whatIsObject;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    //Sprint
    public float sprintSpeed = 2.5f;

    //AntiGravity
    public float antiGravityCeilingLimit = 100f;
    public float antiGravityFlipTime = 0.75f;
    public float antiGravityCoolDown = 2f;
    public float antiGravityDistance;
    public bool antiGravityOff = true;
    private Vector3 playerZ;
    private Vector3 antiGravityPoint;
    private float antiGravityRotate;
    private bool useAntiGravityGravityScale = false;

    //LerpTime
    float lerpTime = 1f;
    float currentLerpTime;
    float perc;

    //Input
    float x, y;
    bool jumping, sprinting, crouching, sprint, antiGravity;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;
    private bool useMovement = true;
    private bool ctrlHeld = false;
    private bool ctrlLatch = false;

    //Wallrunning
    public LayerMask whatIsWall;
    public float wallRunGravity = 10f;
    public float WallRunFallTime = 5f;
    public float wallRunDistance = 1.5f;
    public float wallrunForce = 3000f;
    public float maxWallSpeed = 25f;
    public  bool isWallRight, isWallLeft;
    public bool isWallRunning;
    public float maxWallRunCameraTilt = 15f;
    public float wallRunCameraTilt;
    bool stillOnWall = false;

    public Vector2 magg;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    IEnumerator AntiGravityDelay()
    {
        yield return new WaitForSeconds(antiGravityFlipTime);
    }
    IEnumerator wallRunDelay()
    {
        yield return new WaitForSeconds(WallRunFallTime);
        rb.AddForce(-player.up * wallRunGravity * Time.deltaTime);
    }

    IEnumerator shortJumpDelay()
    {
        yield return new WaitForSeconds(0.25f);
        Vector3 PlayerVelocity = new Vector3(rb.velocity.x, rb.velocity.y * 0, rb.velocity.z);
        rb.velocity = PlayerVelocity;
    }

    IEnumerator wallRunEndDelay()
    {
        yield return new WaitForSeconds(1f);
        stillOnWall = false;
    }

    IEnumerator Rotate(Vector3 axis, float angle, float duration)
    {
        player.transform.rotation = orientation.transform.rotation;
        Quaternion from = orientation.transform.rotation;
        Quaternion to = from;
        to *= Quaternion.Euler(0f * angle, 0f * angle, 1f * angle);
       
        float elapsed = 0.0f;
        while (elapsed <= duration)
        {
            player.transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        player.transform.rotation = to;
    }

    private void FixedUpdate() {
        Movement();
        //Changes the gravity to stick to the ceiling
        if (useAntiGravityGravityScale)
        {
            rb.AddForce(Physics.gravity * (rb.mass * rb.mass) * -1f);
        }

    }

    private void Update() {
        MyInput();
        Look();
        CheckForWall();
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
        }
        perc = currentLerpTime / lerpTime;
        
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput() {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
        antiGravity = Input.GetKey(KeyCode.R);
        sprint = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.R))
            AntiGravity();

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl)) {ctrlHeld = true; ctrlLatch = !ctrlLatch; StartCrouch();}

        if (Input.GetKeyUp(KeyCode.LeftControl)) {ctrlHeld = false; StopCrouch(); }
    
        //Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift))
            StartSprint();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            StopSprint();

        //WallRun
        if (Input.GetKey(KeyCode.D) && (!stillOnWall) && Input.GetButton("Jump") && isWallRight)
            StartWallrun();
        if (Input.GetKey(KeyCode.A) && (!stillOnWall) && Input.GetButton("Jump") && isWallLeft)
            StartWallrun();
    }


    /// Functions
    /// 

    private void StartWallrun()
    {
       StartCoroutine(shortJumpDelay());
       if (antiGravityOff)
       {
          rb.useGravity = false;
       }
       else
       {
          useAntiGravityGravityScale = false;
       }
       isWallRunning = true;
       if (rb.velocity.magnitude <= maxWallSpeed)
       {
           rb.AddForce(orientation.forward * wallrunForce * Time.deltaTime);

           //Make sure char sticks to wall
           if (isWallRight)
                rb.AddForce(orientation.right * wallrunForce / 5 * Time.deltaTime);
           else
                rb.AddForce(-orientation.right * wallrunForce / 5 * Time.deltaTime);

           StartCoroutine(wallRunDelay());       
       }   
    }

    private void StopWallRun()
    {
        isWallRunning = false;
        stillOnWall = true;
        if (antiGravityOff)
        {
            rb.useGravity = true;
        }
        else
        {
            useAntiGravityGravityScale = true;
        }
        StartCoroutine(wallRunEndDelay());
    }

    private void CheckForWall() //make sure to call in void Update
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, wallRunDistance, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, wallRunDistance, whatIsWall);
        if (grounded) stillOnWall = false;
        //leave wall run
        if (!isWallLeft && !isWallRight) StopWallRun();
    }

    private void StartCrouch() {
            transform.localScale = crouchScale;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            if (rb.velocity.magnitude > 0.5f)
            {
                if (grounded)
                {
                    rb.AddForce(orientation.transform.forward * slideForce);
                }
            }
        
        
    }

    private void StopCrouch() {
            transform.localScale = playerScale;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        
    }

    /// Sprint
    private void StartSprint()
    {
        maxSpeed = maxSpeed * sprintSpeed;
    }

    private void StopSprint()
    {
        maxSpeed = maxSpeed / sprintSpeed;
    }

    ///AntiGravity 

    private void AntiGravity()
    {
        //Raycast to check if anti-Gravity flip is possible
        RaycastHit hit;
        if (Physics.Raycast(player.position, player.transform.up, out hit, antiGravityCeilingLimit, whatIsGround))
        {
            antiGravityPoint = hit.point;
            antiGravityDistance = Vector3.Distance(player.position, antiGravityPoint);
            //Changes variables depending on if character is on ceiling or ground
            if(antiGravityDistance < antiGravityCeilingLimit)
            {
                if (antiGravityOff == false)
                {
                    useAntiGravityGravityScale = false;
                    rb.useGravity = true;
                    antiGravityOff = true;
                    antiGravityRotate = -180;
                }
                else
                {
                    useAntiGravityGravityScale = true;
                    rb.useGravity = false;
                    antiGravityOff = false;
                    antiGravityRotate = 180;
                }
                //Transform position
                rb.AddForce(player.up * (antiGravityDistance / antiGravityFlipTime));
                //Rotate character
                StartCoroutine(Rotate(playerCam.right, antiGravityRotate, antiGravityFlipTime));
                

                
                //if (antiGravityOff && grounded)
            }
        }
    }
    
    private void Movement() {
        //Extra gravity
        if (antiGravityOff) rb.AddForce(Vector3.down * Time.deltaTime * 10);
        else rb.AddForce(Vector3.down * Time.deltaTime * -10);
        
        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);
        
        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        float maxSpeed = 20f;
        if (crouching) maxSpeed = 12f;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump && ctrlHeld) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }
        
        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }
        
        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        if (!crouching || !useMovement)
        {
            rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
            rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
        }
    }

    private void Jump() {
        if (grounded && readyToJump) {
            readyToJump = false;
            //Add jump forces
            if (rb.velocity.y < 0) rb.velocity += player.up * Physics.gravity.y * 1.5f * Time.deltaTime;
            if (antiGravityOff)
            {
                rb.AddForce(Vector2.up * jumpForce * 1.5f);
                rb.AddForce(normalVector * jumpForce * 0.5f);
            }
            else
            {
                rb.AddForce((Vector2.down * jumpForce * 1.5f) * 1.35f);
                rb.AddForce((normalVector * jumpForce * 0.5f));
            }
            
            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) 
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
            Invoke(nameof(ResetJump), jumpCooldown);

            
        }
        //Walljump
        if (isWallRunning)
        {
            readyToJump = false;

            //normal jump
            if (isWallLeft && jumping || isWallRight && jumping)
            {
                rb.AddForce(Vector2.up * jumpForce * 1.5f);
                rb.AddForce(normalVector * jumpForce * 0.5f);
                StopWallRun();
            }

            //sidwards wallhop
            if (isWallRight || isWallLeft && Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) rb.AddForce(-orientation.up * jumpForce * 1f);
            if (isWallRight && Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * jumpForce * 3.2f);
            if (isWallLeft && Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * jumpForce * 3.2f);

            //Always add forward force
            rb.AddForce(orientation.forward * jumpForce * 1f);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    private void ResetJump() {
        readyToJump = true;
    }
    
    private float desiredX;
    private void Look() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, wallRunCameraTilt);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);

        //While Wallrunning
        //Tilts camera in .5 second
        if (Math.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallRight)
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 4;
        if (Math.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallLeft)
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 4;

        //Tilts camera back again
        if (wallRunCameraTilt > 0 && !isWallRight && !isWallLeft)
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 4;
        if (wallRunCameraTilt < 0 && !isWallRight && !isWallLeft)
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 4;

    }

    private void CounterMovement(float x, float y, Vector2 mag) {
        if (!antiGravityOff) mag = new Vector2(-mag.x, mag.y);
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }


        //Counter movement 
        magg = mag;
        // if (mag is > than threshold, and holding A or D) OR (left mag is > than -threshold and moving right) OR (right mag is > than threshold and moving left)
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
          rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
          rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
        
        
        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);
        
        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v) {
            float angle = Vector3.Angle(Vector3.up, v);
            return angle < maxSlopeAngle;
        
    }

    private bool cancellingGrounded;
    
    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer)))
        {
            RaycastHit hit;
            if (!Physics.Raycast(player.transform.position, -player.up, out hit, 2f, 10))
                cancellingGrounded = false;
            else
                return;
        }

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
            if (IsFloor(normal * -1f))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }
    
}
