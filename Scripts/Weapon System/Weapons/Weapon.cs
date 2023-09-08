using UnityEngine;
/// <summary>
/// Defines a weapon for use with the WeaponSystem
/// </summary>
[System.SerializableAttribute]
public class Weapon : MonoBehaviour
{
    [SerializeField] public WeaponName weapon;
    [Header("Function")]
    [SerializeField] public AttackFunction AttackScript;
    [Header("Positions")]
    [SerializeField] public Vector3 hipPos;
    [SerializeField] public Vector3 aimPos;
    [Header("Assignables")]
    [SerializeField] public Sprite crossHair;
    [Header("Audio")]
    [SerializeField] public AudioClip AttackAudio;
    [SerializeField] public AudioClip AttackCancelledAudio;
    [SerializeField] public AudioClip ReloadAudio;
    [SerializeField] public AudioClip AimAudio;
    [SerializeField] public AudioClip AimCancelledAudio;
    private void Awake()
    {
        if(!AttackScript) AttackScript = GetComponent<AttackFunction>();
    }
    /// <summary>
    /// The function to perform when using this weapon
    /// </summary>
    public void Attack() { if(AttackScript != null) AttackScript.Attack(); }
    /// <summary>
    /// Cancels the attack function if it latches on
    /// </summary>
    public void AttackCancelled() { if (AttackScript != null) AttackScript.AttackCancelled(); }
    /// <summary>
    /// The function to perform when reloading
    /// </summary>
    public void Reload() { if (AttackScript != null) AttackScript.Reload(); }
}
