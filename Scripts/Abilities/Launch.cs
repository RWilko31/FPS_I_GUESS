using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch : PlayerAbility
{
    [Header("Launch settings")]
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float slamForce = 10f;
    [SerializeField] private float cooldown = 4f;

    [Header("cooldown")]
    [SerializeField] private float launchTimer = 0;
    private bool canLaunch = true;
    private bool useTimer = false;

    private LayerMask groundLayer;
    private Transform orientation;
    private Rigidbody rb;

    GameDataFile GDFile;

    public override void UseAbility()
    { CheckLaunch(); }
    public override string abilityName()
    { return "Launch"; }
    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();

        orientation = GDFile.orientation;
        groundLayer = GDFile.groundLayer;
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (!canLaunch && CharacterState.grounded) useTimer = true; //start reset timer when landed
        if(useTimer) Timer();
    }
    private void Timer()
    { 
        if (launchTimer > 0) launchTimer -= Time.deltaTime;
        if (launchTimer <= 0) { useTimer = false; canLaunch = true; }
    }
    private void CheckLaunch()
    {
        RaycastHit launchHit;
        bool hit = Physics.SphereCast(transform.position, 0.6f, -orientation.up, out launchHit, 15f, groundLayer);
        if (canLaunch && hit && launchHit.distance <= 3f) LaunchJump(launchHit);
        else if(!hit || (hit && launchHit.distance >= 15f)) Slam();
    }
    private void LaunchJump(RaycastHit launchHit)
    {
        rb.AddForce(launchHit.normal.normalized * launchForce * 1000, ForceMode.Force);
        StartCoroutine(TriggerCooldown());
    }
    private void Slam()
    {
        rb.AddForce(-orientation.up * slamForce * 1000, ForceMode.Force);
        StartCoroutine(TriggerCooldown());
    }
    private IEnumerator TriggerCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        launchTimer = cooldown;
        canLaunch = false;
    }
}
