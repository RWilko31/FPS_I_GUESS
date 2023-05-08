using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractableObjects : MonoBehaviour
{
    //Int number to define the object ability. 0 = no ability, 1 = AntiGravity, 2 = Teleport object, 3 = Teleport Player, 4 = Scale, 5 = Launch Object/Player, 6 = Bouncy;
    int ObjectType = 0;
    public string ObjectTag;

    [SerializeField] public GameObject F;
    [SerializeField] public GameObject heldObject;
    [SerializeField] private Transform guide;
    [SerializeField] private Transform obj;
    [SerializeField] private Transform fixedGuide;
    [SerializeField] private GameObject standingObject;
    [SerializeField] private GameObject setRbObject;
    [SerializeField] private GameObject lastRbObject;
    private Vector3 objectSize;
    [SerializeField] private bool isRigidbody = true;
    [SerializeField] private bool stopCheck = false;
    [SerializeField] private bool canInteract = false;
    [SerializeField] private GameObject interactedObject;
    private bool isColliding;
    RaycastHit playerOnObjectHit;
    GameObject playerOnObject;

    private Vector3 lastRotation;
    private Vector3 lastPosition;
    private Vector3 CamVelocity;
    private Vector3 CamAngularVelocity;
    private float viewAngle;

    private Transform playerCam;
    private Transform player;
    private Transform orientation;
    private Transform cameraRig;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public LayerMask interactableLayer;
    public LayerMask objectLayer;
    public LayerMask detectLayer;
    private float maxObjectDistance;
    [SerializeField] private float ObjectSearchRadius = 1f;
    private float moveForce;
    private Vector3 contactPoint;
    private float InteractableDistance;
    private float holdOffset;
    private Vector3 lastHoldPosition;
    private Vector3 guidePos;
    private GameObject doorObject;
    private GameObject[] allInteractables;
    private bool AllowInteract = false;
    private bool objPosLatch = false;
    private bool AllowObjMovement = true;

    private float wallRunOffSetX = 0.5f;
    private float wallRunOffSetY = 0.5f;
    [SerializeField] private Vector3 defaultGuidePos;
    [SerializeField] private Vector3 currentGuidePos;
    private Vector3 defaultFixedGuidePos;
    private bool resetPos = false;
    [SerializeField] private bool guideLeft, guideRight;
    private Transform closestObj = null;

    int frame = 0;
    int SkipFrames = 2; //number of frames to skip between interactable update

    //Scripts
    GameDataFile GameDataFile;
    CharacterMovementV2 CharMovement;
    WeaponAndItemScript WeaponAndItemScript;
    InteractableData InteractableData;
    CollisionDetector collisionDetector;
    objectTrigger objectTrigger;

    private void Awake()
    {
        CharMovement = GetComponent<CharacterMovementV2>();
        GameDataFile =  FindObjectOfType<GameDataFile>();
        WeaponAndItemScript = GetComponent<WeaponAndItemScript>();
        InteractableData = FindObjectOfType<InteractableData>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraRig = GameDataFile.cameraRig;
        playerCam = GameDataFile.playerCam;
        player = GameDataFile.player;
        orientation = GameDataFile.orientation;
        orientation = GameDataFile.cameraRig;
        playerLayer = GameDataFile.playerLayer;
        groundLayer = GameDataFile.groundLayer;
        interactableLayer = GameDataFile.InteractableLayer;
        objectLayer = GameDataFile.objectLayer;
        maxObjectDistance = GameDataFile.maxObjectDistance;
        guide = GameDataFile.objectGuide;
        fixedGuide = GameDataFile.fixedobjectGuide;
        lastPosition = playerCam.position;
        lastRotation = playerCam.eulerAngles;
        moveForce = GameDataFile.objectMoveForce;
        InteractableDistance = GameDataFile.InteractableDistance;
        F = GameDataFile.F;
        holdOffset = GameDataFile.holdOffset;
        viewAngle = GameDataFile.FOVangle;
        //detectLayer = 1 << LayerMask.NameToLayer("Interactable") | LayerMask.NameToLayer("Interactable");

        guidePos = guide.localPosition;
        defaultGuidePos = guide.localPosition;
        defaultFixedGuidePos = fixedGuide.localPosition;

        allInteractables = GameObject.FindGameObjectsWithTag("Interactable");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameDataFile.isPaused) return;

        if (heldObject != null) MoveObject();
        if(setRbObject != null) 
        {
            if (setRbObject.GetComponent<Rigidbody>().isKinematic && 
                (!Physics.Raycast(setRbObject.gameObject.transform.position, -player.up, out playerOnObjectHit, 1f, groundLayer) ||
                !Physics.BoxCast(setRbObject.gameObject.transform.position, setRbObject.gameObject.transform.localScale / 2.5f, player.up, out playerOnObjectHit, setRbObject.gameObject.transform.rotation, 1f, playerLayer)))
            { setRbObject.GetComponent<Rigidbody>().isKinematic = false; setRbObject = null; }
            else if(!setRbObject.GetComponent<Rigidbody>().isKinematic) { setRbObject.GetComponent<Rigidbody>().isKinematic = true; }
        }
        if(lastRbObject != null)
        {
            if (lastRbObject != setRbObject && lastRbObject.GetComponent<Rigidbody>().isKinematic) lastRbObject.GetComponent<Rigidbody>().isKinematic = false;
        }

        if (F.activeSelf)
        {
            F.SetActive(false);
            GameDataFile.Press_PS.SetActive(false);
            GameDataFile.Press_PC.SetActive(false);
            //GameDataFile.Press_XB.SetActive(false);
        }

        if (frame == SkipFrames)
        {
            #region
            //Ray ray = playerCam.GetComponent<Camera>().ViewportPointToRay((new Vector3(0.5F, 0.5F, 0)));
            //if (Physics.Raycast(ray, out RaycastHit hit, InteractableDistance, interactableLayer + groundLayer + objectLayer))
            //{
            //    if (hit.transform.gameObject.layer != groundLayer)
            //    {
            //        //Debug.Log(hit.transform.gameObject.layer);
            //        closestObj = hit.transform;
            //        canInteract = true;
            //        if (hit.transform.gameObject.layer == 12) 
            //        { DisplayText(closestObj.gameObject); }
            //    }
            //    else
            //    {
            //        closestObj = null;
            //        interactedObject = null;
            //        canInteract = false;
            //    }
            //}
            //else 
            //{ 
            //    closestObj = null;
            //    interactedObject = null;
            //    canInteract = false;
            //}
            #endregion

            float closestObjDist = Mathf.Infinity;
            //Transform closestObj = null;
            Ray ray = playerCam.GetComponent<Camera>().ViewportPointToRay((new Vector3(0.5F, 0.5F, 0)));
            if (Physics.Raycast(ray, out RaycastHit hit, InteractableDistance, groundLayer + objectLayer + interactableLayer))
            {
                Collider[] hitObjects = Physics.OverlapSphere(hit.point, 1f, interactableLayer + objectLayer);
                foreach (Collider obj in hitObjects)
                {
                    if (obj.transform.gameObject.layer != 8)
                    {
                        float dist = Vector3.Distance(obj.transform.position, hit.point);
                        if (dist < closestObjDist)// && Vector3.Distance(obj.transform.position, player.position) < maxObjectDistance)
                        {
                            closestObjDist = Vector3.Distance(obj.transform.position, hit.point);
                            ObjectTag = obj.transform.tag;
                            closestObj = obj.transform;
                            canInteract = true;
                            DisplayText(closestObj.gameObject);
                            //Debug.Log(obj.transform);
                        }
                    }
                    //Debug.Log(dist);
                }
            }
            else
            {
                closestObj = null;
                interactedObject = null;
                canInteract = false;
            }
        }
        else frame++;
    }

    public void DisplayText(GameObject interactObject)
    {
        //Set screen text
        if (closestObj.gameObject.layer == 13) { F.SetActive(false); return; }
        GameDataFile.HudText.text = GameDataFile.GetInteractableText(interactObject);
        F.SetActive(true);
    }

    private void FixedUpdate()
    {
        //calculate cam velocity every frame
        Vector3 currentPosition = player.position;
        CamVelocity = (currentPosition - lastPosition) / Time.deltaTime;
        lastPosition = currentPosition;

        if (WeaponAndItemScript.grappleObject != null) WeaponAndItemScript.grappleObject.GetComponent<Rigidbody>().AddForce((player.position - WeaponAndItemScript.grappleObject.transform.position).normalized * 10f);
    }

    public void Interact()
    {
        if (heldObject == null)
        { //change how the text shows so you cant see it through walls

            //float closestObjDist = Mathf.Infinity;
            //Transform closestObj = null;
            //Ray ray = playerCam.GetComponent<Camera>().ViewportPointToRay((new Vector3(0.5F, 0.5F, 0)));
            //if (Physics.Raycast(ray, out RaycastHit hit, maxObjectDistance))
            //{
            //    RaycastHit[] hitObjects = Physics.SphereCastAll(hit.point, ObjectSearchRadius, Vector3.up, 1f, interactableLayer + objectLayer);
            //    foreach (RaycastHit obj in hitObjects)
            //    {
            //        float dist = Vector3.Distance(obj.transform.position, hit.point);
            //        if (dist < closestObjDist && Vector3.Distance(obj.transform.position, player.position) < maxObjectDistance)
            //        {
            //            closestObjDist = Vector3.Distance(obj.transform.position, hit.point);
            //            ObjectTag = obj.transform.tag;
            //            closestObj = obj.transform;
            //            Debug.Log(obj.transform);
            //        }
            //        //Debug.Log(dist);
            //    }
            //}
            if (closestObj == null) { ObjectTag = null; obj = null; }
            else { interactedObject = closestObj.gameObject; ObjectTag = closestObj.transform.tag; }
            ObjectType = 0;
            if (ObjectTag == "Object") ObjectType = 1;
            if (ObjectTag == "AntiGravity") ObjectType = 2;
            if (ObjectTag == "TeleportObject") ObjectType = 3;
            if (ObjectTag == "TeleportPlayer") ObjectType = 4;
            if (ObjectTag == "Scale") ObjectType = 5;
            if (ObjectTag == "Launch") ObjectType = 6;
            if (ObjectTag == "Interactable") ObjectType = 7;
            //Debug.Log(ObjectTag);
            if (ObjectType != 0 && ObjectType != 7) { PickUpObject(closestObj.gameObject); }
            if (AllowInteract && ObjectType == 7) { doorObject.transform.gameObject.GetComponent<InteractableData>().StartInteract(); AllowInteract = false; }
            if ((ObjectType == 7 && interactedObject != null)) {interactedObject.transform.gameObject.GetComponent<InteractableData>().StartInteract(); interactedObject = null; }
        }
        else DropObject();
    }

    void PickUpObject(GameObject viewedObject)
    {
        objectSize = viewedObject.transform.localScale;
        Rigidbody objRig = viewedObject.GetComponent<Rigidbody>();
        objRig.interpolation = RigidbodyInterpolation.Interpolate;
        objRig.useGravity = false;
        objRig.drag = 10;
        //objRig.transform.parent = guide;
        objRig.velocity = new Vector3(0, 0, 0);
        heldObject = viewedObject;
        collisionDetector = heldObject.GetComponent<CollisionDetector>();

        //if (cameraRig.rotation.eulerAngles.x > 16f) heldObject.transform.position = fixedGuide.position;
        //else heldObject.transform.position = guide.position;
    }
    void DropObject()
    {
        Rigidbody heldRig = heldObject.GetComponent<Rigidbody>();
        heldRig.interpolation = RigidbodyInterpolation.None;
        heldRig.useGravity = true;
        heldRig.drag = 0;

        //heldObject.transform.parent = null;

        //Apply camera force to object
        heldRig.velocity += CamVelocity / 1f;
        //heldRig.angularVelocity = CamAngularVelocity * 1.2f;
        heldObject = null;
        //teleportObject = false;
        collisionDetector = null;
    }
    void MoveObject()
    {
        ///Rotation o f heldObject
        //Prevents infinite rotation of the object when held
        float velocitySpeed = 2f;
        Vector3 objectVelocity = new Vector3(0, 0, 0);
        Vector3 angularVelocity = heldObject.GetComponent<Rigidbody>().angularVelocity;
        if (angularVelocity.x > 0)
            objectVelocity.x = angularVelocity.x - (velocitySpeed * Time.deltaTime);
        if (angularVelocity.y > 0)
            objectVelocity.y = angularVelocity.y - (velocitySpeed * Time.deltaTime);
        if (angularVelocity.z > 0)
            objectVelocity.z = angularVelocity.z - (velocitySpeed * Time.deltaTime);
        if (angularVelocity.x < 0)
            objectVelocity.x = angularVelocity.x + (velocitySpeed * Time.deltaTime);
        if (angularVelocity.y < 0)
            objectVelocity.y = angularVelocity.y + (velocitySpeed * Time.deltaTime);
        if (angularVelocity.z < 0)
            objectVelocity.z = angularVelocity.z + (velocitySpeed * Time.deltaTime);
        heldObject.GetComponent<Rigidbody>().angularVelocity = objectVelocity;


        ///Change destination between fixedguide and guide
        //Debug.Log(angle);
        Vector3 guidePos = guide.position;
        float angle = cameraRig.rotation.eulerAngles.x;
        Vector3 objectPosition = heldObject.transform.position;

        //Set which guide to move towards
        if ((angle > 16f && angle < 180) || objPosLatch)
        {
            guidePos = fixedGuide.position;
            objPosLatch = true;
        }
        if (angle < 18 && objPosLatch) { objPosLatch = false; }

        // Sets the guide position offset values while wallrunning
        if (CharMovement.inWallRun && CharMovement.wallLeft)
        {
            guide.localPosition = Vector3.MoveTowards(guide.localPosition, defaultGuidePos - new Vector3(-wallRunOffSetX, 0, 0), 50f);
            fixedGuide.localPosition = Vector3.MoveTowards(fixedGuide.localPosition, defaultFixedGuidePos - new Vector3(-wallRunOffSetX, 0, 0), 50f);
            if (Vector3.Distance(guide.position, defaultGuidePos - new Vector3(-wallRunOffSetX, 0, 0)) <= 0.1f) guideLeft = false;
        }
        else if (CharMovement.inWallRun && CharMovement.wallRight)
        {
            guide.localPosition = Vector3.MoveTowards(guide.localPosition, defaultGuidePos - new Vector3(wallRunOffSetX, 0, 0), 50f);
            fixedGuide.localPosition = Vector3.MoveTowards(fixedGuide.localPosition, defaultFixedGuidePos - new Vector3(wallRunOffSetX, 0, 0), 50f);
            if (Vector3.Distance(guide.position, defaultGuidePos - new Vector3(wallRunOffSetX, 0, 0)) <= 0.1f) guideRight = false;
        }
        else if (Vector3.Distance(guide.position, defaultGuidePos) > 0.1f)
        {

            //Debug.Log("resetPos");
            guide.localPosition = Vector3.MoveTowards(guide.localPosition, defaultGuidePos, 50f);
            fixedGuide.localPosition = Vector3.MoveTowards(fixedGuide.localPosition, defaultFixedGuidePos, 50f);
            guideLeft = false; guideRight = false;
        }
        currentGuidePos = guide.localPosition;
        fixedGuide.rotation = heldObject.transform.rotation;


        //Moves the held object
        if (((guidePos == guide.position && !guide.GetComponent<objectTrigger>().onTrigger) || (guidePos == fixedGuide.position && !fixedGuide.GetComponent<objectTrigger>().onTrigger)) && !collisionDetector.isColliding)
        {
            heldObject.transform.position = guidePos;
            heldObject.transform.rotation = guide.rotation;
            //Debug.Log("Transform");
        }
        else
        {
            Vector3 moveDirection = (guidePos - new Vector3(objectPosition.x, objectPosition.y, objectPosition.z));
            float DM = Vector3.Distance(guidePos, heldObject.transform.position); //multiply force by distance
            if (DM > 12f) DM = 12f;

            if (!heldObject.GetComponent<CollisionDetector>().isColliding)
            {
                //rotate object when not colliding
                heldObject.transform.rotation = guide.rotation;
            }
            //move object with forces when colliding
            heldObject.GetComponent<Rigidbody>().AddForce(moveDirection.normalized * (moveForce * DM) / 2f);
            AllowObjMovement = false;

            //Debug.Log("addForce");
        }
        if (Vector3.Distance(player.position, heldObject.transform.position) > maxObjectDistance) DropObject();
    }


    void OnCollisionEnter(Collision collision)
    {
        contactPoint = collision.contacts[0].normal;
        standingObject = collision.gameObject;
        //Check if object is a door
        string name = collision.gameObject.name;
        if (name.Length > 15)
        {
            name = name.Remove(0, 13);
            int index = name.LastIndexOf("_");
            if (index >= 0) name = name.Substring(0, index);
            if (name == "Door")
            {
                if (!collision.gameObject.GetComponent<DoorSettings>().playerClose)
                {
                    AllowInteract = true;
                    DisplayText(collision.transform.gameObject);
                    doorObject = collision.gameObject;
                }
            }
        }
        else doorObject = null;

        if (Physics.BoxCast(collision.gameObject.transform.position, collision.gameObject.transform.localScale / 2.5f, player.up, out playerOnObjectHit, collision.gameObject.transform.rotation, 1f, playerLayer) && collision.gameObject.tag == "Object") 
        { isRigidbody = false; if (setRbObject == null || setRbObject != standingObject) { lastRbObject = setRbObject; setRbObject = standingObject; } }
        else
        {
            lastRbObject = setRbObject;
        }
         
        //Check for other collisisions
        if (collision.gameObject.layer != 0 //check the int value in layer manager(User Defined starts at 8) 
            && !isColliding)
        {
            isColliding = true;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 8
            && isColliding)
        {
            isColliding = false;
        }
        AllowInteract = false;
        doorObject = null;
    }

}
