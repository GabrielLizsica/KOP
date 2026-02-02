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
    
    [Header("Card Scriptable Objects")]
    [SerializeField] private TowerScriptableObject towerScriptableObject;
    [SerializeField] private BasicTrapScriptableObject basicTrapScriptableObject;
    [SerializeField] private IceTrapScriptableObject iceTrapScriptableObject;
    [SerializeField] private PoisonTrapScriptableObject poisonTrapScriptableObject;
    [SerializeField] private AttackSpeedBuffScriptableObject attackSpeedBuffScriptableObject;
    [SerializeField] private RangeBuffScriptableObject rangeBuffScriptableObject;
    [SerializeField] private DamageBuffScriptableObject damageBuffScriptableObject;
    [SerializeField] private BaseHealSciptableObject baseHealSciptableObject;
    
    [Header("Enemy Scriptable Objects")]
    [SerializeField] private EnemyScriptableObject enemyScriptableObject;

    [Header("Grid")]
    [SerializeField] private Tilemap tilemapGround;
    
    [Header("")]
    [SerializeField] public float baseHealth;

    private DeckHandler deckHandler;
    private List<CardTypes> deck = new List<CardTypes>()
    {
        CardTypes.TOWER,
        CardTypes.TOWER,
        CardTypes.BASIC_TRAP,
        CardTypes.BASIC_TRAP,
        CardTypes.ICE_TRAP,
        CardTypes.ICE_TRAP,
        CardTypes.POISON_TRAP,
        CardTypes.POISON_TRAP,
        CardTypes.ATTACK_SPEED_BUFF,
        CardTypes.ATTACK_SPEED_BUFF,
        CardTypes.RANGE_BUFF,
        CardTypes.RANGE_BUFF,
        CardTypes.DAMAGE_BUFF,
        CardTypes.DAMAGE_BUFF,
        CardTypes.BASE_HEAL,
        CardTypes.BASE_HEAL
    };

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
        deckHandler.setDeck(deck);
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
        
        towerScriptableObject.damage = towerData.stats["level0"].damage;
        towerScriptableObject.range = towerData.stats["level0"].range;
        towerScriptableObject.attackspeed = towerData.stats["level0"].attackspeed;
        
        basicTrapScriptableObject.damage = basicTrapData.stats["level0"].damage;
        basicTrapScriptableObject.health = basicTrapData.stats["level0"].health;
        basicTrapScriptableObject.effectstrength = basicTrapData.stats["level0"].effectstrength;
        basicTrapScriptableObject.effectduration = basicTrapData.stats["level0"].effectduration;

        iceTrapScriptableObject.damage = iceTrapData.stats["level0"].damage;
        iceTrapScriptableObject.health = iceTrapData.stats["level0"].health;
        iceTrapScriptableObject.effectstrength = iceTrapData.stats["level0"].effectstrength;
        iceTrapScriptableObject.effectduration = iceTrapData.stats["level0"].effectduration;
        
        poisonTrapScriptableObject.damage = poisonTrapData.stats["level0"].damage;
        poisonTrapScriptableObject.health = poisonTrapData.stats["level0"].health;
        poisonTrapScriptableObject.effectstrength = poisonTrapData.stats["level0"].effectstrength;
        poisonTrapScriptableObject.effectduration = poisonTrapData.stats["level0"].effectduration;

        attackSpeedBuffScriptableObject.effectstrength = attackSpeedBuffSpellData.stats["level0"].effectstrength;
        attackSpeedBuffScriptableObject.effectduration = attackSpeedBuffSpellData.stats["level0"].effectduration;

        rangeBuffScriptableObject.effectstrength = rangeBuffSpellData.stats["level0"].effectstrength;
        rangeBuffScriptableObject.effectduration = rangeBuffSpellData.stats["level0"].effectduration;

        damageBuffScriptableObject.effectstrength = damageBuffSpellData.stats["level0"].effectstrength;
        damageBuffScriptableObject.effectduration = damageBuffSpellData.stats["level0"].effectduration;
        
        baseHealSciptableObject.effectstrength = baseHealSpellData.stats["level0"].effectstrength;
        baseHealSciptableObject.effectduration = baseHealSpellData.stats["level0"].effectduration;

        enemyScriptableObject.speed = enemyData.speed;
        enemyScriptableObject.health = enemyData.health;
        enemyScriptableObject.weakness = enemyData.weakness;
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