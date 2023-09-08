using UnityEngine;

public class Weapon_Ranged //: Weapon
{
    [SerializeField] private WeaponName weapon;
    [Header("Attack")]
    [SerializeField] private AttackFunction AttackScript;

    [Header("Ammo")]
    [SerializeField] private AmmoData ammoData;

    [Header("Audio")]
    [SerializeField] private AudioClip FireAudio;
    [SerializeField] private AudioClip ReloadAudio;

    [Header("Positions")]
    [SerializeField] public Vector3 hipPos;
    [SerializeField] public Vector3 aimPos;
    [Header("Assignables")]
    [SerializeField] public GameObject Model;
    [SerializeField] public Sprite CrossHair;
        
    //public override weaponName WeaponName() { return weapon; }
    //public override GameObject WeaponModel() { return Model; }
    //public override Vector3 AimPosition() { return aimPos; }
    //public override Vector3 HipPosition() { return hipPos; }
    //public override void Reload() { }
    //public override void Attack() { AttackScript.Attack(); }
    //public override void AttackCancelled() { AttackScript.AttackCancelled(); }       
}
