using System;
using TMPro;
using UnityEngine;


public class MainGameLogic : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private MapHandler mapHandler;
    [SerializeField] private WaveHandler waveHandler;
    
    private void Start()
    {
        mapHandler = GetComponent<MapHandler>();
        waveHandler = GetComponent<WaveHandler>();
        
        mapHandler.createMap();
        waveHandler.spawnWave(1, 1);
    }
}