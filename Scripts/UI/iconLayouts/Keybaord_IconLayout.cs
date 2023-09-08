using UnityEngine;
using TMPro;

public class Keybaord_IconLayout : IconLayout
{
    [SerializeField] private TMP_SpriteAsset iconMap;

    public override string Device()
    {
        return "Keyboard & mouse";
    }

    public override TMP_SpriteAsset IconMap()
    {
        return iconMap;
    }

    public override string M_Cancel()
    {
        return "Press ESC";
    }

    public override string M_CloseInventory()
    {
        return "Press Tab";
    }

    public override string M_Navigate()
    {
        return "Use <sprite index=5 tint>";
    }

    public override string M_ResumeGame()
    {
        return "";
    }

    public override string M_Submit()
    {
        return "Press Enter";
    }

    public override string M_TabLeft()
    {
        return "";
    }

    public override string M_TabRight()
    {
        return "";
    }
    public override string P_Action()
    {
        return "Press F";
    }

    public override string P_Aim()
    {
        return "Hold <sprite index=3 tint>";
    }

    public override string P_Crouch()
    {
        return "Press C";
    }

    public override string P_Fire()
    {
        return "Press <sprite index=1 tint>";
    }

    public override string P_Item1()
    {
        return "Press 3";
    }

    public override string P_Jump()
    {
        return "Press Space";
    }

    public override string P_Lethal()
    {
        return "Press G";
    }

    public override string P_Look()
    {
        return "Use <sprite index=0 tint>";
    }

    public override string P_Melee()
    {
        return "Press V";
    }

    public override string P_Message()
    {
        return "Press =";
    }

    public override string P_Move()
    {
        return "Use <sprite index=5 tint>";
    }

    public override string P_OpenInventory()
    {
        return "Press Tab";
    }

    public override string P_PauseGame()
    {
        return "Press P";
    }

    public override string P_Prone()
    {
        return "Press CTRL";
    }

    public override string P_Reload()
    {
        return "Press R";
    }

    public override string P_Slide()
    {
        return "Hold C";
    }

    public override string P_Sprint()
    {
        return "Hold Shift";
    }

    public override string P_Swap()
    {
        return "Press X";
    }

    public override string P_SwapAbility()
    {
        return "Press 4";
    }

    public override string P_SwapToGrapple()
    {
        return "Press Q";
    }

    public override string P_Tactical()
    {
        return "Press T";
    }

    public override string P_UseAbility()
    {
        return "Press E";
    }
}
