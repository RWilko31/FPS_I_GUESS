using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyCode
{
    Bean = 0,
    MiniBean = 1,
    MegaBean = 2,
    GigaBean = 3,
    TeraBean = 4,
    PetaBean = 5,
    ExaBean = 6,
    Cube = 7
}

public class EnemyMasterSpawn : MonoBehaviour
{    
    private List<EnemySpawnData> spawnList = new List<EnemySpawnData>(); //list of enemies to spawn. holds requests from the spawn points
    [SerializeField] private List<GameObject> inactiveEnemies = new List<GameObject>(); //list of enemies that have been defeated and are inactive. holds them to prevent instatiating as much
    GameDataFile GDFile;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject Bean;
    [SerializeField] private GameObject MiniBean;
    [SerializeField] private GameObject MegaBean;
    [SerializeField] private GameObject GigaBean;
    [SerializeField] private GameObject TeraBean;
    [SerializeField] private GameObject PetaBean;
    [SerializeField] private GameObject ExaBean;
    [SerializeField] private GameObject Cube;

    //[Header("Time")]
     private float time = 0;

    private void Awake()
    { GDFile = FindObjectOfType<GameDataFile>(); }

    void Update()
    {
        if (gameState.paused) return;

        sendTime();
        if (spawnList.Count != 0) spawnEnemy(); 
    }

    public void recievedData(EnemySpawnData data)
    { /*Debug.Log("recieved: " + data.Enemy.ToString());*/ spawnList.Add(data); }
    private void sendTime()
    { 
        time += Time.deltaTime; 
        if (time >= 1f)
        {
            sendPlayerPos();
            sendPlayerArea();
            BroadcastMessage("updateTime");
            time = 0; 
        } 
    }
    private void sendPlayerPos()
    {
        Vector3 pos = GDFile.player.position;
        BroadcastMessage("updateActivity", pos);
    }
    private void sendPlayerArea()
    {
        int area = gameState.area;
        BroadcastMessage("updateActivity", area);
    }

    private void spawnEnemy()
    {
        //read data
        EnemySpawnData data = spawnList[0];
        GameObject prefab = GetPrefab(data.Enemy);
        if (prefab == null) return;
        Vector3 pos = data.tranform.position;
        Quaternion rot = data.tranform.rotation;

        //check if requested enemy is already spawned and inactive
        GameObject enemy = null;
        bool isDisabled = false;
        foreach(GameObject iEnemy in inactiveEnemies)
        {
            //if inactive enable the enemy and remove from pool
            if (iEnemy.name == data.Enemy.ToString()) 
            { 
                isDisabled = true;
                enemy = iEnemy;
                inactiveEnemies.Remove(iEnemy);
                enemy.SetActive(true);
                enemy.transform.position = pos;
                enemy.transform.rotation = rot;
                Debug.Log("Enabling: " + enemy.name);
                break; 
            }
        }
        //if no valid inactive enemy spawn a new prefab
        if (!isDisabled) 
        {
            enemy = Instantiate(prefab, pos, rot);
            enemy.name = data.Enemy.ToString();
            Debug.Log("spawning: " + enemy.name);
        }

        //add to spawn point list
        data.Point.EnemyList.Add(enemy);

        //give enemy its spawn point
        enemy.GetComponent<HealthScript>().spawnPoint = data.Point;
        spawnList.Remove(data);
    }
    public void ReturnToPool(GameObject Enemy, GameObject spawn)
    {
        inactiveEnemies.Add(Enemy);
        Debug.Log("pooling " + Enemy.name + " from " + spawn.name);
    }

    private GameObject GetPrefab(EnemyCode Enemy)
    {
        switch (Enemy)
        {
            case EnemyCode.Bean:
                return Bean;
            case EnemyCode.MiniBean:
                return MiniBean;
            case EnemyCode.MegaBean:
                return MegaBean;
            case EnemyCode.GigaBean:
                return GigaBean;
            case EnemyCode.TeraBean:
                return TeraBean;
            case EnemyCode.PetaBean:
                return PetaBean;
            case EnemyCode.ExaBean:
                return ExaBean;
            case EnemyCode.Cube:
                return Cube;

            default:
                Debug.Log("Requested enemy not found!");
                return null;
        }
    }
}
