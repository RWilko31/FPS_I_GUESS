using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSettings : MonoBehaviour
{
    //Set parameters for the door to be read by the InteractableData script
    [SerializeField] public bool canClose;
    [SerializeField] public bool playerClose;
    [SerializeField] public bool triggerWhenNear;
    [SerializeField] public float openDistance;
    [SerializeField] public float doorSpeed;
    [SerializeField] public float holdTime;
    [SerializeField] public string openDirection;
}
