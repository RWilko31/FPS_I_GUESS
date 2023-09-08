using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private Transform shoot;
    [SerializeField] private Rigidbody projectile;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    private Vector3 PlayerPosition;
    private Vector3 randomisePosition;
    private Vector3 oldPos;

    [Header("Shooting")]
    [SerializeField] private float bulletDistance = Mathf.Infinity;
    [SerializeField] private float targetDistance = 100f;
    [SerializeField] private float EnemyFOV = 60f;
    [SerializeField] private bool reloading = false;
    [SerializeField] private int ammoCount = 10;
    [SerializeField] private int magSize = 10;
    [SerializeField] private float reloadTime = 1f;

    [Header("PathFinding")]
    [SerializeField] private float playerDetectDistance = 30f;
    [SerializeField] private float stoppingDistance = 3f;
    [SerializeField] private float stopFollowingDistance = 75f;
    [SerializeField] private float checkPointTime = 5f;
    [SerializeField] private float AddRandomTime = 1f;
    [SerializeField] private float walkDistance = 150f;
    [SerializeField] private float findCoverDistance = 25f;
    [SerializeField] private float Chance_TakeCover = 12f;
    [SerializeField] public float Chance_TakeCover_ifShot = 1f;
    [SerializeField] private float retreatDistance = 8.5f;
    [SerializeField] private float retreatfrom = 5f;
    [SerializeField] private float retreatTime = 0.65f; //a lower value makes the retreat pattern more random
    [SerializeField] private float setAngularSpeed;
    [SerializeField] private int NoOfCheckPoints = 0;
    [SerializeField] private int currentCheckPoint = 0;
    [SerializeField] private bool isPathFinding = false;
    [SerializeField] private bool moveAway = false;
    [SerializeField] private bool waitAtCheckpoint = false;
    [SerializeField] private bool canSeePlayer = false;
    [SerializeField] private bool followingPlayer = false;
    [SerializeField] private bool findingCover = false;
    [SerializeField] private bool playerHasShot = false;
    [SerializeField] private int coverIterations = 32;
    [SerializeField] private int checkNo = 32;


    [Header("CheckPoints")]
    [SerializeField] private bool useCheckpoints = false;
    [SerializeField] private List<Transform> CheckpointList;
    [SerializeField] private Transform Stop_1;
    [SerializeField] private Transform Stop_2;
    [SerializeField] private Transform Stop_3;
    [SerializeField] private Transform Stop_4;
    [SerializeField] private Transform Stop_5;
    [SerializeField] private Transform Stop_6;
    private List<Transform> CheckPoints = new List<Transform>();
    private Vector3 nearestCheckPoint;


    RaycastHit[] hits;
    GameObject[] CoverPoints;

    private bool allowShot = true;
    UnityEngine.AI.NavMeshAgent agent;
    GameDataFile GameDataFile;
    WeaponAndItemScript weaponAndItemScript;

    private void Awake()
    {
        GameDataFile = FindObjectOfType<GameDataFile>();
        enemyLayer = GameDataFile.EnemyLayer;
        playerLayer = GameDataFile.playerLayer;
    }
    public void Reset()
    {
        isPathFinding = false;
        moveAway = false;
        waitAtCheckpoint = false;
        canSeePlayer = false;
        followingPlayer = false;
        findingCover = false;
        playerHasShot = false;
        reloading = false;
        allowShot = true;
        GetPathFindingData(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        GetPathFindingData();
        setAngularSpeed = agent.angularSpeed;
        Player = GameDataFile.player;
        weaponAndItemScript = Player.GetComponent<WeaponAndItemScript>();
    }
    void GetPathFindingData()
    {
        if (useCheckpoints)
        {
            foreach (Transform Stop in CheckpointList)
            { CheckPoints.Add(Stop); NoOfCheckPoints++; }
            //if (Stop_1) { CheckPoints.Add(Stop_1); NoOfCheckPoints++; }
            //if (Stop_2) { CheckPoints.Add(Stop_2); NoOfCheckPoints++; }
            //if (Stop_3) { CheckPoints.Add(Stop_3); NoOfCheckPoints++; }
            //if (Stop_4) { CheckPoints.Add(Stop_4); NoOfCheckPoints++; }
            //if (Stop_5) { CheckPoints.Add(Stop_5); NoOfCheckPoints++; }
            //if (Stop_6) { CheckPoints.Add(Stop_6); NoOfCheckPoints++; }
        }
        else FindRandomPath();
    }
    void Update()
    {
        if (gameState.paused) return;

        PlayerPosition = Player.transform.position;
        //Physics.BoxCast(transform.position, new Vector3(4f, 2f, 1f), Playerpos.position, transform.rotation, 100f, playerLayer)
        //Physics.Raycast(transform.position, Playerpos.transform.position, 200f, playerLayer)
        if (canSeePlayer || followingPlayer)
        {
            if (Physics.BoxCast(transform.position, new Vector3(4f, this.transform.localScale.y * 2, 1f), transform.forward, transform.rotation, targetDistance, playerLayer))
            {
                if (allowShot && !reloading)
                {
                    /*Shoot();*/
                    ShootRC();
                }
                //else { reloading = true; StartCoroutine(Reload()); }
            }

            Vector3 lookVector = Player.transform.position - transform.position;
            lookVector.y = transform.position.y;
            Quaternion rot = Quaternion.LookRotation(lookVector);
            rot.x = 0f; rot.z = 0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.1f);
        }

        if (ammoCount <= 0f) { reloading = true; StartCoroutine(Reload()); }



        //agent.destination = Playerpos.position;
    }

    public void FindCover()
    {
        playerHasShot = true;
        checkNo -= 1;
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * findCoverDistance * 0.9f;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, findCoverDistance, 1);
        Vector3 finalPosition = hit.position;
        RaycastHit rayhit;
        if (Physics.Raycast(new Vector3(0, transform.localScale.y * 2f, 0) + finalPosition, Player.position, out rayhit, Vector3.Distance(PlayerPosition, new Vector3(0, transform.localScale.y * 2f, 0) + finalPosition), groundLayer))
        {
            //Debug.Log(checkNo);
            findingCover = true;
            //Debug.Log("Finding Cover");
            //Debug.Log(rayhit.transform);
            agent.destination = finalPosition;
            StartCoroutine(waitAtCover());
            return;
        }
        if (!findingCover && checkNo > 0) {  FindCover(); }
        //else { Debug.Log("Exit"); }
    }

    IEnumerator waitAtCover()
    {
        yield return new WaitForSeconds(5f);
        findingCover = false;
        playerHasShot = false;
    }

    void EnemyPathFinding()
    {
        if ((Vector3.Distance(this.transform.position, PlayerPosition) < playerDetectDistance && canSeePlayer) || followingPlayer) //if enemy can see player and is a short distance away start moving towards player
        {
            agent.angularSpeed = 0;
            isPathFinding = false;
            followingPlayer = true;
            if (!findingCover && !playerHasShot)
            {
                checkNo = coverIterations; //reset number of iterations to check for cover
                if (Player.GetComponent<WeaponAndItemScript>().playerHasShot && !playerHasShot && UnityEngine.Random.Range(0, Chance_TakeCover) <= 1) //random chance to find cover when player shoots
                { playerHasShot = true; FindCover(); } 
                else if (!moveAway && Vector3.Distance(this.transform.position, PlayerPosition) > stoppingDistance) //Keep enemy a certain distance away from the player
                { agent.destination = PlayerPosition; }
                else if (!moveAway && Vector3.Distance(this.transform.position, PlayerPosition) < retreatfrom) //if player is too close retreat to a safe distance
                { moveAway = true; MoveAwayFromPlayer(); }
                if (Vector3.Distance(this.transform.position, PlayerPosition) > stopFollowingDistance) //If too far away from player stop following it
                { followingPlayer = false; }
            }
            
        }
        else //Start pathfinding when player is not near
        {
            if (!isPathFinding)
            {
                agent.angularSpeed = setAngularSpeed;
                if (useCheckpoints)
                {
                    float stopDistance = Mathf.Infinity;
                    foreach (Transform stop in CheckPoints) // Check for the nearest checkpoint
                    {
                        if (Vector3.Distance(stop.position, transform.position) < stopDistance)
                        {
                            stopDistance = Vector3.Distance(stop.position, transform.position);
                            nearestCheckPoint = stop.position;
                        }
                    }
                }
                else { FindRandomPath(); }
                isPathFinding = true;
            }
            if(!waitAtCheckpoint && Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(nearestCheckPoint.x, 0, nearestCheckPoint.z)) < 0.1f &&
                transform.position.y + nearestCheckPoint.y < 1.5f) //If at checkPoint find next checkpoint and wait for a while
            {
                waitAtCheckpoint = true;
                if (useCheckpoints)
                {
                    if (currentCheckPoint == NoOfCheckPoints) { nearestCheckPoint = Stop_1.position; currentCheckPoint = 1; }
                    else
                    {
                        currentCheckPoint++;
                        nearestCheckPoint = CheckpointList[currentCheckPoint].position;
                    }
                }
                else { FindRandomPath(); }
                StartCoroutine(StopAtCheckpoint());
            }
            if (!waitAtCheckpoint)
            { agent.destination = nearestCheckPoint; }
        }
    }
    void FindRandomPath()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkDistance;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkDistance, 1);
        Vector3 finalPosition = hit.position; nearestCheckPoint = finalPosition;
    }
    void MoveAwayFromPlayer()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * retreatDistance; //generate random point
        randomDirection += transform.position;
        bool safePos = false;
        NavMeshHit hit;
        if (!safePos)
        {
            NavMesh.SamplePosition(randomDirection, out hit, walkDistance, 1); //get position on navMesh
            if (Vector3.Distance(hit.position, Player.position) > Vector3.Distance(hit.position, transform.position)) //check retreat position is behind enemy and not player
            {
                agent.destination = hit.position; //move to retreat position
                safePos = true;
                StartCoroutine(moveTime());
            }
            else MoveAwayFromPlayer(); //retry if retreat point is near player
        }
        else MoveAwayFromPlayer(); //loop if broken
    }
    IEnumerator moveTime()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(retreatTime, retreatTime + 1.25f)); //allow time to move
        if (Vector3.Distance(transform.position, Player.position) < Vector3.Distance(transform.position, agent.destination))
        { MoveAwayFromPlayer(); } //check if player is still too close
        else { moveAway = false; } //allow normal movement
    }

    IEnumerator StopAtCheckpoint()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(checkPointTime, AddRandomTime));
        waitAtCheckpoint = false;
    }

    bool CanSeeTarget(Transform target, float viewAngle, float viewRange)
    {
        Vector3 toTarget = target.position - transform.position;
        if (Vector3.Angle(transform.forward, toTarget) <= viewAngle && Physics.Raycast(transform.position, toTarget, out RaycastHit hit, viewRange) && hit.transform.root == target)
        { return true; }
        else return false;
    }

    private void FixedUpdate()
    {
        randomisePosition = new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f), 0) / 100f; //add a random offset to the aiming to prevent aimbot
        if(NoOfCheckPoints > 0 || !useCheckpoints) EnemyPathFinding();
        if (CanSeeTarget(Player, EnemyFOV, playerDetectDistance)) canSeePlayer = true;
        else canSeePlayer = false;
    }

    IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        ammoCount = magSize;
        reloading = false;
    }
    void Shoot()
    {
        allowShot = false;
        //Debug.Log("Enemyshoot");
        Rigidbody instantiatedProjectile = Instantiate(projectile, shoot.transform.position, transform.rotation) as Rigidbody;
        instantiatedProjectile.tag = "EnemyRangedAttack";
        instantiatedProjectile.AddForce((transform.forward + randomisePosition) * 80f, ForceMode.Impulse);
        Destroy(instantiatedProjectile.gameObject, 0.1f);
        StartCoroutine(ShootDelay());
    }
    private void ShootRC()
    {
        allowShot = false;
        float X = Screen.width / 2;
        float Y = Screen.height / 2;
        RaycastHit hitInfo;
        float rayDistance;

        if (Physics.BoxCast(this.transform.position, transform.localScale, transform.forward + randomisePosition, out hitInfo, transform.rotation, bulletDistance, groundLayer)) //Blue Laser
        {
            rayDistance = hitInfo.distance;
        }
        else rayDistance = bulletDistance;

        //Debug.Log(hitInfo.transform.gameObject);

        hits = Physics.BoxCastAll(this.transform.position, transform.localScale, transform.forward + randomisePosition, transform.rotation, rayDistance, playerLayer); //Yellow Laser
        Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));

        foreach (RaycastHit obj in hits) 
        {
            if (obj.transform.GetComponent<HealthScript>() != null)
            {
                //obj.transform.GetComponent<HealthScript>().RaycastDamage();
                SendAttackData(obj.transform.gameObject);
            }
        }

        ammoCount--;
        StartCoroutine(ShootDelay());
    }
    private void SendAttackData(GameObject obj)
    {
        string Attack = "shot_standard"; //temporary - sets all enemy attacks to standard shot
        HealthScript healthScript = obj.transform.GetComponent<HealthScript>();
        bool critical = false; //temporary - sets all attacks to not be critical
        healthScript.DamageEntity(GetComponent<HealthScript>().thisEntity, Attack, critical);
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(0.25f);
        allowShot = true;
    }
}