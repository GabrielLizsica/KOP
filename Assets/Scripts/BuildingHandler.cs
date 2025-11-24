using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class BuildingHandler : MonoBehaviour
{
    private MainGameLogic mainGameLogic;
    private MapHandler mapHandler;
    private List<Vector2Int> path;
    private Vector3 prevPos;
    private List<GameObject> newBuilding;
    private GameObject builtBuilding;
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject towerBlueprintValid;
    [SerializeField] private GameObject towerBlueprintInvalid;
    private GameObject[] usedBlueprints;
    private bool isBuilding;
    private bool canBuild;
    private BuildingType buildingType;
    private enum BuildingType
    {
        DEFAULT,
        TOWER,
        TRAP,
        GEM,
    }


    private void Start()
    {
        mainGameLogic = GetComponent<MainGameLogic>();
        mapHandler = GetComponent<MapHandler>();
        path = mapHandler.Path;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            usedBlueprints = beginBuilding(BuildingType.TOWER);
        }
        
        if (isBuilding)
        {
            updateBlueprint(usedBlueprints);
        }
    }
    
    public void placeNewBuilding(InputAction.CallbackContext context)
    {
        if (context.performed && canBuild)
        {
            Vector3 adjustedPos = new Vector3(mainGameLogic.MousePosTile.x + 0.5f, mainGameLogic.MousePosTile.y + 0.5f, mainGameLogic.MousePosTile.z);
            builtBuilding = Instantiate(newBuilding[0], adjustedPos, Quaternion.identity);
        }
    }
    
    private void setBuilding(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.TOWER:
                newBuilding = new List<GameObject>() { tower, towerBlueprintInvalid, towerBlueprintValid};
                break;
            case BuildingType.TRAP:
                break;
            case BuildingType.GEM:
                break;
            default:
                newBuilding = null;
                buildingType = BuildingType.DEFAULT;
                break;
        }
    }
    
    private void updateBlueprint(GameObject[] usedBlueprints)
    {
        Vector2Int mouseTile = new Vector2Int((int)mainGameLogic.MousePosTile.x, (int)mainGameLogic.MousePosTile.y);
        
        if (prevPos != mainGameLogic.MousePosTile)
        {
            Vector3 adjustedPos = new Vector3(mainGameLogic.MousePosTile.x + 0.5f, mainGameLogic.MousePosTile.y + 0.5f, mainGameLogic.MousePosTile.z);
            usedBlueprints[0].transform.position = adjustedPos;
            usedBlueprints[1].transform.position = adjustedPos;
                    
            switch (buildingType)
            {
                case BuildingType.TOWER:
                    if (!path.Contains(mouseTile))
                    {
                        usedBlueprints[0].SetActive(false);
                        usedBlueprints[1].SetActive(true);

                        canBuild = true;
                    }
                    else
                    {
                        usedBlueprints[0].SetActive(true);
                        usedBlueprints[1].SetActive(false);

                        canBuild = false;
                    }
                    
                    break;
                default:
                    break;
            }

            prevPos = mainGameLogic.MousePosTile;
        }
    }
    
    private GameObject[] beginBuilding(BuildingType toBuild)
    {
        isBuilding = true;
        buildingType = toBuild;
        setBuilding(toBuild);
        prevPos = new Vector3(-1, -1, -1);

        GameObject blueprintInvalid = Instantiate(newBuilding[1], new Vector3(mainGameLogic.MousePosTile.x, mainGameLogic.MousePosTile.y, 0), Quaternion.identity);
        blueprintInvalid.name = newBuilding[1].name;
        blueprintInvalid.SetActive(false);
        
        GameObject blueprintValid = Instantiate(newBuilding[2], new Vector3(mainGameLogic.MousePosTile.x, mainGameLogic.MousePosTile.y, 0), Quaternion.identity);
        blueprintValid.name = newBuilding[2].name;
        blueprintValid.SetActive(false);

        return new GameObject[] { blueprintInvalid, blueprintValid };
    }
}
