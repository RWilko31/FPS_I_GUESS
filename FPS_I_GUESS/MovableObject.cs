// Object movement script by Robert Wilkinson.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    public Transform guide;
    public Transform player;
    public Transform playerCam;
    public LayerMask Object;
    public LayerMask ground;
    //Int number to define the object ability. 0 = no ability, 1 = AntiGravity, 2 = Teleport object, 3 = Teleport Player, 4 = Scale, 5 = Launch Object/Player, 6 = Bouncy;
    int ObjectType = 0;
    public String ObjectName; 

    float maxObjectDistance = 5f;
    float moveForce = 250f;
    private GameObject heldObject;
    private Vector3 objectSize;
    private Quaternion stopRotation;
    private Vector3 lastPosition;
    private Vector3 currentPosition;
    private Vector3 CamVelocity;
    private Vector3 lastRotation;
    private Vector3 currentRotation;
    private Vector3 CamAngularVelocity;
    public float antiGravityFlipTime = 0.75f;
    bool isColliding;
    bool teleportObject = false;
    bool isRigidbody = true;

    //Antigravity
    public float antiGravityCeilingLimit = 100f;
    bool ObjectAntiGravityOff = true;
    bool useAntiGravityGravityScale = false;
    float antiGravityRotate;
    private GameObject BufferObject;

    //Scale
    bool StartScale = false;

    //Launch
    public float FlingForce = 3000f;

    //Bouncy
    bool isBouncy = false;
    bool StartBounce;
    private Vector3 contactPoint;
    private GameObject HoldBounceObject;
    public PhysicMaterial BounceMaterial;
    public PhysicMaterial None;

    RaycastHit playerOn;
    

    void start()
    {
        lastPosition = playerCam.position;
        lastRotation = playerCam.eulerAngles;
        
    }
    void FixedUpdate()
    {
        //calculate cam velocity every frame
        currentPosition = playerCam.position;
        CamVelocity = (currentPosition - lastPosition) / Time.deltaTime;
        lastPosition = currentPosition;

        //calculate cam angular velocity every frame
        currentRotation = playerCam.eulerAngles;
        CamAngularVelocity = (currentRotation - lastRotation) / Time.deltaTime;
        lastRotation = currentRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (useAntiGravityGravityScale)
        {
            BufferObject.GetComponent<Rigidbody>().AddForce(Physics.gravity * (BufferObject.GetComponent<Rigidbody>().mass * BufferObject.GetComponent<Rigidbody>().mass) * -1f);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(playerCam.transform.position, playerCam.forward, out hit, maxObjectDistance, Object))
                {
                    if (!isRigidbody)
                    {
                        GameObject thisObject = playerOn.transform.gameObject;
                        Rigidbody newRigidbody = thisObject.AddComponent<Rigidbody>();
                        isRigidbody = true;
                    }
                    ObjectName = hit.transform.gameObject.tag;
                    if (ObjectName == "NoAbility") ObjectType = 0;
                    if (ObjectName == "AntiGravity") ObjectType = 1;
                    if (ObjectName == "TeleportObject") ObjectType = 2;
                    if (ObjectName == "TeleportPlayer") ObjectType = 3;
                    if (ObjectName == "Scale") ObjectType = 4;
                    if (ObjectName == "Launch") ObjectType = 5;
                    if (ObjectName == "Bouncy") ObjectType = 6;
                    PickUpObject(hit.transform.gameObject);
                }
            }
            else
            {
                DropObject();
            }
        }    

        if (Input.GetKeyDown(KeyCode.Q) && heldObject != null) {
            if (ObjectType == 1) AntiGravity();
            if (ObjectType == 2) TeleportObject(); teleportObject = true;
            if (ObjectType == 3) TeleportPlayer();
            if (ObjectType == 4) Scale();
            if (ObjectType == 5) Launch();
            if (ObjectType == 6) Bouncy();
        }

        if (heldObject != null && !isBouncy) {
            MoveObject();
        }
        if(isBouncy){
            //BounceObject();
        }
    }

    IEnumerator Rotate(Vector3 axis, float angle, float duration)
    {
        Quaternion from = BufferObject.transform.rotation;
        Quaternion to = from;
        to *= Quaternion.Euler(0f * angle, 0f * angle, 1f * angle);

        float elapsed = 0.0f;
        while (elapsed <= duration)
        {
            BufferObject.transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        BufferObject.transform.rotation = to;
    }

    void OnCollisionEnter(Collision collision)
    {
        contactPoint = collision.contacts[0].normal;
        if (Physics.Raycast(player.position, -player.up, out playerOn, 1f, Object)) {
            string LaunchTag = playerOn.transform.gameObject.tag;
            if (LaunchTag == "Launch") player.GetComponent<Rigidbody>().AddForce(((player.up * 250f) + (playerCam.forward * 125f)) * FlingForce);
            else {
                Destroy(GetComponent<Rigidbody>());
                isRigidbody = false;
            }
        }
           
        if (collision.gameObject.layer != 0 //check the int value in layer manager(User Defined starts at 8) 
            && !isColliding)
        {
            isColliding = true;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        
        if (!isRigidbody){
            GameObject thisObject = playerOn.transform.gameObject;
            Rigidbody newRigidbody =thisObject.AddComponent<Rigidbody>();
            isRigidbody = true;
        }
            
        if (collision.gameObject.layer == 8
            && isColliding)
        {
            isColliding = false;
        }
    }

    void MoveObject()
    {
        
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

        //scale change of object
        if (StartScale)
        {
            float ScaleBy = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 20f;
            Vector3 CurrentScale = heldObject.transform.localScale;
            heldObject.transform.localScale = new Vector3(CurrentScale.x + ScaleBy, CurrentScale.y + ScaleBy, CurrentScale.z + ScaleBy);
        }

        // Sets the object position offset values while wallrunning
        float wallRunOffSetY = 0f;
        float wallRunOffSetX = 0f;
        Vector3 objectPosition = heldObject.transform.position;

        if (player.GetComponent<PlayerMovement>().isWallRunning) {
                wallRunOffSetY = 0f;
            if (player.GetComponent<PlayerMovement>().isWallRight)
                wallRunOffSetX = 0.5f;
            else
                wallRunOffSetX = -0.5f;  
        }
            
        if (isColliding) {
            if (player.GetComponent<PlayerMovement>().antiGravityOff) { 
                //when colliding with the floor (prevents clipping through floor)
                if ((heldObject.transform.position.y - (heldObject.transform.localScale.y / 2f)) < (player.transform.position.y - (player.transform.localScale.y / 2f) - 0.25f))
                    heldObject.transform.position = new Vector3(guide.position.x + wallRunOffSetX, (player.transform.position.y - ((player.transform.localScale.y + wallRunOffSetY) / 2f) + (heldObject.transform.localScale.y / 2f)), guide.transform.position.z);
                else { 
                //when colliding with anything else
                    Vector3 moveDirection = (guide.position - new Vector3(objectPosition.x + wallRunOffSetX, objectPosition.y + wallRunOffSetY, objectPosition.z));
                    heldObject.GetComponent<Rigidbody>().AddForce(moveDirection * moveForce / 5f);
                }
            }
            else { 
                //when colliding with floor while in antigravity (prevents clipping through floor) 
                if ((heldObject.transform.position.y + (heldObject.transform.localScale.y / 2f)) > (player.transform.position.y + (player.transform.localScale.y / 2f) + 0.25f))
                    heldObject.transform.position = new Vector3(guide.position.x + wallRunOffSetX, (player.transform.position.y + ((player.transform.localScale.y - wallRunOffSetY) / 2f) - (heldObject.transform.localScale.y / 2f)), guide.transform.position.z);
                else { 
                //when colliding with anything else while in antigravity 
                    Vector3 moveDirection = (guide.position - new Vector3(objectPosition.x + wallRunOffSetX, objectPosition.y - wallRunOffSetY, objectPosition.z));
                    heldObject.GetComponent<Rigidbody>().AddForce(moveDirection * moveForce / 5f);
                }
            }   
        }
        else {
            //Moves the object while not colliding 
            if (player.GetComponent<PlayerMovement>().antiGravityOff) {
                //These If-Else statement prvent the object oscillating while just above the ground
                if ((heldObject.transform.position.y - (heldObject.transform.localScale.y / 2f)) >= (player.transform.position.y - (player.transform.localScale.y / 2f) + 0.05f))
                    heldObject.transform.position = new Vector3(guide.position.x + wallRunOffSetX, guide.position.y + wallRunOffSetY, guide.position.z);
                else {
                    heldObject.transform.position = new Vector3(guide.position.x + wallRunOffSetX, (player.transform.position.y - (player.transform.localScale.y / 2f) + (heldObject.transform.localScale.y / 2f)), guide.transform.position.z);
                }
            }
            else {
                if ((heldObject.transform.position.y + (heldObject.transform.localScale.y / 2f)) <= (player.transform.position.y + (player.transform.localScale.y / 2f) + 0.05f))
                    heldObject.transform.position = new Vector3(guide.position.x + wallRunOffSetX, guide.position.y - wallRunOffSetY, guide.position.z);
                else {
                    heldObject.transform.position = new Vector3(guide.position.x + wallRunOffSetX, (player.transform.position.y + (player.transform.localScale.y / 2f) - (heldObject.transform.localScale.y / 2f)), guide.transform.position.z);
                }
            }
            
        }
    }

    void PickUpObject(GameObject viewedObject)
    {
        objectSize = viewedObject.transform.localScale;
        Rigidbody objRig = viewedObject.GetComponent<Rigidbody>();
        objRig.useGravity = false;
        objRig.drag = 10;
        objRig.transform.parent = guide;
        objRig.velocity = new Vector3(0, 0, 0);
        heldObject = viewedObject;
            
    }

    void DropObject()
    {
        Rigidbody heldRig = heldObject.GetComponent<Rigidbody>();
        heldRig.useGravity = true;
        heldRig.drag = 0;
        StartScale = false;

        //fix scale change of object
        if (ObjectType != 4) heldRig.transform.localScale = objectSize;

        heldObject.transform.parent = null;

        //Apply camera force to object
        heldRig.velocity = CamVelocity * 1.2f;
        heldRig.angularVelocity = CamAngularVelocity * 1.2f;
        heldObject = null;
        teleportObject = false;
    }

    void TeleportObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.position, playerCam.forward, out hit, 100000f, ground))
        {
            heldObject.GetComponent<Transform>().position = hit.point;
            DropObject();
        }
    }

    void TeleportPlayer()
    {
        Vector3 ObjectPosition = heldObject.GetComponent<Transform>().position;
        player.transform.position = new Vector3(ObjectPosition.x, ObjectPosition.y, ObjectPosition.z + 10f);
        DropObject(); 
    }

    void AntiGravity()
    {
        Vector3 ObjectPosition = heldObject.GetComponent<Transform>().position;
        BufferObject = heldObject;
        RaycastHit hit;
        if (Physics.Raycast(ObjectPosition, heldObject.GetComponent<Transform>().up, out hit, antiGravityCeilingLimit, ground)) {
            Vector3 antiGravityPoint = hit.point;
            float antiGravityDistance = Vector3.Distance(ObjectPosition, antiGravityPoint);
            //Changes variables depending on if character is on ceiling or ground
            if (antiGravityDistance < antiGravityCeilingLimit) {
                if (ObjectAntiGravityOff == false) {
                    useAntiGravityGravityScale = false;
                    heldObject.GetComponent<Rigidbody>().useGravity = true;
                    ObjectAntiGravityOff = true;
                    antiGravityRotate = -180f;
                }
                else {
                    useAntiGravityGravityScale = true;
                    heldObject.GetComponent<Rigidbody>().useGravity = false;
                    ObjectAntiGravityOff = false;
                    antiGravityRotate = 180f;
                }
                //Transform position
                //heldObject.GetComponent<Rigidbody>().AddForce(ObjectPosition * (antiGravityDistance / antiGravityFlipTime));
                //Rotate Object
                StartCoroutine(Rotate(new Vector3(0, 1, 0), antiGravityRotate, antiGravityFlipTime));
                DropObject();
            }
        }
    }

    void Scale()
    {
        StartScale = !StartScale;
    }

    void Launch()
    {
    }

    void Bouncy()
    {
        isBouncy = !isBouncy;
        if (isBouncy) {
            HoldBounceObject = heldObject;
            HoldBounceObject.transform.parent = null;
            HoldBounceObject.GetComponent<Rigidbody>().drag = 0;
            HoldBounceObject.GetComponent<Rigidbody>().mass = 1f;
            HoldBounceObject.GetComponent<Collider>().material = (PhysicMaterial)Resources.Load("PhysicMaterials/BounceMaterial");
            HoldBounceObject.GetComponent<Rigidbody>().useGravity = true;
            HoldBounceObject.GetComponent<Rigidbody>().AddForce(playerCam.forward * 15f);
        }
        else
        {
            heldObject = HoldBounceObject;
            HoldBounceObject.GetComponent<Collider>().material = (PhysicMaterial)Resources.Load("none");
            heldObject.GetComponent<Rigidbody>().drag = 10;
            heldObject.GetComponent<Rigidbody>().mass = 1;
            heldObject.transform.parent = guide;
            heldObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        } 
    }
    void BounceObject()
    {
        Vector3 AddVelocity = HoldBounceObject.GetComponent<Rigidbody>().velocity;
        if (HoldBounceObject.GetComponent<Rigidbody>().velocity.x < 1)
            AddVelocity.x = (HoldBounceObject.GetComponent<Rigidbody>().velocity.x * 2f);
        if (HoldBounceObject.GetComponent<Rigidbody>().velocity.y < 1)
            AddVelocity.y = (HoldBounceObject.GetComponent<Rigidbody>().velocity.y * 2f);
        if (HoldBounceObject.GetComponent<Rigidbody>().velocity.z < 1)
            AddVelocity.z = (HoldBounceObject.GetComponent<Rigidbody>().velocity.z * 2f);
        HoldBounceObject.GetComponent<Rigidbody>().AddForce(AddVelocity);
    }
 
}
