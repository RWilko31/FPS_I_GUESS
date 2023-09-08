using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractText : MonoBehaviour
{
    //manages the on screen text and changes the symbols based on the current input device
    [SerializeField] private GameObject interactContainer;
    [SerializeField] private TextMeshProUGUI interactText;
    [Header("icon Layouts")]
    [SerializeField] private GameObject IconLayoutContainer;
    [SerializeField] private List<IconLayout> IconLayouts = new List<IconLayout>();
    [Header("Debug")]
    [SerializeField] private string displayText = "";
    [SerializeField] private string inputScheme = "";

    GameDataFile GDFile;
    InputController IC;

    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        IC = GDFile.GetComponent<InputController>();

        //get iconLayouts
        getLayouts(IconLayoutContainer);
    }
    private void Start()
    { inputScheme = IC.GetControlScheme(); }

    /// <summary>
    /// finds all of the iconLayouts attached to the given gameobject and adds them to the list of layouts used in getSymbol()
    /// </summary>
    /// <param name="container"></param>
    public void getLayouts(GameObject container)
    {
        IconLayout[] ILC = container.GetComponents<IconLayout>();
        foreach (IconLayout IL in ILC)
        { if (!IconLayouts.Contains(IL)) IconLayouts.Add(IL); }
    }

    /// <summary>
    /// returns the string icon data for the given input. the string can then be used to display icons on the screen text.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string getSymbol(inputName input) //returns the corrosponding symbol based on the current control scheme
    {
        string scheme = IC.GetControlScheme();
        inputScheme = scheme;
        IconLayout iconLayout = IconLayouts[0];

        foreach(IconLayout IL in IconLayouts)
        { if (IL.Device() == scheme) { iconLayout = IL; break; } }

        TMP_SpriteAsset iconMap = iconLayout.IconMap();
        interactText.spriteAsset = iconMap;

        string text = "";

        switch (input)
        {
            case inputName.P_Move:
                text = iconLayout.P_Move();
                break;
            case inputName.P_Look:
                text = iconLayout.P_Look();
                break;
            case inputName.P_Jump:
                text = iconLayout.P_Jump();
                break;
            case inputName.P_Crouch:
                text = iconLayout.P_Crouch();
                break;
            case inputName.P_Prone:
                text = iconLayout.P_Prone();
                break;
            case inputName.P_Slide:
                text = iconLayout.P_Slide();
                break;
            case inputName.P_Action:
                text = iconLayout.P_Action();
                break;
            case inputName.P_Reload:
                text = iconLayout.P_Reload();
                break;
            case inputName.P_Swap:
                text = iconLayout.P_Swap();
                break;
            case inputName.P_Fire:
                text = iconLayout.P_Fire();
                break;
            case inputName.P_Aim:
                text = iconLayout.P_Aim();
                break;
            case inputName.P_Tactical:
                text = iconLayout.P_Tactical();
                break;
            case inputName.P_Lethal:
                text = iconLayout.P_Lethal();
                break;
            case inputName.P_Sprint:
                text = iconLayout.P_Sprint();
                break;
            case inputName.P_Melee:
                text = iconLayout.P_Melee();
                break;
            case inputName.P_Item1:
                text = iconLayout.P_Item1();
                break;
            case inputName.P_SwapToGrapple:
                text = iconLayout.P_SwapToGrapple();
                break;
            case inputName.P_SwapAbility:
                text = iconLayout.P_SwapAbility();
                break;
            case inputName.P_UseAbility:
                text = iconLayout.P_UseAbility();
                break;
            case inputName.P_PauseGame:
                text = iconLayout.P_PauseGame();
                break;
            case inputName.P_OpenInventory:
                text = iconLayout.P_OpenInventory();
                break;
            case inputName.P_Message:
                text = iconLayout.P_Message();
                break;

            case inputName.M_ResumeGame:
                text = iconLayout.M_ResumeGame();
                break;
            case inputName.M_Navigate:
                text = iconLayout.M_Navigate();
                break;
            case inputName.M_Submit:
                text = iconLayout.M_Submit();
                break;
            case inputName.M_Cancel:
                text = iconLayout.M_Cancel();
                break;
            case inputName.M_TabLeft:
                text = iconLayout.M_TabLeft();
                break;
            case inputName.M_TabRight:
                text = iconLayout.M_TabRight();
                break;
            case inputName.M_CloseInventory:
                text = iconLayout.M_CloseInventory();
                break;
        }
        return text;
    }

    /// <summary>
    /// updates the screen text to the given string. The text will not be visable unless the text is toggled using ToggleText()
    /// </summary>
    /// <param name="displayText"></param>
    public void SetText(string displayText) //updates the interact text 
    {
        if (textLock) return;
        interactText.text = displayText;
        this.displayText = displayText;
    }

    private bool textLock = false;
    /// <summary>
    /// Used to lock or unlock the display text. If locked the text cannot be updated. 
    /// This can be used to leave text on screen for longer periods without it being overwritten e.g for tutorial text.
    /// </summary>
    /// <param name="lockState"></param>
    public void lockText(bool lockState) //locks the text until unlocked again
    { textLock = lockState; }

    /// <summary>
    /// Sets the text visibilty to the given state. The state cannot be changed if lockText is active
    /// </summary>
    /// <param name="showText"></param>
    public void ToggleText(bool showText) //shows or hides the interact text
    { if(!textLock) interactContainer.SetActive(showText);  }

    /// <summary>
    /// Clears the display text
    /// </summary>
    public void ResetText()
    {
        interactContainer.SetActive(false);
        textLock = false;
        interactText.text = "";
        this.displayText = "";
    }
}
