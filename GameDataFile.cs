using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class GameDataFile : MonoBehaviour
{
    //------------load/save---------------\\
    [Header("Save/load data")]
    [SerializeField] public bool overwriteData = false;
    [SerializeField] public bool loadData = false;
    #region savedata lists
    public List<string[]> Read_leveldata = new List<string[]>();
    public List<string[]> Read_healthdata = new List<string[]>();
    public List<string[]> Read_ammodata = new List<string[]>();
    public List<string[]> Read_weapondata = new List<string[]>();
    #endregion

    [Header("Server")]
    [SerializeField] public GameObject ServerContainer;
    [SerializeField] public string serverIpAddress;
    [SerializeField] public GameObject ServerPrefab;
    [SerializeField] public GameObject ClientPrefab;
    [SerializeField] public GameObject ActiveServer;
    [SerializeField] public GameObject ActiveClient;

    //-----Audio-----\\
    [Header("Audio")]
    #region AudioOutputs
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

    public GameObject MenuContainer;
    public GameObject EventSystem;

    #region FPS
    [Header("FpsCounter DONT ASSIGN")]
    public bool FpsCountEnabled = false;
    public TextMeshProUGUI FpsCounterText;
    public GameObject FpsCounterObject;
    private float timer, refresh, avgFramerate;
    string display = "{0} FPS";
    #endregion

    #region PauseMenu
    public bool isPaused = false;
    public float DefaultTime = 1;
    #endregion

    //-----ControlSchemes-----\\
    #region ControlSchemes
    [Header("Input Control Schemes")]
    [SerializeField] public string PcScheme = "Keyboard & Mouse";
    [SerializeField] public string PsScheme = "Playstation Controller";
    [SerializeField] public string XbScheme = "XBOX Controller";
    [SerializeField] public string GenControllerScheme = "Controller";
    public InputManager UserInput;
    #endregion

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

    //--------Variables-------\\
    #region PlayerHUD
    [Header("Player_HUD")]
    [SerializeField] public GameObject UI_Canvas;
    [SerializeField] public TextMeshProUGUI AmmoCounter;
    [SerializeField] public TextMeshProUGUI WeaponName;
    [SerializeField] public TextMeshProUGUI HudText;
    [SerializeField] public float FOVangle;
    #endregion

    #region Player
    [Header("Player")]
    //[SerializeField] public float player_Height = 2f;
    [SerializeField] public Transform player;
    [SerializeField] public Transform playerModel;
    [SerializeField] public Transform playerCam;
    [SerializeField] public Transform orientation;
    [SerializeField] public Transform cameraRig;

    [Header("Player_Gravity")]
    [SerializeField] public float Player_downForce = 16f;

    [Header("PlayerMovement")]
    [SerializeField] public float Player_maxSlopeAngle = 35f;
    [SerializeField] public float Player_walkSpeed = 8f;
    [SerializeField] public float Player_crouchSpeed = 5f;
    [SerializeField] public float Player_proneSpeed = 2.5f;
    [SerializeField] public float Player_sprintSpeed = 18f;
    [SerializeField] public float Player_overSpeed = 20f;
    [SerializeField] public float Player_airSpeed = 0.2f;
    [SerializeField] public float Player_acceleration = 16f;

    [Header("Player_Jump")]
    [SerializeField] public float Player_jumpForce = 20f;

    [Header("Player_Drag")]
    [SerializeField] public float Player_groundDrag = 6f;
    [SerializeField] public float Player_airDrag = 0.15f;

    [Header("Player_Antigravity")]
    [SerializeField] public float Player_antiGravityCeilingLimit = 100f;
    [SerializeField] public float Player_antiGravityFlipTime = 0.75f;
    [SerializeField] public float Player_antiGravityCoolDown = 2f;

    [Header("Player_Sliding")]
    [SerializeField] public float Player_slideCoolDown = 5f;
    [SerializeField] public float Player_slideForce = 75f;
    [SerializeField] public float Player_slideTime = 0.75f;
    [SerializeField] public float Player_slideAngle = 20f;
    [SerializeField] public float Player_slideHoldTime = 0.025f;

    [Header("Player_GrapplingHook")]
    [SerializeField] public float Player_maxGrapples = 3f;
    [SerializeField] public float Player_grappleHookCoolDown = 6f;

    [Header("Player_WallRunning")]
    [SerializeField] public float Player_maxWallJumps = 3f;
    [SerializeField] public float Player_wallRunningDistance = 0.6f;
    [SerializeField] public float Player_minimumJumpHeight = 1.5f;
    [SerializeField] public float Player_wallRunGravity = 6.81f;
    [SerializeField] public float Player_wallForce = 6f;
    [SerializeField] public float Player_wallRunTime = 1.5f;
    [SerializeField] public float Player_wallRunFallTime = 0.75f;
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
    #endregion

    //-------Functions-------\\
    private void Awake()
    {
        DefaultTime = Time.timeScale;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(UI_Canvas);
        DontDestroyOnLoad(player);
        MasterAudio = GetComponent<AudioSource>();
        UserInput = new InputManager();
        WeaponManager = player.GetComponent<WeaponManager>();
        WeaponManager.enabled = false;
        Time.timeScale = 0;
        OnOpenGame();
        findSaveFiles();
    }
    private void Update()
    {
        if (FpsCountEnabled) { FpsCounter(); }
    }
    public void PauseGame()
    {
        //player.GetComponent<CharacterMovementV2>().CursorLockAndUnlockState();
        isPaused = !isPaused;
        if (isPaused)
        {   //Pause game
            //Debug.Log("Pause GDS");
            Cursor.lockState = CursorLockMode.None;
            player.GetComponent<CharacterMovementV2>().PauseCharacterInput();
            player.GetComponent<WeaponAndItemScript>().PauseWeaponAndItemInput();
            MenuContainer.GetComponent<MainMenuScript>().PauseGame();
            Time.timeScale = 0f;
        }
        else
        {   //Resume
            //Debug.Log("Resume GDS");
            Cursor.lockState = CursorLockMode.Locked;
            player.GetComponent<CharacterMovementV2>().PauseCharacterInput();
            player.GetComponent<WeaponAndItemScript>().PauseWeaponAndItemInput();
            Time.timeScale = 1f;
        }
    }
    private void OnOpenGame()
    {   //Run as soon as game has been opened
        //if(loadData) LoadData(); //load savedata
        if (SceneManager.GetActiveScene().name == "OnStart")
        {
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, Screen.fullScreenMode);
            LoadLevel("MainMenu"); 
        }
    }
    public void LoadLevel(string LevelName)
    {
        if(LevelName == "MainMenu")
        {
            //ServerContainer.SetActive(false);
            Time.timeScale = 0;
            SceneManager.LoadScene("MainMenu");
            cameraRig.gameObject.SetActive(false);
            UI_Canvas.gameObject.SetActive(false);
            player.transform.position = new Vector3(-50, -50, 0);
            MasterAudio.clip = MainTheme;
            MasterAudio.Play();
            isPaused = false;
            MenuContainer.GetComponent<MainMenuScript>().inGame = false;
        }
        else
        {
            //ServerContainer.SetActive(true);
            MasterAudio.Stop();
            if (LevelName == "Level_Debug")
            { 
                SceneManager.LoadScene("Level_Debug");

                //Set player position/rotation at level start
                //player.position = new Vector3(0, 0, 0); 
                player.rotation = Quaternion.Euler(0, 0, 0); 
            }
            else if (LevelName == "Level_Test")
            { 
                SceneManager.LoadScene("Level_Test");

                //Set player position/rotation at level start
                //player.position = new Vector3(0, 0, 0);
                player.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (LevelName == "Level_MultiplayerTest")
            {
                SceneManager.LoadScene("Level_MultiplayerTest");

                //Set player position/rotation at level start
                //player.position = new Vector3(0, 0, 0);
                player.rotation = Quaternion.Euler(0, 0, 0);
            }

            //execute when level loads
            player.gameObject.SetActive(true);
            cameraRig.gameObject.SetActive(true);
            UI_Canvas.gameObject.SetActive(true);
            player.transform.position = new Vector3(0, 50, 0);
            player.GetComponent<Rigidbody>().Sleep();
            Time.timeScale = 1;
            isPaused = false;
        }

        //Always execute on load
        player.GetComponent<CharacterMovementV2>().xyRotation = Vector2.zero;
    }

    public string GetInteractableText(GameObject InteractableObject)
    {   //Get string data to display

        string controlScheme = player.GetComponent<PlayerInput>().currentControlScheme;

        if (InteractableObject.GetComponent<InteractableData>() == null) return "";
        string setPostText;
        string inputType = "";
        string displayInput = "";
        string type = InteractableObject.GetComponent<InteractableData>().InteractableType;

        if (type == "InteractableWeapon") { setPostText = weaponText; inputType = "interact"; }
        else if (type == "InteractableCube") { setPostText = interactText; inputType = "interact"; }
        else if (type == "InteractableDoor") { setPostText = DoorText; inputType = "interact"; }
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

    private void FpsCounter()
    {
        if (Time.unscaledTime > timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            FpsCounterText.text = fps + " FPS";
            timer = Time.unscaledTime + refresh;
        }
    }


    //-------save/load functions-----\\
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

            //debug for testing read
            //string[] debug = Read_ammodata[3];
            //Debug.Log(debug[1]);

            currentSaveFile = destination;
            dataLoaded = true;
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
    private string saveDestination;
    public void saveData(string destination) //saves the players current data
    {
        if (!overwriteData) return;
        //string destination = Application.dataPath + "/Files/" + "Save_Data.csv";
        //string destination = saveDestination;

        string[] saveFile = File.ReadAllLines(destination);
        StreamWriter writer = new StreamWriter(destination);
        writeData(writer, Read_leveldata);
        writeData(writer, Read_healthdata);
        writeData(writer, Read_weapondata);
        writeData(writer, WeaponManager.saveWeaponData());
        writer.Close();
    }
    private void writeData(StreamWriter writer, List<string[]> writeList)
    { //writes data into the savefile

        foreach(string[] line in writeList)
        {
            string fullLine ="";
            foreach(string data in line)
            {
                if (fullLine == "") fullLine = data;
                else fullLine += "," + data; 
            }
            Debug.Log(fullLine);
            writer.WriteLine(fullLine); 
        }
        writer.WriteLine("*end*");
    }
    public void createNewSave()
    {
        string newSave = Application.dataPath + "/Files/" + saveFiles.Count + "Save_Data.csv";
        StreamWriter writer = new StreamWriter(newSave);
        writer.Close();
        saveData(newSave);
    }

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
}

