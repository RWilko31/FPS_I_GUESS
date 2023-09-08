using UnityEngine;
/// <summary>
/// Holds ammo data for the IWeapon Interface it is attached to
/// </summary>
[System.Serializable]
public class AmmoData
{
    [Header("Current Ammo")]
    public int ReserveAmmo;
    public int MagAmmo;
    [Header("Max Ammo")]
    public int ReserveMax;
    public int MagMax;
    [Header("Timers")]
    public float ReloadTime;
    public float RateOfFire;
}
