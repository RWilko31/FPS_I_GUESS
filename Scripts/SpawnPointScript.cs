using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    /// Player checkpoint/spawnpoint script.
    /// Add this script to spawnpoints/checkpoints to update the spawn point when near it

    GameDataFile GDFile;

    private void Awake() 
    {
        GDFile = FindObjectOfType<GameDataFile>();
        if (this.name.Contains("PlayerSpawnPoint")) SetSpawn();
    }
    void SetSpawn() //set spawn point at start of level
    {
        GDFile.currentCheckPoint = gameObject;
        GDFile.MoveToPoint(GDFile.currentCheckPoint);
    }
    private void OnTriggerEnter(Collider other) //Update spawn during a level
    {
        if (GDFile.currentCheckPoint != gameObject)
        {
            GDFile.currentCheckPoint = gameObject;
            Debug.Log("Checkpoint set! : " + this.name);
        }
    }
}
