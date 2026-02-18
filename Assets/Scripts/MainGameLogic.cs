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

    [Header("Grid")]
    [SerializeField] private Tilemap tilemapGround;
    
    [Header("")]
    [SerializeField] public float baseHealth;

    private bool isPaused;

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
    
    private void Start()
    {   
        isPaused = false;
        mapHandler = GetComponent<MapHandler>();
        waveHandler = GetComponent<WaveHandler>();
        deckHandler = GetComponent<DeckHandler>();

        tilemapGround = mapHandler.tilemapGround;
        baseHealth = 100f;
        
        mapHandler.createMap();
        deckHandler.setDeck(createDeck());
        StartCoroutine(waveHandler.spawnWave(2, 5, 5f, 1.5f));
    }
    
    private void Update()
    {
        mousePosTile = tilemapGround.WorldToCell(getMousePosTile());

        //Debug.Log(mousePosTile);
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

    public bool togglePause()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
        
        return isPaused;
    }
}