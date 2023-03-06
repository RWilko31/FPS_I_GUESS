using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lethal : MonoBehaviour
{
    private GameDataFile gameDataFile;
    private WeaponManager WeaponManager;
    private LayerMask groundLayer, enemyLayer;
    // Start is called before the first frame update
    void Start()
    {
        gameDataFile = FindObjectOfType<GameDataFile>();
        WeaponManager = FindObjectOfType<WeaponManager>();
        groundLayer = gameDataFile.groundLayer;
        enemyLayer = gameDataFile.EnemyLayer;
    }
    public IEnumerator StartGrenade()
    {
        StartCoroutine(projectileGravity());
        yield return new WaitForSeconds(3f);
        BlowUp();
    }
    private void BlowUp()
    {
        float radius = WeaponManager.lethal_Radius;
        List<GameObject> EnemyList = new List<GameObject>();
        List<GameObject> KillList = new List<GameObject>();
        RaycastHit[] hitObjects = Physics.SphereCastAll(transform.position, radius, Vector3.up, 1f);
        foreach (RaycastHit hit in hitObjects)
        { EnemyList.Add(hit.transform.gameObject); }

        foreach (GameObject Enemy in EnemyList)
        {
            if (Enemy.tag == "Enemy")
            {
                RaycastHit info;
                if (!Physics.Linecast(this.transform.position, Enemy.transform.position, out info, groundLayer))
                { KillList.Add(Enemy); }
                else if (Vector3.Distance(info.point, this.transform.position) > Vector3.Distance(Enemy.transform.position, this.transform.position)) 
                { KillList.Add(Enemy); }

                Debug.Log(info.point);
            }
        }
        foreach (GameObject enemy in KillList)
        {
            enemy.transform.GetComponent<HealthScript>().LethalDamage();
        }
        Debug.Log("end");
        Destroy(this.gameObject, 0.01f);
    }
    IEnumerator projectileGravity()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Rigidbody>().AddForce(Vector3.down * gameDataFile.lethal_Downforce, ForceMode.VelocityChange);
        StartCoroutine(projectileGravity());
    }
}
