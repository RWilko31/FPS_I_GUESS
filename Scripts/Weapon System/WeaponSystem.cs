using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponSystem : MonoBehaviour
{
    [Header("Current weapon")]
    [SerializeField] string currentWeapon = "";
    public Weapon weaponScript;
    [Header("Current Equipment")]
    [SerializeField] string currentLethal = "";
    public IEquipment lethalScript;
    [SerializeField] string currentTactical = "";
    public IEquipment tacticalScript;

    GameDataFile GDFile;
    InputController IM;

    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        IM = GDFile.GetComponent<InputController>();
        SubEvents();
    }

    void SubEvents()
    {
        IM.fire += Attack;
        IM.fireCancelled += AttackCancelled;
        IM.aim += Aim;
        IM.aimCancelled += AimCancelled;
        IM.lethal += UseLethal;
        IM.tactical += UseTactical;
        IM.reload += Reload;
    }

    private void Attack()
    {
        if (weaponScript == null) return;
        weaponScript.Attack();
    }
    private void AttackCancelled()
    {
        if (weaponScript == null) return;
        weaponScript.AttackCancelled();
    }
    private void Reload()
    {
        if (weaponScript == null) return;
        weaponScript.Reload();
    }
    private void Aim()
    {
        if (weaponScript == null) return;
        weaponScript.transform.localPosition = weaponScript.aimPos;
    }
    private void AimCancelled()
    {
        if (weaponScript == null) return;
        weaponScript.transform.localPosition = weaponScript.hipPos;
    }
    private void UseLethal()
    {
        if (lethalScript == null) return;
        lethalScript.Use();
    }
    private void UseTactical()
    {
        if (tacticalScript == null) return;
        tacticalScript.Use();
    }

    /// <summary>
    /// Sets the current weapon
    /// </summary>
    /// <param name="weapon"></param>
    public void SetWeapon(Weapon weapon)
    {
        weaponScript = weapon;
        currentWeapon = weapon.weapon.ToString();
    }
    /// <summary>
    /// Sets the current lethal
    /// </summary>
    /// <param name="lethal"></param>
    public void SetLethal(IEquipment lethal)
    {
        lethalScript = lethal;
        currentLethal = lethal.EquipmentName();
    }
    /// <summary>
    /// Sets the current tactical
    /// </summary>
    /// <param name="tactical"></param>
    public void SetTactical(IEquipment tactical)
    {
        tacticalScript = tactical;
        currentTactical = tactical.EquipmentName();
    }
}
