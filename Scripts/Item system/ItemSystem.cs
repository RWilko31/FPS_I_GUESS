using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSystem : MonoBehaviour
{
    public int quantityLimit = 999;

    //the position of an item in the list matches its position in the quantity list
    public List<string> playerItems; //items player has
    public List<int> itemQuantity; //quantity of the items

    //current equiped accessories
    public int maxAccSlots = 6;
    public List<string> currentAccessories;
    public List<int> AccessoryPos;
    //current equiped armour
    public List<string> currentArmour;
    public List<int> ArmourPos;

    //stats
    [Header("Player accessory stats")]
    public int DEF = 0;
    public List<string> ATKList;

    //craftlist
    public List<string> craftableItems;

    //scripts
    GameDataFile GDFile;
    CsvManager CSVmanager;

    private void Awake()
    {
        GDFile = GetComponent<GameDataFile>();
        CSVmanager = GetComponent<CsvManager>();
        for (int i = 0; i < 4; i++) currentArmour.Add("");
        for (int i = 0; i < maxAccSlots; i++) currentAccessories.Add("");
    }

    public void ResetItemSystem()
    {
        currentAccessories.Clear();
        currentArmour.Clear();
        playerItems.Clear();
        itemQuantity.Clear();
        for (int i = 0; i < 4; i++) currentArmour.Add("");
        for (int i = 0; i < maxAccSlots; i++) currentAccessories.Add("");
    }

    public void addItem(string item)
    {
        int pos = 0;
        if (playerItems.Contains(item))
        {
            pos = playerItems.IndexOf(item);
            if (itemQuantity[pos] >= quantityLimit) return;
            itemQuantity[pos] = itemQuantity[pos] + 1;
        }
        else 
        { 
            playerItems.Add(item);
            itemQuantity.Add(1);
            pos = playerItems.IndexOf(item);
        }
    }
    public void removeItem(string item)
    {
        int pos = playerItems.IndexOf(item);
        if (itemQuantity[pos] > 1) itemQuantity[pos] -= 1;
        else
        {
            playerItems.RemoveAt(pos);
            itemQuantity.RemoveAt(pos);
        }
    }
    public string getItemQuantity(string item)
    {
        string quantity = "0";
        if (playerItems.Contains(item))
        {
            int pos = playerItems.IndexOf(item);
            if (itemQuantity[pos] >= quantityLimit) quantity = "MAX";
            else quantity = itemQuantity[pos].ToString();
        }
        return quantity;
    }

    //change Type
    public void ItemToAccessories(string item, int slot, string type)
    {
        //remove accessories already in the slot
        if (type == "accessory")
        { if (AccessoryPos.Contains(slot)) { AccessoriesToItem(currentAccessories[slot], slot, type); } }
        else { if (ArmourPos.Contains(slot)) { AccessoriesToItem(currentArmour[slot], slot, type); } }
        //remove item from itemlist
        int pos = playerItems.IndexOf(item);
        if (itemQuantity[pos] > 1) { itemQuantity[pos] = itemQuantity[pos] - 1; }
        else
        {
            playerItems.RemoveAt(pos);
            itemQuantity.RemoveAt(pos);
        }
        //add item to accessories list
        if (type == "accessory") { currentAccessories[slot] = item; }
        else { currentArmour[slot] = item; }
        calculateStats();
    }
    public void AccessoriesToItem(string item, int slot, string type)
    {
        //if already in list increase quantity
        if (playerItems.Contains(item))
        {
            int pos = playerItems.IndexOf(item);
            itemQuantity[pos] += 1;
        }
        //else add new item
        else 
        { 
            playerItems.Add(item);
            itemQuantity.Add(1);
        }
        if (type == "accessory") currentAccessories[slot] = "";
        else currentArmour[slot] = "";
        calculateStats();
    }

    public bool checkItemCompatibility(string item, string slotType, int slotNo)
    {
        string itemType = CSVmanager.getItemType(item);
        switch (itemType)
        {
            case "item":
                return false;

            case "accessory":
                if (slotType != "accessory") return false;
                else return true;

            case "armour":
                if(slotType != "armour") return false;
                else
                {
                    string subType = CSVmanager.getSubType(item);
                    int pos = 0;
                    switch (subType)
                    {
                        case "head":
                            pos = 0;
                            break;

                        case "chest":
                            pos = 1;
                            break;

                        case "legs":
                            pos = 2;
                            break;

                        case "feet":
                            pos = 3;
                            break;                                    
                    }
                    if (pos != slotNo) return false;
                    else return true;
                }

            default:
                return false;
        }
    }

    //stats
    private void calculateStats()
    {
        //reset values
        DEF = 0;
        ATKList.Clear();
        List<string> checkList = new List<string>();
        foreach(string item in currentAccessories) { checkList.Add(item); }
        foreach(string item in currentArmour) { checkList.Add(item); }

        //check all accessories
        foreach (string item in checkList)
        {
            //check all effects on accessory
            List<string> accFXlist = new List<string>();
            accFXlist = CSVmanager.getItemEffect(item);
            foreach(string effect in accFXlist)
            {
                int index = effect.IndexOf("_");
                if (index >= 0)
                {
                    if (effect.Contains("DEF")) //get extra defense
                    { DEF += int.Parse(effect.Substring(index + 1, effect.Length - index - 1)); }
                    if (effect.Contains("ATK")) //get extra attack damage
                    { ATKList.Add(effect); }
                }
            }
        }
    }

    //crafting
    public List<string> showCraftList()
    {
        List<string> CraftableItems = new List<string>();

        foreach(string item in playerItems)
        {
            foreach(string craftItem in CSVmanager.getCraftList(item))
            { CraftableItems.Add(craftItem); }
        }
        return CraftableItems;
    }
    public void craftItem(string item)
    {
        List<string> craftItems = new List<string>();
        List<int> itemAmount = new List<int>();

        //get items needed to craft
        foreach(string val in CSVmanager.getItemsToCraft(item))
        {
            string Quantity = "1";
            string name = val;

            //get data
            int index = name.IndexOf("*");
            if (index >= 0)
            {
                //seperate and read quantity
                Quantity = name.Substring(index + 1, name.Length - index - 1);
                //read name
                name = name.Substring(0, index);
            }
            //Debug.Log(name + ": " + Quantity);

            craftItems.Add(name);
            itemAmount.Add(int.Parse(Quantity));
        }

        //loop for each item in the recipe
        int i = 0;
        foreach (string craftitem in craftItems)
        {
            //find object position in player items
            int pos = playerItems.IndexOf(craftitem);
            //check quantity and remove
            if (itemQuantity[pos] > itemAmount[i]) itemQuantity[pos] -= itemAmount[i];
            else { playerItems.RemoveAt(pos); itemQuantity.RemoveAt(pos); }

            i++;
        }
        //if player already has item increase quantity
        if (playerItems.Contains(item))
        {
            int pos = playerItems.IndexOf(item);
            itemQuantity[pos]++;
        }
        //else give player the item
        else
        {
            playerItems.Add(item);
            itemQuantity.Add(1);
        }
    }

    //save player items
    public List<string[]> saveItemData() //puts the player item data in an array to write to the save file
    {
        List<string[]> itemData = new List<string[]>();

        string[] tag = new string[1]; //add section tag
        tag[0] = "itemdata";
        itemData.Add(tag);
        string[] headings = new string[2]; //add table headings
        headings[0] = "item"; headings[1] = "quantity";
        itemData.Add(headings);

        int pos = 0;
        foreach (string item in playerItems) //add item data
        {
            string[] data = new string[2];
            data[0] = item; data[1] = itemQuantity[pos].ToString();
            itemData.Add(data);
            pos++;
        }

        return itemData;
    }
    public List<string[]> saveAccessoryData() //puts the current equipment (armour/accessories) data in an array to write to the save file
    {
        List<string[]> AccData = new List<string[]>();

        //add section data
        string[] tag = new string[1]; //add section tag
        tag[0] = "accessorydata";
        AccData.Add(tag);
        string[] headings = new string[3]; //add table headings
        headings[0] = "item"; headings[1] = "type"; headings[2] = "slot";
        AccData.Add(headings);

        //add armour
        int pos = 0;
        foreach (string item in currentArmour) //add item data
        {
            string[] data = new string[3];
            data[0] = item; data[1] = "armour"; data[2] = pos.ToString();
            if (data[0] == "") data[0] = "none";
            AccData.Add(data);
            pos++;
        }
        //add accessories
        pos = 0;
        foreach (string item in currentAccessories) //add item data
        {
            string[] data = new string[3];
            data[0] = item; data[1] = "accessory"; data[2] = pos.ToString();
            if (data[0] == "") data[0] = "none";
            AccData.Add(data);
            pos++;
        }

        return AccData;
    }

    //load save data
    public void LoadItemData()
    {
        int i = 0;
        foreach (string[] data in GDFile.Read_itemdata)
        {
            if (i > 1)
            {
                //Debug.Log(data[0] + " " + data[1]);
                if (!playerItems.Contains(data[0])) 
                {
                    playerItems.Add(data[0]);
                    itemQuantity.Add(int.Parse(data[1]));
                }
            }
            i++;
        }
        i = 0;
        foreach (string[] data in GDFile.Read_accessorydata)
        {
            if (i > 1 && data[0] != "none")
            {
                switch (data[1])
                {
                    case "armour":
                        currentArmour[int.Parse(data[2])] = data[0];
                        break;

                    case "accessory":
                        currentAccessories[int.Parse(data[2])] = data[0];
                        break;
                }                
            }
            i++;
        }
        calculateStats();
    }
}
