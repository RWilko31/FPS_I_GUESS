﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;


public class GameDataFile : MonoBehaviour
{
    //--------Variables-------\\
    #region Variables
    //-----CheckPoints-----\\
    #region Player Checkpoints
    public GameObject SpawnPoint;
    public GameObject currentCheckPoint;
    #endregion

    //------------load/save---------------\\
    #region save/load bools
    [Header("Save/load data")]
    [SerializeField] public bool overwriteData = false;
    [SerializeField] public bool loadData = false;
    #endregion

    #region savedata lists
    [HideInInspector] public List<string[]> Read_leveldata = new List<string[]>();
    [HideInInspector] public List<string[]> Read_healthdata = new List<string[]>();
    [HideInInspector] public List<string[]> Read_ammodata = new List<string[]>();
    [HideInInspector] public List<string[]> Read_weapondata = new List<string[]>();
    [HideInInspector] public List<string[]> Read_itemdata = new List<string[]>();
    [HideInInspector] public List<string[]> Read_accessorydata = new List<string[]>();
    #endregion

    //-----Server-----\\
    #region ServerData
    [Header("Server")]
    [SerializeField] public GameObject ServerContainer;
    [SerializeField] public bool ipv = false; //false = 4, true = 6
    [SerializeField] public string serverIpAddress;
    [SerializeField] public string serverPort;
    #endregion

    #region Server/Client gameobjects
    [SerializeField] public GameObject ServerPrefab;
    [SerializeField] public GameObject ClientPrefab;
    [HideInInspector] public GameObject ActiveServer;
    [HideInInspector] public GameObject ActiveClient;
    #endregion

    //-----Audio-----\\
    #region AudioOutputs
    [Header("Audio")]

    public string currentTrack = "";
    public AudioSource MasterAudio;
    public AudioSource PlayerAudio;
    #endregion

    #region Music
    [Header("Music")]
    public AudioClip Expectations;
    public AudioClip Degredation;
    public AudioClip MainTheme;
    #endregion

    //-----Menu----\\
    #region Menu
    [Header("Menu")]
    public GameObject MenuContainer;
    public GameObject EventSystem;
    #endregion

    #region PauseMenu
    public float DefaultTime = 1;
    #endregion

    //-----ControlSchemes-----\\

    #region ControlSchemes
    [Header("Input Control Schemes")]
    [HideInInspector] public string PcScheme = "Keyboard & Mouse";
    [HideInInspector] public string PsScheme = "Playstation Controller";
    [HideInInspector] public string XbScheme = "XBOX Controller";
    [HideInInspector] public string GenControllerScheme = "Controller";
    #endregion

    //-----HealthSystem-----\\
    [Header("HealthSystem")]
    [SerializeField] public int lowDmg = 2;

    //---------Layers---------\\

    #region layers
    [Header("Layers")]
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public LayerMask InteractableLayer;
    [SerializeField] public LayerMask playerCamLayer;
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask EnemyLayer;
    [SerializeField] public LayerMask EverythingLayer;
    [SerializeField] public LayerMask searchLayer;
    [SerializeField] public LayerMask objectLayer;
    [SerializeField] public LayerMask deathBarrierLayer;
    [SerializeField] public LayerMask respawnLayer;
    [SerializeField] public LayerMask noClipLayer;
    [SerializeField] public LayerMask AreaLayer;
    #endregion

    //------Assignables-------\\

    #region Player
    [Header("ShootPoint")]
    [SerializeField] public Transform shootPoint;
    #endregion

    #region WeaponManager
    [SerializeField] public TextAsset WeaponTableCsv;
    [SerializeField] public bool limitSlots;
    [SerializeField] public int setMaxSlots;
    #endregion

    #region Equipment
    [Header("Lethal Models")]
    [SerializeField] public GameObject grenade_Model;
    #endregion

    #region PlayerHUD
    [Header("Player_HUD")]
    [SerializeField] public GameObject UI_Canvas;
    [SerializeField] public Image HealthBar;
    [SerializeField] public TextMeshProUGUI AmmoCounter;
    [SerializeField] public TextMeshProUGUI WeaponName;
    [SerializeField] public TextMeshProUGUI HudText;
    [SerializeField] public float FOVangle;
    #endregion

    #region Player
    [Header("Player")]
    [SerializeField] public Transform player;
    [SerializeField] public Transform orientation;
    [SerializeField] public Transform cameraRig;
    [SerializeField] public Transform playerCam;
    #endregion

    #region InteractableObjects
    [Header("Interactable Objects")]
    [SerializeField] public Transform objectGuide;
    [SerializeField] public Transform fixedobjectGuide;
    [SerializeField] public float objectSpeed;
    [SerializeField] public GameObject F;
    [SerializeField] public float holdOffset = 2f;
    [SerializeField] public string press_Text = "Press";
    [SerializeField] public GameObject Press_PC;
    [SerializeField] public GameObject Press_PS;
    [SerializeField] public GameObject Press_XB;
    //Interactable strings
    [Header("Interactable_Object_Text")]
    [SerializeField] public string interactText = "to Interact";
    [Header("Interactable_Weapon_Text")]
    [SerializeField] public string weaponText = "to pick up weapon";
    [Header("Interactable_Door_Text")]
    [SerializeField] public string DoorText = "to Open Door";
    #endregion

    #region Lethals
    [Header("Lethal")]
    [SerializeField] public float lethal_ThrowSpeed = 12f; 
    [SerializeField] public float lethal_Downforce = 2f; 
    [Header("Grenade")]
    [SerializeField] public int grenade_MaxAmount = 4;
    [SerializeField] public float grenade_Radius = 12f;
    [SerializeField] public float grenade_ThrowDistance = 8f;
    [SerializeField] public float grenade_Time = 3f;
    #endregion

    #region InteractableObjects
    [Header("Interactable Objects")]
    [SerializeField] public float maxObjectDistance = 5f;
    [SerializeField] public float objectMoveForce = 250f;
    [SerializeField] public float InteractableDistance = 5f;
    [SerializeField] public float moveDoorBy = 2.5f;
    [SerializeField] public float doorSpeed = 2.5f;
    [SerializeField] public float doorHoldTime = 2.5f;
    #endregion

    #region scripts
    public WeaponManager WeaponManager;
    public CsvManager csvManager;
    public EventManager EM;
    public ItemSystem itemSystem;
    public InputController IC;
    public MainMenuScript MMScript;
    public CharacterMovementV2 playerScript;
    public WeaponAndItemScript WeaponAndItemScript;
    #endregion

    #endregion

    //-------Functions-------\\
    #region Functions
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(UI_Canvas);
        DontDestroyOnLoad(player);

        DefaultTime = Time.timeScale;
        Time.timeScale = 0;

        MasterAudio = GetComponent<AudioSource>();
        IC = GetComponent<InputController>();
        EM = GetComponent<EventManager>();
        WeaponManager = player.GetComponent<WeaponManager>();
        csvManager = GetComponent<CsvManager>();
        MMScript = MenuContainer.GetComponent<MainMenuScript>();
        playerScript = player.GetComponent<CharacterMovementV2>();
        WeaponAndItemScript = player.GetComponent<WeaponAndItemScript>();
        itemSystem = GetComponent<ItemSystem>();
        subEvents();

        OnOpenGame();
    }
    void subEvents()
    {
        //Input controller
        IC.start += pauseCheck;

        //Event manager
        EM.respawn += Respawn;
    }
    #region Event functions
    void Respawn()
    {
        player.GetComponent<HealthScript>().ResetHealth();
        Pause("Resume"); 
    }
    #endregion

    private void OnOpenGame() //Run as soon as game has been opened
    {
        findSaveFiles();

        if (SceneManager.GetActiveScene().name == "OnStart")
        {
            LoadLevel("MainMenu");
        }
    }

    #region Interactable Text
    public string GetInteractableText(GameObject InteractableObject) //returns the display text for interactable objects
    {   //Get string data to display

        string controlScheme = GetComponent<PlayerInput>().currentControlScheme;

        if (InteractableObject.GetComponent<InteractableData>() == null) return "";
        string setPostText;
        string inputType = "";
        string displayInput = "";
        string type = InteractableObject.GetComponent<InteractableData>().InteractableType;

        if (type == "InteractableWeapon") { setPostText = weaponText; inputType = "interact"; }
        else if (type == "InteractableCube") { setPostText = interactText; inputType = "interact"; }
        else if (type == "InteractableDoor") { setPostText = DoorText; inputType = "interact"; }
        else if (type == "CraftInterface") { setPostText = "to open craft interface"; inputType = "interact"; }
        else setPostText = ""; inputType = "interact"; ;

        if (controlScheme == "Keyboard & Mouse")
        {
            if (inputType == "interact") displayInput = "F";
        }
        else if (controlScheme == "Playstation Controller")
        {
            if (inputType == "interact") displayInput = "<sprite index=23 tint>";
        }
        string setDisplayText = press_Text + " " + displayInput + " " + setPostText;

        return setDisplayText;
    }
    #endregion

    #region gcAlloc
    private void cleanGcAlloc()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    #endregion

    #region Pause Game
    void pauseCheck()
    { if (gameState.inGame) Pause("Menu"); }
    public void Pause(string type) //pauses game (Menu ,Temp, Resume)
    {
        ///About type
        /// Menu - is used to trigger the MMScript to bring up the pause menu
        /// Temp - Pause the game temporarily for events such as the deathScreen or Inventory
        /// Resume - resumes game

        if (type != "Resume")
        {
            //Pause game
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            UI_Canvas.SetActive(false);
            gameState.paused = true;
            IC.PauseCharacterInput("Menu");
            if (type == "Menu")  MMScript.PauseMenu();
        }
        else
        {
            //Resume
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            UI_Canvas.SetActive(true);
            gameState.paused = false;
            IC.PauseCharacterInput("Player");
            cleanGcAlloc();
        }
    }
    #endregion

    #region Load levels
    public string GetCurrentLevel() //returns the name of the current level
    { return SceneManager.GetActiveScene().name; }
    public void LoadLevel(string LevelName) //Loads the level with the name provided
    {
        //ServerContainer.SetActive(true);
        currentCheckPoint = null;
        MasterAudio.Stop();

        //Execute for specific level (such as play music etc..)
        switch (LevelName)
        {
            case "MainMenu":
                Time.timeScale = 0;
                cameraRig.gameObject.SetActive(false);
                UI_Canvas.gameObject.SetActive(false);
                player.transform.position = new Vector3(-50, -50, 0);
                MasterAudio.clip = MainTheme;
                currentTrack = "MainTheme";
                MasterAudio.Play();
                gameState.paused = true;
                gameState.inGame = false;
                SceneManager.LoadScene(LevelName);
                return;

            case "Level_Debug":
                //player.transform.position = new Vector3(0, 1000, 0);
                break;

            case "Level_Test":
                break;

            case "Level_MultiplayerTest":
                break;

            default:
                break;
        }
        SceneManager.LoadScene(LevelName);
        EM.LoadScene();

        //clean up menu assets
        cleanGcAlloc();

        //Always execute on load
        player.gameObject.SetActive(true);
        cameraRig.gameObject.SetActive(true);
        UI_Canvas.gameObject.SetActive(true);
        playerScript.rb.Sleep();
        playerScript.xyRotation = Vector2.zero;
        Time.timeScale = 1;
        gameState.paused = false;        
    }
    private void LoadLevelFromSave(string level, Vector3 position) //Loads level from save data
    {
        SceneManager.LoadScene(level);
        player.position = position;
    }
    #endregion

    #region Checkpoints
    public void MoveToPoint(GameObject MoveToObj) //move to spawnpoint or checkpoint
    {
        player.transform.position = MoveToObj.transform.position;
        var rot = MoveToObj.transform.rotation.eulerAngles;
        player.transform.rotation = Quaternion.Euler(rot.x, rot.y + 90, rot.z);
        playerScript.xyRotation = Vector2.zero;
        currentInput.xyLook = Vector2.zero;
    }
    #endregion

    #region save/Load functions
    public List<string> saveFiles = new List<string>();
    public string currentSaveFile;
    public bool dataLoaded = false;
    public void findSaveFiles()
    {
        string destination = Application.dataPath + "/Files";
        string[] files = System.IO.Directory.GetFiles(destination);
        foreach (string file in files)
        { if (file.Contains("Save_Data.csv") && !file.Contains("meta")) { /*Debug.Log(file);*/ saveFiles.Add(file); } }
    }
    public void LoadData(string destination) //reads save data and assigns it to lists for other scripts to use
    {
        //string destination = Application.dataPath + "/Files/" + "Save_Data.csv";

        if (!File.Exists(destination)) //if save file is missing create a new one
        {
            Debug.Log("No save data found!");
            dataLoaded = true;
            return;
        }
        else //load save data if file is found
        {
            string[] saveFile = File.ReadAllLines(destination); //read all lines and seperate in array
            List<string[]> lines = new List<string[]>();

            foreach (string line in saveFile) //iterates each line into and seperates the rows into arrays to recreate the table
            {
                string[] row = line.Split(',');
                lines.Add(row);
            }
            //clear lists and read data
            Read_leveldata.Clear(); readData(lines, Read_leveldata, "leveldata");
            Read_healthdata.Clear(); readData(lines, Read_healthdata, "healthdata");
            Read_ammodata.Clear(); readData(lines, Read_ammodata, "ammodata");
            Read_weapondata.Clear(); readData(lines, Read_weapondata, "weapondata");
            Read_itemdata.Clear(); readData(lines, Read_itemdata, "itemdata");
            Read_accessorydata.Clear(); readData(lines, Read_accessorydata, "accessorydata");

            //debug for testing read
            #region debug
            //string[] debug = Read_leveldata[2]; //row
            //Debug.Log(debug[2]); //column
            //Debug.Log(debug[3]); //column
            //Debug.Log(debug[4]); //column
            #endregion

            currentSaveFile = destination;
            dataLoaded = true;

            //load level
            string[] loadlevelData = Read_leveldata[2]; //row
            string loadLevel = loadlevelData[0]; //column
            Vector3 pos = new Vector3(float.Parse(loadlevelData[2]), float.Parse(loadlevelData[3]), float.Parse(loadlevelData[4]));
            //Vector3 pos = Vector3.zero; load at 0,0,0 if error with position
            LoadLevelFromSave(loadLevel, pos);

            //load items
            itemSystem.LoadItemData();
            WeaponManager.startWeaponManager();
        }
    }
    private void readData(List<string[]> lines ,List<string[]> dataList, string identifier)
    {   //goes through the lines to find the section of data requested

        int readLine = 0;
        bool record = false;
        foreach (string[] row in lines) //check through each line to seperate the sections
        {
            if (row[0].Contains(identifier)) record = true;
            if (row[0].Contains("*end*")) record = false;
            if (record) dataList.Add(row);
            readLine++;
        }
    }
    public void saveData(string destination) //saves the players current data
    {
        if (!overwriteData) return;

        string[] saveFile = File.ReadAllLines(destination);
        StreamWriter writer = new StreamWriter(destination);
        writeData(writer, saveLevelData()); //level info
        writeData(writer, Read_healthdata); //player health
        writeData(writer, WeaponManager.saveWeaponData()); //no of weapon slots and current weapon
        writeData(writer, WeaponManager.saveAmmoData()); //equiped weapons
        writeData(writer, itemSystem.saveItemData()); //player items
        writeData(writer, itemSystem.saveAccessoryData()); //player accessories
        writer.Close();
    }
    private void writeData(StreamWriter writer, List<string[]> writeList) //Writes data into the save file
    { //writes data into the savefile

        foreach(string[] line in writeList)
        {
            string fullLine ="";
            foreach(string data in line)
            {
                if (fullLine == "") fullLine = data;
                else fullLine += "," + data; 
            }
            //Debug.Log(fullLine);
            writer.WriteLine(fullLine); 
        }
        writer.WriteLine("*end*");
    } 
    public void createNewSave() //creates a new file for save data
    {
        string newSave = Application.dataPath + "/Files/" + saveFiles.Count + "Save_Data.csv";
        StreamWriter writer = new StreamWriter(newSave);
        writer.Close();
        saveData(newSave);
    }
    public List<string[]> saveLevelData() //formats the curent level data to write into the save file
    {
        List<string[]> leveldata = new List<string[]>();

        //add section tag
        string[] tag = new string[1]; 
        tag[0] = "leveldata";
        leveldata.Add(tag);

        //add table headings for Line 1
        string[] headings = new string[5]; 
        headings[0] = "levelname";
        headings[1] = "checkpoint";
        headings[2] = "posx";
        headings[3] = "posy";
        headings[4] = "posz";
        leveldata.Add(headings);

        //get current data
        string levelname = GetCurrentLevel();
        int checkpoint = 0;
        Vector3 pos = player.position;

        //write data to line 2
        string[] data = new string[5];
        data[0] = levelname;
        data[1] = checkpoint.ToString();
        data[2] = pos.x.ToString();
        data[3] = pos.y.ToString();
        data[4] = pos.z.ToString();
        leveldata.Add(data);

        //return formatted data
        return leveldata;
    }
    #endregion

    #region server
    public static string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    #endregion

    #endregion
}

