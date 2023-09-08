using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Interact_WeaponPickup : MonoBehaviour, IInteractFunction
//{
//    [Header("weapon pickup")]
//    [SerializeField] WeaponName weapon;

//    WeaponManager WM;
//    WeaponAndItemScript WI;
//    private GameDataFile GDFile;
//    private EventManager EM;

//    private void Awake()
//    {
//        GDFile = FindObjectOfType<GameDataFile>();
//        EM = GDFile.GetComponent<EventManager>();
//        WM = GDFile.WeaponManager;
//        WI = GDFile.WeaponAndItemScript;
//    }

//    public void Interact() { WeaponPickup(); }
//    public string InteractType()
//    { return "weapon pickup: " + weapon.ToString(); }
//    public string InteractText()
//    { return "to pick up weapon"; }
//    private void WeaponPickup()
//    {
//        //Check player has a weapon script
//        if (!GDFile.player.GetComponent<WeaponManager>()) return;

//        //get weapon name
//        string weaponName = weapon.ToString();
//        //string weaponName = this.name.Remove(0, 13);
//        //int index = weaponName.IndexOf(" ");
//        //if (index >= 0) weaponName = weaponName.Substring(0, index);

//        //check if player already has weapon
//        bool weaponCheck = WM.EquipedWeapons.Contains(weaponName);
//        if (weaponCheck)
//        {   //add ammo to gun if player already has it
//            //Check if the ammo neads adding to the current gun
//            if (WM.currentWeapon == weaponName) { WI.ReserveAmmo += WI.MagSize; }
//            else
//            {
//                //Debug.Log("Secondaryammo");
//                int weaponSlot = (int)WM.checkCurrentWeapons(weaponName).x;
//                //Add ammo to gun if it is not in use
//                WM.WeaponInventory.WeaponSlot[weaponSlot].reserveAmmo +=
//                    WM.WeaponList.WeaponTableRows[WM.WeaponInventory.WeaponSlot[weaponSlot].tableRow].magsize;
//            }
//            WI.HUDTextUpdate();
//        }
//        else
//        {   //If player doesnt have the gun add it to a weapon slot
//            int slot = WM.NoOfWeaponSlots + 1;
//            WM.SetPlayerWeapon(slot, weaponName);
//            if (WM.NoOfWeaponSlots > 1)
//            {
//                int gunslot = (int)WM.checkCurrentWeapons(weaponName).x;
//                WM.SwapWeapon(gunslot);
//            }
//        }
//    }
//}

public class Interact_WeaponPickup : MonoBehaviour, IInteractFunction
{
    [SerializeField] GameObject prefab;

    GameDataFile GDFile;
    EquipmentManager EqM;

    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        EqM = GDFile.player.GetComponent<EquipmentManager>();
    }
    void IInteractFunction.Interact()
    { EqM.EquipWeapon(prefab); }
    string IInteractFunction.InteractText()
    { return "to pick up " + prefab.name;  }
    string IInteractFunction.InteractType()
    { return "weapon pickup: " + prefab.name; }
}
