using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public GameObject Background;
    public Button button;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI QuantityText;
    public GameObject EquipedIcon;
    public string Item;
    public string Quantity;
    public string Type;
    public int Slot;

    public Color slotColour;
    public Color itemColour;
    public Color armourColour;
    public Color accessoryColour;
    public Color currencycolour;

    MainMenuScript MMscript;

    public void updateSlotInfo(string item, string ItemType, string quantity, int slot, string slotType, bool equiped, MainMenuScript MMScript)
    {
        MMscript = MMScript;
        Item = item;
        Quantity = quantity;
        Type = slotType;
        Slot = slot;
        NameText.text = item;
        EquipedIcon.SetActive(equiped);

        if (Type == "accessory" || Type.Contains("armour")) QuantityText.text = "";
        else QuantityText.text = quantity;

        setColour(ItemType);
    }

    public void itemSelected()
    {
        MMscript.itemSelected(this);
    }

    public void setColour(string Type)
    {
        Color typecolour = slotColour;
        switch (Type)
        {
            case "item":
                typecolour = itemColour;
                break;

            case "accessory":
                typecolour = accessoryColour;
                break;

            case "armour":
                typecolour = armourColour;
                break;

            case "currency":
                typecolour = currencycolour;
                break;

            default:
                break;
        }

        Background.GetComponent<Image>().color = typecolour;
    }
}
