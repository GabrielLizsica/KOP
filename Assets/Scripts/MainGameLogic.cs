using System;
using TMPro;
using UnityEngine;


public class MainGameLogic : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private mapHandler mapHandler;
    
    void Start()
    {
        mapHandler.createMap();
    }
}