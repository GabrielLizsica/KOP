using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class MainGameLogic : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private MapHandler mapHandler;
    [SerializeField] private WaveHandler waveHandler;

    [Header("Grid")]
    [SerializeField] private Tilemap tilemapGround;
    
    private void Start()
    {
        mapHandler = GetComponent<MapHandler>();
        waveHandler = GetComponent<WaveHandler>();

        tilemapGround = mapHandler.tilemapGround;
        
        mapHandler.createMap();
        StartCoroutine(waveHandler.spawnWave(2, 5, 5f, 1.5f));
    }
    
    private void Update()
    {
        Vector3Int mousePosTile = tilemapGround.WorldToCell(getMousePosTile());

        Debug.Log(mousePosTile);
    }
    
    private Vector3 getMousePosTile()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        return mouseWorldPos;
    }
}