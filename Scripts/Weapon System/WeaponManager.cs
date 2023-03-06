using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class WeaponManager : MonoBehaviour
{
    //Reads weapon data from a csv file and stores the relevant data in an array of weapon slots.
    //stores old weapon counts in a seperate csv file.

    /////-----Variables-----\\\\\
    [SerializeField] public string currentWeapon;
    [SerializeField] public TextAsset weaponTableCSV;
    [SerializeField] public int No_columns;
    private int maxSlots = 4;
    [SerializeField] public int setMaxSlots = 4;
    [SerializeField] public bool limitslots = false;

    [SerializeField] public readWeaponList WeaponList = new readWeaponList();
    [SerializeField] public WeaponsInventory WeaponInventory = new WeaponsInventory();
    [SerializeField] public GameObject WeaponContainer;
    [SerializeField] public List<GameObject> WeaponModels;
    [SerializeField] public List<string> EquipedWeapons = new List<string>();
    [SerializeField] public List<string> debugList = new List<string>();

    public class readWeaponList //creates an array for the table data
    { public WeaponTableData[] WeaponTableRows; }
    public class WeaponTableData //Stores the table data for each weapon
    {
        public string weapon;
        public int magsize;
        public int reserveAmmo;
        public float shootTime;
        public float reloadTime;
        public float swapTime;
        public string fireType;
        public float aimPos_x;
        public float aimPos_y;
        public float aimPos_z;
        public float hipPos_x;
        public float hipPos_y;
        public float hipPos_z;
    }
    public class WeaponsInventory  //Creates array of weapon slots
    { public CreateWeaponSlot[] WeaponSlot; }
    public class CreateWeaponSlot //stores data for the weapon slot. Instantiated for each weapon
    {
        public string weapon;
        public int tableRow;
        public int magAmmo;
        public int reserveAmmo;
        public GameObject weaponModel;
    }
    private void ReadCSV() //Reads the weaponTable.csv file to get weapon info
    {
        string[] data = weaponTableCSV.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);
        int tableSize = data.Length / No_columns - 1;
        WeaponList.WeaponTableRows = new WeaponTableData[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            WeaponList.WeaponTableRows[i] = new WeaponTableData();
            WeaponList.WeaponTableRows[i].weapon = data[No_columns * (i + 1)];
            WeaponList.WeaponTableRows[i].magsize = int.Parse(data[No_columns * (i + 1) + 1]);
            WeaponList.WeaponTableRows[i].reserveAmmo = int.Parse(data[No_columns * (i + 1) + 2]);
            WeaponList.WeaponTableRows[i].shootTime = float.Parse(data[No_columns * (i + 1) + 3]);
            WeaponList.WeaponTableRows[i].reloadTime = float.Parse(data[No_columns * (i + 1) + 4]);
            WeaponList.WeaponTableRows[i].swapTime = float.Parse(data[No_columns * (i + 1) + 5]);
            WeaponList.WeaponTableRows[i].fireType = data[No_columns * (i + 1) + 6];
            WeaponList.WeaponTableRows[i].aimPos_x = float.Parse(data[No_columns * (i + 1) + 7]);
            WeaponList.WeaponTableRows[i].aimPos_y = float.Parse(data[No_columns * (i + 1) + 8]);
            WeaponList.WeaponTableRows[i].aimPos_z = float.Parse(data[No_columns * (i + 1) + 9]);
            WeaponList.WeaponTableRows[i].hipPos_x = float.Parse(data[No_columns * (i + 1) + 10]);
            WeaponList.WeaponTableRows[i].hipPos_y = float.Parse(data[No_columns * (i + 1) + 11]);
            WeaponList.WeaponTableRows[i].hipPos_z = float.Parse(data[No_columns * (i + 1) + 12]);
            //Debug.Log(data[No_columns * (i + 1)]); //Reads out each weaponName
        }
    }
    private void LoadWeaponData() //reads the data loaded by GDFile and then sets weapon data
    {
        List<string[]> readData = new List<string[]>();
        readData = GDFile.Read_ammodata;
        int slot = 0;
        debugList.Clear();
        foreach(string[] data in readData)
        {
            debugList.Add(data[0]);
            if (data[0] != "ammodata" && data[0] != "weapon") //should just remove from list but glitches
            {
                SetPlayerWeapon(slot, data[0]);//equip old weapon
                WeaponInventory.WeaponSlot[slot].magAmmo = int.Parse(data[1]); //give old mag ammo
                WeaponInventory.WeaponSlot[slot].reserveAmmo = int.Parse(data[2]); //give old reserve ammo
                slot++;
            }
        }
        string[] getprimary = GDFile.Read_weapondata[2];
        DeactivateWeapon(); //hide set weapons
        SetWeaponActive((int) checkCurrentWeapons(getprimary[0]).x); //activate last held weapon;
    }
    public List<string[]> saveWeaponData() //puts the curent weapon data in an array to write to the save file
    {
        CycleWeapons(); //Cycle weapons to get most recent ammo data from attack script
        List<string[]> weapondata = new List<string[]>();

        string[] tag = new string[1]; //add section tag
        tag[0] = "ammodata"; 
        weapondata.Add(tag);
        string[] headings = new string[3]; //add table headings
        headings[0] = "weapon"; headings[1] = "ammo"; headings[2] = "reserve";
        weapondata.Add(headings);

        foreach(string weapon in EquipedWeapons) //add weapon data
        {
            int slot = (int)checkCurrentWeapons(weapon).x;
            int magAmmo = WeaponInventory.WeaponSlot[slot].magAmmo;
            int reserveAmmo = WeaponInventory.WeaponSlot[slot].reserveAmmo;
            string[] data = new string[3];
            data[0] = weapon; data[1] = magAmmo.ToString(); data[2] = reserveAmmo.ToString();
            weapondata.Add(data);
        }
        string[] end = new string[1]; //add end identifier
        end[0] = "*end*";
        weapondata.Add(end);
        return weapondata;
    }

    #region CurrentWeaponData
    [SerializeField] public int NoOfWeaponSlots;
    [SerializeField] public int currentWeaponSlot = 0;
    [SerializeField] public int GrappleGunSlot = 0;
    #endregion

    #region CurrentLethalData
    [SerializeField] public string currentLethal = null;
    [SerializeField] public GameObject lethal_Model;
    [SerializeField] public float lethal_ThrowDistance;
    [SerializeField] public float lethal_Radius;
    [SerializeField] public float lethal_Time;
    [SerializeField] public int NoOfLethals = 0;
    #endregion

    #region scripts
    GameDataFile GDFile;
    WeaponAndItemScript AttackScript;
    #endregion

    /////-----Functions-----\\\\\
    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        AttackScript = GetComponent<WeaponAndItemScript>();
        WeaponList = new readWeaponList();
        WeaponInventory = new WeaponsInventory();
        //weaponTableCSV = GDFile.WeaponTableCsv;
        //setMaxSlots = GDFile.setMaxSlots;
        //limitslots = GDFile.limitSlots;
    }
    public void startWeaponManager()
    {
        WeaponModels.Clear();
        foreach (Transform model in WeaponContainer.transform) //finds all weapon models on player
        { WeaponModels.Add(model.gameObject); }

        //Reset if quit and reloading
        clearValues();

        //creating max number of slots allows the max amount the player can hold to be changed at run time up to the amount of models present.
        //extra models cannot be added on top without restarting
        WeaponInventory.WeaponSlot = new CreateWeaponSlot[WeaponModels.Count]; //Creates all weapon slots for player

        if (GDFile.loadData) //load previous slot value
        {
            string[] getData = GDFile.Read_weapondata[2];
            maxSlots = int.Parse(getData[1]);
        }
        //creates new slot data  if not loading
        else if (!limitslots) maxSlots = WeaponModels.Count + 1; //sets max amount of assignable slots
        else maxSlots = setMaxSlots; //limits the slots if chosen 

        ReadCSV(); //Reads weapon table csv and stores data in array

        if (GDFile.loadData) LoadWeaponData();
        //if not loading data use these to set weapons on start
        else
        {
            //SetPlayerWeapon(1,"pistol");
            //SetPlayerWeapon(2,"shotGun");
            //SetPlayerWeapon(3,"miniGun");
            //SetPlayerWeapon(4,"sniper");
        }

        lethal_Model = GDFile.grenade_Model;
        NoOfLethals = GDFile.grenade_MaxAmount;
        lethal_ThrowDistance = GDFile.grenade_ThrowDistance;
        lethal_Radius = GDFile.grenade_Radius;
        lethal_Time = GDFile.grenade_Time;
        currentLethal = "grenade";
    }
    public Vector2 checkCurrentWeapons(string weapon) //returns the slot with the weapon in and the current number of used slots 
    {
        int No_Slots = 0;
        int weaponSlot = 0;
        int usedSlots = 0;
        foreach(CreateWeaponSlot slot in WeaponInventory.WeaponSlot)
        {
            No_Slots++;
            if (slot != null) {  usedSlots++; if (slot.weapon == weapon) weaponSlot = No_Slots - 1; }
        }
        Vector2 slotData = new Vector2(weaponSlot, usedSlots);
        NoOfWeaponSlots = usedSlots;
        return slotData;
    }
    public void SetPlayerWeapon(int weaponSlot, string weaponName) //assigns the weapon data to the corresponding weapon slot
    {
        if (weaponSlot >= maxSlots)
        {
            //Debug.Log("replacing current weapon");
            weaponSlot = currentWeaponSlot;
            DeactivateWeapon();
            EquipedWeapons.Remove(currentWeapon);
        }
        else NoOfWeaponSlots++;
        int rows = WeaponList.WeaponTableRows.Length;
        int weaponRow = 0;
        if (weaponName == "pistol") GrappleGunSlot = weaponSlot;

        for (int i = 0; i <= rows - 1; i++)
        {
            weaponRow = i;
            if (WeaponList.WeaponTableRows[i].weapon == weaponName) { break; }
        }
        if (weaponRow >= WeaponList.WeaponTableRows.Length) { Debug.Log("failed to assign weapon " + weaponName); return; }
        //Debug.Log(WeaponInventory.WeaponSlot.Length);
        WeaponInventory.WeaponSlot[weaponSlot] = new CreateWeaponSlot();
        WeaponInventory.WeaponSlot[weaponSlot].tableRow = weaponRow;
        WeaponInventory.WeaponSlot[weaponSlot].weapon = WeaponList.WeaponTableRows[weaponRow].weapon;
        WeaponInventory.WeaponSlot[weaponSlot].magAmmo = WeaponList.WeaponTableRows[weaponRow].magsize;
        WeaponInventory.WeaponSlot[weaponSlot].reserveAmmo = WeaponList.WeaponTableRows[weaponRow].reserveAmmo;
        WeaponInventory.WeaponSlot[weaponSlot].weaponModel = WeaponModels.Where(obj => obj.name == weaponName).SingleOrDefault();
        EquipedWeapons.Add(weaponName);
        if (NoOfWeaponSlots <= 1) SetWeaponActive(weaponSlot);
        //Debug.Log("Assigned " + weaponName + " to slot " + weaponSlot);
    }
    public void SetWeaponActive(int weaponSlot) //sets the selected weapon active for use in game
    {
        int tableSlot = WeaponInventory.WeaponSlot[weaponSlot].tableRow;
        Vector3 aim = Vector3.zero;
        Vector3 hip = Vector3.zero;

        currentWeapon = WeaponInventory.WeaponSlot[weaponSlot].weapon;
        currentWeaponSlot = weaponSlot;
        WeaponInventory.WeaponSlot[weaponSlot].weaponModel.SetActive(true);
        WeaponInventory.WeaponSlot[weaponSlot].weaponModel.GetComponent<WeaponData>().crosshair.SetActive(true);
        AttackScript.currentWeapon = WeaponInventory.WeaponSlot[weaponSlot].weaponModel;
        AttackScript.currentCrosshair = WeaponInventory.WeaponSlot[weaponSlot].weaponModel.GetComponent<WeaponData>().crosshair;
        AttackScript.currentWeaponFireAudio = WeaponInventory.WeaponSlot[weaponSlot].weaponModel.GetComponent<WeaponData>().fireAudio;
        AttackScript.MagSize = WeaponList.WeaponTableRows[tableSlot].magsize;
        AttackScript.Ammocount = WeaponInventory.WeaponSlot[weaponSlot].magAmmo;
        AttackScript.ReserveAmmo = WeaponInventory.WeaponSlot[weaponSlot].reserveAmmo;
        AttackScript.reloadTime = WeaponList.WeaponTableRows[tableSlot].reloadTime;
        AttackScript.shootTime = WeaponList.WeaponTableRows[tableSlot].shootTime;
        AttackScript.swapTime = WeaponList.WeaponTableRows[tableSlot].swapTime;
        AttackScript.fireType = WeaponList.WeaponTableRows[tableSlot].fireType;
        hip.x = WeaponList.WeaponTableRows[tableSlot].hipPos_x;
        hip.y = WeaponList.WeaponTableRows[tableSlot].hipPos_y;
        hip.z= WeaponList.WeaponTableRows[tableSlot].hipPos_z;
        aim.x = WeaponList.WeaponTableRows[tableSlot].aimPos_x;
        aim.y = WeaponList.WeaponTableRows[tableSlot].aimPos_y;
        aim.z= WeaponList.WeaponTableRows[tableSlot].aimPos_z;

        AttackScript.aimPos = new Vector3(aim.x, aim.y,aim.z);
        AttackScript.hipPos = new Vector3(hip.x, hip.y, hip.z);
        AttackScript.HUDTextUpdate();
    }
    public void CycleWeapons()
    {
        if (NoOfWeaponSlots == 0) return;
        int weaponPos = 0;
        foreach (string weapon in EquipedWeapons)
        {
            if (weapon == currentWeapon) { weaponPos++; break; }
            weaponPos++;
        }
        if (weaponPos == EquipedWeapons.Count) weaponPos = 0;
        //Debug.Log(EquipedWeapons[weaponPos]); //gives the next equpied weapon to swap to

        string nextWeapon = EquipedWeapons[weaponPos];
        weaponPos = 0;
        foreach (CreateWeaponSlot weaponSlot in WeaponInventory.WeaponSlot)
        {
            if (weaponSlot != null)
            { if (weaponSlot.weapon == nextWeapon) { break; } }
            weaponPos++;
        }
        //Debug.Log(weaponPos);
        SwapWeapon(weaponPos);
    }
    public void SwapWeapon(int weaponSlot)
    {
        if (EquipedWeapons.Count <= 1) return;
        if (weaponSlot != currentWeaponSlot) 
        { 
            WeaponInventory.WeaponSlot[currentWeaponSlot].magAmmo = AttackScript.Ammocount;
            WeaponInventory.WeaponSlot[currentWeaponSlot].reserveAmmo = AttackScript.ReserveAmmo;
            WeaponInventory.WeaponSlot[currentWeaponSlot].weaponModel.SetActive(false);
            WeaponInventory.WeaponSlot[currentWeaponSlot].weaponModel.GetComponent<WeaponData>().crosshair.SetActive(false); 
        }
        SetWeaponActive(weaponSlot);

    }
    public void DeactivateWeapon()
    {   //Deactivate old weapon when removing it from player
        WeaponInventory.WeaponSlot[currentWeaponSlot].weaponModel.SetActive(false);
        WeaponInventory.WeaponSlot[currentWeaponSlot].weaponModel.GetComponent<WeaponData>().crosshair.SetActive(false);
    }
    public void clearValues()
    {
        foreach (GameObject model in WeaponModels)
        { model.SetActive(false); model.GetComponent<WeaponData>().crosshair.SetActive(false); }
        EquipedWeapons.Clear();
        NoOfWeaponSlots = 0;
        currentWeapon = null;

        AttackScript.currentWeapon = null;
        AttackScript.currentCrosshair = null;
        AttackScript.currentWeaponFireAudio = null;
        AttackScript.MagSize = 0;
        AttackScript.Ammocount = 0;
        AttackScript.ReserveAmmo = 0;
        AttackScript.reloadTime = 0;
        AttackScript.shootTime = 0;
        AttackScript.swapTime = 0;
        AttackScript.fireType = null;
        AttackScript.HUDTextUpdate();
    }
}
