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

public class MainMenuScript : MonoBehaviour
{
    /////-----Variables-----\\\\\
    GameDataFile gameDataFile;
    GameObject EventSystem;
    [SerializeField] GameObject menuContainer;
    [SerializeField] public bool inGame;

    #region MainMenu
    [Header("StartMenu")]
    [SerializeField] GameObject StartMenuContainer;
    [SerializeField] GameObject StartGameText;
    [SerializeField] GameObject ResumeGameText;
    [SerializeField] GameObject ExitGameText;
    [SerializeField] GameObject OptionsText;
    [SerializeField] GameObject StartGameMenuContainer;
    #endregion

    #region PauseMenu
    [Header("StartMenu")]
    [SerializeField] GameObject PauseMenuContainer;
    #endregion

    #region Loading Screen
    [Header("Loading Screen")]
    [SerializeField] GameObject loadingScreenContainer;
    [SerializeField] TextMeshProUGUI loadingText;
    #endregion

    #region SaveMenu
    [SerializeField] public GameObject loadSaveMenuContainer;
    [SerializeField] public TextMeshProUGUI save1text;
    [SerializeField] public TextMeshProUGUI save2text;
    [SerializeField] public TextMeshProUGUI save3text;
    [SerializeField] public GameObject save1;
    [SerializeField] public GameObject save2;
    [SerializeField] public GameObject save3;
    #endregion

    #region ErrorMessage
    [SerializeField] public GameObject errorContainer;
    [SerializeField] public GameObject okButton;
    [SerializeField] public TextMeshProUGUI ErrorText;
    #endregion

    #region OptionsMenu
    [Header("OptionsMenu")]
    [SerializeField] GameObject OptionsMenuContainer;
    [SerializeField] GameObject OptionsCommonAssets;
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
    #endregion

    #region FPS
    [Header("FpsCounter")]
    [SerializeField] public GameObject FpsCounter;
    bool FpsCountEnabled = false;
    int FpsCap = 60;
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
    #endregion

    #region multiplayerMenu
    [Header("MultiplayerMenu")]
    [SerializeField] public GameObject MultiplayerContainer;
    [SerializeField] public TMP_InputField EnterIP;
    [SerializeField] public TextMeshProUGUI userIP;
    #endregion

    /////-----Functions-----\\\\\
    private void Awake()
    {
        gameDataFile = FindObjectOfType<GameDataFile>();
        EventSystem = GameObject.Find("EventSystem");
        gameDataFile.MenuContainer = transform.gameObject;
        EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(StartGameText);
        gameDataFile.player.GetComponent<PlayerInput>().uiInputModule = EventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        //gameDataFile.MasterAudio.clip = gameDataFile.MainTheme;
        //gameDataFile.MasterAudio.volume = 0.5f;
        //gameDataFile.MasterAudio.Play();

        gameDataFile.FpsCounterObject = FpsCounter;
        gameDataFile.FpsCounterText = FpsCounter.GetComponent<TextMeshProUGUI>();
        //gameDataFile.MenuContainer = menuContainer;
        gameDataFile.EventSystem = EventSystem;

        //set default text in options
        LoadOptions();
        DisplayOptionsDropDown.GetComponent<TMP_Dropdown>().captionText.SetText("Display Options");
        SetResolutionDropDown.GetComponent<TMP_Dropdown>().captionText.SetText("Resolution");
        FpsEnabledDropdown.GetComponent<TMP_Dropdown>().captionText.SetText("Fps Count");


        DontDestroyOnLoad(gameObject);
    }
    private int i = 0;
    private void Update()
    {
        if (inOptionsMenu)
        {
            CurrentResolutionText.GetComponent<TextMeshProUGUI>().text = Screen.currentResolution.ToString();
            if (SetResolutionDropDown.GetComponent<TMP_Dropdown>().IsExpanded || DisplayOptionsDropDown.GetComponent<TMP_Dropdown>().IsExpanded || FpsEnabledDropdown.GetComponent<TMP_Dropdown>().IsExpanded) inDropDown = true;
            else inDropDown = false;
        }
    }
    private void FixedUpdate()
    {
        i++;
        if (loadingScreenContainer.activeSelf && i >= 5)
        {
            i = 0;
            if (loadingText.text == "Loading....") loadingText.text = "Loading";
            else loadingText.text += ".";
        }
    }

    #region Start Menu Functions
    public void startGame() //Loads first level
    {
        StartMenuContainer.SetActive(false);
        StartGameMenuContainer.SetActive(false);
        gameDataFile.player.GetComponent<CharacterMovementV2>().PauseCharacterInput();
        gameDataFile.player.GetComponent<WeaponAndItemScript>().PauseWeaponAndItemInput();
    }
    IEnumerator loadingScreen()
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
        //StartGameText.SetActive(false);
        //ResumeGameText.SetActive(true);
        PauseMenuContainer.SetActive(true);
        EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(ResumeGameText);
    }
    public void ResumeGame() //Resumes game when paused
    {
        if (ApplySettingsContainer.activeSelf) return;
        if (VideoContainer.activeSelf) ApplySettingsCheck();
        if (!inGame) return; //Add apply settings option here instead

        //Deactivate Options menu text
        gameDataFile.PauseGame();
        StartMenuContainer.SetActive(false);
        PauseMenuContainer.SetActive(false);
        OptionsMenuContainer.SetActive(false);
        inOptionsMenu = false;
    }
    public void ExitGame() //Function to load mainmenu
    {
        //Control scheme doesnt need changing as long as player exits from pause menu
        //gameDataFile.player.GetComponent<CharacterMovementV2>().PauseCharacterInput();
        //gameDataFile.player.GetComponent<WeaponAndItemScript>().PauseWeaponAndItemInput();

        //if(gameDataFile.overwriteData && gameDataFile.loadData) gameDataFile.saveData(); //only saves if data was loaded and overwrite is active
        ExitOnlineMatch();
        gameDataFile.LoadLevel("MainMenu");
        //ResumeGameText.SetActive(false);
        //StartGameText.SetActive(true);
        PauseMenuContainer.SetActive(false);
        StartMenuContainer.SetActive(true);
        EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(StartGameText);
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
        //Deactivate mainMenu text
        StartMenuContainer.SetActive(false);

        //Activate Options menu text
        CurrentResolutionText.GetComponent<TextMeshProUGUI>().text = Screen.currentResolution.ToString();
        OptionsMenuContainer.SetActive(true);

        //set default text in options
        DisplayOptionsDropDown.GetComponent<TMP_Dropdown>().captionText.SetText("Display Options");
        SetResolutionDropDown.GetComponent<TMP_Dropdown>().captionText.SetText("Resolution");
        FpsEnabledDropdown.GetComponent<TMP_Dropdown>().captionText.SetText("Fps Count");

        inOptionsMenu = true;
        VideoOptions();
    }
    public void VideoOptions() //Shows the video Options menu
    {
        //Set current active menu
        VideoContainer.SetActive(true);
        SoundContainer.SetActive(false);
        ControlsContainer.SetActive(false);
        ApplySettingsContainer.SetActive(false);

        //Change tab colour
        VideoMenuButton.GetComponent<Image>().color = TabSelectedColour;
        SoundMenuButton.GetComponent<Image>().color = TabNormalColour;
        ControlsMenuButton.GetComponent<Image>().color = TabNormalColour;

        //Default selection for controller
        EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(DisplayOptionsDropDown);

        //set default text in options
        DisplayOptionsDropDown.GetComponent<TMP_Dropdown>().captionText.SetText("Display Options");
        SetResolutionDropDown.GetComponent<TMP_Dropdown>().captionText.SetText("Resolution");
        FpsEnabledDropdown.GetComponent<TMP_Dropdown>().captionText.SetText("Fps Count");

        if (!VideoUiEnabled) EnableDisableVideoUI();
    }
    public void SoundOptions() //Shows audio options menu
    {
        //Set current active menu
        VideoContainer.SetActive(false);
        SoundContainer.SetActive(true);
        ControlsContainer.SetActive(false);

        //Change tab colour
        VideoMenuButton.GetComponent<Image>().color = TabNormalColour;
        SoundMenuButton.GetComponent<Image>().color = TabSelectedColour;
        ControlsMenuButton.GetComponent<Image>().color = TabNormalColour;

        //Default selection for controller
        EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(MusicVolumeSlider);
    }
    public void ControlOptions() //Shows control options menu
    {
        //Set current active menu
        VideoContainer.SetActive(false);
        SoundContainer.SetActive(false);
        ControlsContainer.SetActive(true);

        //Change tab colour
        VideoMenuButton.GetComponent<Image>().color = TabNormalColour;
        SoundMenuButton.GetComponent<Image>().color = TabNormalColour;
        ControlsMenuButton.GetComponent<Image>().color = TabSelectedColour;

        //Default selection for controller
        EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(ControlsMenuButton);
    }
    public void BackButton() //Returns to start Menu
    {
        //Deactivate Options menu
        MessageContainer.SetActive(false);
        OptionsMenuContainer.SetActive(false);
        StartGameMenuContainer.SetActive(false);
        loadSaveMenuContainer.SetActive(false);
        if (inOptionsMenu || !inGame)
        {
            inOptionsMenu = false;

            //Re-activate MainMenu/ResumeMenu text
            if(gameDataFile.isPaused) PauseMenuContainer.SetActive(true);
            else StartMenuContainer.SetActive(true);

            //set first selection in start menu
            if (!inGame) EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(StartGameText);
            else EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(ResumeGameText);
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
        if (SetFS == 1) { DisplayMode = "FullScreen"; ScreenMode = FullScreenMode.ExclusiveFullScreen; }
        else if (SetFS == 2) { DisplayMode = "Windowed"; ScreenMode = FullScreenMode.Windowed; }
        else if (SetFS == 3) { DisplayMode = "Windowed FullScreen"; ScreenMode = FullScreenMode.MaximizedWindow; }
        SetFS = 0;
    }
    public void ShowFpsCounter()
    {
        ApplyText.SetActive(true);
        int showFps = FpsEnabledDropdown.GetComponent<TMP_Dropdown>().value;
        if (showFps == 0)
        {
            FpsCountEnabled = false;
            FpsEnabledText.GetComponent<TextMeshProUGUI>().text = "Disabled";
        }
        else if (showFps == 1)
        {
            FpsCountEnabled = true;
            FpsEnabledText.GetComponent<TextMeshProUGUI>().text = "Enabled";
        }
    }
    public void SetFpsCap()
    {
        ApplyText.SetActive(true);
        FpsCap = (int)FpsCapSlider.GetComponent<Slider>().value;
        FpsCapText.GetComponent<TextMeshProUGUI>().text = FpsCap.ToString() + "hz";
    }
    public void ApplySettingsCheck()
    {
        EnableDisableVideoUI();
        ApplySettingsContainer.SetActive(true);
        EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(CancelSettings);

    }
    public void CancelBackSwitch()
    {
        if (ApplySettingsContainer.activeSelf) cancelSettings();
        else if (!inDropDown) BackButton();
    }
    public void cancelSettings()
    {
        ApplySettingsContainer.SetActive(false);
        VideoOptions();
    }
    public void applySettings()
    {
        ApplyText.SetActive(false);
        Screen.SetResolution(screenResolutionX, screenResolutionY, ScreenMode);
        Application.targetFrameRate = FpsCap;

        setSensitivity();

        gameDataFile.FpsCountEnabled = FpsCountEnabled;
        FpsCounter.SetActive(FpsCountEnabled);
        FullScreenText.GetComponent<TextMeshProUGUI>().text = DisplayMode;
        DisplayOptionsDropDown.GetComponent<TMP_Dropdown>().captionText.SetText("Display Options");
        SetResolutionDropDown.GetComponent<TMP_Dropdown>().captionText.SetText("Resolution");
        FpsEnabledDropdown.GetComponent<TMP_Dropdown>().captionText.SetText("Fps Count");
        ApplySettingsContainer.SetActive(false);

        EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(DisplayOptionsDropDown);
        SaveOptions();
        VideoOptions();
    }
    #endregion

    #region Sound Options Funcions

    public void SetVolume()
    {
        gameDataFile.MasterAudio.volume = MusicVolumeSlider.GetComponent<Slider>().value;
        //gameDataFile.GameAudio.volume = GameVolumeSlider.GetComponent<Slider>().value;
        gameDataFile.PlayerAudio.volume = sfxVolumeSlider.GetComponent<Slider>().value;
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
        if (!inGame) return;
        if (MessageContainer.activeSelf == false)
        {
            if (inGame)
            {
                gameDataFile.isPaused = true;
                gameDataFile.UI_Canvas.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                gameDataFile.player.GetComponent<CharacterMovementV2>().PauseCharacterInput();
                gameDataFile.player.GetComponent<WeaponAndItemScript>().PauseWeaponAndItemInput();
            }
            MessageContainer.SetActive(true);
            EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(TextBox.gameObject);
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
            gameDataFile.player.GetComponent<CharacterMovementV2>().PauseCharacterInput();
            gameDataFile.player.GetComponent<WeaponAndItemScript>().PauseWeaponAndItemInput();
        }
        MessageContainer.SetActive(false);
        //EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(StartGameText);
    }
    public void EnterMessage()
    {
        if (TextBox.text == "") return;
        Net_ChatMessage msg = new Net_ChatMessage(TextBox.text);
        gameDataFile.ActiveClient.GetComponent<Base_Client>().SendToServer(msg);
        TextBox.text = "";
        EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(TextBox.gameObject);
        TextBox.Select();
    }
    #endregion

    #region MultiplayerMenu
    public void MultiplayerMenu()
    {
        userIP.text = " User IP: " + GameDataFile.GetLocalIPAddress();

        #if UNITY_EDITOR
        userIP.text = " User IP: " + " 192.168.1.220";
        #endif
        StartMenuContainer.SetActive(false);
        MultiplayerContainer.SetActive(true);
    }
    public void ExitMultiplayerMenu()
    {
        MultiplayerContainer.SetActive(false);
        StartMenuContainer.SetActive(true); 
    }
    public void HostGame()
    {
        if (gameDataFile.ActiveServer != null) { Debug.Log("A server is already running!"); return; }
        if(gameDataFile.ActiveClient != null) { Debug.Log("You must exit the server before hosting a new game!"); return; }
        if (EnterIP.text != "") gameDataFile.serverIpAddress = EnterIP.text;
        StartNewGame();
        MultiplayerContainer.SetActive(false);
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
        if (gameDataFile.ActiveServer != null) { Debug.Log("Could not join as a you are currently hosting a server"); return; }
        string ip = EnterIP.text;
        if (ip == "") { Debug.Log("You must enter an IP address!"); return; }
        gameDataFile.serverIpAddress = ip;
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
        yield return new WaitUntil(() => !client || client.GetComponent<Base_Client>().connectedToServer); ;
        if (client) 
        { 
            StopCoroutine(ConnectionTimer(client));
            StartNewGame();
            MultiplayerContainer.SetActive(false);
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
        Debug.Log("Failed to connect to server!");
        ExitOnlineMatch();
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
    public void SaveOptions()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        SaveOptions data = new SaveOptions(MusicVolumeSlider.GetComponent<Slider>().value, GameVolumeSlider.GetComponent<Slider>().value, sfxVolumeSlider.GetComponent<Slider>().value,
            screenResolutionX, screenResolutionY, DisplayMode, FpsCountEnabled, FpsCap, xSensitivitySlider.GetComponent<Slider>().value, ySensitivitySlider.GetComponent<Slider>().value);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
        //gameDataFile.player.GetComponent<WeaponManager>().saveWeaponData();
    }
    public void LoadOptions()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        SaveOptions data = (SaveOptions)bf.Deserialize(file);
        file.Close();

        MusicVolumeSlider.GetComponent<Slider>().value = data.musicVolume;
        GameVolumeSlider.GetComponent<Slider>().value = data.gameVolume;
        sfxVolumeSlider.GetComponent<Slider>().value = data.sfxVolume;
        xSensitivitySlider.GetComponent<Slider>().value = data.xSensitivty;
        ySensitivitySlider.GetComponent<Slider>().value = data.ySensitivty;
        xSensitivityValue.GetComponent<TextMeshProUGUI>().text = data.xSensitivty.ToString();
        ySensitivityValue.GetComponent<TextMeshProUGUI>().text = data.ySensitivty.ToString();

        FpsCapSlider.GetComponent<Slider>().value = data.targetFps;
        FpsCountEnabled = data.fpsEnabled;
        screenResolutionX = data.xResolution;
        screenResolutionY = data.yResolution;
        DisplayMode = data.displayMode;
        applySettings();
    }
    public void StartGameMenu()
    {
        StartMenuContainer.SetActive(false);
        StartGameMenuContainer.SetActive(true);
    }
    public void StartNewGame()
    {
        gameDataFile.LoadLevel("Level_Debug");
        gameDataFile.loadData = false;
        loadingScreenContainer.SetActive(false);
        Time.timeScale = 1;
        inGame = true;
        gameDataFile.player.GetComponent<WeaponManager>().startWeaponManager();
        startGame();
    }
    public void LoadSaveMenu()
    {
        save1.SetActive(true); save1text.text = gameDataFile.saveFiles[0];
        if (gameDataFile.saveFiles.Count > 1) { save2.SetActive(true); save2text.text = gameDataFile.saveFiles[1]; }
        if (gameDataFile.saveFiles.Count > 2) { save3.SetActive(true); save3text.text = gameDataFile.saveFiles[2]; }
        loadSaveMenuContainer.SetActive(true);
    }
    public void LoadGame(int saveSlot)
    {
        loadSaveMenuContainer.SetActive(false);
        loadingScreenContainer.SetActive(true);
        StartCoroutine(loadingScreen());
        gameDataFile.loadData = true;
        gameDataFile.LoadData(gameDataFile.saveFiles[saveSlot]);
        gameDataFile.player.GetComponent<WeaponManager>().startWeaponManager();
        startGame();
    }
    public void SaveGame(int saveSlot)
    {
        loadSaveMenuContainer.SetActive(false);
        gameDataFile.saveData(gameDataFile.saveFiles[saveSlot]);
        BackButton();
    }
    public void saveSlot1()
    {
        if (gameDataFile.saveFiles.Count > 0 && !inGame) { Debug.Log("load"); LoadGame(0); }
        else { Debug.Log("save"); SaveGame(0); }
    }
    public void saveSlot2()
    { 
        if (gameDataFile.saveFiles.Count > 1 && !inGame) LoadGame(1);
        else SaveGame(1);
    }
    public void saveSlot3()
    { 
        if (gameDataFile.saveFiles.Count > 2 && !inGame) LoadGame(2);
        else SaveGame(2);
    }
    public void importCreateSavebutton()
    {
        if (!inGame) importSave();
        else CreateNewSave();
    }
    public void importSave()
    {
        string path = "";// = EditorUtility.OpenFilePanel("import save file", "", "csv");
        if (path.Length != 0)
        {
            if (!path.Contains("Save_Data.csv")) { fileCorrupt(); return; }
            loadSaveMenuContainer.SetActive(false);
            StartCoroutine(loadingScreen());
            gameDataFile.loadData = true;
            gameDataFile.LoadData(path);
            gameDataFile.player.GetComponent<WeaponManager>().startWeaponManager();
            startGame();
        }
    }
    public void CreateNewSave()
    {
        loadSaveMenuContainer.SetActive(false);
        gameDataFile.createNewSave();
        gameDataFile.saveFiles.Clear();
        gameDataFile.findSaveFiles();
        BackButton();
    }
    public void fileCorrupt()
    { errorContainer.SetActive(true); }
    public void acceptError()
    { errorContainer.SetActive(false); }

    #endregion
}
