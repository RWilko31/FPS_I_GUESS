using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum inputName
{
    P_Move,
    P_Look,
    P_Jump,
    P_Crouch,
    P_Prone,
    P_Slide,
    P_Action,
    P_Reload,
    P_Swap,
    P_Fire,
    P_Aim,
    P_Tactical,
    P_Lethal,
    P_Sprint,
    P_Melee,
    P_Item1,
    P_SwapToGrapple,
    P_SwapAbility,
    P_UseAbility,
    P_PauseGame,
    P_OpenInventory,
    P_Message,

    M_ResumeGame,
    M_Navigate,
    M_Submit,
    M_Cancel,
    M_TabLeft,
    M_TabRight,
    M_CloseInventory
}
public abstract class IconLayout : MonoBehaviour //defines the icon associated with each input for a control scheme
{
    //device
    public abstract string Device();
    //iconMap
    public abstract TMP_SpriteAsset IconMap();

    //Character scheme
    public abstract string P_Move();
    public abstract string P_Look();
    public abstract string P_Jump();
    public abstract string P_Crouch();
    public abstract string P_Prone();
    public abstract string P_Slide();
    public abstract string P_Action();
    public abstract string P_Reload();
    public abstract string P_Swap();
    public abstract string P_Fire();
    public abstract string P_Aim();
    public abstract string P_Tactical();
    public abstract string P_Lethal();
    public abstract string P_Sprint();
    public abstract string P_Melee();
    public abstract string P_Item1();
    public abstract string P_SwapToGrapple();
    public abstract string P_SwapAbility();
    public abstract string P_UseAbility();
    public abstract string P_PauseGame();
    public abstract string P_OpenInventory();
    public abstract string P_Message();
    
    //Menu scheme
    public abstract string M_ResumeGame();
    public abstract string M_Navigate();
    public abstract string M_Submit();
    public abstract string M_Cancel();
    public abstract string M_TabLeft();
    public abstract string M_TabRight();
    public abstract string M_CloseInventory();
}
