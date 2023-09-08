using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Interact_Door : MonoBehaviour, IInteractFunction
{
    enum DoorDirection
    {
        Left,
        Right,
        Up,
        Down,
        LeftUp,
        LeftDown,
        RightUp,
        RightDown
    }

    //Set parameters for the door to be read by the InteractableData script
    [Header("Door settings")]
    [SerializeField] private bool canClose = true;
    [SerializeField] private bool isAutomatic;
    [SerializeField] private float moveDistance;
    [SerializeField] private float doorSpeed;
    [SerializeField] private float holdTime;
    [SerializeField] private DoorDirection openDirection;

    #region Door movement
    private bool openDoor, closeDoor, playerNear, timerBool;
    private bool resetStart, resetPos;
    private Vector3 startPos, openDir;

    bool isOpen = false;
    #endregion

    public void Interact()
    { 
        if (!isOpen) OpenDoor();
        else if(!isAutomatic) closeDoor = true;
    }
    public string InteractType()
    { return "Door"; }
    public string InteractText()
    { return "to open door"; }

    private void Awake()
    { SetDoorSettings(); }
    void SetDoorSettings()
    {
        switch (openDirection)
        {
            case DoorDirection.Left: openDir = -transform.right; break;
            case DoorDirection.Right: openDir = transform.right; break;
            case DoorDirection.Up: openDir = transform.up; break;
            case DoorDirection.Down: openDir = -transform.up; break;
            case DoorDirection.LeftUp: openDir = transform.up - transform.right; break;
            case DoorDirection.LeftDown: openDir = -transform.up - transform.right; break;
            case DoorDirection.RightUp: openDir = transform.up + transform.right; break;
            case DoorDirection.RightDown: openDir = -transform.up + transform.right; break;
        }
        openDir = openDir.normalized;
        startPos = transform.position;
    }
    private void Update()
    {
        if (gameState.paused) return;

        if (isAutomatic && playerNear) OpenDoor();
        if (openDoor)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos + (openDir * moveDistance), Time.deltaTime * doorSpeed);
            if (Mathf.Abs(Mathf.Round(Vector3.Distance(startPos + (openDir * moveDistance), transform.position))) <= 0.1f) 
            {
                if (isAutomatic) StartCoroutine(HoldDoor());
                else { openDoor = false; isOpen = true; }
            }
        }
        if (closeDoor && canClose)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * doorSpeed);
            if (Mathf.Abs(Mathf.Round(Vector3.Distance(startPos, transform.position))) <= 0.1f) { closeDoor = false; isOpen = false; } 
        }
        
    }

    void OpenDoor()
    {
        closeDoor = false;
        if (openDoor || timerBool) return;
        openDoor = true;
    }
    IEnumerator HoldDoor()
    {
        openDoor = false;
        timerBool = true;
        yield return new WaitForSeconds(holdTime);
        timerBool = false;
        closeDoor = true;
        Debug.Log("closing");
    }    

    private void OnTriggerEnter(Collider other)
    { playerNear = true; }
    private void OnTriggerExit(Collider other)
    { playerNear = false; }

}
