using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using NUnit.Framework;


public class MainGameLogic : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private MapHandler mapHandler;
    [SerializeField] private WaveHandler waveHandler;
    [SerializeField] private PlayerProfleScriptableObject playerProfileScriptableObject;
    [SerializeField] private GameObject battleUIObject;

    [Header("Grid")]
    [SerializeField] private Tilemap tilemapGround;
    
    [Header("")]
    [SerializeField] public float baseHealth;
    [SerializeField] public float currentHealth;
    [SerializeField] public int baseEnergy;
    [SerializeField] public int currentEnergy;
    private float prevHealth = -1;
    private int prevEnergy = -1;
    [SerializeField] private int rewardDefeat;
    [SerializeField] private int rewardVictory;
    private DeckHandler deckHandler;
    private InBattleMenuHandler battleUI;
    private SaveLoadSystem saveLoadSystem;
    private List<CardTypes> deck;

    private Vector3 mousePosTile;

    public Vector3 MousePosTile { get; }
    
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

    private void Awake()
    {
        mapHandler = GetComponent<MapHandler>();
        waveHandler = GetComponent<WaveHandler>();
        deckHandler = GetComponent<DeckHandler>();
        tilemapGround = mapHandler.tilemapGround;
        saveLoadSystem = SaveLoadSystem.Instance;
        battleUI = battleUIObject.GetComponent<InBattleMenuHandler>();

        currentHealth = baseHealth;
        currentEnergy = baseEnergy;
    }

    private void Start()
    { 
        mapHandler.createMap();
        deckHandler.setDeck(createDeck());
        StartCoroutine(waveHandler.spawnWave(UnityEngine.Random.Range(5, 10), UnityEngine.Random.Range(3, 5), UnityEngine.Random.Range(3f, 6f), UnityEngine.Random.Range(1f, 3f)));
    }
    
    private void Update()
    {
        mousePosTile = tilemapGround.WorldToCell(getMousePosTile());

        if (prevHealth != currentHealth)
        {
            battleUI.updateLabel(InBattleMenuHandler.displayLabels.HEALTH);
            prevHealth = currentHealth;
        }
        if (prevEnergy != currentEnergy)
        {
            battleUI.updateLabel(InBattleMenuHandler.displayLabels.ENERGY);
            prevEnergy = currentEnergy;
        }
        
        

        if (currentHealth <= 0 && !battleUI.isfinishMenuOpen)
        {
            Time.timeScale = 0f;
            battleUI.displayFinishMenu(false, rewardDefeat);
            saveLoadSystem.playerProfileScriptableObject.gold += rewardDefeat;
            
            saveLoadSystem.saveProfile();
        }
        else if (waveHandler.remainingEnemies == 0 && waveHandler.aliveEnemies == 0 && !battleUI.isfinishMenuOpen)
        {
            Time.timeScale = 0f;
            battleUI.displayFinishMenu(true, rewardVictory);
            saveLoadSystem.playerProfileScriptableObject.gold += rewardVictory;
            
            saveLoadSystem.saveProfile();
        }

        
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

    public bool togglePause(bool isPaused)
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;

            return true;
        }
        else
        {
            Time.timeScale = 1f;

            return false;
        }
    }
}