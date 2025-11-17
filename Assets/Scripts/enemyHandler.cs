using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class enemyHandler : MonoBehaviour
{
    [SerializeField] private GameObject mainGameObject;
    [SerializeField] private mapHandler mapHandler;
    [SerializeField] private List<Vector2Int> path;
    
    private void Start()
    {
        mainGameObject = FindAnyObjectByType<MainGameLogic>().gameObject;
        mapHandler = mainGameObject.GetComponent<mapHandler>();
        path = mapHandler.Path;
    }
    
    private void Update()
    {
        if (transform.position != new Vector3Int(path[0].x, path[0].y, 0))
        {
            moveAlongPath();
        }
    }
    
    private void moveAlongPath()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3Int(path[0].x, path[0].y, 0), Time.deltaTime * 2f);
    }
}
