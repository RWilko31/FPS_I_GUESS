using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_StartLevel : MonoBehaviour, IInteractFunction
{
    [Header("Level to load")]
    [SerializeField] string LevelName; 
    
    private GameDataFile GDFile;
    private EventManager EM;

    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        EM = GDFile.GetComponent<EventManager>();
    }
    public void Interact() { StartLevel(); }
    public string InteractType()
    { return "Load Level: " + LevelName; }
    public string InteractText()
    { return "to go to " + LevelName; }
    private void StartLevel()
    {
        if (LevelName == "") return;
        GDFile.MasterAudio.Stop();
        //string levelName = this.name.Remove(0, 24);
        GDFile.LoadLevel(LevelName);
    }
}
