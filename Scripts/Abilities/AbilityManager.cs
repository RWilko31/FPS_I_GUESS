using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] string playerAbility = "None";
    private int currentAbility = 1;
    [SerializeField] bool useAbility_1 = true;
    [SerializeField] bool useAbility_2 = true;
    [SerializeField] bool useAbility_3 = false;
    [SerializeField] bool useAbility_4 = false;

    [Header("Ability scripts")]
    [SerializeField] private PlayerAbility Ability_1;
    [SerializeField] private PlayerAbility Ability_2;
    [SerializeField] private PlayerAbility Ability_3;
    [SerializeField] private PlayerAbility Ability_4;

    GameDataFile GDFile;
    InputController IC;


    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        IC = GDFile.GetComponent<InputController>();
        SubEvents();

        //Deactivate if script not assigned 
        if(!Ability_1) useAbility_1 = false;
        if (!Ability_2) useAbility_2 = false;
        if (!Ability_3) useAbility_3 = false;
        if (!Ability_4) useAbility_4 = false;
    }
    private void SubEvents()
    {
        IC.item2 += SwapAbility;
        IC.item4 += UseAbility;
    }
    private void UseAbility()
    {
        switch (currentAbility)
        {
            case 1:
                Ability_1.UseAbility();
                break;

            case 2:
                Ability_2.UseAbility();
                break;

            case 3:
                Ability_3.UseAbility();
                break;

            case 4:
                Ability_4.UseAbility();
                break;
        }
    }
    private void SwapAbility()
    {
        switch (currentAbility)
        {
            case 1:
                if (useAbility_2) activateAbility_2();
                else if (useAbility_3) activateAbility_3();
                else if (useAbility_4) activateAbility_4();
                break;

            case 2:
                if (useAbility_3) activateAbility_3();
                else if (useAbility_4) activateAbility_4();
                else if (useAbility_1) activateAbility_1();
                break;

            case 3:
                if (useAbility_4) activateAbility_4();
                else if (useAbility_1) activateAbility_1();
                else if (useAbility_2) activateAbility_2();
                break;

            case 4:
                if (useAbility_1) activateAbility_1();
                else if (useAbility_2) activateAbility_2();
                else if (useAbility_3) activateAbility_3();
                break;
        }
    }
    void activateAbility_1()
    {
        currentAbility = 1;
        playerAbility = Ability_1.abilityName();
    }
    void activateAbility_2()
    {
        currentAbility = 2;
        playerAbility = Ability_2.abilityName();
    }
    void activateAbility_3()
    {
        currentAbility = 3;
        playerAbility = Ability_3.abilityName();
    }
    void activateAbility_4()
    {
        currentAbility = 4;
        playerAbility = Ability_4.abilityName();
    }
}

public class PlayerAbility : MonoBehaviour
{    
    public virtual string abilityName() { return "None"; }
    public virtual void UseAbility()
    { Debug.Log("Default"); }
}
