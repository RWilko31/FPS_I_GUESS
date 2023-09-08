using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPointV2 : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private int activeArea = 0;               //the required area number to activate the spawn point
    [SerializeField] private bool inactive = true;             //indicates if the spawn point is active depending on how close the player is
    [Header("Spawn settings")]
    [SerializeField] private List<EnemyCode> defaultEnemies;    //default enemies to spawn at start
    [SerializeField] private float spawnTime;                   //Time between each spawn
    [SerializeField] private bool spawnAll;                     //spawn all enemies at once
    [SerializeField] private bool respawn;                      //respawn an enemy as soon as it is defeated
    [SerializeField] private bool respawnOnDefeat;              //respawn after all enemies are defeated
    [SerializeField] private float spawnTimeOnDefeat;           //Time after defeat before respan starts

    [Header("radius")] //(could be used to keep bosses spawned but idle until player is near)
    [SerializeField] private bool useRadius = false;                             //enemies wont spawn until player is in radius 
    [SerializeField] [Range(1, 1000)] private float spawnRadius = 10f;           //area around the spawn the point will be active
    [SerializeField] private bool showRadius = true;
    private Transform radiusObj;

    private bool allDefeated = false, respawnAll = false;       //alldefeated: true when all enemies from this spawn have been beaten. respawnAll: false when all enemies have been respawned once
    private float time = 0, timeDefeat = 0, inactiveTime = 0;   //timers
    private bool allowSpawn = true;                             //true if an enemy should be spawned
    private int respawnCount = 0;                               // number of enemies to respawn

    [HideInInspector] public List<GameObject> EnemyList = new List<GameObject>();   //The current enemies from this point
    private List<EnemyCode> spawnList = new List<EnemyCode>();                      //list of enemies to spawn
    private List<GameObject> respawnList = new List<GameObject>();                  //List of defeated enemies from this spawn requiring respawn

    EnemyMasterSpawn masterSpawn; //master script to spawn enemies
    EnemySpawnData spawnData;     //data to send to master script

    private void OnValidate()
    {
        radiusObj = transform.GetChild(0);
        radiusObj.gameObject.SetActive(showRadius);
        radiusObj.localScale = new Vector3(spawnRadius, 0.005f, spawnRadius) * 2f;
    }
    private void Awake()
    {
        masterSpawn = FindObjectOfType<EnemyMasterSpawn>();
        if (defaultEnemies.Count != 0) DefaultData();
        else this.gameObject.SetActive(false); //deactivate if no enemies to spawn

        if (respawnOnDefeat) respawn = false; //deactivate normal respawn if respawn on defeat active
    }
    void DefaultData() //adds default enemies to spawnlist
    {
        foreach(EnemyCode enemy in defaultEnemies)
        { spawnList.Add(enemy); }
    }
    public void updateEnemyData(EnemySpawnData newData) //update spawn data during run time
    { spawnData = newData; }
    private void updateTime() //recieve time update from master script and enable respawn if enough time has passed
    {
        time++;
        if (inactive && respawnList.Count != 0) inactiveTime++;
        if (time >= spawnTime && !allDefeated) { time = 0; allowSpawn = true; }
        else if (allDefeated) { timeDefeat++; if (timeDefeat >= spawnTimeOnDefeat) { allowSpawn = true; respawnAll = true; timeDefeat = 0; } }
    }
    private void updateActivity(Vector3 pos) //recieve player location from master and activate/deactivate spawner
    {
        if (!useRadius) return;
        float dist = Mathf.Abs(Vector3.Distance(transform.position, pos));
        inactive = (dist >= spawnRadius);
    }
    private void updateActivity(float area) //recieve player location from master and activate/deactivate spawner
    {
        if (useRadius) return;
        inactive = area != activeArea;
    }
    private void Update()
    {
        if (inactive && inactiveTime > 20f) makeInactive();
        if (!allowSpawn || inactive) return;
        if (spawnList.Count > 0) spawnEnemy();
        else if (respawnList.Count > 0) Respawn();
        allowSpawn = false;
    }
    private void makeInactive()
    {
        foreach (GameObject enemy in respawnList) { masterSpawn.ReturnToPool(enemy, this.gameObject); } 
        respawnList.Clear(); 
        spawnList = defaultEnemies; 
        inactiveTime = 0;
    }
    private void spawnEnemy() //send request to master script for enemy spawn
    {
        allowSpawn = false;

        //create and send data
        spawnData = new EnemySpawnData();
        spawnData.Enemy = spawnList[0];
        spawnData.tranform = transform;
        spawnData.Point = transform.GetComponent<EnemySpawnPointV2>();
        masterSpawn.recievedData(spawnData);
        spawnList.RemoveAt(0);

        //iterate through all enemies if spawning all at once
        if (spawnAll && spawnList.Count > 0) spawnEnemy();
    }
    private void Respawn()
    {
        if (respawnOnDefeat && EnemyList.Count == 0 && !allDefeated) //if all enemies defeated trigger bool
        {
            respawnCount = respawnList.Count; //store number of enemies to respawn
            allDefeated = true;
        }
        else if (respawn || respawnAll) //respawn enemy
        {
            respawnList[0].SetActive(true);
            EnemyList.Add(respawnList[0]);
            respawnList[0].transform.position = this.transform.position;
            respawnList[0].transform.rotation = this.transform.rotation;
            respawnList.RemoveAt(0);
        }
        if (respawnAll) respawnCount--;
        if (respawnCount <= 0) { respawnAll = false; allDefeated = false; }


        //iterate through all enemies if spawning all at once
        if (spawnAll && respawnAll && respawnList.Count > 0) Respawn();
    }
    public void updateStatus(GameObject enemy) //update the spawn point when an enemy is defeated
    {
        respawnList.Add(enemy);
        EnemyList.Remove(enemy);
    }


}
