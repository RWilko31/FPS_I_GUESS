using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InteractableData : MonoBehaviour
{
    /////------Variables------\\\\\
    [SerializeField] public string InteractableType;
    GameDataFile GameDataFile;
    WeaponManager WeaponManager;
    WeaponAndItemScript weaponAndItemScript;
    CharacterMovementV2 PlayerScript;
    DoorSettings doorSettings;

    #region Strings
    private string InteractableWeapon = "InteractableWeapon";
    private string InteractableCube = "InteractableCube";
    private string InteractableDoor = "InteractableDoor";
    private string Checkpoint = "Checkpoint";
    #endregion

    #region Door movement
    private float moveDoorBy, doorSpeed, doorHoldTime;
    private bool moveDoor, closeDoor, canClose, playerClose, triggerWhenNear, playerNear, timerBool;
    private bool resetStart, resetPos;
    private Vector3 startPos, openDirection;
    #endregion

    /////------Functions------\\\\\
    private void Awake()
    {
        GameDataFile = FindObjectOfType<GameDataFile>();
        WeaponManager = GameDataFile.player.GetComponent<WeaponManager>();
        weaponAndItemScript = GameDataFile.player.GetComponent<WeaponAndItemScript>();
        PlayerScript = GameDataFile.player.GetComponent<CharacterMovementV2>();
        moveDoorBy = GameDataFile.moveDoorBy;
        doorSpeed = GameDataFile.doorSpeed;
        doorHoldTime = GameDataFile.doorHoldTime;
        startPos = transform.position;
        IdentifyType();
    }
    private void IdentifyType()
    {
        //find the interactable based on the name of the object
        //remove any characters after " " to get interactable name/type
        string name = this.name;
        int index = name.IndexOf(" ");
        if (index >= 0) name = name.Substring(0, index);
        //check objects with specific names
        if (name == "Interactable_Object") InteractableType = InteractableCube;
        if (name == "CraftInterface") InteractableType = "CraftInterface";
        if (name == "Interactable_LevelStart") InteractableType = "Level Start";

        //check objects that have a single "_" to give other information
        index = name.IndexOf("_");
        if (index >= 0) name = name.Substring(0, index);
        //Debug.Log(name);
        if (name == "WeaponPickup") InteractableType = InteractableWeapon;

        //check objects that have 2 underscores
        name = this.name;
        index = name.LastIndexOf("_");
        if (index >= 0) name = name.Substring(0, index);
        if (name == "Interactable_Door") { InteractableType = InteractableDoor; SetDoorSettings(); }
    }
    private void Update()
    {
        if (GameDataFile.isPaused) return;

        if (triggerWhenNear && playerNear) OpenDoor();
        if (moveDoor)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos + (openDirection * moveDoorBy), Time.deltaTime * doorSpeed);
            if (Mathf.Abs(Vector3.Distance(startPos + (openDirection * moveDoorBy), transform.position)) - 0.1f <= 0) { StartCoroutine(HoldDoor()); }
            if (!resetStart) StartCoroutine(ResetDoor()); //if Door gets stuck reset after a few seconds;
        }
        if (closeDoor && (canClose || triggerWhenNear))
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * doorSpeed);
            if (transform.position == startPos) closeDoor = false;
        }
        if (resetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * doorSpeed);
            if (transform.position == startPos) resetPos = false;
        }
    }
    public void StartInteract()
    {
        // Start interact function for interactable type
        if (InteractableType == InteractableWeapon) { WeaponPickup(); }
        if (InteractableType == InteractableCube) Debug.Log("Jordan is OK");
        if (InteractableType == InteractableDoor) { OpenDoor(); }
        if (InteractableType == "CraftInterface") { GameDataFile.MMScript.OpenCrafting(); }
        if (InteractableType == "Level Start") { StartLevel(); }
        
    }
    private void StartLevel()
    {
        GameDataFile.MasterAudio.Stop();
        string levelName = this.name.Remove(0, 24);
        //Debug.Log(levelName);
        
        if (levelName == "Expectations")
        {
            GameDataFile.MasterAudio.clip = GameDataFile.Expectations;
            //GameDataFile.MasterAudio.volume = 0.5f;
            GameDataFile.MasterAudio.loop = false;
            GameDataFile.MasterAudio.Play();
        }
        else if (levelName == "Degredation")
        {

            Debug.Log("play");
            if (GameDataFile.MasterAudio.clip != GameDataFile.Degredation)
            {
                GameDataFile.MasterAudio.clip = GameDataFile.Degredation;
                //GameDataFile.MasterAudio.volume = 0.5f;
                GameDataFile.MasterAudio.loop = true;
                GameDataFile.MasterAudio.Play();
            }
            else
            { 
                GameDataFile.MasterAudio.Stop();
                GameDataFile.MasterAudio.clip = null;
            }
        }
        else{ GameDataFile.LoadLevel(levelName); }
    }
    void SetDoorSettings()
    {
        doorSettings = GetComponent<DoorSettings>();
        canClose = doorSettings.canClose;
        playerClose = doorSettings.playerClose;
        moveDoorBy = doorSettings.openDistance;
        doorHoldTime = doorSettings.holdTime;
        doorSpeed = doorSettings.doorSpeed;
        triggerWhenNear = doorSettings.triggerWhenNear;
        string getOpenDirection = doorSettings.openDirection;

        if (getOpenDirection == "Left") openDirection = -transform.right;
        if (getOpenDirection == "Right") openDirection = transform.right;
        if (getOpenDirection == "Up") openDirection = transform.up;
        if (getOpenDirection == "Down") openDirection = -transform.up;
        if (getOpenDirection == "Left + Up") openDirection = transform.up -transform.right;
        if (getOpenDirection == "Left + Down") openDirection = -transform.up -transform.right;
        if (getOpenDirection == "Right + Up") openDirection = transform.up + transform.right;
        if (getOpenDirection == "Right + Down") openDirection = -transform.up + transform.right;
    }
    void OpenDoor()
    {
        if (moveDoor || timerBool) return;
        if (playerClose) canClose = !canClose;
        moveDoor = true;
    }
    IEnumerator HoldDoor()
    {
        moveDoor = false;
        timerBool = true;
        yield return new WaitForSeconds(doorHoldTime);
        timerBool = false;
        closeDoor = true;
    }
    IEnumerator ResetDoor()
    {
        resetStart = true;
        yield return new WaitForSeconds(5f);
        if (moveDoor) { resetPos = true; moveDoor = false; }
        resetStart = false;
    }

    private void OnTriggerEnter(Collider other)
    { 
        playerNear = true;
    }
    private void OnTriggerExit(Collider other)
    { playerNear = false; }

    private void WeaponPickup()
    {
        //get weapon name
        string weaponName = this.name.Remove(0, 13);
        int index = weaponName.IndexOf(" ");
        if (index >= 0) weaponName = weaponName.Substring(0, index);
        //check if player already has weapon
        bool weaponCheck = WeaponManager.EquipedWeapons.Contains(weaponName);
        if (weaponCheck) 
        {   //add ammo to gun if player already has it
            //Check if the ammo neads adding to the current gun
            if (WeaponManager.currentWeapon == weaponName) { weaponAndItemScript.ReserveAmmo += weaponAndItemScript.MagSize; }
            else
            {
                //Debug.Log("Secondaryammo");
                int weaponSlot = (int) WeaponManager.checkCurrentWeapons(weaponName).x;
                //Add ammo to gun if it is not in use
                WeaponManager.WeaponInventory.WeaponSlot[weaponSlot].reserveAmmo += 
                    WeaponManager.WeaponList.WeaponTableRows[WeaponManager.WeaponInventory.WeaponSlot[weaponSlot].tableRow].magsize;
            }
            weaponAndItemScript.HUDTextUpdate();
        }
        else
        {   //If player doesnt have the gun add it to a weapon slot
            int slot = WeaponManager.NoOfWeaponSlots + 1;
            WeaponManager.SetPlayerWeapon(slot, weaponName);
            if(WeaponManager.NoOfWeaponSlots > 1)
            {
                int gunslot = (int) WeaponManager.checkCurrentWeapons(weaponName).x;
                WeaponManager.SwapWeapon(gunslot);
            }
        } 
    }
}
