using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsvManager : MonoBehaviour
{
    ///This script is used to read data from csv files on awake allowing other scripts to read the data from.
    ///This prevents errors with multiple scripts opening the file at the same time

    ///NOTE: This script does not read save data csv files
    ///This is done by the gameDataScript to allow data to be more accessible

    //-----variables-----\\
    #region WeaponTable
    [Header("WeaponTable")]
    [SerializeField] public TextAsset weaponTableCSV;
    [SerializeField] public int No_columns_wt = 13;
    #endregion

    #region HealthTable
    [Header("HealthTable")]
    [SerializeField] public TextAsset healthTableCSV;
    [SerializeField] public int No_columns_ht = 4;
    [SerializeField] public readHealthList healthList = new readHealthList();
    public class readHealthList //creates an array for the health table data
    { public HealthTableData[] HealthTableRows; }
    public class HealthTableData //Stores the table data for each entity
    {
        public string Entity;
        public int MaxHealth;
        public string EnemyTag;
        public int reward;
    }
    #endregion

    #region DamageTable
    [Header("DamageTable")]
    [SerializeField] public TextAsset damageTableCSV;
    [SerializeField] public int No_columns_dt = 3;
    [SerializeField] public readDamageList damageList = new readDamageList();
    public class readDamageList //creates an array for the damage table data
    { public DamageTableData[] DamageTableRows; }
    public class DamageTableData //Stores the table data for each attack
    {
        public string Attack;
        public int Damage_standard;
        public int Damage_critical;
    }
    #endregion

    #region ItemTable
    [Header("ItemTable")]
    [SerializeField] public TextAsset ItemTableCSV;
    [SerializeField] public int No_columns_it = 12;
    [SerializeField] public readItemList itemList = new readItemList();
    public class readItemList //creates an array for the damage table data
    { public ItemTableData[] ItemTableRows; }
    public class ItemTableData //Stores the table data for each attack
    {
        public string itemName; //name of item
        public List<string> acquisistion; //how the item is aquired
        public List<string> fallsFrom; //where the item comes from
        public float chance; //chance to be dropped
        public int priority; //priority to prevent other items dropping
        public List<string> preventedBy; //what items prevent the item being dropped
        public string craftable; //if the item can be crafted
        public List<string> craftItems; //items required to make the item
        public string itemType;  //what catagory the item is e.g weapon/armour/equipment
        public List<string> value; //data relevent to the item
        public int sellValue; //amount given if item sold
        public string itemDescription; //about text for the item
    }
    #endregion

    #region Scripts
    GameDataFile GDFile;
    WeaponManager WMScript;
    HealthScript HScript;
    ItemSystem ItemSys;
    #endregion

    //-----Functions-----\\
    private void Awake()
    {
        //get scripts
        GDFile = GetComponent<GameDataFile>();
        ItemSys = GetComponent<ItemSystem>();
        WMScript = GDFile.player.GetComponent<WeaponManager>();
        HScript = GDFile.player.GetComponent<HealthScript>();

        //read CSV Tables
        ReadHealthTableCSV();
        ReadDamageTableCSV();
        ReadItemTableCSV();
    }

    #region weapon data
    public void ReadWeaponTableCSV() //Reads the weaponTable.csv file to get weapon info and assigns it to the weapon manager
    {   
        ///NOTE: Reading this data
        ///Reading of weapon table has been moved to this script, however the read data is still stored in the WeaponManger
        ///when reading values it should be read from the weapon manager instead of re-reading the csv file as otherwise
        ///the weapon manager data will be overwritten. if multiple scripts require the original data then the stored data
        ///needs to be moved to this script and requested by the weapon manager instead, and the function modified for this.

        string[] data = weaponTableCSV.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);
        int tableSize = data.Length / No_columns_wt - 1;
        WMScript.createDataArray(tableSize);

        for (int i = 0; i < tableSize; i++)
        {
            WMScript.createNewRow(i);
            WMScript.WeaponList.WeaponTableRows[i].weapon = data[No_columns_wt * (i + 1)];
            WMScript.WeaponList.WeaponTableRows[i].magsize = int.Parse(data[No_columns_wt * (i + 1) + 1]);
            WMScript.WeaponList.WeaponTableRows[i].reserveAmmo = int.Parse(data[No_columns_wt * (i + 1) + 2]);
            WMScript.WeaponList.WeaponTableRows[i].shootTime = float.Parse(data[No_columns_wt * (i + 1) + 3]);
            WMScript.WeaponList.WeaponTableRows[i].reloadTime = float.Parse(data[No_columns_wt * (i + 1) + 4]);
            WMScript.WeaponList.WeaponTableRows[i].swapTime = float.Parse(data[No_columns_wt * (i + 1) + 5]);
            WMScript.WeaponList.WeaponTableRows[i].fireType = data[No_columns_wt * (i + 1) + 6];
            WMScript.WeaponList.WeaponTableRows[i].aimPos_x = float.Parse(data[No_columns_wt * (i + 1) + 7]);
            WMScript.WeaponList.WeaponTableRows[i].aimPos_y = float.Parse(data[No_columns_wt * (i + 1) + 8]);
            WMScript.WeaponList.WeaponTableRows[i].aimPos_z = float.Parse(data[No_columns_wt * (i + 1) + 9]);
            WMScript.WeaponList.WeaponTableRows[i].hipPos_x = float.Parse(data[No_columns_wt * (i + 1) + 10]);
            WMScript.WeaponList.WeaponTableRows[i].hipPos_y = float.Parse(data[No_columns_wt * (i + 1) + 11]);
            WMScript.WeaponList.WeaponTableRows[i].hipPos_z = float.Parse(data[No_columns_wt * (i + 1) + 12]);
            //Debug.Log(data[No_columns * (i + 1)]); //Reads out each weaponName
        }
    }
    #endregion

    #region Health data
    public void ReadHealthTableCSV() //get healthTable data
    {
        string[] data = healthTableCSV.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);
        int tableSize = data.Length / No_columns_ht - 1;
        healthList.HealthTableRows = new HealthTableData[tableSize];
        for (int i = 0; i < tableSize; i++)
        {
            healthList.HealthTableRows[i] = new HealthTableData();
            healthList.HealthTableRows[i].Entity = data[No_columns_ht * (i + 1)];
            healthList.HealthTableRows[i].MaxHealth = int.Parse(data[No_columns_ht * (i + 1) + 1]);
            healthList.HealthTableRows[i].EnemyTag = data[No_columns_ht * (i + 1) + 2];
            healthList.HealthTableRows[i].reward = int.Parse(data[No_columns_ht * (i + 1) + 3]);
        }
    }
    public void GetHealthData(HealthScript healthScript) //Give requested health data
    {
        string entity = healthScript.thisEntity;
        int row = 0;

        //find what row the required data is in
        foreach (HealthTableData HTD in healthList.HealthTableRows)
        {
            if (HTD.Entity == entity) break;
            else row++;
        }

        //apply data
        healthScript.thisMaxHealth = healthList.HealthTableRows[row].MaxHealth;
        healthScript.reward = healthList.HealthTableRows[row].reward;

        //get opposing tags (enemies of the entity)
        ///NOTE: Using Multiple tags
        ///tags read from the file are split by looking for a + between them
        ///allowing for multiple different tags to be used for enemies
        string EnemyTag = healthList.HealthTableRows[row].EnemyTag;
        string[] tags = EnemyTag.Split(new string[] {"+"}, System.StringSplitOptions.None);
        foreach(string tag in tags) healthScript.opposingEntity.Add(tag);
    }
    #endregion

    #region Damage data
    public void ReadDamageTableCSV() //get damageTable data
    {
        string[] data = damageTableCSV.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);
        int tableSize = data.Length / No_columns_dt - 1;
        damageList.DamageTableRows = new DamageTableData[tableSize];
        for (int i = 0; i < tableSize; i++)
        {
            damageList.DamageTableRows[i] = new DamageTableData();
            damageList.DamageTableRows[i].Attack = data[No_columns_dt * (i + 1)];
            damageList.DamageTableRows[i].Damage_standard = int.Parse(data[No_columns_dt * (i + 1) + 1]);
            damageList.DamageTableRows[i].Damage_critical = int.Parse(data[No_columns_dt * (i + 1) + 2]);
        }
    }
    public int GetDamageData(string Attack, bool Critical) //Give requested damage data
    {
        //find what row the attack is in
        int i = 0;
        foreach(DamageTableData DTD in damageList.DamageTableRows)
        {
            if (DTD.Attack == Attack) break;
            else i++;
        }

        //find if critical and return damage value
        if (Critical) return damageList.DamageTableRows[i].Damage_critical;
        else return damageList.DamageTableRows[i].Damage_standard;
    }
    #endregion

    #region Inventory data
    public void ReadItemTableCSV() //get ItemTable data
    {
        string[] data = ItemTableCSV.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);
        int tableSize = data.Length / No_columns_it - 1;
        itemList.ItemTableRows = new ItemTableData[tableSize];
        for (int i = 0; i < tableSize; i++)
        {
            itemList.ItemTableRows[i] = new ItemTableData();
            itemList.ItemTableRows[i].itemName = data[No_columns_it * (i + 1)];
            itemList.ItemTableRows[i].chance = float.Parse(data[No_columns_it * (i + 1) + 3]);
            itemList.ItemTableRows[i].priority = int.Parse(data[No_columns_it * (i + 1) + 4]);
            itemList.ItemTableRows[i].craftable = data[No_columns_it * (i + 1) + 6];
            itemList.ItemTableRows[i].itemType = data[No_columns_it * (i + 1) + 7];
            itemList.ItemTableRows[i].sellValue = int.Parse(data[No_columns_it * (i + 1) + 9]);
            itemList.ItemTableRows[i].itemDescription = data[No_columns_it * (i + 1) + 11];

            itemList.ItemTableRows[i].acquisistion = SplitString(data[No_columns_it * (i + 1) + 1]);
            itemList.ItemTableRows[i].fallsFrom = SplitString(data[No_columns_it * (i + 1) + 2]);
            itemList.ItemTableRows[i].preventedBy = SplitString(data[No_columns_it * (i + 1) + 5]);
            itemList.ItemTableRows[i].craftItems = SplitString(data[No_columns_it * (i + 1) + 7]);
            itemList.ItemTableRows[i].value = SplitString(data[No_columns_it * (i + 1) + 10]);
            //Debug.Log(itemList.ItemTableRows[i].itemName);
        }
    }
    private List<string> SplitString(string data)
    {
        string[] splitString = data.Split(new string[] { "+" }, System.StringSplitOptions.None);
        List<string> giveList = new List<string>();
        foreach(string str in splitString)
        { giveList.Add(str); }
        return giveList;
    } //splits any table data with multiple strings into lists (signified by "+")
    public void getDrops(string entity, HealthScript Hscript)
    {
        //create item lists
        List<string> possibleItemDrops = new List<string>();
        List<int> itemPos = new List<int>();

        //find what items can be dropped
        int i = 0;
        foreach (ItemTableData itemlist in itemList.ItemTableRows)
        {
            if (itemlist.fallsFrom.Contains(entity) || itemlist.fallsFrom.Contains("all")) 
            { 
                possibleItemDrops.Add(itemlist.itemName);
                itemPos.Add(i);
            }
            i++;
        }
        //use drop rate to decide to drop items or not
        List<string> removeItems = new List<string>();
        List<int> removePos = new List<int>();
        i = 0;
        foreach (string item in possibleItemDrops)
        {
            float dropRate = itemList.ItemTableRows[itemPos[i]].chance;
            float rand = Mathf.RoundToInt(Random.Range(1, dropRate));
            //Debug.Log(rand);
            if (rand != 1) //random to decide if item should not drop
            {
                //Debug.Log( "chance Remove: " + item);
                removeItems.Add(item);
                removePos.Add(itemPos[i]);
            }
            i++;
        }
        //remove the blocked items from the list
        i = 0;
        foreach (string item in removeItems)
        {
            //Debug.Log("Removing: " + item);
            possibleItemDrops.Remove(item);
            itemPos.Remove(removePos[i]);
            i++;
        }
        //remove items that are blocked by others
        removeItems.Clear();
        removePos.Clear();
        i = 0;
        foreach (string item in possibleItemDrops) //check each possible drop
        {
            //Debug.Log("checking " + item);
            foreach (string preventedItem in itemList.ItemTableRows[itemPos[i]].preventedBy) //check what items prevent it
            {
                //Debug.Log("prevented by: " + preventedItem);
                if (possibleItemDrops.Contains(preventedItem)) { removeItems.Add(item); removePos.Add(i); } //remove item if it is prevented by another item
            }
            i++;
        }
        //remove the blocked items from the list
        i = 0;
        foreach(string item in removeItems)
        {
            //Debug.Log("Removing: " + item);
            possibleItemDrops.Remove(item);
            itemPos.Remove(removePos[i]);
            i++;
        }
        //debugList = possibleItemDrops;

        //return drop items
        Hscript.dropList = possibleItemDrops;
        Hscript.dropPos = itemPos;
    } //returns the items the enemy should drop (already filters out items and randomises chance)
    public string getItemDescription(string item)
    {
        string description = "";
        foreach (ItemTableData itemlist in itemList.ItemTableRows)
        { if (itemlist.itemName == item) { description = itemlist.itemDescription; break; } }
        if (description.Contains("$")) description = description.Replace('$', ',');
        return description;
    } //returns the item description for the inventory
    public List<string> getCraftList(string checkItem) //returns the items the player can craft
    {
        List<string> craftList = new List<string>();

        //check what items use the given item for crafting
        foreach (ItemTableData itemlist in itemList.ItemTableRows)
        {
            List<string> requiredItems = new List<string>();
            List<int> quantity = new List<int>();

            //seperate the amount required from each item
            foreach(string name in itemlist.craftItems)
            {
                string item = name;
                string amount = "1";
                int index = name.IndexOf("*");
                if (index >= 0) 
                { 
                    item = name.Substring(0, index);
                    amount = name.Substring(index + 1, name.Length - index - 1);
                }
                requiredItems.Add(item);
                quantity.Add(int.Parse(amount));
            }
            //check the items required to make the item
            if (requiredItems.Contains(checkItem))
            {
                //Debug.Log(itemlist.itemName + " uses " + checkItem + " in craft recipe");

                //check if player has the other items needed to craft the item
                bool check = true;
                int i = 0;
                foreach(string item in requiredItems) 
                { 
                    //check if player has item
                    if (!ItemSys.playerItems.Contains(item)) { check = false; }
                    else
                    {
                        //check player has correct quantity
                        int pos = ItemSys.playerItems.IndexOf(item);
                        if (ItemSys.itemQuantity[pos] < quantity[i]) check = false;
                    }
                    i++;
                }

                //if player has all items add to craft list                
                if (check) 
                {
                    craftList.Add(itemlist.itemName); 
                    Debug.Log(itemlist.itemName + " can be crafted"); 
                }
            }
        }
        return craftList;
    }
    public List<string> getItemsToCraft(string item) //returns the items required to craft the given item
    {
        List<string> craftList = new List<string>();

        foreach (ItemTableData itemlist in itemList.ItemTableRows)
        { if (itemlist.itemName == item) craftList = itemlist.craftItems; }

        return craftList;
    }
    #endregion
}
