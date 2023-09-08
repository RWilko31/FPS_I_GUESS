using UnityEngine;
/// <summary>
/// Defines an attack function which can be attached to a IWeapon interface for use with the WeaponSystem
/// </summary>
public abstract class AttackFunction : MonoBehaviour
{
    /// <summary>
    /// Defines the max and curent ammo data for a weapon. If not present the weapon system will assume the weapon does not use ammo
    /// </summary>
    [HideInInspector] public AmmoData ammoData = null;
    [HideInInspector] public Camera playerCamera;
    public abstract bool UsesAmmo();
    
    public abstract void Attack();
    public abstract void AttackCancelled();
    public abstract void Reload();
}
