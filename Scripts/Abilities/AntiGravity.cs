using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGravity : PlayerAbility
{
    //Flips the objects gravity when the antigravity event is called
    //inherits from PlayerAbility class allowing use in the abilityManager script

    GameDataFile GDfile;
    EventManager EM;

    #region AntiGravity
    [Header("AntiGravity")]
    [SerializeField] private bool isPlayer = false;
    [SerializeField] public bool inAntigravity = false;
    [SerializeField] public bool AgCoolDownBool = false;
    [SerializeField] private float antiGravityCeilingLimit = 100f;
    [SerializeField] private float antiGravityFlipTime = 0.75f;
    [SerializeField] private float antiGravityCoolDown = 2f;
    [SerializeField] private float antiGravityCoolDownTimer;
    private float gravityDir = 1f;

    [Header("Layer")]
    [SerializeField] private LayerMask groundLayer;
    #endregion

    //transforms
    Rigidbody rb;
    Transform orientation;
    public override void UseAbility()
    { antiGravity(); }
    public override string abilityName()
    { return "AntiGravity"; }
    private void Awake()
    {
        GDfile = FindObjectOfType<GameDataFile>();
        EM = GDfile.GetComponent<EventManager>();
        rb = GetComponent<Rigidbody>();

        if (GDfile.GetCurrentLevel() == "OnStart" && !isPlayer) return; // prevent subscribing to events before object should be loaded
        if (isPlayer) initPlayer();
        else initObj();
    }
    private void subEvents() //subscribe to required events
    {
        EM.antigravity += antiGravity; 
    }
    private void initPlayer() // initialises antigravity for player
    {
        orientation = GDfile.orientation;
        subEvents();
    }
    private void initObj() // initialises antigravity for an Object
    {
        orientation = transform;
        subEvents();
    }
    private void Update()
    {
        antiGravityTimer();
        gravity();
    }
    private void gravity()
    {
        if (rb.useGravity || CharacterState.inWallRun) return;
        rb.AddForce(-Physics.gravity, ForceMode.Acceleration);
    }
    private void antiGravityTimer()
    {
        //antigravity cooldown timer
        if (AgCoolDownBool) antiGravityCoolDownTimer -= Time.deltaTime; //AG cooldown
        if (antiGravityCoolDownTimer <= 0) AgCoolDownBool = false; //turn off AG cooldown
    }
    public void antiGravity()
    {   //Performs antigravity

        if (AgCoolDownBool || !this.isActiveAndEnabled) return;

        //Raycast to check if anti-Gravity flip is possible
        RaycastHit hit;
        if (Physics.Raycast(transform.position, orientation.up, out hit, antiGravityCeilingLimit, groundLayer))
        {
            AgCoolDownBool = true;
            antiGravityCoolDownTimer = antiGravityCoolDown;

            Vector3 antiGravityPoint = hit.point;
            float antiGravityDistance = Vector3.Distance(transform.position, antiGravityPoint);
            //Changes variables depending on if character is on ceiling or ground
            if (antiGravityDistance < antiGravityCeilingLimit)
            {
                if (isPlayer)
                {
                    CharacterState.inAntigravity = !CharacterState.inAntigravity;
                    CharacterState.gravityDir *= -1;
                    EM.AntigravityEvent();
                }
                rb.useGravity = !rb.useGravity;
                inAntigravity = !inAntigravity;
                gravityDir *= -1;
                rb.AddForce(transform.up * (antiGravityDistance / antiGravityFlipTime)); //Transform position   
                StartCoroutine(Rotate(180f, antiGravityFlipTime)); //Rotate
            }
        }
    }
    IEnumerator Rotate(float angle, float duration)
    {   //Rotates the character when antigravity is triggered

        Quaternion from = transform.rotation;
        Quaternion to = from;
        to *= Quaternion.Euler(0f, 0f, angle);

        float elapsed = 0.0f;
        while (elapsed <= duration * 2f)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
    }
}
