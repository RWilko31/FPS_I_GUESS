using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectTrigger : MonoBehaviour
{
    InteractableObjects InteractableObjects;
    public bool onTrigger = false;
    [SerializeField] private GameObject Player;

    private void Awake()
    {
        InteractableObjects = Player.GetComponent<InteractableObjects>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != InteractableObjects.heldObject) onTrigger = true;
    }
    private void OnTriggerExit(Collider other)
    { onTrigger = false;
    }
}
