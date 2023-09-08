using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_CraftInterface : MonoBehaviour, IInteractFunction
{
    GameDataFile GDFile;
    EventManager EM;

    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        EM = GDFile.GetComponent<EventManager>();
    }
    public void Interact() { EM.CraftEvent(); }
    public string InteractType() { return "Craft interface"; }
    public string InteractText() { return "to open craft interface"; }
}
