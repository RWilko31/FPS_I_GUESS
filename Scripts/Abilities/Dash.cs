using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : PlayerAbility
{
    [Header("Dash settings")]
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashDrag = 10f;
    [SerializeField] private float stopVelocity = 12f;
    [SerializeField] private float cooldown = 10f;
    [SerializeField] private LayerMask teleportLayers;
    private LayerMask playerLayer;

    [Header("cooldown")]
    [SerializeField] private float dashTimer = 0f;
    private bool canDash = true;
    private bool useTimer = false;
    private Transform orientation;
    private Rigidbody rb;

    GameDataFile GDFile;

    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        playerLayer = GDFile.playerLayer;

        orientation = GDFile.orientation;
        rb = GetComponent<Rigidbody>();
    }
    public override string abilityName()
    { return "Dash"; }
    public override void UseAbility()
    { CheckDash(); }

    private void Update()
    {
        if (useTimer) Timer();
    }
    private void Timer()
    {
        if (dashTimer > 0) dashTimer -= Time.deltaTime;
        if (dashTimer <= 0) { useTimer = false; canDash = true; }
    }
    void CheckDash()
    {
        if (canDash) DashPlayer();
    }
    void DashPlayer()
    {
        Togglecollision(true);
        CharacterState.useDrag = false;
        canDash = false;
        rb.drag = dashDrag;
        dashTimer = cooldown;
        rb.AddForce(orientation.forward * dashForce, ForceMode.VelocityChange);
        StartCoroutine(DashTime());
    }
    void Togglecollision(bool ignoreLayer)
    {
        int layer1 = (int) Mathf.Log(playerLayer.value, 2), layer2 = (int)Mathf.Log(teleportLayers.value, 2);
        //Debug.Log("Layer 1: " + layer1 + " layer2: " + layer2);
        Physics.IgnoreLayerCollision(layer1, layer2, ignoreLayer);
    }
    IEnumerator DashTime()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude <= stopVelocity);
        Togglecollision(false);
        CharacterState.useDrag = true;
        useTimer = true;
    }
}
