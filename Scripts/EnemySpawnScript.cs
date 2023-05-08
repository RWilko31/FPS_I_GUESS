using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnScript : MonoBehaviour
{
    [Header("EnemySpawnSettings")]
    [SerializeField] private GameObject PrefabToSpawn;
    [SerializeField] private bool SpawnAllOnStart = false;
    [SerializeField] private int No_Enemies;
    [SerializeField] private float respawnTime;
    [SerializeField] private float respawnRadius;

    [Header("Debug")]
    [SerializeField] private bool respawn = false;
    [SerializeField] private bool respawnCD = false;

    [Header("SpawnedEnemyList")]
    [SerializeField] private List<Transform> EnemyList = new List<Transform>();
    [SerializeField] public List<Transform> RespawnList = new List<Transform>();

    private void Awake()
    {
        //deactivate if no prefab is given to stop errors
        if (!PrefabToSpawn) this.enabled = false;

        //spawn all enemies by default
        if(SpawnAllOnStart) for(int i = No_Enemies; i > 0; i--) { SpawnEnemy(); }
    }

    private void Update()
    {
        ////if max enemies spawned in return
        if (EnemyList.Count > No_Enemies && RespawnList.Count == 0) return;
        ////wait for respawnTime
        else if (!respawnCD) StartCoroutine(RespawnCoolDown());
        //spawn enemy in if not all on start
        else if (EnemyList.Count < No_Enemies && respawn) SpawnEnemy(); 
        //respawn enemies
        else if (RespawnList.Count > 0 && respawn) RespawnEnemy();
    }
    
    private IEnumerator RespawnCoolDown()
    {
        respawnCD = true;
        yield return new WaitForSecondsRealtime(respawnTime);
        respawn = true;
    }

    private void SpawnEnemy()
    {
        respawn = false;
        Transform Enemy = Instantiate(PrefabToSpawn, transform.position, transform.rotation).transform;
        Enemy.parent = this.transform;
        Enemy.GetComponent<HealthScript>().Spawnpoint = this.gameObject;

        //Add to list
        EnemyList.Clear();
        foreach(Transform enemy in this.transform)
        { if(enemy.name == PrefabToSpawn.name) EnemyList.Add(enemy); }

        randomisePosition(Enemy);
        respawnCD = false;
    }
    private void RespawnEnemy()
    {
        respawn = false;
        Transform Enemy = RespawnList[RespawnList.Count -1]; 
        randomisePosition(Enemy);
        //reactivate enemy and allow next to start respawn
        RespawnList.RemoveAt(RespawnList.Count -1);
        Enemy.gameObject.SetActive(true);
        Enemy.GetComponent<EnemyScript>().Reset();
        respawnCD = false;
    }
    private void randomisePosition(Transform Enemy)
    {
        Enemy.position = transform.position;
        Vector3 randomDirection = Random.insideUnitSphere * respawnRadius;
        NavMeshHit hit;
        //set respawn point
        if (NavMesh.SamplePosition(randomDirection, out hit, respawnRadius, 1)) Enemy.position = hit.position;
    }

    public void RemoveEnemy(Transform Enemy)
    {
        EnemyList.Remove(Enemy);
    }
}
