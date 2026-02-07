using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;


public class MainGameLogic : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private MapHandler mapHandler;
    [SerializeField] private WaveHandler waveHandler;
    [SerializeField] private GameObject a;
    
    [Header("Scriptable Objects")]
    [SerializeField] private PlayerProfleScriptableObject playerProfileScriptableObject;
    [SerializeField] private TowerScriptableObject towerScriptableObject;
    [SerializeField] private TrapScriptableObject basicTrapScriptableObject;
    [SerializeField] private TrapScriptableObject iceTrapScriptableObject;
    [SerializeField] private TrapScriptableObject poisonTrapScriptableObject;
    [SerializeField] private SpellScriptableObject attackSpeedBuffScriptableObject;
    [SerializeField] private SpellScriptableObject rangeBuffScriptableObject;
    [SerializeField] private SpellScriptableObject damageBuffScriptableObject;
    [SerializeField] private SpellScriptableObject baseHealSciptableObject;
    
    [Header("Enemy Scriptable Objects")]
    [SerializeField] private EnemyScriptableObject enemyScriptableObject;

    [Header("Grid")]
    [SerializeField] private Tilemap tilemapGround;
    
    [Header("")]
    [SerializeField] public float baseHealth;

    private DeckHandler deckHandler;
    private List<CardTypes> deck;

    private Vector3 mousePosTile;

    public Vector3 MousePosTile { get { return mousePosTile; } }
    
    public enum TrapEffects
    {
        DEFAULT,
        ICE,
        FIRE,
        POISON
    }
    
    public enum CardTypes
    {
        DEFAULT,
        TOWER,
        BASIC_TRAP,
        ICE_TRAP,
        POISON_TRAP,
        ATTACK_SPEED_BUFF,
        RANGE_BUFF,
        DAMAGE_BUFF,
        BASE_HEAL
    }
    
    public class EnemyData
    {
        public int speed;
        public int health;
        public int weakness;
    }
    
    public class TowerData
    {
        public Dictionary<string, string> title;
        public Dictionary<string, string> description;
        public Dictionary<string, TowerStats> stats;
    }
    
    public class TrapData
    {
        public Dictionary<string, string> title;
        public Dictionary<string, string> description;
        public Dictionary<string, TrapStats> stats;
    }
    
    public class SpellData
    {
        public Dictionary<string, string> title;
        public Dictionary<string, string> description;
        public Dictionary<string, SpellStats> stats;
    }
    
    public class TowerStats
    {
        public int damage;
        public int range;
        public float attackspeed;
    }
    
    public class TrapStats
    {
        public float damage;
        public int health;
        public float effectstrength;
        public float effectduration;
    }
    
    public class SpellStats
    {
        public float effectstrength;
        public float effectduration;
    }
    
    private void Start()
    {   
        mapHandler = GetComponent<MapHandler>();
        waveHandler = GetComponent<WaveHandler>();
        deckHandler = GetComponent<DeckHandler>();

        tilemapGround = mapHandler.tilemapGround;
        baseHealth = 100f;
        
        mapHandler.createMap();
        deckHandler.setDeck(createDeck());
        InitializeCardScriptableObjects();
        StartCoroutine(waveHandler.spawnWave(2, 5, 5f, 1.5f));
    }
    
    private void Update()
    {
        mousePosTile = tilemapGround.WorldToCell(getMousePosTile());

        //Debug.Log(mousePosTile);
    }
    
    private void InitializeCardScriptableObjects()
    {   
        

        TextAsset towerJson = Resources.Load<TextAsset>("TextAssets/Tower");
        TextAsset basicTrapJson = Resources.Load<TextAsset>("TextAssets/BasicTrap");
        TextAsset iceTrapJson = Resources.Load<TextAsset>("TextAssets/IceTrap");
        TextAsset poisonTrapJson = Resources.Load<TextAsset>("TextAssets/PoisonTrap");
        TextAsset baseHealJson = Resources.Load<TextAsset>("TextAssets/BaseHealSpell");
        TextAsset rangeBuffJson = Resources.Load<TextAsset>("TextAssets/RangeBuffSpell");
        TextAsset damageBuffJson = Resources.Load<TextAsset>("TextAssets/DamageBuffSpell");
        TextAsset attackSpeedBuffJson = Resources.Load<TextAsset>("TextAssets/AttackSpeedBuffSpell");
    
        TowerData towerData = JsonConvert.DeserializeObject<TowerData>(towerJson.text);
        TrapData basicTrapData = JsonConvert.DeserializeObject<TrapData>(basicTrapJson.text);
        TrapData iceTrapData = JsonConvert.DeserializeObject<TrapData>(iceTrapJson.text);
        TrapData poisonTrapData = JsonConvert.DeserializeObject<TrapData>(poisonTrapJson.text);
        SpellData baseHealSpellData = JsonConvert.DeserializeObject<SpellData>(baseHealJson.text);
        SpellData rangeBuffSpellData = JsonConvert.DeserializeObject<SpellData>(rangeBuffJson.text);
        SpellData damageBuffSpellData = JsonConvert.DeserializeObject<SpellData>(damageBuffJson.text);
        SpellData attackSpeedBuffSpellData = JsonConvert.DeserializeObject<SpellData>(attackSpeedBuffJson.text);

        TextAsset enemyJson = Resources.Load<TextAsset>("TextAssets/Enemy");
        EnemyData enemyData = JsonConvert.DeserializeObject<EnemyData>(enemyJson.text);
        
        int towerLevel = playerProfileScriptableObject.cards[CardTypes.TOWER].level;
        TowerStats towerStats = towerData.stats[$"level{towerLevel}"];
        towerScriptableObject.Init(towerStats.damage, towerStats.range, towerStats.attackspeed);

        int basicTrapLevel = playerProfileScriptableObject.cards[CardTypes.BASIC_TRAP].level;
        TrapStats basicTrapStats = basicTrapData.stats[$"level{basicTrapLevel}"];
        basicTrapScriptableObject.Init(basicTrapStats.damage, basicTrapStats.health, basicTrapStats.effectstrength, basicTrapStats.effectduration);
        
        int iceTrapLevel = playerProfileScriptableObject.cards[CardTypes.ICE_TRAP].level;
        TrapStats iceTrapStats = iceTrapData.stats[$"level{iceTrapLevel}"];
        iceTrapScriptableObject.Init(iceTrapStats.damage, iceTrapStats.health, iceTrapStats.effectstrength, iceTrapStats.effectduration);

        int poisonTrapLevel = playerProfileScriptableObject.cards[CardTypes.POISON_TRAP].level;
        TrapStats poisonTrapStats = poisonTrapData.stats[$"level{poisonTrapLevel}"];
        poisonTrapScriptableObject.Init(poisonTrapStats.damage, poisonTrapStats.health, poisonTrapStats.effectstrength, poisonTrapStats.effectduration);
        
        int attackSpeedBuffLevel = playerProfileScriptableObject.cards[CardTypes.ATTACK_SPEED_BUFF].level;
        SpellStats attackSpeedBuffStats = attackSpeedBuffSpellData.stats[$"level{attackSpeedBuffLevel}"];
        attackSpeedBuffScriptableObject.Init(attackSpeedBuffStats.effectstrength, attackSpeedBuffStats.effectduration);

        int rangeBuffLevel = playerProfileScriptableObject.cards[CardTypes.RANGE_BUFF].level;
        SpellStats rangeBuffStats = rangeBuffSpellData.stats[$"level{rangeBuffLevel}"];
        rangeBuffScriptableObject.Init(rangeBuffStats.effectstrength, rangeBuffStats.effectduration);

        int damageBuffLevel = playerProfileScriptableObject.cards[CardTypes.DAMAGE_BUFF].level;
        SpellStats damageBuffStats = damageBuffSpellData.stats[$"level{damageBuffLevel}"];
        damageBuffScriptableObject.Init(damageBuffStats.effectstrength, damageBuffStats.effectduration);
        
        Debug.Log(damageBuffScriptableObject.effectstrength);

        int baseHealLevel = playerProfileScriptableObject.cards[CardTypes.BASE_HEAL].level;
        SpellStats baseHealStats = baseHealSpellData.stats[$"level{baseHealLevel}"];
        baseHealSciptableObject.Init(baseHealStats.effectstrength, baseHealStats.effectduration);

        enemyScriptableObject.speed = enemyData.speed;
        enemyScriptableObject.health = enemyData.health;
        enemyScriptableObject.weakness = enemyData.weakness;

        Debug.Log("Scriptable objects initialized!");
    }

    private List<CardTypes> createDeck()
    {
        List<CardTypes> cardList = new List<CardTypes>();

        foreach (var pair in playerProfileScriptableObject.cards)
        {
            for (int i = 0; i < pair.Value.deck; i++)
            {
                cardList.Add(pair.Key);
            }
        }

        return cardList;
    }
    
    private Vector3 getMousePosTile()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        return mouseWorldPos;
    }
    
    public void OnPlace()
    {
        Debug.LogWarning("Placed (LMB)");
    }
}