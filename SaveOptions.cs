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
    public bool fpsEnabled;
    public int targetFps;
    #endregion

    public SaveOptions(float saveMusicvolume, float saveGameVolume, float saveSfxVolume,  int savexResolution, int saveyResolution, string saveDisplayMode, bool saveFpsEnabled, int saveTargetFps, float saveXsensitivity, float saveYsensitivity)
    {
        musicVolume = saveMusicvolume;
        gameVolume = saveGameVolume;
        sfxVolume = saveSfxVolume;

        xResolution = savexResolution;
        yResolution = saveyResolution;
        displayMode = saveDisplayMode;
        fpsEnabled = saveFpsEnabled;
        targetFps = saveTargetFps;
        xSensitivty = saveXsensitivity;
        ySensitivty = saveYsensitivity;
    }


}
