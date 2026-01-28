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
    
    [Header("Enemy Scriptable Objects")]
    [SerializeField] private EnemyScriptableObject enemyScriptableObject;

    [Header("Grid")]
    [SerializeField] private Tilemap tilemapGround;

    private Vector3 mousePosTile;

    public Vector3 MousePosTile { get { return mousePosTile; } }
    
    public enum TrapEffects
    {
        DEFAULT,
        ICE,
        FIRE,
        POISON
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
        public int damage;
        public int health;
        public float effectstrength;
        public float effectduration;
    }
    
    public class SpellStats
    {
        public float effectstrength;
    }
    
    private void Start()
    {
        
        mapHandler = GetComponent<MapHandler>();
        waveHandler = GetComponent<WaveHandler>();

        tilemapGround = mapHandler.tilemapGround;
        
        mapHandler.createMap();
        StartCoroutine(waveHandler.spawnWave(2, 5, 5f, 1.5f));
        InitializeCardScriptableObjects();
    }
    
    private void Update()
    {
        mousePosTile = tilemapGround.WorldToCell(getMousePosTile());

        //Debug.Log(mousePosTile);
    }
    
    private void InitializeCardScriptableObjects()
    {
        TowerData towerData = JsonConvert.DeserializeObject<TowerData>(File.ReadAllText(Application.dataPath + "/Scripts/TextAssets/Tower.json"));
        TrapData basicTrapData = JsonConvert.DeserializeObject<TrapData>(File.ReadAllText(Application.dataPath + "/Scripts/TextAssets/BasicTrap.json"));
        TrapData iceTrapData = JsonConvert.DeserializeObject<TrapData>(File.ReadAllText(Application.dataPath + "/Scripts/TextAssets/IceTrap.json"));
        TrapData poisonTrapData = JsonConvert.DeserializeObject<TrapData>(File.ReadAllText (Application.dataPath + "/Scripts/TextAssets/PoisonTrap.json"));
        SpellData baseHealSpellData = JsonConvert.DeserializeObject<SpellData>(File.ReadAllText(Application.dataPath + "/Scripts/TextAssets/BaseHealSpell.json"));
        SpellData rangeBuffSpellData = JsonConvert.DeserializeObject<SpellData>(File.ReadAllText(Application.dataPath + "/Scripts/TextAssets/RangeBuffSpell.json"));
        SpellData damageBuffSpellData = JsonConvert.DeserializeObject<SpellData>(File.ReadAllText(Application.dataPath + "/Scripts/TextAssets/DamageBuffSpell.json"));
        SpellData attackSpeedBuffSpellData = JsonConvert.DeserializeObject<SpellData>(File.ReadAllText(Application.dataPath + "/Scripts/TextAssets/AttackSpeedBuffSpell.json"));

        EnemyData enemyData = JsonConvert.DeserializeObject<EnemyData>(File.ReadAllText(Application.dataPath + "/Scripts/TextAssets/enemy.json"));
        
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

        enemyScriptableObject.speed = enemyData.speed;
        enemyScriptableObject.health = enemyData.health;
        enemyScriptableObject.weakness = enemyData.weakness;
                
        Debug.Log(baseHealSpellData.stats["level0"].effectstrength);
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