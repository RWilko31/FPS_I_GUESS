[System.Serializable]
public class SaveOptions
{
    #region Audio
    public float musicVolume;
    public float gameVolume;
    public float sfxVolume;
    #endregion

    #region Input
    public float xSensitivty;
    public float ySensitivty;
    #endregion

    #region VideoSettings
    public int xResolution;
    public int yResolution;
    public string displayMode;
    public string vSyncMode;
    public bool fpsEnabled;
    public int targetFps;
    #endregion

    #region Dropdown
    public int ScreenMode;
    public int Resolution;
    public int vSync;
    public int Fps;
    #endregion

    public SaveOptions(float saveMusicvolume, float saveGameVolume, float saveSfxVolume,  int savexResolution, int saveyResolution, string saveDisplayMode, string saveVSyncMode, bool saveFpsEnabled, int saveTargetFps, float saveXsensitivity, float saveYsensitivity, int screenMode, int res, int vsync, int fps)
    {
        musicVolume = saveMusicvolume;
        gameVolume = saveGameVolume;
        sfxVolume = saveSfxVolume;

        xResolution = savexResolution;
        yResolution = saveyResolution;
        displayMode = saveDisplayMode;
        vSyncMode = saveVSyncMode;
        fpsEnabled = saveFpsEnabled;
        targetFps = saveTargetFps;
        xSensitivty = saveXsensitivity;
        ySensitivty = saveYsensitivity;

        ScreenMode = screenMode;
        Resolution = res;
        vSync = vsync;
        Fps = fps;
    }
}