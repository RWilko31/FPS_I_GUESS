using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.InputSystem;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour
{
    /////-----Variables-----\\\\\
    GameDataFile gameDataFile;
    EventSystem EventSystem;
    [SerializeField] GameObject menuContainer;
    [SerializeField] public bool inGame;
    [SerializeField] public bool inputFieldActive = false;

    #region MainMenu
    [Header("StartMenu")]
    [SerializeField] private GameObject StartMenuContainer;
    [SerializeField] private GameObject StartGameText;
    [SerializeField] private GameObject ResumeGameText;
    [SerializeField] private GameObject ExitGameText;
    [SerializeField] private GameObject OptionsText;
    [SerializeField] private GameObject StartGameMenuContainer;
    #endregion

    #region PauseMenu
    [Header("StartMenu")]
    [SerializeField] public GameObject PauseMenuContainer;
    #endregion

    #region Loading Screen
    [Header("Loading Screen")]
    [SerializeField] GameObject loadingScreenContainer;
    [SerializeField] TextMeshProUGUI loadingText;
    #endregion

    #region SaveMenu
    [SerializeField] public GameObject loadSaveMenuContainer;
    [SerializeField] public GameObject loadSaveText;
    [SerializeField] public GameObject newSaveText;
    [SerializeField] public TextMeshProUGUI save1text;
    [SerializeField] public TextMeshProUGUI save2text;
    [SerializeField] public TextMeshProUGUI save3text;
    [SerializeField] public GameObject save1;
    [SerializeField] public GameObject save2;
    [SerializeField] public GameObject save3;
    #endregion

    #region ErrorMessage
    [SerializeField] public GameObject PopUpMsgContainer;
    [SerializeField] public GameObject okButton;
    [SerializeField] public TextMeshProUGUI MsgText;
    #endregion

    #region OptionsMenu
    [Header("OptionsMenu")]
    [SerializeField] GameObject OptionsMenuContainer;
    [SerializeField] GameObject OptionsCommonAssets;
    [SerializeField] GameObject BackToMenuButton;
    [SerializeField] GameObject VideoMenuButton;
    [SerializeField] GameObject SoundMenuButton;
    [SerializeField] GameObject ControlsMenuButton;
    [SerializeField] Color TabSelectedColour;
    [SerializeField] Color TabNormalColour;

    [Header("VideoMenu")]
    [SerializeField] GameObject VideoContainer;
    [SerializeField] GameObject CurrentResolutionText;
    [SerializeField] GameObject SetResolutionDropDown;
    [SerializeField] GameObject FullScreenText;
    [SerializeField] GameObject DisplayOptionsDropDown;
    [SerializeField] GameObject vSyncText;
    [SerializeField] GameObject vSyncDropDown;
    [SerializeField] GameObject FpsEnabledText;
    [SerializeField] GameObject FpsEnabledDropdown;
    [SerializeField] GameObject FpsCapText;
    [SerializeField] GameObject FpsCapSlider;
    [SerializeField] GameObject CancelText;
    [SerializeField] GameObject ApplyText;

    //ApplySettings
    [SerializeField] GameObject ApplySettingsContainer;
    [SerializeField] GameObject ApplySettings;
    [SerializeField] GameObject CancelSettings;
    bool VideoUiEnabled = true;

    FullScreenMode ScreenMode = 0;
    int screenResolutionX = 1920;
    int screenResolutionY = 1080;
    string DisplayMode = "FullScreen";
    public bool inOptionsMenu = false;
    int SetFS = 1;
    string vSyncMode = "V-Sync disabled";
    string fpsCountMode = "Disabled";
    int SetVS = 0;
    int vsyncValue = 0;

    [Header("SoundMenu")]
    [SerializeField] GameObject SoundContainer;
    [SerializeField] GameObject MusicVolumeText;
    [SerializeField] GameObject GameVolumeText;
    [SerializeField] GameObject sfxVolumeText;
    [SerializeField] GameObject MusicVolumeSlider;
    [SerializeField] GameObject GameVolumeSlider;
    [SerializeField] GameObject sfxVolumeSlider;

    [Header("ControlsMenu")]
    [SerializeField] GameObject ControlsContainer;
    [SerializeField] GameObject xSensitivitySlider;
    [SerializeField] GameObject ySensitivitySlider;
    [SerializeField] GameObject xSensitivityValue;
    [SerializeField] GameObject ySensitivityValue;
    [SerializeField] GameObject km_rebindTab;
    [SerializeField] GameObject c_rebindTab;
    #endregion

    #region FPS
    [Header("FpsCounter")]
    [SerializeField] public GameObject FpsCounter;
    public TextMeshProUGUI FpsCounterText;
    bool FpsCountEnabled = false;
    int FpsCap = 60;
    private float timer;
    private float refresh = 0.05f;
    #endregion

    #region Input
    InputManager Controls;
    Vector2 xyMove = Vector2.zero;
    public bool inDropDown = false;
    #endregion

    #region MessageGUI
    [Header("MessageGUI")]
    [SerializeField] public GameObject MessageContainer;
    [SerializeField] public TMP_InputField TextBox;
    [SerializeField] public TextMeshProUGUI serverButton;
    [SerializeField] public TextMeshProUGUI chatScreen;
    #endregion

    #region multiplayerMenu
    [Header("MultiplayerMenu")]
    [SerializeField] public GameObject MultiplayerContainer;
    [SerializeField] public GameObject hostGameButton;
    [SerializeField] public GameObject joinGameButton;

    [SerializeField] public TMP_InputField EnterIP;
    [SerializeField] public TMP_InputField EnterPort;
    [SerializeField] public bool EditText = false;
    [SerializeField] public TextMeshProUGUI userIP;
    [SerializeField] public Toggle IpvToggle;
    #endregion

    #region deathScreen
    [Header("Death Screen")]
    [SerializeField] public GameObject deathScreenContainer;
    [SerializeField] public GameObject respawnButton;
    [SerializeField] public TextMeshProUGUI deathMessage;
    #endregion

    #region Inventory
    [Header("Inventory")]
    public GameObject InterfaceContainer;
    public GameObject InventoryContainer;
    public GameObject CraftContainer;
    public GameObject PlayTimeCounter;
    public GameObject InterfaceBackButton;
    public float startTime;
    public float lastPlayTime = 0;
    public GameObject accessoriesContainer;
    public GameObject itemsContainer;
    public GameObject craftableItemsContainer;
    public TextMeshProUGUI itemDescName;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI craftDescName;
    public TextMeshProUGUI craftDescription;
    public GameObject itemSlotPrefab;
    public Color itemSlotDefaultColour;
    public Color itemSlotSelectedColour;
    #endregion

    /////-----Functions-----\\\\\
    private void Awake()
    {
        gameDataFile = FindObjectOfType<GameDataFile>();
        EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        gameDataFile.MenuContainer = transform.gameObject;
        gameDataFile.gameObject.GetComponent<PlayerInput>().uiInputModule = EventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        //gameDataFile.MenuContainer = menuContainer;
        gameDataFile.EventSystem = EventSystem.gameObject;

        //set default text in options
        LoadOptions();
        DontDestroyOnLoad(gameObject);
    }
    private int i = 0;
    private void Update()
    {
        if (inOptionsMenu)
        {
            //CurrentResolutionText.GetComponent<TextMeshProUGUI>().text = Screen.currentResolution.ToString();
            if (SetResolutionDropDown.GetComponent<TMP_Dropdown>().IsExpanded || DisplayOptionsDropDown.GetComponent<TMP_Dropdown>().IsExpanded || vSyncDropDown.GetComponent<TMP_Dropdown>().IsExpanded || FpsEnabledDropdown.GetComponent<TMP_Dropdown>().IsExpanded) inDropDown = true;
            else inDropDown = false;
        }
        if (FpsCountEnabled) { UpdateFpsCounter(); }
        if (InterfaceContainer.activeSelf)  {  playTimeCounter(); }
    }
    private void FixedUpdate()
    {
        if (loadingScreenContainer.activeSelf) updateLoadingScreen();

    }
    private void updateLoadingScreen()
    {
        i++;
        if (i >= 5)
        {
            i = 0;
            if (loadingText.text == "Loading....") loadingText.text = "Loading";
            else loadingText.text += ".";
        }
    }
    private void Start()
    {
        ShowMainMenu();
    }
    private void ShowMainMenu()
    {
        DisableAll();
        if (!inGame)
        {
            StartMenuContainer.SetActive(true);
            EventSystem.SetSelectedGameObject(StartGameText);
        }
        else
        {
            PauseMenuContainer.SetActive(true);
            EventSystem.SetSelectedGameObject(ResumeGameText);
        }
    }
    public void OnInputField() //de-activates the textfield when navigating
    {
        if (!MultiplayerContainer.activeSelf) return;
        if (gameDataFile.player.GetComponent<PlayerInput>().currentControlScheme != "Keyboard & Mouse") EnterIP.DeactivateInputField();
        //if (!EditText) { EnterIP.DeactivateInputField(); }
    }
    public void SelectOnNavigate() //Re-selects the last object if nothing is currently selected when navigating
    {
        if (EventSystem.currentSelectedGameObject == null)
        {
            EventSystem.SetSelectedGameObject(LastSelected);
        }
        else LastSelected = EventSystem.currentSelectedGameObject;
    }
    public void SelectLast() //selects and submits the last selected object if nothing is currently selected when submitting
    {
        if(EventSystem.currentSelectedGameObject == null)
        {
            EventSystem.SetSelectedGameObject(LastSelected);
        }
    }

    #region Start Menu Functions
    public void startGame() //Loads first level
    {
        DisableAll();
        startTime = Time.realtimeSinceStartup;
        Cursor.lockState = CursorLockMode.Locked;
        gameDataFile.inputScript.PauseCharacterInput();
    }
    IEnumerator loadingScreen() //displays the loading screen until the level is loaded
    {
        Time.timeScale = 0;
        gameDataFile.LoadLevel("Level_Debug");
        yield return new WaitWhile(() => gameDataFile.dataLoaded != true); ;
        loadingScreenContainer.SetActive(false);
        Time.timeScale = 1;
        inGame = true;
    }
    public void PauseGame() //Shows the pause menu
    {
        PauseMenuContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(null);
        EventSystem.SetSelectedGameObject(ResumeGameText);
    }
    public void ResumeGame() //Resumes game when paused
    {
        if (ApplySettingsContainer.activeSelf) return;
        if (VideoContainer.activeSelf) ApplySettingsCheck();
        if (!inGame) return; //Add apply settings option here instead

        //Deactivate Options menu text
        gameDataFile.PauseGame();
        DisableAll();
        inOptionsMenu = false;
    }
    public void ExitGame() //Function to load mainmenu
    {
        //Control scheme doesnt need changing as long as player exits from pause menu
        //gameDataFile.player.GetComponent<CharacterMovementV2>().PauseCharacterInput();
        //gameDataFile.player.GetComponent<WeaponAndItemScript>().PauseWeaponAndItemInput();

        ExitOnlineMatch();
        gameDataFile.LoadLevel("MainMenu");
        DisableAll();
        StartMenuContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(StartGameText);
        inGame = false;
    }
    public void QuitGame() //Chooses between quiting application and going to mainmenu(ExitGame())
    {
        if (!inGame) { Debug.Log("QuitGame"); Application.Quit(); }
        else ExitGame();
    }
    #endregion

    #region Options Menu
    public void Options() //hides mainmenu and brings up options menu
    {
        //Deactivate mainMenu
        DisableAll();
        //Activate Options menu
        inOptionsMenu = true;
        VideoOptions();
    }
    public void VideoOptions() //Shows the video Options menu
    {
        //Set current active menu
        DisableAll();
        OptionsMenuContainer.SetActive(true);
        VideoContainer.SetActive(true);

        if (!VideoUiEnabled) EnableDisableVideoUI();
        UpdateTabNavigation("video");

        //Change tab colour
        VideoMenuButton.GetComponent<Image>().color = TabSelectedColour;
        SoundMenuButton.GetComponent<Image>().color = TabNormalColour;
        ControlsMenuButton.GetComponent<Image>().color = TabNormalColour;

        //Default selection for controller
        EventSystem.SetSelectedGameObject(DisplayOptionsDropDown);

        //set default text in options
        setPreviousOptionsData();
        DropDownCaptionText();
    }
    public void SoundOptions() //Shows audio options menu
    {
        //Set current active menu
        DisableAll();
        OptionsMenuContainer.SetActive(true);
        SoundContainer.SetActive(true);

        //Change tab colour
        VideoMenuButton.GetComponent<Image>().color = TabNormalColour;
        SoundMenuButton.GetComponent<Image>().color = TabSelectedColour;
        ControlsMenuButton.GetComponent<Image>().color = TabNormalColour;

        //Default selection for controller
        EventSystem.SetSelectedGameObject(MusicVolumeSlider);

        UpdateTabNavigation("sound");
    }
    public void ControlOptions() //Shows control options menu
    {
        //Set current active menu
        DisableAll();
        OptionsMenuContainer.SetActive(true);
        ControlsContainer.SetActive(true);

        //Change tab colour
        VideoMenuButton.GetComponent<Image>().color = TabNormalColour;
        SoundMenuButton.GetComponent<Image>().color = TabNormalColour;
        ControlsMenuButton.GetComponent<Image>().color = TabSelectedColour;

        //Default selection for controller
        EventSystem.SetSelectedGameObject(km_rebindTab);

        UpdateTabNavigation("controls");
    }
    public void BackButton() //Returns to start Menu
    {
        if (EventSystem.currentSelectedGameObject == EnterIP) return;

        if (loadSaveMenuContainer.activeSelf)
        {
            loadSaveMenuContainer.SetActive(false);
            if (!inGame)
            {
                StartGameMenuContainer.SetActive(true);
                EventSystem.SetSelectedGameObject(loadSaveText);
            }
            else
            {
                PauseMenuContainer.SetActive(true);
                EventSystem.SetSelectedGameObject(ResumeGameText);
            }
            return;
        }
        if (ApplyText.activeSelf)
        {
            ApplySettingsCheck();
            return;
        }
        //Deactivate Options menu
        DisableAll();

        if (inOptionsMenu || !inGame)
        {
            inOptionsMenu = false;

            //Re-activate MainMenu/ResumeMenu text
            if (gameDataFile.isPaused) PauseMenuContainer.SetActive(true);
            else StartMenuContainer.SetActive(true);

            //set first selection in start menu
            if (!inGame) EventSystem.SetSelectedGameObject(StartGameText);
            else EventSystem.SetSelectedGameObject(ResumeGameText);
        }
        else if (inGame) ResumeGame();
    }
    public void LeftTab() //Scrolls Left between tabs when L1 is pressed
    {
        if (!inOptionsMenu) return;
        if (VideoContainer.activeSelf) ControlOptions();
        else if (ControlsContainer.activeSelf) SoundOptions();
        else VideoOptions();
    }
    public void RightTab() //Scrolls Right between tabs when R1 is pressed
    {
        if (!inOptionsMenu) return;
        if (VideoContainer.activeSelf) SoundOptions();
        else if (ControlsContainer.activeSelf) VideoOptions();
        else ControlOptions();
    }
    public void UpdateTabNavigation(string menu) //updates the navigation of the tab headings in the options menu
    {
        Selectable navTo = null;
        if (menu == "video") { navTo = DisplayOptionsDropDown.GetComponent<TMP_Dropdown>(); }
        else if (menu == "sound") { navTo = MusicVolumeSlider.GetComponent<Slider>(); }
        else if (menu == "controls") { navTo = km_rebindTab.GetComponent<Button>(); }

        Navigation nav = BackToMenuButton.GetComponent<Button>().navigation;
        nav.selectOnDown = navTo;
        nav.selectOnRight = VideoMenuButton.GetComponent<Button>();
        BackToMenuButton.GetComponent<Button>().navigation = nav;
        Navigation nav2 = VideoMenuButton.GetComponent<Button>().navigation;
        nav.selectOnDown = navTo;
        nav.selectOnRight = SoundMenuButton.GetComponent<Button>();
        nav.selectOnLeft = BackToMenuButton.GetComponent<Button>();
        VideoMenuButton.GetComponent<Button>().navigation = nav;
        Navigation nav3 = SoundMenuButton.GetComponent<Button>().navigation;
        nav.selectOnDown = navTo;
        nav.selectOnRight = ControlsMenuButton.GetComponent<Button>();
        nav.selectOnLeft = VideoMenuButton.GetComponent<Button>();
        SoundMenuButton.GetComponent<Button>().navigation = nav;
        Navigation nav4 = ControlsMenuButton.GetComponent<Button>().navigation;
        nav.selectOnDown = navTo;
        nav.selectOnLeft = SoundMenuButton.GetComponent<Button>();
        ControlsMenuButton.GetComponent<Button>().navigation = nav;
    }
    #endregion

    #region Disable All
    public void DisableAll()
    {
        StartMenuContainer.SetActive(false);
        PauseMenuContainer.SetActive(false);
        OptionsMenuContainer.SetActive(false);
        StartGameMenuContainer.SetActive(false);
        loadSaveMenuContainer.SetActive(false);
        MultiplayerContainer.SetActive(false);
        VideoContainer.SetActive(false);
        SoundContainer.SetActive(false);
        ControlsContainer.SetActive(false);
        ApplySettingsContainer.SetActive(false);
        InterfaceContainer.SetActive(false);
        InventoryContainer.SetActive(false);
        CraftContainer.SetActive(false);
    }
    #endregion

    #region Video Options Functions
    public void ChangeResolution()
    {
        ApplyText.SetActive(true);
        int SetRes = SetResolutionDropDown.GetComponent<TMP_Dropdown>().value;
        int Xres = screenResolutionX;
        int Yres = screenResolutionY;

        if (SetRes == 0) { Xres = 3840; Yres = 2160; }
        else if (SetRes == 1) { Xres = 3200; Yres = 1800; }
        else if (SetRes == 2) { Xres = 2560; Yres = 1440; }
        else if (SetRes == 3) { Xres = 1920; Yres = 1080; }
        else if (SetRes == 4) { Xres = 1600; Yres = 900; }
        else if (SetRes == 5) { Xres = 1366; Yres = 768; }
        else if (SetRes == 6) { Xres = 1280; Yres = 720; }
        else if (SetRes == 7) { Xres = 960; Yres = 540; }
        else if (SetRes == 8) { Xres = 960; Yres = 540; }
        else if (SetRes == 9) { Xres = 720; Yres = 480; }
        else if (SetRes == 10) { Xres = 640; Yres = 360; }

        screenResolutionX = Xres;
        screenResolutionY = Yres;
    }
    public void ChangeFullScreen()
    {
        ApplyText.SetActive(true);
        SetFS = DisplayOptionsDropDown.GetComponent<TMP_Dropdown>().value;
        if (SetFS == 0) { DisplayMode = "FullScreen"; ScreenMode = FullScreenMode.ExclusiveFullScreen; }
        else if (SetFS == 1) { DisplayMode = "Windowed"; ScreenMode = FullScreenMode.Windowed; }
        else if (SetFS == 2) { DisplayMode = "Windowed FullScreen"; ScreenMode = FullScreenMode.MaximizedWindow; }
    }
    public void ChangeVsync()
    {
        ApplyText.SetActive(true);
        SetVS = vSyncDropDown.GetComponent<TMP_Dropdown>().value;
        if (SetVS == 0) { vSyncMode = "V-Sync Enabled"; vsyncValue = 1; }
        else { vSyncMode = "V-Sync Disabled"; vsyncValue = 0; }
    }
    public void ShowFpsCounter()
    {
        ApplyText.SetActive(true);
        int showFps = FpsEnabledDropdown.GetComponent<TMP_Dropdown>().value;
        if (showFps == 0)
        {
            FpsCountEnabled = false;
            fpsCountMode = "Disabled";
        }
        else if (showFps == 1)
        {
            FpsCountEnabled = true;
            fpsCountMode = "Enabled";
        }
    }
    public void SetFpsCap()
    {
        ApplyText.SetActive(true);
        FpsCap = (int)FpsCapSlider.GetComponent<Slider>().value;
        FpsCapText.GetComponent<TextMeshProUGUI>().text = FpsCap.ToString() + "hz";
    }
    public void DropDownCaptionText()
    {
        DisplayOptionsDropDown.GetComponent<TMP_Dropdown>().captionText.SetText("Display Options");
        SetResolutionDropDown.GetComponent<TMP_Dropdown>().captionText.SetText("Resolution");
        vSyncDropDown.GetComponent<TMP_Dropdown>().captionText.SetText("V-Sync");
        FpsEnabledDropdown.GetComponent<TMP_Dropdown>().captionText.SetText("Fps Count");
    }
    public void ApplySettingsCheck() //displays pop up to cancel or save changes
    {
        EnableDisableVideoUI();
        ApplySettingsContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(CancelSettings);
    }
    public void CancelBackSwitch() //stops the backbutton when in a dropdown
    {
        if (ApplySettingsContainer.activeSelf) cancelSettings();
        else if (!inDropDown) BackButton();
    }
    public void cancelSettings() //cancels changes and goes to main menu
    {
        ApplySettingsContainer.SetActive(false);
        ApplyText.SetActive(false);
        DisableAll();
        ShowMainMenu();
    }
    public void applySettings() //Apply the current settings
    {
        ApplyText.SetActive(false);
        Screen.SetResolution(screenResolutionX, screenResolutionY, ScreenMode);
        Application.targetFrameRate = FpsCap;
        QualitySettings.vSyncCount = vsyncValue;

        setSensitivity();

        FpsCounter.SetActive(FpsCountEnabled);
        CurrentResolutionText.GetComponent<TextMeshProUGUI>().text = screenResolutionX.ToString() + " X " + screenResolutionY.ToString();
        FullScreenText.GetComponent<TextMeshProUGUI>().text = DisplayMode;
        vSyncText.GetComponent<TextMeshProUGUI>().text = vSyncMode;
        FpsEnabledText.GetComponent<TextMeshProUGUI>().text = fpsCountMode;

        //set default text in options
        DropDownCaptionText();

        EventSystem.SetSelectedGameObject(DisplayOptionsDropDown);
        SaveOptions();

        if (ApplySettingsContainer.activeSelf) BackButton();
        else VideoOptions();

        ApplySettingsContainer.SetActive(false);
    }
    #endregion

    #region Sound Options Funcions

    public void SetVolume()
    {
        gameDataFile.MasterAudio.volume = MusicVolumeSlider.GetComponent<Slider>().value;
        //gameDataFile.GameAudio.volume = GameVolumeSlider.GetComponent<Slider>().value;
        gameDataFile.PlayerAudio.volume = sfxVolumeSlider.GetComponent<Slider>().value;
        SaveOptions();
    }

    #endregion

    #region Control options Functions

    public void setSensitivity()
    {
        gameDataFile.player.GetComponent<CharacterMovementV2>().xySensitivity = new Vector2(xSensitivitySlider.GetComponent<Slider>().value, ySensitivitySlider.GetComponent<Slider>().value);
        xSensitivityValue.GetComponent<TextMeshProUGUI>().text = xSensitivitySlider.GetComponent<Slider>().value.ToString();
        ySensitivityValue.GetComponent<TextMeshProUGUI>().text = ySensitivitySlider.GetComponent<Slider>().value.ToString();
    }

    #endregion

    #region Message Functions
    public void ShowMesssageGUI()
    {
        if (!inGame) { return; }
        if (MessageContainer.activeSelf == false)
        {
            if (inGame)
            {
                gameDataFile.isPaused = true;
                gameDataFile.UI_Canvas.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                gameDataFile.inputScript.PauseCharacterInput();
            }
            MessageContainer.SetActive(true);
            EventSystem.SetSelectedGameObject(TextBox.gameObject);
            TextBox.Select();
        }
        else MessageBackButton();
    }
    public void MessageBackButton()
    {
        if (inGame)
        {
            gameDataFile.isPaused = false;
            gameDataFile.UI_Canvas.SetActive(true);
            gameDataFile.inputScript.PauseCharacterInput();
        }
        DisableAll();
    }

    List<string> chatList = new List<string>();
    public void EnterMessage()
    {
        if (TextBox.text == "") return;
        string idText = gameDataFile.ActiveClient.GetComponent<SendPlayerData>().PlayerId.ToString() + ": ";
        Net_ChatMessage msg = new Net_ChatMessage(idText + TextBox.text);
        gameDataFile.ActiveClient.GetComponent<Base_Client>().SendToServer(msg);
        TextBox.text = "";
        EventSystem.SetSelectedGameObject(TextBox.gameObject);
        TextBox.Select();
    }
    public void DisplayChat(string chatMsg)
    {
        if (chatList.Count >= 14) { chatList.RemoveAt(0); }
        chatList.Add(chatMsg + "\n");
        if (chatScreen.text == "") { chatScreen.text = chatMsg; return; }
        string currentText = "";
        foreach (string msg in chatList)
        { currentText = currentText + msg; }
        chatScreen.text = currentText;
    }
    #endregion

    #region MultiplayerMenu
    public void MultiplayerMenu()
    {
        //userIP.text = " User IP: " + GameDataFile.GetLocalIPAddress();

#if UNITY_EDITOR
        userIP.text = " User IP: " + " 192.168.1.220";
#endif
        DisableAll();
        MultiplayerContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(hostGameButton);
    }
    public void CopyIPToClipBoard()
    {
        string ipText = userIP.text;
        UnityEngine.TextEditor te = new UnityEngine.TextEditor();
        te.text = GameDataFile.GetLocalIPAddress();
#if UNITY_EDITOR
        te.text = "192.168.1.220";
#endif
        te.SelectAll();
        te.Copy();
        userIP.alignment = TextAlignmentOptions.Center;
        userIP.text = "Copied!";
        StartCoroutine(copiedTimer(ipText));
    }
    public void toggleIPV()
    {
        gameDataFile.ipv = IpvToggle.isOn;
    }
    IEnumerator copiedTimer(string ip)
    {
        yield return new WaitForSecondsRealtime(1f);
        userIP.alignment = TextAlignmentOptions.Left;
        userIP.text = ip;
    }
    public void ExitMultiplayerMenu()
    {
        DisableAll();
        StartMenuContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(StartGameText);
    }
    public void HostGame()
    {
        if (gameDataFile.ActiveServer != null) { Debug.Log("A server is already running!"); return; }
        if (gameDataFile.ActiveClient != null) { Debug.Log("You must exit the server before hosting a new game!"); return; }
        if (EnterIP.text != "") gameDataFile.serverIpAddress = EnterIP.text;
        if (EnterPort.text != "") gameDataFile.serverPort = EnterPort.text;
        StartNewGame();
        DisableAll();
        GameObject ServerPrefab = gameDataFile.ServerPrefab;
        GameObject server = Instantiate(ServerPrefab);
        GameObject ClientPrefab = gameDataFile.ClientPrefab;
        GameObject client = Instantiate(ClientPrefab);
        gameDataFile.ActiveServer = server;
        gameDataFile.ActiveClient = client;
        DontDestroyOnLoad(server);
        DontDestroyOnLoad(client);
    }
    public void JoinGame()
    {
        if (gameDataFile.ActiveServer != null) { PopUpMessage("Can not join while hosting a server"); /*Debug.Log("Could not join as a you are currently hosting a server");*/ return; }
        string ip = EnterIP.text;
        string port = EnterPort.text;
        if (ip == "") { PopUpMessage("You must enter an IP address!"); /*Debug.Log("You must enter an IP address!");*/ return; }
        gameDataFile.serverIpAddress = ip;
        gameDataFile.serverPort = port;
        GameObject ClientPrefab = gameDataFile.ClientPrefab;
        GameObject client = Instantiate(ClientPrefab);
        gameDataFile.ActiveClient = client;
        DontDestroyOnLoad(client);
        StartCoroutine(CheckServerConnected(client));
        StartCoroutine(ConnectionTimer(client));
    }
    public void ExitOnlineMatch()
    {
        EnterIP.text = "";
        if (gameDataFile.ActiveServer) { Destroy(gameDataFile.ActiveServer); gameDataFile.ActiveServer = null; }
        if (gameDataFile.ActiveClient) { Destroy(gameDataFile.ActiveClient); gameDataFile.ActiveClient = null; }
    }
    IEnumerator CheckServerConnected(GameObject client)
    {
        yield return new WaitUntil(() => !client || client.GetComponent<Base_Client>().connectedToServer);
        if (client)
        {
            StopCoroutine(ConnectionTimer(client));
            StartNewGame();
            DisableAll();
        }
    }
    IEnumerator ConnectionTimer(GameObject client)
    {
        Debug.Log("timer");
        yield return new WaitForSecondsRealtime(12f);
        if (client)
        {
            if (!client.GetComponent<Base_Client>().connectedToServer)
            {
                StopCoroutine(CheckServerConnected(client));
                FailedToConnect();
            }
        }
    }
    private void FailedToConnect()
    {
        //Debug.Log("Failed to connect to server!");
        ExitOnlineMatch();
        PopUpMessage("Failed to connect to server!");
    }
    #endregion

    #region DisableUI
    //Disables all UI interaction so that background elements cannot be clicked
    public void EnableDisableVideoUI()
    {
        VideoUiEnabled = !VideoUiEnabled;
        foreach (var dropDown in VideoContainer.GetComponentsInChildren<TMP_Dropdown>(true))
        { dropDown.interactable = VideoUiEnabled; }
        foreach (var btn in VideoContainer.GetComponentsInChildren<Button>(true))
        { btn.interactable = VideoUiEnabled; }
        foreach (var btn in OptionsCommonAssets.GetComponentsInChildren<Button>(true))
        { btn.interactable = VideoUiEnabled; }
    }

    #endregion

    #region save/Load settings
    public void SaveOptions() //saves the current settings 
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        SaveOptions data = new SaveOptions(MusicVolumeSlider.GetComponent<Slider>().value, GameVolumeSlider.GetComponent<Slider>().value, sfxVolumeSlider.GetComponent<Slider>().value,
            screenResolutionX, screenResolutionY, DisplayMode, vSyncMode, FpsCountEnabled, FpsCap, xSensitivitySlider.GetComponent<Slider>().value, ySensitivitySlider.GetComponent<Slider>().value,
            DisplayOptionsDropDown.GetComponent<TMP_Dropdown>().value, SetResolutionDropDown.GetComponent<TMP_Dropdown>().value, vSyncDropDown.GetComponent<TMP_Dropdown>().value,
            FpsEnabledDropdown.GetComponent<TMP_Dropdown>().value);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }
    SaveOptions optionsSaveData;
    public void LoadOptions() //loads the previous settings
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, Screen.fullScreenMode);
            //Debug.LogError("File not found");
            //PopUpMessage("No save data found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        optionsSaveData = (SaveOptions)bf.Deserialize(file);
        file.Close();

        setPreviousOptionsData();
        applySettings();
    }
    private void setPreviousOptionsData()  //resets dropdowns to correct values from savefile
    {
        if (optionsSaveData == null) return;

        //set slider values
        FpsCapSlider.GetComponent<Slider>().value = optionsSaveData.targetFps;
        MusicVolumeSlider.GetComponent<Slider>().value = optionsSaveData.musicVolume;
        GameVolumeSlider.GetComponent<Slider>().value = optionsSaveData.gameVolume;
        sfxVolumeSlider.GetComponent<Slider>().value = optionsSaveData.sfxVolume;
        xSensitivitySlider.GetComponent<Slider>().value = optionsSaveData.xSensitivty;
        ySensitivitySlider.GetComponent<Slider>().value = optionsSaveData.ySensitivty;
        xSensitivityValue.GetComponent<TextMeshProUGUI>().text = optionsSaveData.xSensitivty.ToString();
        ySensitivityValue.GetComponent<TextMeshProUGUI>().text = optionsSaveData.ySensitivty.ToString();

        //set variable values
        FpsCountEnabled = optionsSaveData.fpsEnabled;
        screenResolutionX = optionsSaveData.xResolution;
        screenResolutionY = optionsSaveData.yResolution;
        DisplayMode = optionsSaveData.displayMode;
        vSyncMode = optionsSaveData.vSyncMode;

        if (FpsCountEnabled) fpsCountMode = "Enabled";
        else fpsCountMode = "Disabled";

        //set dropdown values
        DisplayOptionsDropDown.GetComponent<TMP_Dropdown>().value = optionsSaveData.ScreenMode;
        SetResolutionDropDown.GetComponent<TMP_Dropdown>().value = optionsSaveData.Resolution;
        vSyncDropDown.GetComponent<TMP_Dropdown>().value = optionsSaveData.vSync;
        FpsEnabledDropdown.GetComponent<TMP_Dropdown>().value = optionsSaveData.Fps;
    }
    public void StartGameMenu() //show Load game Menu
    {
        DisableAll();
        StartGameMenuContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(loadSaveText);
    }
    public void StartNewGame() //show campaign menu
    {
        gameDataFile.LoadLevel("Level_Debug");
        gameDataFile.loadData = false;
        loadingScreenContainer.SetActive(false);
        Time.timeScale = 1;
        inGame = true;
        gameDataFile.player.GetComponent<WeaponManager>().startWeaponManager();
        startGame();
    }
    public void LoadSaveMenu() //show load save file menu
    {
        save1.SetActive(true); save1text.text = gameDataFile.saveFiles[0];
        if (gameDataFile.saveFiles.Count > 1) { save2.SetActive(true); save2text.text = gameDataFile.saveFiles[1]; }
        if (gameDataFile.saveFiles.Count > 2) { save3.SetActive(true); save3text.text = gameDataFile.saveFiles[2]; }
        loadSaveMenuContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(save1);
    } 
    public void LoadGame(int saveSlot) //load a given save file
    {
        DisableAll();
        loadingScreenContainer.SetActive(true);
        StartCoroutine(loadingScreen());
        gameDataFile.loadData = true;
        gameDataFile.LoadData(gameDataFile.saveFiles[saveSlot]);
        gameDataFile.player.GetComponent<WeaponManager>().startWeaponManager();
        startGame();
    }
    public void SaveGame(int saveSlot) //save to a given save file
    {
        DisableAll();
        gameDataFile.saveData(gameDataFile.saveFiles[saveSlot]);
        BackButton();
    } 
    public void saveSlot1() //save slot 1
    {
        if (gameDataFile.saveFiles.Count > 0 && !inGame) { Debug.Log("load"); LoadGame(0); }
        else { Debug.Log("save"); SaveGame(0); }
    } 
    public void saveSlot2() //save slot 2
    {
        if (gameDataFile.saveFiles.Count > 1 && !inGame) LoadGame(1);
        else SaveGame(1);
    }
    public void saveSlot3() //save slot 3
    {
        if (gameDataFile.saveFiles.Count > 2 && !inGame) LoadGame(2);
        else SaveGame(2);
    }
    public void importCreateSavebutton() //swap between import save or create new save file
    {
        if (!inGame) importSave();
        else CreateNewSave();
    }
    public void importSave() //open explorer to import a save file
    {
#if !UNITY_EDITOR
        return;
#endif
        string path = "";// = EditorUtility.OpenFilePanel("import save file", "", "csv");
        if (path.Length != 0)
        {
            if (!path.Contains("Save_Data.csv")) { PopUpMessage("Save file could not be loaded!"); return; }
            DisableAll();
            StartCoroutine(loadingScreen());
            gameDataFile.loadData = true;
            gameDataFile.LoadData(path);
            gameDataFile.player.GetComponent<WeaponManager>().startWeaponManager();
            startGame();
        }
    }
    public void CreateNewSave() //create a new save file
    {
        DisableAll();
        gameDataFile.createNewSave();
        gameDataFile.saveFiles.Clear();
        gameDataFile.findSaveFiles();
        BackButton();
    }
    #endregion

    #region PopUpMessage
    GameObject LastSelected;
    public void PopUpMessage(string msg) //displays a given message on screen
    {
        LastSelected = EventSystem.currentSelectedGameObject;
        EventSystem.SetSelectedGameObject(okButton);
        MsgText.text = msg; PopUpMsgContainer.SetActive(true); }
    public void acceptError() //accepts the message
    {
        PopUpMsgContainer.SetActive(false); MsgText.text = "An error occured";
        EventSystem.SetSelectedGameObject(LastSelected);
    }
    #endregion

    #region DeathScreen
    public void ShowDeathScreen(string KilledByEntity) //shows the deathscreen and displays the enemy that killed the player
    {
        gameDataFile.TemporaryPause();
        deathMessage.text = "Killed by " + KilledByEntity;
        gameDataFile.UI_Canvas.SetActive(false);
        deathScreenContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(respawnButton);
    }
    public void Respawn() //respawns the player
    {
        gameDataFile.UI_Canvas.SetActive(true);
        deathScreenContainer.SetActive(false);
        gameDataFile.TemporaryPause();
        gameDataFile.player.GetComponent<CharacterMovementV2>().Respawn();
        gameDataFile.player.GetComponent<HealthScript>().ResetHealth();
    }
    #endregion

    #region FPS Counter

    public void UpdateFpsCounter() //updates the fps counter
    {
        if (Time.unscaledTime > timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            FpsCounterText.text = fps + " FPS";
            timer = Time.unscaledTime + refresh;
        }
    }

    #endregion

    #region Interface
    public void CloseInterface() //closes all interfaces
    {
        if (!InventoryContainer.activeSelf && !CraftContainer.activeSelf) return;
        resetCrafting();
        resetInventory();
        DisableAll();
        LastSelected = null;
        gameDataFile.TemporaryPause();
    }

    int totalAccessorySlots = 0;
    int totalItemSlots = 0;
    int totalCraftSlots = 0;

    //crafting functions
    public void OpenCrafting() //opens crafting interface
    {
        gameDataFile.TemporaryPause();
        DisableAll();
        PreviousSlot = null;
        InterfaceContainer.SetActive(true);
        CraftContainer.SetActive(true);
        interfaceDefaultText();
        resetCrafting();
        ShowCraftableItems();
        EventSystem.SetSelectedGameObject(InterfaceBackButton);
    } 
    public void resetCrafting() //clears slots
    {
        foreach (Transform itemSlot in craftableItemsContainer.transform) { Destroy(itemSlot.gameObject); }
    }
    public void ShowCraftableItems() //adds itemslots to the craftable item column
    {
        totalCraftSlots = 0;
        foreach (string item in gameDataFile.itemSystem.showCraftList())
        { addSlot(item, "craft"); }
    }
    private void interfaceDefaultText() //updates the description and name to default settings
    {
        if (craftableItemsContainer.activeSelf)
        {
            craftDescName.text = "Crafting Terminal";
            craftDescription.text = "This crafting terminal can be used to combine items into gear or other useful items";
        }
    }

    //Inventory Functions
    public void OpenInventory() //opens inventory interface
    {
        gameDataFile.TemporaryPause();
        resetInventory();
        ShowItems();
        showAccessories();
        DisableAll();
        PreviousSlot = null;
        InterfaceContainer.SetActive(true);
        InventoryContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(InterfaceBackButton);
    }
    public void resetInventory() //clears slots
    {
        foreach (Transform itemSlot in accessoriesContainer.transform) { Destroy(itemSlot.gameObject); }
        foreach (Transform itemSlot in itemsContainer.transform) Destroy(itemSlot.gameObject);
    }
    public void ShowItems() //adds itemslots to the item column
    {
        totalItemSlots = 0;
        foreach (string item in gameDataFile.itemSystem.playerItems)
        { addSlot(item, "item"); }
    }
    public void showAccessories() //adds itemslots to the accessories column
    {
        totalAccessorySlots = 0;
        //assign current accessories to empty slots
        foreach(string accessory in gameDataFile.itemSystem.currentAccessories)
        {
            addSlot(accessory, "accessory");
        }
    }

    public void addSlot(string item, string type) //creates an itemslot
    {
        //add item to correct section in inventory
        GameObject column;
        if (type == "item") column = itemsContainer;
        else if (type == "craft") column = craftableItemsContainer;
        else column = accessoriesContainer;

        //create the itemslot
        GameObject itemslot = Instantiate(itemSlotPrefab);
        itemslot.transform.SetParent(column.transform, false);
        itemslot.transform.localScale = new Vector3(1, 1, 1); //fixes scale issues

        //update the itemslots slotnumber
        string quantity = "";
        int slot = 0;
        if (type == "accessory") 
        { 
            slot = totalAccessorySlots;
            itemslot.name = "accessorySlot" + slot;
            totalAccessorySlots++; 
        }
        else if (type == "item")
        { 
            slot = totalItemSlots;
            itemslot.name = "itemSlot" + slot;
            totalItemSlots++; 
        }
        else if(type == "craft")
        {
            slot = totalCraftSlots;
            itemslot.name = "craftSlot" + slot;
            totalCraftSlots++;
        }
        quantity = gameDataFile.itemSystem.getItemQuantity(item);

        //update itemSlot variables
        itemslot.GetComponent<ItemSlot>().updateSlotInfo(item, quantity, slot, type, this);
    }

    private ItemSlot PreviousSlot;
    public void itemSelected(ItemSlot itemSlot) //logic for when an item is selected
    {
        UpdateDescription(itemSlot.Item);
        if (itemSlot.Type == "craft") { craftItemSelected(itemSlot); return; }

        //if no previous slot selected or selecting same type or selecting the same item
        if (!PreviousSlot || (gameDataFile.itemSystem.currentAccessories.Contains(PreviousSlot.Item) && itemSlot.Item != PreviousSlot.Item) || (PreviousSlot.Type == itemSlot.Type && PreviousSlot != itemSlot)) 
        {
            if(PreviousSlot) PreviousSlot.Background.GetComponent<Image>().color = itemSlotDefaultColour;
            itemSlot.Background.GetComponent<Image>().color = itemSlotSelectedColour;
            PreviousSlot = itemSlot;
            return;
        }
        //if clicking off an accessory to an item only change colour
        if (PreviousSlot.Type == "accessory" && itemSlot.Type == "item") 
        {
            PreviousSlot.Background.GetComponent<Image>().color = itemSlotDefaultColour;
            itemSlot.Background.GetComponent<Image>().color = itemSlotSelectedColour;
            PreviousSlot = itemSlot;
            return;
        }
        string findName = itemSlot.name;
        //if double click accessory remove it
        if (PreviousSlot == itemSlot && itemSlot.Type != "item") 
        {
            //convert item
            if (itemSlot.Item == "" ) return;
            gameDataFile.itemSystem.AccessoriesToItem(itemSlot.Item, itemSlot.Slot);
            itemSlot.Item = "";
            itemSlot.Slot = 0;

            //update slots
            resetInventory();
            ShowItems();
            showAccessories();
            return;
        }
        //assign an item to an accessory Slot
        if (itemSlot.Type == "accessory") 
        {
            //change itemslot colours
            PreviousSlot.Background.GetComponent<Image>().color = itemSlotDefaultColour;
            itemSlot.Background.GetComponent<Image>().color = itemSlotSelectedColour;

            //convert items
            if(itemSlot.Item != "") gameDataFile.itemSystem.AccessoriesToItem(itemSlot.Item, itemSlot.Slot);
            gameDataFile.itemSystem.ItemToAccessories(PreviousSlot.Item, itemSlot.Slot);

            //update slots
            resetInventory();
            ShowItems();
            showAccessories();
        }
        EventSystem.SetSelectedGameObject(InterfaceBackButton);
        Debug.Log(findName);
        foreach (Transform accessory in accessoriesContainer.transform)
        {
            Debug.Log(accessory.name);
            if (accessory.name == findName)
            { EventSystem.SetSelectedGameObject(accessory.gameObject); return; }
        }
    }
    public void craftItemSelected(ItemSlot itemSlot) //logic for when a craftable item is selected
    {
        if (PreviousSlot) PreviousSlot.Background.GetComponent<Image>().color = itemSlotDefaultColour;
        itemSlot.Background.GetComponent<Image>().color = itemSlotSelectedColour;
        UpdateDescription(itemSlot.Item);
        PreviousSlot = itemSlot;
    }
    public void craftItem()
    {
        if (!PreviousSlot) return;
        gameDataFile.itemSystem.craftItem(PreviousSlot.Item);
        resetCrafting();
        ShowCraftableItems();
    }

    private void UpdateDescription(string item)
    {
        if (CraftContainer.activeSelf)
        {
            craftDescName.text = item;
            craftDescription.text = gameDataFile.csvManager.getItemDescription(item);
        }
        else if (InventoryContainer.activeSelf)
        {
            itemDescName.text = item;
            itemDescription.text = gameDataFile.csvManager.getItemDescription(item);
        }
    }
    public void playTimeCounter() //updates the playtime
    {
        int days = 0;
        int hours = 0;
        int mins = 0;
        int seconds = 0;

        string counter = "";
        int time = Mathf.RoundToInt(lastPlayTime + (Time.realtimeSinceStartup - startTime));
        if (time >= 86400) { Mathf.RoundToInt(days = time / 86400); time -= (days * 86400);  counter = days + "d:"; }
        if (time >= 3600) { Mathf.RoundToInt(hours = time / 3600); time -= (hours * 3600); counter = counter + hours + "h:"; }
        if (time >= 60) { mins = Mathf.RoundToInt(time / 60); time -= (mins * 60); counter = counter + mins + "m:"; }
        seconds = time; counter = counter + seconds + "s";

        PlayTimeCounter.GetComponent<TextMeshProUGUI>().text = counter;
    }
    #endregion
}
