using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;

public class MainMenuScript : MonoBehaviour
{
    /////-----Variables-----\\\\\
    GameDataFile GDfile;
    InputController IC;
    EventManager EM;
    EventSystem EventSystem;
    [SerializeField] GameObject menuContainer;
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
    private float refresh = 0.5f;
    #endregion

    #region Input
    InputManager Controls;
    Vector2 xyMove = Vector2.zero;
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
    public TextMeshProUGUI PlayTimeCounterText;
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

    #region DropDowns
    private TMP_Dropdown resolutionDd;
    private TMP_Dropdown displayOptionDd;
    private TMP_Dropdown vSyncDd;
    private TMP_Dropdown fpsDd;
    #endregion

    /////-----Functions-----\\\\\
    private void Awake()
    {
        GDfile = FindObjectOfType<GameDataFile>();
        IC = GDfile.GetComponent<InputController>();
        EM = GDfile.GetComponent<EventManager>();

        EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        GDfile.MenuContainer = transform.gameObject;
        GDfile.gameObject.GetComponent<PlayerInput>().uiInputModule = EventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        PlayTimeCounterText = PlayTimeCounter.GetComponent<TextMeshProUGUI>();
        resolutionDd = SetResolutionDropDown.GetComponent<TMP_Dropdown>();
        displayOptionDd = DisplayOptionsDropDown.GetComponent<TMP_Dropdown>();
        vSyncDd = vSyncDropDown.GetComponent<TMP_Dropdown>();
        fpsDd = FpsEnabledDropdown.GetComponent<TMP_Dropdown>();
        subEvents();

        //gameDataFile.MenuContainer = menuContainer;
        GDfile.EventSystem = EventSystem.gameObject;

        //set default text in options
        LoadOptions();
        setPreviousOptionsData();
        applySettings();
        DontDestroyOnLoad(gameObject);
    }
    private void subEvents() //subscribe to required input events
    {
        IC.resume += ResumeGame;
        IC.cancel += cancel;
        IC.leftTab += LeftTab;
        IC.rightTab += RightTab;
        IC.message += messageCheck;
        IC.messageMenu += ShowMesssageGUI;
        IC.submit += submit;
        IC.select += OpenInventory;
        IC.navigate += navigate;
        IC.inventory += CloseInterface;

        EM.craft += OpenCrafting;
    }
    private int i = 0;
    private float time = 0;
    void submit()
    {
        EditText = true;
        SelectLast();
        if (MessageContainer.activeSelf) { EnterMessage(); }
    }
    void navigate()
    {
        //playerScript
        OnInputField();
        SelectOnNavigate();
        /*if (inDropDown) { autoScrollDropDown(ctx.ReadValue<Vector2>());}*/
    }

    private bool inDropDown()
    {
        return(
                resolutionDd.IsExpanded ||
                displayOptionDd.IsExpanded ||
                vSyncDd.IsExpanded ||
                fpsDd.IsExpanded
              );
    }
    private void Update()
    {
        if (FpsCountEnabled) { UpdateFpsCounter(); }
        if (InterfaceContainer.activeSelf && time >= 1f)  { time = 0; PlayTimeCounterText.SetText(playTimeCounter(lastPlayTime, startTime)); }
        time += Time.unscaledDeltaTime;
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
        if (!gameState.inGame)
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
        if (GDfile.player.GetComponent<PlayerInput>().currentControlScheme != "Keyboard & Mouse") EnterIP.DeactivateInputField();
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
        GDfile.IC.PauseCharacterInput("Player");
    }
    IEnumerator loadingScreen() //displays the loading screen until the level is loaded
    {
        Time.timeScale = 0;
        GDfile.LoadLevel("Level_Debug");
        yield return new WaitWhile(() => GDfile.dataLoaded != true); ;
        loadingScreenContainer.SetActive(false);
        Time.timeScale = 1;
        gameState.inGame = true;
    }
    public void PauseMenu() //Shows the pause menu
    {
        PauseMenuContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(null);
        EventSystem.SetSelectedGameObject(ResumeGameText);
    }
    public void ResumeGame() //Resumes game when paused
    {
        if (!PauseMenuContainer.activeSelf) return;
        if (ApplySettingsContainer.activeSelf) return;
        if (VideoContainer.activeSelf) ApplySettingsCheck();
        if (!gameState.inGame) return; //Add apply settings option here instead

        //Deactivate Options menu text
        GDfile.Pause("Resume");
        DisableAll();
        inOptionsMenu = false;
    }
    public void ExitGame() //Function to load mainmenu
    {
        //Control scheme doesnt need changing as long as player exits from pause menu
        //gameDataFile.player.GetComponent<CharacterMovementV2>().PauseCharacterInput();
        //gameDataFile.player.GetComponent<WeaponAndItemScript>().PauseWeaponAndItemInput();

        ExitOnlineMatch();
        GDfile.LoadLevel("MainMenu");
        DisableAll();
        StartMenuContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(StartGameText);
        gameState.inGame = false;
    }
    public void QuitGame() //Chooses between quiting application and going to mainmenu(ExitGame())
    {
        if (!gameState.inGame) { Debug.Log("QuitGame"); Application.Quit(); }
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
            if (!gameState.inGame)
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

        if (inOptionsMenu && gameState.inGame)
        {
            inOptionsMenu = false;
            PauseMenuContainer.SetActive(true);
            EventSystem.SetSelectedGameObject(ResumeGameText);

        }
        else if (!gameState.inGame)
        {
            inOptionsMenu = false;

            //Re-activate MainMenu/ResumeMenu text
            StartMenuContainer.SetActive(true);

            //set first selection in start menu
            EventSystem.SetSelectedGameObject(StartGameText);
        }
        else if (gameState.inGame) ResumeGame();
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
        else if (!inDropDown()) BackButton();
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
        GDfile.MasterAudio.volume = MusicVolumeSlider.GetComponent<Slider>().value;
        //gameDataFile.GameAudio.volume = GameVolumeSlider.GetComponent<Slider>().value;
        GDfile.PlayerAudio.volume = sfxVolumeSlider.GetComponent<Slider>().value;
        SaveOptions();
    }

    #endregion

    #region Control options Functions

    public void setSensitivity()
    {
        GDfile.player.GetComponent<CharacterMovementV2>().xySensitivity = new Vector2(xSensitivitySlider.GetComponent<Slider>().value, ySensitivitySlider.GetComponent<Slider>().value);
        xSensitivityValue.GetComponent<TextMeshProUGUI>().text = xSensitivitySlider.GetComponent<Slider>().value.ToString();
        ySensitivityValue.GetComponent<TextMeshProUGUI>().text = ySensitivitySlider.GetComponent<Slider>().value.ToString();
    }

    #endregion

    #region Message Functions
    void messageCheck()
    {
        if (GDfile.ActiveClient) ShowMesssageGUI();
    }
    public void ShowMesssageGUI()
    {
        if (!gameState.inGame) { return; }
        if (MessageContainer.activeSelf == false)
        {
            if (gameState.inGame)
            {
                gameState.paused = true;
                GDfile.UI_Canvas.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                GDfile.IC.PauseCharacterInput("Menu");
            }
            MessageContainer.SetActive(true);
            EventSystem.SetSelectedGameObject(TextBox.gameObject);
            TextBox.Select();
        }
        else MessageBackButton();
    }
    public void MessageBackButton()
    {
        if (gameState.inGame)
        {
            gameState.paused = false;
            GDfile.UI_Canvas.SetActive(true);
            GDfile.IC.PauseCharacterInput("Player");
        }
        DisableAll();
    }

    List<string> chatList = new List<string>();
    public void EnterMessage()
    {
        if (TextBox.text == "") return;
        string idText = GDfile.ActiveClient.GetComponent<SendPlayerData>().PlayerId.ToString() + ": ";
        Net_ChatMessage msg = new Net_ChatMessage(idText + TextBox.text);
        GDfile.ActiveClient.GetComponent<Base_Client>().SendToServer(msg);
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
        GDfile.ipv = IpvToggle.isOn;
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
        if (GDfile.ActiveServer != null) { Debug.Log("A server is already running!"); return; }
        if (GDfile.ActiveClient != null) { Debug.Log("You must exit the server before hosting a new game!"); return; }
        if (EnterIP.text != "") GDfile.serverIpAddress = EnterIP.text;
        if (EnterPort.text != "") GDfile.serverPort = EnterPort.text;
        StartNewGame();
        DisableAll();
        GameObject ServerPrefab = GDfile.ServerPrefab;
        GameObject server = Instantiate(ServerPrefab);
        GameObject ClientPrefab = GDfile.ClientPrefab;
        GameObject client = Instantiate(ClientPrefab);
        GDfile.ActiveServer = server;
        GDfile.ActiveClient = client;
        DontDestroyOnLoad(server);
        DontDestroyOnLoad(client);
    }
    public void JoinGame()
    {
        if (GDfile.ActiveServer != null) { PopUpMessage("Can not join while hosting a server"); /*Debug.Log("Could not join as a you are currently hosting a server");*/ return; }
        string ip = EnterIP.text;
        string port = EnterPort.text;
        if (ip == "") { PopUpMessage("You must enter an IP address!"); /*Debug.Log("You must enter an IP address!");*/ return; }
        GDfile.serverIpAddress = ip;
        GDfile.serverPort = port;
        GameObject ClientPrefab = GDfile.ClientPrefab;
        GameObject client = Instantiate(ClientPrefab);
        GDfile.ActiveClient = client;
        DontDestroyOnLoad(client);
        StartCoroutine(CheckServerConnected(client));
        StartCoroutine(ConnectionTimer(client));
    }
    public void ExitOnlineMatch()
    {
        EnterIP.text = "";
        if (GDfile.ActiveServer) { Destroy(GDfile.ActiveServer); GDfile.ActiveServer = null; }
        if (GDfile.ActiveClient) { Destroy(GDfile.ActiveClient); GDfile.ActiveClient = null; }
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
    }
    private void setPreviousOptionsData()  //resets dropdowns to correct values from savefile
    {
        LoadOptions();
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
        GDfile.itemSystem.ResetItemSystem();
        GDfile.LoadLevel("Level_Debug");
        //GDfile.LoadLevel("Level_desert");
        GDfile.loadData = false;
        loadingScreenContainer.SetActive(false);
        Time.timeScale = 1;
        gameState.inGame = true;
        //GDfile.player.GetComponent<WeaponManager>().startWeaponManager();
        startGame();
    }
    public void LoadSaveMenu() //show load save file menu
    {
        //GDfile.itemSystem.ResetItemSystem();
        save1.SetActive(true); save1text.text = GDfile.saveFiles[0];
        if (GDfile.saveFiles.Count > 1) { save2.SetActive(true); save2text.text = GDfile.saveFiles[1]; }
        if (GDfile.saveFiles.Count > 2) { save3.SetActive(true); save3text.text = GDfile.saveFiles[2]; }
        loadSaveMenuContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(save1);
    } 
    public void LoadGame(int saveSlot) //load a given save file
    {
        DisableAll();
        loadingScreenContainer.SetActive(true);
        StartCoroutine(loadingScreen());
        GDfile.loadData = true;
        GDfile.LoadData(GDfile.saveFiles[saveSlot]);
        startGame();
    }
    public void SaveGame(int saveSlot) //save to a given save file
    {
        DisableAll();
        GDfile.saveData(GDfile.saveFiles[saveSlot]);
        BackButton();
    } 
    public void saveSlot1() //save slot 1
    {
        if (GDfile.saveFiles.Count > 0 && !gameState.inGame) { Debug.Log("load"); LoadGame(0); }
        else { Debug.Log("save"); SaveGame(0); }
    } 
    public void saveSlot2() //save slot 2
    {
        if (GDfile.saveFiles.Count > 1 && !gameState.inGame) LoadGame(1);
        else SaveGame(1);
    }
    public void saveSlot3() //save slot 3
    {
        if (GDfile.saveFiles.Count > 2 && !gameState.inGame) LoadGame(2);
        else SaveGame(2);
    }
    public void importCreateSavebutton() //swap between import save or create new save file
    {
        if (!gameState.inGame) importSave();
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
            GDfile.loadData = true;
            GDfile.LoadData(path);
            GDfile.player.GetComponent<WeaponManager>().startWeaponManager();
            startGame();
        }
    }
    public void CreateNewSave() //create a new save file
    {
        DisableAll();
        GDfile.createNewSave();
        GDfile.saveFiles.Clear();
        GDfile.findSaveFiles();
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
        GDfile.Pause("Temp");
        deathMessage.text = "Killed by " + KilledByEntity;
        GDfile.UI_Canvas.SetActive(false);
        deathScreenContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(respawnButton);
    }
    public void RespawnButton() //respawns the player
    {
        GDfile.UI_Canvas.SetActive(true);
        deathScreenContainer.SetActive(false);
        EM.RespawnEvent();
    }
    #endregion

    #region FPS Counter
    int lastFps = 0;
    public void UpdateFpsCounter() //updates the fps counter
    {
        if (Time.unscaledTime > timer)
        {
            int fps = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            if (fps != lastFps) FpsCounterText.SetText(fps.ToString());
            timer = Time.unscaledTime + refresh;
        }
    }
    #endregion

    #region Interface
    private void cancel()
    {
        if (InterfaceContainer.activeSelf) { CloseInterface(); }
        else CancelBackSwitch();
    }
    public void CloseInterface() //closes all interfaces
    {
        if (!InventoryContainer.activeSelf && !CraftContainer.activeSelf) return;
        resetInterface();
        DisableAll();
        LastSelected = null;
        GDfile.Pause("Resume");
    }

    int totalAccessorySlots = 0;
    int totalArmourSlots = 0;
    int totalItemSlots = 0;
    int totalCraftSlots = 0;

    //crafting functions
    public void OpenCrafting() //opens crafting interface
    {
        GDfile.Pause("Temp");
        DisableAll();
        PreviousSlot = null;
        InterfaceContainer.SetActive(true);
        CraftContainer.SetActive(true);
        interfaceDefaultText("craft");
        ShowSlots("craft");
        EventSystem.SetSelectedGameObject(InterfaceBackButton);
    } 
    public void resetInterface() //clears slots
    {
        foreach (Transform itemSlot in accessoriesContainer.transform) { Destroy(itemSlot.gameObject); }
        foreach (Transform itemSlot in itemsContainer.transform) Destroy(itemSlot.gameObject);
        foreach (Transform itemSlot in craftableItemsContainer.transform) { Destroy(itemSlot.gameObject); }
    }
    private void interfaceDefaultText(string menu) //updates the description and name to default settings
    {
        switch (menu)
        {
            case "craft":
                craftDescName.text = "Crafting Terminal";
                craftDescription.text = "This crafting terminal can be used to combine items into gear or other useful items";
                break;

            case "inventory":
                itemDescName.text = "Inventory";
                itemDescription.text = "This menu can be used to equip armour and accessories or view gathered items";
                break;
        }
    }

    //Inventory Functions
    public void OpenInventory() //opens inventory interface
    {
        GDfile.Pause("Temp");
        interfaceDefaultText("inventory");
        ShowSlots("item");
        ShowSlots("accessory");
        DisableAll();
        PreviousSlot = null;
        InterfaceContainer.SetActive(true);
        InventoryContainer.SetActive(true);
        EventSystem.SetSelectedGameObject(InterfaceBackButton);
    }
    string currentSlots = "accessory";
    public void showArmour() //swap accessories for armour menu
    { currentSlots = "armour"; ShowSlots("armour"); }
    public void showAccessories() //swap armour for accessories menu
    { currentSlots = "accessory"; ShowSlots("accessory"); }

    public void ShowSlots(string menu) //adds itemslots to the inventory UI
    {
        List<string> getList = new List<string>();

        switch (menu)
        {
            case "item":
                foreach (Transform itemSlot in itemsContainer.transform) Destroy(itemSlot.gameObject);
                totalItemSlots = 0;
                getList = GDfile.itemSystem.playerItems;
                break;

            case "craft":
                foreach (Transform itemSlot in craftableItemsContainer.transform) { Destroy(itemSlot.gameObject); }
                totalCraftSlots = 0;
                getList = GDfile.itemSystem.showCraftList();
                break;

            case "accessory":
                foreach (Transform itemSlot in accessoriesContainer.transform) { Destroy(itemSlot.gameObject); }
                totalAccessorySlots = 0;
                getList = GDfile.itemSystem.currentAccessories;
                break;

            case "armour":
                foreach (Transform itemSlot in accessoriesContainer.transform) { Destroy(itemSlot.gameObject); }
                totalArmourSlots = 0;
                getList = GDfile.itemSystem.currentArmour;
                break;
        }
        foreach (string item in getList)
        { addSlot(item, menu); }
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
        bool equiped = false;
        switch (type)
        {
            case "accessory":
                slot = totalAccessorySlots;
                itemslot.name = "accessorySlot" + slot;
                totalAccessorySlots++;
                break;

            case "armour":
                slot = totalArmourSlots;
                itemslot.name = "armourSlot" + slot;
                totalArmourSlots++;
                break;

            case "item":
                slot = totalItemSlots;
                itemslot.name = "itemSlot" + slot;
                totalItemSlots++;
                equiped = GDfile.itemSystem.currentAccessories.Contains(item) || GDfile.itemSystem.currentArmour.Contains(item);
                break;

            case "craft":
                slot = totalCraftSlots;
                itemslot.name = "craftSlot" + slot;
                totalCraftSlots++;
                break;
        }        
        quantity = GDfile.itemSystem.getItemQuantity(item);

        //update itemSlot variables
        string ItemType = GDfile.csvManager.getItemType(item);
        itemslot.GetComponent<ItemSlot>().updateSlotInfo(item, ItemType, quantity, slot, type, equiped, this);
    }

    public ItemSlot PreviousSlot;
    public void itemSelected(ItemSlot itemSlot) //logic for when an item is selected
    {
        if (itemSlot.Type == "craft") { craftItemSelected(itemSlot); return; }
        string Type = itemSlot.Type;

        //switch condition for items - assign (move item to selected slot), remove (remove item and put in item list), select (highlight the current item)
        string function;

        //set switch statement condition based on last and current slot selection
        if (Type == "item")
        {
            function = "select"; //always select if selecting an item
        }
        else
        {
            if (!PreviousSlot) 
            {
                //if (itemSlot.Item != "") function = "select";
                //else return;

                function = "select"; //select accessory/armour slot if no previous selection
            }
            else
            {
                string prevType = PreviousSlot.Type;
                if (prevType == "item")
                {
                    if (GDfile.itemSystem.currentAccessories.Contains(PreviousSlot.Item)) function = "select"; //if item already equiped select the newest slot
                    else if (GDfile.itemSystem.currentArmour.Contains(PreviousSlot.Item)) function = "select"; //if item already equiped select the newest slot
                    else if (Type == GDfile.csvManager.getItemType(PreviousSlot.Item)) { function = "assign"; } //assign last selection to accessory/armour slot
                    //else if (itemSlot.Item != "") function = "select"; //
                    else function = "select"; //return;
                }
                else if (itemSlot == PreviousSlot) function = "remove";
                else function = "select";
            }
        }
        //string selectedItem = PreviousSlot.Item;
        
        switch (function)
        {
            case "select":
                if(itemSlot.Item != "") UpdateDescription(itemSlot.Item);
                updateSlotColour(itemSlot);
                PreviousSlot = itemSlot;
                return;

            case "assign":
                //check item can be assigned
                if (!GDfile.itemSystem.checkItemCompatibility(PreviousSlot.Item, itemSlot.Type, itemSlot.Slot)) return;

                //convert items
                if (itemSlot.Item != "") GDfile.itemSystem.AccessoriesToItem(itemSlot.Item, itemSlot.Slot, itemSlot.Type);
                GDfile.itemSystem.ItemToAccessories(PreviousSlot.Item, itemSlot.Slot, itemSlot.Type);

                //update slots
                string item = PreviousSlot.Item;
                ShowSlots("item");
                ShowSlots(currentSlots);

                //find slots after update and change colours
                foreach(Transform Item in accessoriesContainer.transform)
                {
                    if(Item.GetComponent<ItemSlot>().Item == item)
                    {
                        Item.GetComponent<ItemSlot>().Background.GetComponent<Image>().color = itemSlotSelectedColour;
                        Item.GetComponent<ItemSlot>().setColour(GDfile.csvManager.getItemType(PreviousSlot.Item));
                        PreviousSlot = Item.GetComponent<ItemSlot>();
                        EventSystem.SetSelectedGameObject(Item.gameObject); 
                        break;
                    }
                    else EventSystem.SetSelectedGameObject(InterfaceBackButton);
                }                
                break;

            case "remove":
                //convert item
                if (itemSlot.Item == "") return;
                GDfile.itemSystem.AccessoriesToItem(itemSlot.Item, itemSlot.Slot, itemSlot.Type);
                itemSlot.Item = "";
                itemSlot.Slot = 0;

                //update slots
                ShowSlots("item");
                ShowSlots(currentSlots);
                updateSlotColour(itemSlot);
                //UpdateDescription(itemSlot.Item);
                PreviousSlot = null;
                return;
        }

        //foreach (Transform accessory in accessoriesContainer.transform)
        //{
        //    //Debug.Log(accessory.name);
        //    if (accessory.name == itemSlot.name)
        //    { EventSystem.SetSelectedGameObject(accessory.gameObject); return; }
        //}
    }
    private void updateSlotColour(ItemSlot itemSlot)
    {
        //change itemslot colours

        //if (PreviousSlot) PreviousSlot.Background.GetComponent<Image>().color = itemSlotDefaultColour;
        //itemSlot.Background.GetComponent<Image>().color = itemSlotSelectedColour;
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
        GDfile.itemSystem.craftItem(PreviousSlot.Item);
        ShowSlots("craft");
    }

    private void UpdateDescription(string item)
    {
        if (CraftContainer.activeSelf)
        {
            craftDescName.text = item;
            craftDescription.text = GDfile.csvManager.getItemDescription(item);
        }
        else if (InventoryContainer.activeSelf)
        {
            itemDescName.text = item;
            itemDescription.text = GDfile.csvManager.getItemDescription(item);
        }
    }
    public static string playTimeCounter(float lastPlayTime, float startTime) //updates the playtime
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

        return counter;
    }
    #endregion
}
