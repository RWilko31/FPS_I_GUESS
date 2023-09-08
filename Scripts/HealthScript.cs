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
    //[Header("Entity Enemy tags")]
    [HideInInspector] public List<string> opposingEntity = new List<string>(); //list of tags for enemies of this entity
    [Header("Damaged By")]
    [SerializeField] private string hitByEntity = "";
    [Header("Respawn")]
    [HideInInspector] public GameObject Spawnpoint;
    [HideInInspector] public EnemySpawnPointV2 spawnPoint;

    //[Header("HealthBar")]
    private Image healthBar;

    [Header("Drops")]
    public int reward = 0;
    public List<string> dropList;
    public List<int> dropPos;

    private LayerMask objectLayer;
    GameDataFile GDfile;
    CsvManager csvManager;
    MainMenuScript mainMenuScript;
    private int lowDmg;

    //old variables *delete when finished*
    private float LethalAttackDamage = 8f;


    //-----Functions-----\\
    private void Awake()
    {
        GDfile = FindObjectOfType<GameDataFile>();
        mainMenuScript = GDfile.MenuContainer.GetComponent<MainMenuScript>();

        healthBar = GDfile.HealthBar;
        objectLayer = GDfile.objectLayer;
        lowDmg = GDfile.lowDmg;
        DetermineEntity();
    }
    private void Start()
    {
        //read health table and damage table
        csvManager = GDfile.csvManager;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.paused || !gameState.inGame) return;

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
            //if (Spawnpoint) Spawnpoint.GetComponent<EnemySpawnScript>().RespawnList.Add(this.transform);
            if (spawnPoint) spawnPoint.updateStatus(this.gameObject);
            giveDrops();
            ResetHealth();
        }
    }
    public void DamageEntity(string EntityName, string Attack, bool critical) //applies damage to entity
    {
        int damage = csvManager.GetDamageData(Attack, critical);
        hitByEntity = EntityName;

        //Damage enemies
        if (thisEntity != "Player")
        {
            List<string> ATKList = GDfile.itemSystem.ATKList;
            int AccDmg = 0;
            foreach (string Atk in ATKList)
            {
                if(Atk.Contains("all") || Atk.Contains(thisEntity))
                {
                    int index = Atk.IndexOf("_");
                    int EndIndex = Atk.IndexOf("(");
                    AccDmg += int.Parse(Atk.Substring(index + 1, EndIndex - index - 1));
                }
            }
            //Debug.Log(AccDmg);
            thisCurrentHealth -= damage + AccDmg;
        }
        //Damage player
        else
        {
            float DefDamage = damage;
            float DEF = GDfile.itemSystem.DEF;
            if (DEF > 0) 
            {
                DefDamage *= (1 - (DEF / 100)); 
            }
            thisCurrentHealth -= DefDamage;
            //Debug.Log("damage: " + DefDamage + ", DEF: " + DEF);
        }
    }
    public void giveDrops()
    {
        csvManager.getDrops(thisEntity, this);
        foreach(string drop in dropList)
        {
            GDfile.itemSystem.addItem(drop);
        }
    }
    public void LethalDamage()
    {
        thisCurrentHealth -= LethalAttackDamage;
    }
    public void ResetHealth()
    {
        thisCurrentHealth = thisMaxHealth;
    }
}
