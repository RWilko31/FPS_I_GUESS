using UnityEngine;
using TMPro;

public class Playstation_IconLayout : IconLayout
{
    [SerializeField] private TMP_SpriteAsset iconMap;

    public override string Device()
    {
        return "Playstation Controller";
    }

    public override TMP_SpriteAsset IconMap()
    {
        return iconMap;
    }

    public override string M_Cancel()
    {
        return "Press <sprite index=25 tint>";
    }

    public override string M_CloseInventory()
    {
        return "Press CloseInventory";
    }

    public override string M_Navigate()
    {
        return "Use <sprite index=0 tint>";
    }

    public override string M_ResumeGame()
    {
        return "Press <sprite index=23 tint>";
    }

    public override string M_Submit()
    {
        return "Press <sprite index=26 tint>";
    }

    public override string M_TabLeft()
    {
        return "Press <sprite index=14 tint>";
    }

    public override string M_TabRight()
    {
        return "Press <sprite index=15 tint>";
    }
    public override string P_Action()
    {
        return "Hold <sprite index=23 tint>";
    }

    public override string P_Aim()
    {
        return "Hold <sprite index=18 tint>";
    }

    public override string P_Crouch()
    {
        return "Press <sprite index=25 tint>";
    }

    public override string P_Fire()
    {
        return "Press <sprite index=17 tint>";
    }

    public override string P_Item1()
    {
        return "Press <sprite index=2 tint>";
    }

    public override string P_Jump()
    {
        return "Press <sprite index=26 tint>";
    }

    public override string P_Lethal()
    {
        return "Press <sprite index=15 tint>";
    }

    public override string P_Look()
    {
        return "Use <sprite index=9 tint>";
    }

    public override string P_Melee()
    {
        return "Press <sprite index=12 tint>";
    }

    public override string P_Message()
    {
        return "Press Message";
    }

    public override string P_Move()
    {
        return "Use <sprite index=8 tint>";
    }

    public override string P_OpenInventory()
    {
        return "Press <sprite index=19 tint>";
    }

    public override string P_PauseGame()
    {
        return "Press <sprite index=21 tint>";
    }

    public override string P_Prone()
    {
        return "Hold <sprite index=25 tint>";
    }

    public override string P_Reload()
    {
        return "Press <sprite index=23 tint>";
    }

    public override string P_Slide()
    {
        return "Hold <sprite index=25 tint> while sprinting";
    }

    public override string P_Sprint()
    {
        return "Press <sprite index=11 tint>";
    }

    public override string P_Swap()
    {
        return "Press <sprite index=24 tint>";
    }

    public override string P_SwapAbility()
    {
        return "Press SwapAbility";
    }

    public override string P_SwapToGrapple()
    {
        return "Press SwapToGraple";
    }

    public override string P_Tactical()
    {
        return "Press <sprite index=14 tint>";
    }

    public override string P_UseAbility()
    {
        return "Press useAbility";
    }
}
