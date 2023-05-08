using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class WeaponAndItemScript : MonoBehaviour {

    [SerializeField] public string currentControlScheme;
    private LineRenderer lr;
    public Rigidbody rb;
    public Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform grappleGunTip, player, orientation, gunBarrel;
    public GameObject grappleObject = null;
    public float grappleDistance = 100f;
    private SpringJoint joint;
    public Rigidbody projectile;
    public bool isShooting;
    public bool isGrappling;
    private bool useGrapple;
    public bool AscendGrapple;
    float distanceFromPoint;
    private Vector3 currentGrapplePosition;
    [SerializeField] public int MaxAmmo = 0;
    [SerializeField] public int ReserveAmmo = 0;
    [SerializeField] public int MagSize = 0;
    [SerializeField] public int Ammocount = 0;

    [SerializeField] private bool reloading = false;
    [SerializeField] public float reloadTime = 0f;
    [SerializeField] public float shootTime = 0f;
    [SerializeField] public float swapTime = 0f;
    [SerializeField] public float aimTime = 0f;
    [SerializeField] public Vector3 aimPos = Vector3.zero;
    [SerializeField] public Vector3 hipPos = Vector3.zero;

    private float bulletDistance = 10000f;
    private float bulletSpread = 5f;
    public string fireType = "semi";
    public bool fullAutoShoot = false;
    private bool shootDelay = false;
    public bool allowSwap = true;
    public bool cancelReload = false;
    public int no_Swap = 0;
    public bool buttonHeld = false;
    [SerializeField] public GameObject currentWeapon;
    [SerializeField] public GameObject currentCrosshair;
    [SerializeField] public AudioClip currentWeaponFireAudio;
    private string Attack;

    [SerializeField] public bool playerHasShot = false;

    RaycastHit[] hits;

    //Layers
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask EnemyLayer;
    [SerializeField] private LayerMask EverythingLayer;

    private List<Rigidbody> instantiatedObjects = new List<Rigidbody>();

    // private Vector3 offset;
    public float grappleSpeed = 2.5f;
    public Camera cam;

    private TextMeshProUGUI AmmoCounter;
    private TextMeshProUGUI WeaponName;

    //scripts
    GameDataFile GameDataFile;
    WeaponManager WeaponManager;
    CharacterMovementV2 CharMovement;
    InteractableObjects InteractableObjects;
    Lethal Lethal;

    //Input
    InputManager Controls;
    PlayerInput playerInput;


    int i = 0;

    //private void OnEnable() { Controls.Menu.Enable(); } //Enables user input 
    //private void OnDisable() { Controls.Default.Disable(); Controls.Menu.Disable(); } //Disables user input

    void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
        CharMovement = GetComponent<CharacterMovementV2>();
        WeaponManager = GetComponent<WeaponManager>();
        GameDataFile = FindObjectOfType<GameDataFile>();
        InteractableObjects = GetComponent<InteractableObjects>();
        playerInput = GameDataFile.gameObject.GetComponent<PlayerInput>();
        Controls = new InputManager();
        EverythingLayer = GameDataFile.EverythingLayer;
        EnemyLayer = GameDataFile.EnemyLayer;
        groundLayer = GameDataFile.groundLayer;
        AmmoCounter = GameDataFile.AmmoCounter;
        WeaponName = GameDataFile.WeaponName;


        //DefaultInput();
        isShooting = false;
        isGrappling = false;
        useGrapple = false;
        //Ammocount = MagSize;
        //offset = new Vector3(0, -(transform.localScale.y + 0.25f), 0);
    }
    private void Start()
    {
        StartGrapple();
        StopGrapple();
        StartCoroutine(LateStart());
    }
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.01f);
    }

    //void DefaultInput()
    //{
    //    Controls.Default.Fire.performed += ctx => { if (!isGrappling) CheckWeapon(); };
    //    Controls.Default.Fire.canceled += ctx => { fullAutoShoot = false; if (isGrappling) StopGrapple(); };
    //    Controls.Default.Aim.performed += ctx => Aim();
    //    Controls.Default.Aim.canceled += ctx => StopAim();
    //    Controls.Default.Lethal.performed += ctx => { if (isGrappling) AscendGrapple = true; else ThrowLethal(); };
    //    Controls.Default.Reload.canceled += ctx => { ReloadCheck(); };
    //    Controls.Default.Action.performed += ctx => { buttonHeld = true; InteractableObjects.Interact(); };
    //    //Controls.Default.Lethal.canceled += ctx => { if (isGrappling) AscendGrapple = false; };
    //    Controls.Default.Item3.performed += ctx => { SwitchToGrapple();};
    //    Controls.Default.Swap.performed += ctx => { StopAim(); cancelReload = true; if (!allowSwap) no_Swap++; else StartCoroutine(SwitchGun()); };
    //}
    public void ReloadCheck()
    {
        /* drop objects if held*/
        if (buttonHeld && playerInput.currentControlScheme != "Keyboard & Mouse")
        { return; }

        if (InteractableObjects.heldObject != null) 
        { InteractableObjects.Interact(); } 
        else if (!reloading && allowSwap && ReserveAmmo > 0 && Ammocount != MagSize)
        { StartCoroutine(Reload()); }
    }
    //public void PauseWeaponAndItemInput()
    //{
    //    if (GameDataFile.isPaused || GameDataFile.tempPause) { Controls.Default.Disable(); Controls.Menu.Enable(); playerInput.SwitchCurrentActionMap("Menu"); }
    //    else { Controls.Menu.Disable(); Controls.Default.Enable(); playerInput.SwitchCurrentActionMap("Default"); }
    //    //Debug.Log(playerInput.currentActionMap);
    //}
    public void SwitchToGrapple()
    {
        int currentSlot = WeaponManager.currentWeaponSlot;
        int grappleSlot = WeaponManager.GrappleGunSlot; 

        if (grappleSlot == -1) return;
        if (currentSlot != grappleSlot) { WeaponManager.SwapWeapon(WeaponManager.GrappleGunSlot); }
        useGrapple = !useGrapple;
    }
    public void Aim()
    {
        if (WeaponManager.getCurrentWeapon() == "minigun") { return; }
        if (WeaponManager.NoOfWeaponSlots == 0) { return; }
        else { currentWeapon.transform.localPosition = aimPos; currentCrosshair.SetActive(false); }
    }
    public void StopAim()
    {
        if (WeaponManager.getCurrentWeapon() == "minigun") { return; }
        if (WeaponManager.NoOfWeaponSlots == 0) { return; }
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

        if (useGrapple && WeaponManager.currentWeapon == "pistol")
        {
            if (CharMovement.canGrapple) { StartGrapple(); }
            else return;
        }
        if (Ammocount > 0 && !reloading && !isShooting && !useGrapple)
        {
            if (fireType == "semi") { Attack = "shot_standard"; Shoot(); ShootRC(); } //semi-auto
            else if (fireType == "auto") { Attack = "shot_auto"; fullAutoShoot = true; } //Full-auto
            else if (fireType == "burst" && !shootDelay) { Attack = "shot_burst"; StartCoroutine(BurstFire()); } //Burst-fire
            else if (fireType == "spread") { Attack = "shot_spread"; Shoot(); ScatterShootRC(); } //Scattershot
        }
        else if(Ammocount == 0 && !reloading && !useGrapple) StartCoroutine(Reload());
    }

    void Update()
    {
        if (GameDataFile.isPaused) return;

        currentControlScheme = playerInput.currentControlScheme.ToString();
        if (allowSwap && no_Swap > 0)
        {
            no_Swap--;
            { WeaponManager.CycleWeapons(); }
        }
        if (AscendGrapple)
        {
            AscendGrappleFunction(); //The distance grapple will try to keep from grapple point. 
            distanceFromPoint = Vector3.Distance(orientation.position, grapplePoint);
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }
        if (fullAutoShoot && !shootDelay) 
        { 
            if (Ammocount > 0 && !reloading)  StartCoroutine(FullAuto());  
            else if (!reloading) StartCoroutine(Reload());
        }
    }

    public void HUDTextUpdate()
    {
        AmmoCounter.text = (Ammocount).ToString() + " / " + (ReserveAmmo).ToString();
        WeaponName.text = WeaponManager.currentWeapon;
    }

    //Called after Update
    void LateUpdate() 
    {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple()
    {
        //Debug.Log("grapple");
        if (isShooting || isGrappling) return; 
        isShooting = true; 
        RaycastHit hit;
        Ray ray = cam.ViewportPointToRay((new Vector3(0.5F, 0.5F, 0)));
        if (Physics.Raycast(ray, out hit, grappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            isGrappling = true;
            joint = player.gameObject.AddComponent<SpringJoint>();
            if (hit.transform.gameObject.tag == "Object" ) grappleObject = hit.transform.gameObject;
            else grappleObject = null;
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;
            lr.enabled = true;

            distanceFromPoint = Vector3.Distance(orientation.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = 7.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = grappleGunTip.position;
            
        }
        isShooting = false;
    }
    void AscendGrappleFunction()
    {
        //rb.useGravity = false;
        //transform.position = Vector3.MoveTowards(transform.position, grapplePoint, grappleSpeed * Time.deltaTime * 10f);
        rb.AddForce(((grapplePoint - player.position) * Time.deltaTime) * grappleSpeed * 15f, ForceMode.Force);
        if (Mathf.Abs(Vector3.Distance(orientation.position, grapplePoint)) < 3f)
        {
            StopGrapple();
            AscendGrapple = false;
        }
    }

    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    public void StopGrapple()
    {
        //Debug.Log("StopGrapple");
        AscendGrapple = false;
        lr.positionCount = 0;
        lr.enabled = false;
        Destroy(joint);
        isGrappling = false;
        grappleObject = null;
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
        GameDataFile.PlayerAudio.clip = currentWeaponFireAudio;
        GameDataFile.PlayerAudio.Play();
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
        {
            rayDistance = hitInfo.distance;
        }
        else rayDistance = bulletDistance;

        //Debug.Log(hitInfo.transform.gameObject);

        hits = Physics.RaycastAll(ray, rayDistance, EnemyLayer); //Yellow Laser
        if(Physics.Raycast(ray, rayDistance, EnemyLayer))
        {
            Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
            foreach (RaycastHit obj in hits) 
            {
                if (obj.transform.GetComponent<HealthScript>() != null)
                {
                    //obj.transform.GetComponent<HealthScript>().RaycastDamage();
                    SendAttackData(obj.transform.gameObject);
                }
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
        {
            rayDistance = hitInfo.distance;
        }
        else rayDistance = bulletDistance;

        //Debug.Log(hitInfo.transform.gameObject);

        hits = Physics.SphereCastAll(ray, bulletSpread , rayDistance, EnemyLayer); //Yellow Laser
        if (hits.Length != 0)
        {
            Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
            foreach (RaycastHit obj in hits) 
            {
                if (obj.transform.GetComponent<HealthScript>() != null)
                {
                    //obj.transform.GetComponent<HealthScript>().RaycastDamage();
                    SendAttackData(obj.transform.gameObject);
                }
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
        Rigidbody instantiatedProjectile = Instantiate(WeaponManager.lethal_Model.GetComponent<Rigidbody>(), GameDataFile.objectGuide.position, cam.transform.rotation) as Rigidbody;
        instantiatedProjectile.AddForce((ray.direction + orientation.up) * (GameDataFile.lethal_ThrowSpeed / WeaponManager.lethal_ThrowDistance), ForceMode.Impulse);

        Lethal = instantiatedProjectile.GetComponent<Lethal>();
        StartCoroutine(Lethal.StartGrenade());
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
