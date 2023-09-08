using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponAndItemScript : MonoBehaviour 
{
    private LineRenderer lr;
    [SerializeField] private float spring = 7.5f;
    [SerializeField] private float damper = 7.5f;
    [SerializeField] private float massScale = 7.5f;

    //public for input controller
    [HideInInspector] public Rigidbody rb;
     public Vector3 grapplePoint;
    [HideInInspector] public Transform player;
     public bool AscendGrapple;
    private bool canGrapple = true;
    private bool inGrapple = false;

    public LayerMask whatIsGrappleable;
    public Transform grappleGunTip, orientation, gunBarrel;
    [HideInInspector] public GameObject grappleObject = null;
    public Rigidbody projectile;

    [Header("Weapon Information")]
    [SerializeField] public GameObject currentWeapon;
    [SerializeField] public GameObject currentCrosshair;
    [SerializeField] public AudioClip currentWeaponFireAudio;
    [SerializeField] public string fireType = "semi";
    [SerializeField] public int MaxAmmo = 0;
    [SerializeField] public int ReserveAmmo = 0;
    [SerializeField] public int MagSize = 0;
    [SerializeField] public int Ammocount = 0;
    [SerializeField] public bool isShooting;//, isGrappling;
    [SerializeField] private bool reloading = false;
    [SerializeField] public float reloadTime = 0f;
    [SerializeField] public float shootTime = 0f;
    [SerializeField] public float swapTime = 0f;
    [SerializeField] public float aimTime = 0f;
    [SerializeField] public Vector3 aimPos = Vector3.zero;
    [SerializeField] public Vector3 hipPos = Vector3.zero;

    [SerializeField] private bool useGrapple = false;
    private float distanceFromPoint;
    private float grappleDistance = 100f;
    private SpringJoint joint;
    private Vector3 currentGrapplePosition;
    private bool StartGrappleTimer = true;
    private float maxGrapples = 3f;
    private float no_Grapples = 0;
    private float grappleHookCoolDown = 3f;
    private float grappleHookCoolDownTimer;

    private float bulletDistance = 10000f;
    private float bulletSpread = 5f;
    private bool shootDelay = false;
    private string Attack;
    [HideInInspector] public bool fullAutoShoot = false;
    [HideInInspector] public bool allowSwap = true;
    [HideInInspector] public bool cancelReload = false;
    [HideInInspector] public int no_Swap = 0;
    [HideInInspector] public bool buttonHeld = false;
    [HideInInspector] public bool playerHasShot = false;

    RaycastHit[] hits;

    //Layers
    private LayerMask groundLayer;
    private LayerMask EnemyLayer;

    private List<Rigidbody> instantiatedObjects = new List<Rigidbody>();

    public float grappleSpeed = 2.5f;
    public Camera cam;

    private TextMeshProUGUI AmmoCounter;
    private TextMeshProUGUI WeaponName;

    //scripts
    GameDataFile GDFile;
    InputController IC;
    EventManager EM;
    WeaponManager WeaponManager;
    //CharacterMovementV2 CharMovement;
    InteractableObjects InteractableObjects;
    Lethal lethal;

    void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        EM = GDFile.GetComponent<EventManager>();
        IC = GDFile.GetComponent<InputController>();
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
        //CharMovement = GetComponent<CharacterMovementV2>();
        WeaponManager = GetComponent<WeaponManager>();
        InteractableObjects = GetComponent<InteractableObjects>();
        subEvents();

        EnemyLayer = GDFile.EnemyLayer;
        groundLayer = GDFile.groundLayer;
        AmmoCounter = GDFile.AmmoCounter;
        WeaponName = GDFile.WeaponName;

        isShooting = false;
        //isGrappling = false;
        useGrapple = false;
        inGrapple = false;
    }
    private void subEvents() //subscribe to required events
    {
        //input controller
        IC.fire += CheckWeapon;
        IC.fireCancelled += StopGrapple;
        IC.aim += aim;
        IC.aimCancelled += aimCancelled;
        IC.lethal += Lethal;
        IC.reload += TriggerReload;
        IC.action += action;
        IC.swap += swap;
        IC.item3 += SwitchToGrapple;

        //game data
        EM.respawn += StopGrapple; //stop grapple when respawning
    }
    private void Start()
    {
        StartGrapple();
        StopGrapple();
    }
    void Update()
    {
        if (gameState.paused) return;

        if (allowSwap && no_Swap > 0)
        {
            no_Swap--;
            { WeaponManager.CycleWeapons(); }
        }
        if (AscendGrapple) AscendGrappleFunction(); 
        if (fullAutoShoot && !shootDelay)
        {
            if (Ammocount > 0 && !reloading) StartCoroutine(FullAuto());
            else if (!reloading) StartCoroutine(Reload());
        }
        grappleUpdate();
    }
    public void HUDTextUpdate()
    {
        AmmoCounter.text = (Ammocount).ToString() + " / " + (ReserveAmmo).ToString();
        WeaponName.text = WeaponManager.currentWeapon;
    }

    public void ReloadCheck()
    {
        /* drop objects if held*/
        if (buttonHeld && GDFile.IC.GetControlScheme() != "Keyboard & Mouse")
        { return; }

        //if (InteractableObjects.heldObject != null) 
        //{ InteractableObjects.Interact(); } 
        else if (!reloading && allowSwap && ReserveAmmo > 0 && Ammocount != MagSize)
        { StartCoroutine(Reload()); }
    }
    public void SwitchToGrapple()
    {
        int currentSlot = WeaponManager.currentWeaponSlot;
        int grappleSlot = WeaponManager.GrappleGunSlot;

        if (grappleSlot == 0) { useGrapple = false; return; } //if player doesnt have grapple do nothing
        if (currentSlot != grappleSlot) { WeaponManager.SwapWeapon(grappleSlot); }
        useGrapple = !useGrapple;
    }
    void aim()
    { ToggleAim(true); }
    void aimCancelled()
    { ToggleAim(false); }
    void action()
    { buttonHeld = true; }
    void swap()
    {
        ToggleAim(false);
        cancelReload = true;
        if (!allowSwap) no_Swap++;
        else StartCoroutine(SwitchGun());
    }
    public void ToggleAim(bool aim)
    {
        if (WeaponManager.getCurrentWeapon() == "minigun") { return; }
        if (WeaponManager.NoOfWeaponSlots == 0) { return; }

        //aim
        if (aim) { currentWeapon.transform.localPosition = aimPos; currentCrosshair.SetActive(false); }
        //hip 
        else { currentWeapon.transform.localPosition = hipPos; currentCrosshair.SetActive(true); }
    }
    public IEnumerator SwitchGun()
    {
        no_Swap = 1;
        allowSwap = false;
        StopGrapple();
        useGrapple = false;
        reloading = false;
        yield return new WaitForSeconds(swapTime);
        allowSwap = true;
    }
    public void CheckWeapon()
    {

        if (useGrapple) { StartGrapple(); }
        else if (Ammocount > 0 && !reloading && !isShooting)
        {
            switch (fireType)
            {
                case "semi": //semi-auto
                    Attack = "shot_standard";
                    Shoot();
                    ShootRC();
                    break;

                case "auto": //Full-auto
                    Attack = "shot_auto";
                    fullAutoShoot = true;
                    break;

                case "burst": //Burst-fire
                    if (shootDelay) break;
                    Attack = "shot_burst";
                    StartCoroutine(BurstFire());
                    break;

                case "spread": //Scattershot
                    Attack = "shot_spread";
                    Shoot();
                    ScatterShootRC();
                    break;
            }
        }
        else if (Ammocount == 0 && !reloading) StartCoroutine(Reload());
    }

    void LateUpdate() 
    { DrawRope(); }

    void StartGrapple()
    {
        if (isShooting || inGrapple || !canGrapple) return;
        if (no_Grapples >= maxGrapples) return;

        isShooting = true;
        CharacterState.inGrapple = true;
        RaycastHit hit;
        Ray ray = cam.ViewportPointToRay((new Vector3(0.5F, 0.5F, 0)));
        if (Physics.Raycast(ray, out hit, grappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            inGrapple = true; //mark as inGrapple
            canGrapple = false; //turn off ability to grapple
            StartGrappleTimer = true; //start grapple timer
            no_Grapples += 1f; //increase used grapples

            joint = player.gameObject.AddComponent<SpringJoint>(); //create grapple
            if (hit.transform.gameObject.tag == "Object" ) grappleObject = hit.transform.gameObject;
            else grappleObject = null;
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;
            lr.enabled = true;

            distanceFromPoint = Vector3.Distance(orientation.position, grapplePoint);
            
            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint *  0.2f;

            //Adjust these values to fit your game.
            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

            lr.positionCount = 2;
            currentGrapplePosition = grappleGunTip.position;            
        }
        isShooting = false;
    }
    void AscendGrappleFunction()
    {   
        //The distance grapple will try to keep from grapple point. 
        distanceFromPoint = Vector3.Distance(orientation.position, grapplePoint);
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        //CharMovement.useDownForce = false; //turn off downforce while using grapple
        rb.AddForce(((grapplePoint - player.position) * Time.deltaTime) * grappleSpeed * 15f, ForceMode.Acceleration);
        if (Mathf.Abs(Vector3.Distance(orientation.position, grapplePoint)) < 3f)
        {   StopGrapple(); }
    }
    public void StopGrapple()
    {
        if (no_Grapples >= maxGrapples) canGrapple = false; // if used max number of grapples stop grapple working

        //Debug.Log("StopGrapple");
        AscendGrapple = false;
        fullAutoShoot = false;
        //CharMovement.useDownForce = true;
        lr.positionCount = 0;
        lr.enabled = false;
        Destroy(joint);
        inGrapple = false;
        grappleObject = null;
        CharacterState.inGrapple = false;
    }
    void grappleUpdate() 
    {
        //grappling hook cooldown timer
        if (StartGrappleTimer) grappleHookCoolDownTimer -= Time.deltaTime;
        if (grappleHookCoolDownTimer <= 0f)  //reset timer and allow next grapple
        { 
            StartGrappleTimer = false; 
            grappleHookCoolDownTimer = grappleHookCoolDown; 
            canGrapple = true; 
        }
        if (CharacterState.grounded && !inGrapple) { no_Grapples = 0f; canGrapple = true; } //reset grapple when grounded and not in use
    }
    void DrawRope() 
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        if (grappleObject != null) currentGrapplePosition = grappleObject.transform.position;
        else currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, grappleGunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    IEnumerator FullAuto()
    {
        if (Ammocount > 0)
        {
            shootDelay = true;
            Shoot();
            ShootRC();
            yield return new WaitForSeconds(shootTime);
            shootDelay = false;
        }
    }
    IEnumerator BurstFire()
    {
        shootDelay = true;
        for (int i = 0; i < 3; i++)
        {
            if (Ammocount > 0) 
            { 
                Shoot();
                ShootRC();
                yield return new WaitForSeconds(shootTime);
            }
        }
        yield return new WaitForSeconds(0.15f);
        shootDelay = false;
    }
    private void Shoot()
    {
        isShooting = true;
        StartCoroutine(monitorPlayerShooting());
        Ammocount --;
        float x = Screen.width / 2;
        float y = Screen.height / 2;
        var ray = cam.ScreenPointToRay(new Vector3(x, y, 0));

        //Debug.Log("shoot");
        Rigidbody instantiatedProjectile = Instantiate(projectile, gunBarrel.transform.position, cam.transform.rotation) as Rigidbody;
        instantiatedProjectile.tag = "PlayerRangedAttack";
        instantiatedProjectile.AddForce(ray.direction * 80f, ForceMode.Impulse);
        GDFile.PlayerAudio.clip = currentWeaponFireAudio;
        GDFile.PlayerAudio.Play();
        Destroy(instantiatedProjectile.gameObject, 0.1f);
        isShooting = false;

        HUDTextUpdate();
    }
    private void ShootRC()
    {
        isShooting = true;
        StartCoroutine(monitorPlayerShooting());
        float X = Screen.width / 2;
        float Y = Screen.height / 2;
        Ray ray = cam.ScreenPointToRay(new Vector3(X, Y, 0));
        RaycastHit hitInfo;
        float rayDistance;

        if (Physics.Raycast(ray, out hitInfo, bulletDistance, groundLayer)) //Blue Laser
        { rayDistance = hitInfo.distance; }
        else rayDistance = bulletDistance;

        hits = Physics.RaycastAll(ray, rayDistance, EnemyLayer); //Yellow Laser
        if(Physics.Raycast(ray, rayDistance, EnemyLayer))
        {
            Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
            foreach (RaycastHit obj in hits) 
            {
                if (obj.transform.GetComponent<HealthScript>() != null)
                { SendAttackData(obj.transform.gameObject); }
            }
        }
        isShooting = false;
        HUDTextUpdate();
    }    
    private void ScatterShootRC()
    {
        isShooting = true;
        StartCoroutine(monitorPlayerShooting());
        float X = Screen.width / 2;
        float Y = Screen.height / 2;
        Ray ray = cam.ScreenPointToRay(new Vector3(X, Y, 0));
        RaycastHit hitInfo;
        float rayDistance;

        if (Physics.Raycast(ray, out hitInfo, bulletDistance, groundLayer)) //Blue Laser
        { rayDistance = hitInfo.distance; }
        else rayDistance = bulletDistance;

        hits = Physics.SphereCastAll(ray, bulletSpread , rayDistance, EnemyLayer); //Yellow Laser
        if (hits.Length != 0)
        {
            Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
            foreach (RaycastHit obj in hits) 
            {
                if (obj.transform.GetComponent<HealthScript>() != null)
                { SendAttackData(obj.transform.gameObject); }
            }
        }
        isShooting = false;

        HUDTextUpdate();
    }
    private void SendAttackData(GameObject obj)
    {
        HealthScript healthScript = obj.transform.GetComponent<HealthScript>();
        bool critical = false; //temporary - sets all attacks to not be critical
        healthScript.DamageEntity("Player", Attack, critical);
    }
    void TriggerReload()
    {
        ReloadCheck();
        buttonHeld = false;
    }
    IEnumerator Reload()
    {
        int addAmmo;
        cancelReload = false;
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        if (!cancelReload) 
        { 
            addAmmo = MagSize - Ammocount;
            if (Ammocount + ReserveAmmo > MagSize) { ReserveAmmo -= addAmmo; Ammocount = MagSize; }
            else if (ReserveAmmo > 0) { Ammocount = ReserveAmmo + Ammocount; ReserveAmmo = 0; }
        }
        else { addAmmo = 0; }
        reloading = false;

        HUDTextUpdate();
    }
    void Lethal()
    {
        if (inGrapple)
        {
            //*ADD* make initial force smaller the closer you are to the point
            rb.AddForce((grapplePoint - player.position) * grappleSpeed * 0.15f, ForceMode.Impulse);
            AscendGrapple = true;
        }
        else ThrowLethal();
    }
    public void ThrowLethal()
    {
        if (WeaponManager.NoOfLethals <= 0) return;
        GameObject lethalObject = WeaponManager.lethal_Model;

        isShooting = true;
        WeaponManager.NoOfLethals--;
        float x = Screen.width / 2;
        float y = Screen.height / 2;
        var ray = cam.ScreenPointToRay(new Vector3(x, y, 0));

        //Debug.Log("shoot");
        Rigidbody instantiatedProjectile = Instantiate(WeaponManager.lethal_Model.GetComponent<Rigidbody>(), GDFile.objectGuide.position, cam.transform.rotation) as Rigidbody;
        instantiatedProjectile.AddForce((ray.direction + orientation.up) * (GDFile.lethal_ThrowSpeed / WeaponManager.lethal_ThrowDistance), ForceMode.Impulse);

        lethal = instantiatedProjectile.GetComponent<Lethal>();
        StartCoroutine(lethal.StartGrenade());
        isShooting = false;

    }

    IEnumerator monitorPlayerShooting()
    {
        //Debug.Log("shot");
        playerHasShot = true;
        yield return new WaitForSeconds(1.25f);
        playerHasShot = false;
    }
}
