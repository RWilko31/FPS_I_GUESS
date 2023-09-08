using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private GameObject weaponsContainer;
    [SerializeField] private GameObject Crosshair;
    private Image CrosshairImage;

    [Header("Weapons")]
    [SerializeField] private int maxWeaponSlots = 4;
    [SerializeField] private int currentSlot = 0;
    [SerializeField] private List<WeaponSlot> equipedWeapons = new List<WeaponSlot>();

    GameDataFile GDFile;
    CsvManager CsvM;
    InputController IC;
    WeaponSystem WS;

    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        CsvM = GDFile.GetComponent<CsvManager>();
        IC = GDFile.GetComponent<InputController>();
        WS = GetComponent<WeaponSystem>();
        CrosshairImage = Crosshair.GetComponent<Image>();
        SubEvents();
    }
    void SubEvents()
    {
        IC.swap += SwapWeapons;
        IC.item3 += SwapToFunction;
    }
    void SwapWeapons()
    {
        int slot = currentSlot +1;
        if (slot >= equipedWeapons.Count) slot = 0;
        SetWeaponActive(slot);
    }
    void SwapToFunction() //Grapple
    {

    }

    [System.Serializable]
    public struct WeaponSlot
    {
        public string name;
        public GameObject model;
        public int slot;
    }
   
    public void EquipWeapon(GameObject prefab) //add weapon to player equipment
    {
        if(equipedWeapons.Count > 0) HideWeapon();

        //create slot
        WeaponSlot newSlot = new WeaponSlot();
        newSlot.name = prefab.name;

        //if player hasnt filled weapon slots
        if (equipedWeapons.Count < maxWeaponSlots)
        {
            newSlot.model = InstantiateWeapon(prefab);
            equipedWeapons.Add(newSlot);
            SetWeaponActive(equipedWeapons.Count - 1);
        }
        //if player has filled weapon slots
        else
        {
            DeleteCurrentWeapon();
            newSlot.model = InstantiateWeapon(prefab);
            equipedWeapons[currentSlot] = newSlot;
            SetWeaponActive(currentSlot);
        }
    }
    public void SetWeaponActive(int slot) //swaps the Weapon triggered by the weaponSystem script
    {
        if (!WS || equipedWeapons.Count < slot) return;

        ////deactivate current weapon
        WeaponSlot old = equipedWeapons[currentSlot];
        old.model.SetActive(false);

        //activate new weapon
        currentSlot = slot;
        WeaponSlot newSlot = equipedWeapons[slot];
        Weapon w = newSlot.model.GetComponent<Weapon>();
        CrosshairImage.sprite = w.crossHair;
        Crosshair.SetActive(true);
        newSlot.model.SetActive(true);

        //give the weponSystem the new weapon
        WS.SetWeapon(w);
    }
    public GameObject InstantiateWeapon(GameObject prefab) //adds the weapon model to the player
    {
        //add weapon to player
        GameObject weapon =  Instantiate(prefab, weaponsContainer.transform);
        weapon.name = prefab.name;

        //pass through variables
        AttackFunction af = weapon.GetComponent<AttackFunction>();
        af.playerCamera = GDFile.playerCam.GetComponent<Camera>();
        Weapon w = weapon.GetComponent<Weapon>();
        CsvM.SetWeaponData(w);
        weapon.transform.localPosition = w.hipPos;

        return weapon;
    }
    public void DeleteCurrentWeapon() //removes old weapon model from the player
    {
        WeaponSlot oldWeapon = equipedWeapons[currentSlot];
        Destroy(oldWeapon.model);
    }
    public void HideWeapon()
    {
        WeaponSlot slot = equipedWeapons[currentSlot];
        slot.model.SetActive(false);
        Crosshair.SetActive(false);
        CrosshairImage.sprite = null;
    }
}
