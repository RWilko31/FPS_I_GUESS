using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    //Stores the relevent models and audio for the weapon
    [SerializeField] public GameObject weaponModel;
    [SerializeField] public GameObject crosshair;
    [SerializeField] public AudioClip fireAudio;
    [SerializeField] public AudioClip reloadAudio;
}
