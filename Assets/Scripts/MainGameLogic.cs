using System;
using TMPro;
using UnityEngine;


public class MainGameLogic : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private mapHandler mapHandler;
    [SerializeField] private waveHandler waveHandler;
    
    private void Start()
    {
        mapHandler = GetComponent<mapHandler>();
        waveHandler = GetComponent<waveHandler>();
        
        mapHandler.createMap();
        waveHandler.spawnWave(1, 1);
    }
}