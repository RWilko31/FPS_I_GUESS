using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    //-----Variables-----\\
    [Header("Entity data")]
    [SerializeField] public string thisEntity;
    [SerializeField] public float thisMaxHealth;
    [SerializeField] private float thisCurrentHealth;
    [Header("Entity Enemy tag")]
    [SerializeField] public List<string> opposingEntity = new List<string>();
    [Header("Damaged By")]
    [SerializeField] private string hitByEntity = "";
    [Header("Respawn")]
    [SerializeField] public GameObject Spawnpoint;

    [Header("HealthBar")]
    [SerializeField] private Image healthBar;

    [Header("Drops")]
    public int reward = 0;
    public List<string> dropList;
    public List<int> dropPos;

    //old variables *delete when finished*
    private float LethalAttackDamage = 8f;

    private LayerMask objectLayer;
    GameDataFile gameDataFile;
    static CsvManager csvManager;
    EnemyScript enemyScript;
    MainMenuScript mainMenuScript;

    //-----Functions-----\\
    private void Awake()
    {
        gameDataFile = FindObjectOfType<GameDataFile>();
        mainMenuScript = gameDataFile.MenuContainer.GetComponent<MainMenuScript>();
        objectLayer = gameDataFile.objectLayer;
        DetermineEntity();
    }
    private void Start()
    {
        //read health table and damage table
        csvManager = gameDataFile.csvManager;
        csvManager.GetHealthData(transform.GetComponent<HealthScript>());

        //set health
        thisCurrentHealth = thisMaxHealth;
        if (CompareTag("Player")) { healthBar.fillAmount = thisMaxHealth / thisMaxHealth; }
    }
    private void DetermineEntity() //finds what entity the scipt is on
    {
        //find what this entity is
        string name = this.name;

        //remove clone from name if present
        int index2 = name.IndexOf("(");
        if (index2 >= 0) name = name.Substring(0, index2);
        transform.name = name;

        //remove numbers from end of name
        int index = name.IndexOf(" ");
        if (index >= 0) name = name.Substring(0, index);

        //rename if entity is the player
        if (name == "PlayerV2") name = "Player";
        thisEntity = name;

        //get tag
        if (CompareTag("Enemy")) enemyScript = transform.GetComponent<EnemyScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameDataFile.isPaused || !mainMenuScript.inGame) return;

        if (thisCurrentHealth <= 0f) KillEntity();
        if (CompareTag("Player")) healthBar.fillAmount = thisCurrentHealth / thisMaxHealth;
    }
    private void KillEntity() //kills entity when it has no health
    {
        if (CompareTag("Player"))
        {
            if (mainMenuScript.deathScreenContainer.activeSelf) return;
            else mainMenuScript.ShowDeathScreen(hitByEntity);
        }
        else
        {
            this.gameObject.SetActive(false);
            if (Spawnpoint) Spawnpoint.GetComponent<EnemySpawnScript>().RespawnList.Add(this.transform);
            giveDrops();
            ResetHealth();
        }
    }
    public void DamageEntity(string EntityName, string Attack, bool critical) //applies damage to entity
    {
        int damage = csvManager.GetDamageData(Attack, critical);
        hitByEntity = EntityName;
        thisCurrentHealth -= damage;
    }
    public void giveDrops()
    {
        csvManager.getDrops(thisEntity, this);
        foreach(string drop in dropList)
        {
            gameDataFile.itemSystem.addItem(drop);
        }
    }
    //public void RaycastDamage()
    //{
    //    thisCurrentHealth -= rangedAttackDamage;
    //    if (thisEntity == "Enemy") { Debug.Log("hit"); if (UnityEngine.Random.Range(0, enemyScript.Chance_TakeCover_ifShot) <= 1) { Debug.Log("Cover"); enemyScript.FindCover(); } }
    //}
    public void LethalDamage()
    {
        thisCurrentHealth -= LethalAttackDamage;
    }
    public void ResetHealth()
    {
        thisCurrentHealth = thisMaxHealth;
    }
}
