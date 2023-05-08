using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScriptOld : MonoBehaviour
{
    //set the max health for each entity in the game
    [Header("Set Health")]
    [SerializeField] private float playerMaxHealth = 12f;
    [SerializeField] private float enemyMaxHealth = 6f;

    //set attck damage
    [Header("Set Damage")]
    [SerializeField] private float physicalAttckDamage = 3f;
    [SerializeField] private float rangedAttackDamage = 2f;
    [SerializeField] private float LethalAttackDamage = 8f;

    //Tags
    private string Player = "Player";
    private string Enemy = "Enemy";
    private string PlayerRangedAttack = "PlayerRangedAttack";
    private string EnemyRangedAttack = "EnemyRangedAttack";

    //The object this script is attatched to is referenced as "this"
    [Header("Health of this entity")]
    [SerializeField] private string thisEntity;
    [SerializeField] private string opposingEntity;
    [SerializeField] private string opposingRangedAttack;
    [SerializeField] private float thisMaxHealth;
    [SerializeField] private float thisCurrentHealth;

    [Header("HealthBar")]
    [SerializeField] private Image healthBar;

    private LayerMask objectLayer;
    GameDataFile GameDataFile;
    EnemyScript EnemyScript;

    private void Awake()
    {
        GameDataFile = FindObjectOfType<GameDataFile>();
        objectLayer = GameDataFile.objectLayer;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        //find what this entity is
        if (CompareTag(Player)) { thisMaxHealth = playerMaxHealth; thisEntity = Player; opposingEntity = Enemy; opposingRangedAttack = EnemyRangedAttack; }
        if (CompareTag(Enemy)) { thisMaxHealth = enemyMaxHealth; thisEntity = Enemy; opposingEntity = Player; opposingRangedAttack = PlayerRangedAttack; }
        if (thisEntity == Enemy) EnemyScript = transform.GetComponent<EnemyScript>();
        //set health
        thisCurrentHealth = thisMaxHealth;
        if (CompareTag(Player)) healthBar.fillAmount = thisMaxHealth / thisMaxHealth;

    }

    // Update is called once per frame 
    void Update()
    {
        if (GameDataFile.isPaused) return;

        if (/*thisEntity == Enemy &&*/ thisCurrentHealth <= 0f) Destroy(gameObject);
        if (CompareTag(Player)) healthBar.fillAmount = thisCurrentHealth / thisMaxHealth;
    }

    //check collisions for damage
    private void OnCollisionEnter(Collision collision)
    {
        string collisionTag = collision.gameObject.tag;
        if (collisionTag == opposingEntity && CompareTag(Player)) thisCurrentHealth -= physicalAttckDamage; //damage player if enemy touches it
        if (collisionTag == opposingRangedAttack) thisCurrentHealth -= rangedAttackDamage; //damage this entity when hit by opposing entities ranged attack
        if (collision.gameObject.layer == objectLayer && !CompareTag(Player)) thisCurrentHealth -= physicalAttckDamage; //damage this entity when hit by Objects XD
    }

    public void RaycastDamage()
    {
        thisCurrentHealth -= rangedAttackDamage;
        if (thisEntity == Enemy) { Debug.Log("hit"); if (UnityEngine.Random.Range(0, EnemyScript.Chance_TakeCover_ifShot) <= 1) { Debug.Log("Cover"); EnemyScript.FindCover(); } }
    }
    public void LethalDamage()
    {
        thisCurrentHealth -= LethalAttackDamage;
    }
}
