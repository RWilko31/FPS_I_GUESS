using UnityEngine;

public enum objectType
{   
    none = 0,
    Object = 1,
    AntiGravity = 2,
    TeleportObject = 3,
    TeleportPlayer = 4,
    Scale = 5,
    Launch = 6,
    Interactable = 7
}
public class InteractableObjects : MonoBehaviour
{
    //Int number to define the object ability. 0 = no ability, 1 = AntiGravity, 2 = Teleport object, 3 = Teleport Player, 4 = Scale, 5 = Launch Object/Player, 6 = Bouncy;
    objectType ObjectType = 0;
    public string ObjectTag;

    [SerializeField] public GameObject F;
    [SerializeField] public GameObject heldObject;
    [SerializeField] private Transform guide;
    [SerializeField] private Transform fixedGuide;
    [SerializeField] private GameObject standingObject;
    [SerializeField] private GameObject setRbObject;
    [SerializeField] private GameObject lastRbObject;
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

    //private float wallRunOffSetX = 0.5f;
    [SerializeField] private Vector3 defaultGuidePos;
    [SerializeField] private Vector3 currentGuidePos;
    private Vector3 defaultFixedGuidePos;
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
    InputController IC;

    private void Awake()
    {
        CharMovement = GetComponent<CharacterMovementV2>();
        GameDataFile =  FindObjectOfType<GameDataFile>();
        WeaponAndItemScript = GetComponent<WeaponAndItemScript>();
        InteractableData = FindObjectOfType<InteractableData>();
        IC = GameDataFile.GetComponent<InputController>();
        subEvents();
    }
    private void subEvents() //subscribe to required input events
    {
        IC.action += Interact;
        IC.item1 += Activate;
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

        guidePos = guide.localPosition;
        defaultGuidePos = guide.localPosition;
        defaultFixedGuidePos = fixedGuide.localPosition;

        allInteractables = GameObject.FindGameObjectsWithTag("Interactable");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.paused) return;

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
            float closestObjDist = Mathf.Infinity;
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
                            DisplayText(closestObj.gameObject);
                        }
                    }
                }
            }
            else
            {
                closestObj = null;
                interactedObject = null;
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

        if (WeaponAndItemScript.grappleObject != null) 
            WeaponAndItemScript.grappleObject.GetComponent<Rigidbody>().AddForce((player.position - WeaponAndItemScript.grappleObject.transform.position).normalized * 10f);
    }

    public void Interact()
    {
        if (heldObject == null)
        { 
            if (closestObj == null) { ObjectTag = null; return; }
            else { interactedObject = closestObj.gameObject; ObjectTag = closestObj.transform.tag; }
            getType();
            //Debug.Log(ObjectTag);
            if (ObjectType != objectType.Interactable && ObjectType != objectType.none) { PickUpObject(closestObj.gameObject); }
            if (AllowInteract && ObjectType == objectType.none) { doorObject.transform.gameObject.GetComponent<InteractableData>().StartInteract(); AllowInteract = false; }
            if ((ObjectType == objectType.Interactable && interactedObject != null)) {interactedObject.transform.gameObject.GetComponent<InteractableData>().StartInteract(); interactedObject = null; }
        }
        else DropObject();
    }
    public void Activate()
    {
        if (closestObj == null) { ObjectTag = null; return; }
        else { interactedObject = closestObj.gameObject; ObjectTag = closestObj.transform.tag; }
        getType();
        if (ObjectType != objectType.Object && ObjectType != objectType.none && ObjectType != objectType.Interactable)
        {
            interactedObject.transform.gameObject.GetComponent<InteractableData>().ActivateEffect(ObjectType); 
            interactedObject = null; 
        }
    }
    void getType()
    {
        switch (ObjectTag)
        {
            case "Object": ObjectType = objectType.Object; break;
            case "Antigravity": ObjectType = objectType.AntiGravity; break;
            case "TeleportObject": ObjectType = objectType.TeleportObject; break;
            case "TeleportPlayer": ObjectType = objectType.TeleportPlayer; break;
            case "Scale": ObjectType = objectType.Scale; break;
            case "Launch": ObjectType = objectType.Launch; break;
            case "Interactable": ObjectType = objectType.Interactable; break;
            default: ObjectType = objectType.none; break;
        }
    }
    void PickUpObject(GameObject viewedObject)
    {
        Rigidbody objRig = viewedObject.GetComponent<Rigidbody>();
        objRig.interpolation = RigidbodyInterpolation.Interpolate;
        objRig.useGravity = false;
        objRig.drag = 10;
        objRig.velocity = new Vector3(0, 0, 0);
        heldObject = viewedObject;
        collisionDetector = heldObject.GetComponent<CollisionDetector>();
    }
    void DropObject()
    {
        Rigidbody heldRig = heldObject.GetComponent<Rigidbody>();
        heldRig.interpolation = RigidbodyInterpolation.None;
        heldRig.useGravity = true;
        heldRig.drag = 0;

        //Apply camera force to object
        heldRig.velocity += CamVelocity / 1f;
        heldObject = null;
        collisionDetector = null;
    }
    void MoveObject()
    {
        ///Rotation of heldObject
        //Prevents infinite rotation of the object when held
        float velocitySpeed = 2f;
        Vector3 objectVelocity = new Vector3(0, 0, 0);
        Vector3 angularVelocity = heldObject.GetComponent<Rigidbody>().angularVelocity;

        if (angularVelocity.x > 0) objectVelocity.x = angularVelocity.x - (velocitySpeed * Time.deltaTime);
        else objectVelocity.x = angularVelocity.x + (velocitySpeed * Time.deltaTime);
        if (angularVelocity.y > 0) objectVelocity.y = angularVelocity.y - (velocitySpeed * Time.deltaTime);
        else objectVelocity.y = angularVelocity.y + (velocitySpeed * Time.deltaTime);
        if (angularVelocity.z > 0) objectVelocity.z = angularVelocity.z - (velocitySpeed * Time.deltaTime);
        else  objectVelocity.z = angularVelocity.z + (velocitySpeed * Time.deltaTime);

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
        //if (CharacterState.inWallRun && CharMovement.wallLeft)
        //{
        //    guide.localPosition = Vector3.MoveTowards(guide.localPosition, defaultGuidePos - new Vector3(-wallRunOffSetX, 0, 0), 50f);
        //    fixedGuide.localPosition = Vector3.MoveTowards(fixedGuide.localPosition, defaultFixedGuidePos - new Vector3(-wallRunOffSetX, 0, 0), 50f);
        //}
        //else if (CharacterState.inWallRun && CharMovement.wallRight)
        //{
        //    guide.localPosition = Vector3.MoveTowards(guide.localPosition, defaultGuidePos - new Vector3(wallRunOffSetX, 0, 0), 50f);
        //    fixedGuide.localPosition = Vector3.MoveTowards(fixedGuide.localPosition, defaultFixedGuidePos - new Vector3(wallRunOffSetX, 0, 0), 50f);
        //}
        if (Vector3.Distance(guide.position, defaultGuidePos) > 0.1f)
        {
            //Debug.Log("resetPos");
            guide.localPosition = Vector3.MoveTowards(guide.localPosition, defaultGuidePos, 50f);
            fixedGuide.localPosition = Vector3.MoveTowards(fixedGuide.localPosition, defaultFixedGuidePos, 50f);
        }
        currentGuidePos = guide.localPosition;
        fixedGuide.rotation = heldObject.transform.rotation;


        //Moves the held object
        if (((guidePos == guide.position && !guide.GetComponent<objectTrigger>().onTrigger) || (guidePos == fixedGuide.position && !fixedGuide.GetComponent<objectTrigger>().onTrigger)) && !collisionDetector.isColliding)
        {
            heldObject.transform.position = guidePos;
            heldObject.transform.rotation = guide.rotation;
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
        { if (setRbObject == null || setRbObject != standingObject) { lastRbObject = setRbObject; setRbObject = standingObject; } }
        else
        { lastRbObject = setRbObject; }
         
        //Check for other collisisions
        if (collision.gameObject.layer != 0 && !isColliding) //check the int value in layer manager(User Defined starts at 8) 
        {  isColliding = true; }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 8 && isColliding)
        { isColliding = false; }
        AllowInteract = false;
        doorObject = null;
    }

}
