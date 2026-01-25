using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class BuildingHandler : MonoBehaviour
{
    private MainGameLogic mainGameLogic;
    private MapHandler mapHandler;
    private List<Vector2Int> path;
    private List<OccupiedPlot> occupiedTiles = new List<OccupiedPlot>();
    private Vector3 prevPos;
    private Dictionary<BuildingAssetType, GameObject> newBuildingAssets;
    private Dictionary<BuildingAssetType, GameObject> usedBlueprints;
    private GameObject builtBuilding;
    
    [Header("Tower Assets")]
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject towerBlueprintValid;
    [SerializeField] private GameObject towerBlueprintInvalid;
    
    [Header("BasicTrap Assets")]
    [SerializeField] private GameObject basicTrap;
    [SerializeField] private GameObject basicTrapBlueprintValid;
    [SerializeField] private GameObject basicTrapBlueprintInvalid;
    
    [Header("IceTrap Assets")]
    [SerializeField] private GameObject iceTrap;
    [SerializeField] private GameObject iceTrapBlueprintValid;
    [SerializeField] private GameObject iceTrapBlueprintInvalid;
    
    [Header("IceTrap Assets")]
    [SerializeField] private GameObject poisonTrap;
    [SerializeField] private GameObject poisonTrapBlueprintValid;
    [SerializeField] private GameObject poisonTrapBlueprintInvalid;
    
    private bool isBuilding;
    private bool canBuild;
    private BuildingType buildingType;
    private struct OccupiedPlot
    {
        public OccupiedPlot(Vector2Int _pos, BuildingType _type)
        {
            pos = _pos;
            type = _type;
        }

        public Vector2Int pos { get; }
        public BuildingType type { get; }
    }
    private enum BuildingType
    {
        DEFAULT,
        TOWER,
        BASIC_TRAP,
        ICE_TRAP,
        POISON_TRAP
    }
    
    private enum BuildingAssetType
    {
        DEFAULT,
        BUILDING,
        BLUEPRINT_INVALID,
        BLUEPRINT_VALID
    }

    private void Start()
    {
        mainGameLogic = GetComponent<MainGameLogic>();
        mapHandler = GetComponent<MapHandler>();
        path = mapHandler.Path;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isBuilding)
        {
            usedBlueprints = beginBuilding(BuildingType.TOWER);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2) && !isBuilding)
        {
            usedBlueprints = beginBuilding(BuildingType.BASIC_TRAP);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3) && !isBuilding)
        {
            usedBlueprints = beginBuilding(BuildingType.ICE_TRAP);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4) && !isBuilding)
        {
            usedBlueprints = beginBuilding(BuildingType.POISON_TRAP);
        }
        
        if (isBuilding)
        {
            updateBlueprint(usedBlueprints);
        }
    }
    
    public void placeNewBuilding(InputAction.CallbackContext context)
    {
        if (context.performed && canBuild && isBuilding)
        {
            Vector3 adjustedPos = new Vector3(mainGameLogic.MousePosTile.x + 0.5f, mainGameLogic.MousePosTile.y + 0.5f, mainGameLogic.MousePosTile.z);
            builtBuilding = Instantiate(newBuildingAssets[BuildingAssetType.BUILDING], adjustedPos, Quaternion.identity);
            occupiedTiles.Add(new OccupiedPlot(new Vector2Int((int)mainGameLogic.MousePosTile.x, (int)mainGameLogic.MousePosTile.y), buildingType));
            finishBuilding();
        }
    }
    
    private void setBuilding(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.TOWER:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, tower}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, towerBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, towerBlueprintValid}
                };
                
                break;
            case BuildingType.BASIC_TRAP:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, basicTrap}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, basicTrapBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, basicTrapBlueprintValid}
                };
            
                break;
            case BuildingType.ICE_TRAP:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, iceTrap}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, iceTrapBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, iceTrapBlueprintValid}
                };
            
                break;
                
            case BuildingType.POISON_TRAP:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, poisonTrap}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, poisonTrapBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, poisonTrapBlueprintValid}
                };
            
                break;
            default:
                newBuildingAssets = null;
                buildingType = BuildingType.DEFAULT;
                break;
        }
    }
    
    private void updateBlueprint(Dictionary<BuildingAssetType, GameObject> usedBlueprints)
    {
        Vector2Int mouseTile = new Vector2Int((int)mainGameLogic.MousePosTile.x, (int)mainGameLogic.MousePosTile.y);
        
        if (prevPos != mainGameLogic.MousePosTile)
        {
            Vector3 adjustedPos = new Vector3(mainGameLogic.MousePosTile.x + 0.5f, mainGameLogic.MousePosTile.y + 0.5f, mainGameLogic.MousePosTile.z);
            usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].transform.position = adjustedPos;
            usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].transform.position = adjustedPos;
                    
            switch (buildingType)
            {
                case BuildingType.TOWER:
                    if (path.Contains(mouseTile) || checkOccupied(mouseTile))
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(true);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(false);

                        canBuild = false;
                    }
                    else
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(false);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(true);

                        canBuild = true;
                    }
                    
                    break;
                case BuildingType.BASIC_TRAP:
                case BuildingType.ICE_TRAP:
                case BuildingType.POISON_TRAP:
                    if (!path.Contains(mouseTile) || checkOccupied(mouseTile))
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(true);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(false);

                        canBuild = false;
                    }
                    else
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(false);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(true);

                        canBuild = true;
                    }

                    break;
                default:
                    break;
            }

            prevPos = mainGameLogic.MousePosTile;
        }
    }
    
    private Dictionary<BuildingAssetType, GameObject> beginBuilding(BuildingType toBuild)
    {
        isBuilding = true;
        buildingType = toBuild;
        setBuilding(toBuild);
        prevPos = new Vector3(-1, -1, -1);

        GameObject blueprintInvalid = Instantiate(newBuildingAssets[BuildingAssetType.BLUEPRINT_INVALID], new Vector3(mainGameLogic.MousePosTile.x, mainGameLogic.MousePosTile.y, 0), Quaternion.identity);
        blueprintInvalid.name = newBuildingAssets[BuildingAssetType.BLUEPRINT_INVALID].name;
        blueprintInvalid.SetActive(false);
        
        GameObject blueprintValid = Instantiate(newBuildingAssets[BuildingAssetType.BLUEPRINT_VALID], new Vector3(mainGameLogic.MousePosTile.x, mainGameLogic.MousePosTile.y, 0), Quaternion.identity);
        blueprintValid.name = newBuildingAssets[BuildingAssetType.BLUEPRINT_VALID].name;
        blueprintValid.SetActive(false);

        return new Dictionary<BuildingAssetType, GameObject> { { BuildingAssetType.BLUEPRINT_INVALID, blueprintInvalid }, { BuildingAssetType.BLUEPRINT_VALID, blueprintValid } };
    }
    
    private void finishBuilding()
    {
        newBuildingAssets = null;
        Destroy(usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID]);
        Destroy(usedBlueprints[BuildingAssetType.BLUEPRINT_VALID]);
        usedBlueprints = null;
        buildingType = BuildingType.DEFAULT;
        isBuilding = false;
    }
    
    private bool checkOccupied(Vector2Int pos)
    {
        for (int i = 0; i < occupiedTiles.Count; i++)
        {
            if (occupiedTiles[i].pos == pos)
            {
                return true;
            }
        }

        return false;
    }
}
