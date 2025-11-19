using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    [Header("RFeferences")]
    [SerializeField] private GameObject mainGameObject;
    [SerializeField] private MapHandler mapHandler;
    
    [Header("Enemy variables")]
    [SerializeField] private List<Vector2Int> path;
    [SerializeField] private float speed;

    private Vector3 targetTile;
    private int targetIndex = 0;
    private int posX;
    private int posY;
    
    private void Start()
    {
        mainGameObject = FindAnyObjectByType<MainGameLogic>().gameObject;
        mapHandler = mainGameObject.GetComponent<MapHandler>();
        path = mapHandler.Path;
    }
    
    private void Update()
    {
        if (Vector3.Distance(transform.position, new Vector3(path[path.Count - 1].x + 0.5f, path[path.Count - 1].y + 0.5f, 0)) > 0.1f)
        {
            targetTile = new Vector3(path[targetIndex].x + 0.5f, path[targetIndex].y + 0.5f, 0);
            
            if (Vector3.Distance(transform.position, targetTile) < 0.1f)
            {
                targetIndex++;
            }
            
            moveAlongPath(targetTile);
        }
    }
    
    private void moveAlongPath(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
    }
}
