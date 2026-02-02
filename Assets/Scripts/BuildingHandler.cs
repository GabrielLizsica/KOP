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
    public List<OccupiedPlot> occupiedTiles = new List<OccupiedPlot>();
    private List<MainGameLogic.CardTypes> buildings = new List<MainGameLogic.CardTypes> 
    {
        MainGameLogic.CardTypes.TOWER,
        MainGameLogic.CardTypes.BASIC_TRAP,
        MainGameLogic.CardTypes.ICE_TRAP,
        MainGameLogic.CardTypes.POISON_TRAP
    };
    private List<MainGameLogic.CardTypes> spells = new List<MainGameLogic.CardTypes>
    {
        MainGameLogic.CardTypes.ATTACK_SPEED_BUFF,
        MainGameLogic.CardTypes.RANGE_BUFF,
        MainGameLogic.CardTypes.DAMAGE_BUFF,
        MainGameLogic.CardTypes.BASE_HEAL
    };
    private List<MainGameLogic.CardTypes> traps = new List<MainGameLogic.CardTypes>
    {
        MainGameLogic.CardTypes.BASIC_TRAP,
        MainGameLogic.CardTypes.ICE_TRAP,
        MainGameLogic.CardTypes.POISON_TRAP
    };
    private Vector3 prevPos;
    private Dictionary<BuildingAssetType, GameObject> newBuildingAssets;
    private Dictionary<BuildingAssetType, GameObject> usedBlueprints;
    private GameObject builtBuilding;
    private SpellScriptableObject spell;
    
    [Header("Tower Assets")]
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject towerBlueprintValid;
    [SerializeField] private GameObject towerBlueprintInvalid;
    
    [Header("Trap Assets")]
    [SerializeField] private GameObject basicTrap;
    [SerializeField] private GameObject trapBlueprintValid;
    [SerializeField] private GameObject trapBlueprintInvalid;
    [SerializeField] private GameObject iceTrap;
    [SerializeField] private GameObject poisonTrap;

    [Header("Spell Assets")]
    [SerializeField] private GameObject spellBlueprintValid;
    [SerializeField] private GameObject spellBlueprintInvalid;
    [SerializeField] private AttackSpeedBuffScriptableObject attackSpeedBuffScriptableObject;
    [SerializeField] private RangeBuffScriptableObject rangeBuffScriptableObject;
    [SerializeField] private DamageBuffScriptableObject damageBuffScriptableObject;
    [SerializeField] private DamageBuffScriptableObject baseHealScriptableObject;
    
    private bool isBuilding;
    private bool canBuild;
    private MainGameLogic.CardTypes buildingType;
    private OccupiedPlot occupiedPlot;
    public struct OccupiedPlot
    {
        public OccupiedPlot(Vector2Int _pos, MainGameLogic.CardTypes _type, GameObject _buildingObject)
        {
            pos = _pos;
            type = _type;
            buildingObject = _buildingObject;
        }

        public Vector2Int pos { get; }
        public MainGameLogic.CardTypes type { get; }
        public GameObject buildingObject { get; }
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
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isBuilding)
        {
            usedBlueprints = beginBuilding(MainGameLogic.CardTypes.TOWER);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2) && !isBuilding)
        {
            usedBlueprints = beginBuilding(MainGameLogic.CardTypes.BASIC_TRAP);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3) && !isBuilding)
        {
            usedBlueprints = beginBuilding(MainGameLogic.CardTypes.ICE_TRAP);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4) && !isBuilding)
        {
            usedBlueprints = beginBuilding(MainGameLogic.CardTypes.POISON_TRAP);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5) && !isBuilding)
        {
            usedBlueprints = beginBuilding(MainGameLogic.CardTypes.ATTACK_SPEED_BUFF);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha6) && !isBuilding)
        {
            usedBlueprints = beginBuilding(MainGameLogic.CardTypes.RANGE_BUFF);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7) && !isBuilding)
        {
            usedBlueprints = beginBuilding(MainGameLogic.CardTypes.DAMAGE_BUFF);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8) && !isBuilding)
        {
            usedBlueprints = beginBuilding(MainGameLogic.CardTypes.BASE_HEAL);
        }
        */
        
        if (isBuilding)
        {
            updateBlueprint(usedBlueprints);
        }
    }
    
    public void cardSelected(MainGameLogic.CardTypes type)
    {
        usedBlueprints = beginBuilding(type);
    }
    
    private Dictionary<BuildingAssetType, GameObject> beginBuilding(MainGameLogic.CardTypes toBuild)
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
    
    private void setBuilding(MainGameLogic.CardTypes type)
    {
        switch (type)
        {
            case MainGameLogic.CardTypes.TOWER:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, tower}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, towerBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, towerBlueprintValid}
                };
                
                break;
            case MainGameLogic.CardTypes.BASIC_TRAP:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, basicTrap}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, trapBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, trapBlueprintValid}
                };
            
                break;
            case MainGameLogic.CardTypes.ICE_TRAP:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, iceTrap}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, trapBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, trapBlueprintValid}
                };
            
                break;
                
            case MainGameLogic.CardTypes.POISON_TRAP:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, poisonTrap}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, trapBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, trapBlueprintValid}
                };
            
                break;
            
            case MainGameLogic.CardTypes.ATTACK_SPEED_BUFF:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, null}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, spellBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, spellBlueprintValid}
                };

                spell = attackSpeedBuffScriptableObject;

                break;
                
            case MainGameLogic.CardTypes.RANGE_BUFF:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, null}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, spellBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, spellBlueprintValid}
                };

                spell = rangeBuffScriptableObject;

                break;
                
            case MainGameLogic.CardTypes.DAMAGE_BUFF:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, null}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, spellBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, spellBlueprintValid}
                };

                spell = damageBuffScriptableObject;

                break;
                
            case MainGameLogic.CardTypes.BASE_HEAL:
                newBuildingAssets = new Dictionary<BuildingAssetType, GameObject> 
                {
                    {BuildingAssetType.BUILDING, null}, 
                    {BuildingAssetType.BLUEPRINT_INVALID, spellBlueprintInvalid}, 
                    {BuildingAssetType.BLUEPRINT_VALID, spellBlueprintValid}
                };

                spell = baseHealScriptableObject;

                break;
            default:
                newBuildingAssets = null;
                buildingType = MainGameLogic.CardTypes.DEFAULT;
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
                case MainGameLogic.CardTypes.TOWER:
                    if (path.Contains(mouseTile) || checkOccupied(mouseTile) != MainGameLogic.CardTypes.DEFAULT)
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
                    
                case MainGameLogic.CardTypes.BASIC_TRAP:
                case MainGameLogic.CardTypes.ICE_TRAP:
                case MainGameLogic.CardTypes.POISON_TRAP:
                    if (!path.Contains(mouseTile) || checkOccupied(mouseTile) != MainGameLogic.CardTypes.DEFAULT)
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
                
                case MainGameLogic.CardTypes.ATTACK_SPEED_BUFF:
                    if (checkOccupied(mouseTile) == MainGameLogic.CardTypes.TOWER)
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(false);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(true);

                        canBuild = true;
                    }
                    else
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(true);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(false);

                        canBuild = false;
                    }
                    
                    break;
                    
                case MainGameLogic.CardTypes.RANGE_BUFF:
                    if (checkOccupied(mouseTile) == MainGameLogic.CardTypes.TOWER)
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(false);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(true);

                        canBuild = true;
                    }
                    else
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(true);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(false);

                        canBuild = false;
                    }
                    
                    break;
                    
                case MainGameLogic.CardTypes.DAMAGE_BUFF:
                    if (checkOccupied(mouseTile) == MainGameLogic.CardTypes.TOWER || traps.Contains(checkOccupied(mouseTile)))
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(false);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(true);

                        canBuild = true;
                    }
                    else
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(true);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(false);

                        canBuild = false;
                    }
                    
                    break;
                    
                case MainGameLogic.CardTypes.BASE_HEAL:
                    if (mouseTile == path[path.Count - 1])
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(false);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(true);

                        canBuild = true;
                    }
                    else
                    {
                        usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID].SetActive(true);
                        usedBlueprints[BuildingAssetType.BLUEPRINT_VALID].SetActive(false);

                        canBuild = false;
                    }
                    
                    break;
                default:
                    break;
            }

            prevPos = mainGameLogic.MousePosTile;
        }
    }
    
    private MainGameLogic.CardTypes checkOccupied(Vector2Int pos)
    {
        for (int i = 0; i < occupiedTiles.Count; i++)
        {
            if (occupiedTiles[i].pos == pos)
            {
                occupiedPlot = occupiedTiles[i];
                return occupiedTiles[i].type;
            }
        }

        return MainGameLogic.CardTypes.DEFAULT;
    }
    
    public void placeNewBuilding()
    {
        if (canBuild && isBuilding)
        {
            if (buildings.Contains(buildingType))
            {
                Vector3 adjustedPos = new Vector3(mainGameLogic.MousePosTile.x + 0.5f, mainGameLogic.MousePosTile.y + 0.5f, mainGameLogic.MousePosTile.z);
                builtBuilding = Instantiate(newBuildingAssets[BuildingAssetType.BUILDING], adjustedPos, Quaternion.identity);
                occupiedTiles.Add(new OccupiedPlot(new Vector2Int((int)mainGameLogic.MousePosTile.x, (int)mainGameLogic.MousePosTile.y), buildingType, builtBuilding));
            }
            else if (spells.Contains(buildingType))
            {
                if (buildingType == MainGameLogic.CardTypes.BASE_HEAL)
                {
                    mainGameLogic.baseHealth += ((mainGameLogic.baseHealth + spell.effectstrength) > 100) ? 100 - mainGameLogic.baseHealth : spell.effectstrength;
                }
                else
                {
                    switch (occupiedPlot.type)
                    {
                        case MainGameLogic.CardTypes.TOWER:
                            occupiedPlot.buildingObject.GetComponent<Tower>().applyEffect(buildingType, spell.effectstrength, spell.effectduration);
                            Debug.Log("Casted on Tower!");
                            break;
                            
                        case MainGameLogic.CardTypes.BASIC_TRAP:
                            occupiedPlot.buildingObject.GetComponent<BasicTrap>().applyEffect(buildingType, spell.effectstrength, spell.effectduration);
                            break;
                            
                        case MainGameLogic.CardTypes.ICE_TRAP:
                            occupiedPlot.buildingObject.GetComponent<IceTrap>().applyEffect(buildingType, spell.effectstrength, spell.effectduration);
                            break;
                            
                        case MainGameLogic.CardTypes.POISON_TRAP:
                            occupiedPlot.buildingObject.GetComponent<PoisonTrap>().applyEffect(buildingType, spell.effectstrength, spell.effectduration);
                            break;
                    }
                }
            }
            
            finishBuilding();
        }
    }
    
    public void finishBuilding()
    {
        newBuildingAssets = null;
        Destroy(usedBlueprints[BuildingAssetType.BLUEPRINT_INVALID]);
        Destroy(usedBlueprints[BuildingAssetType.BLUEPRINT_VALID]);
        usedBlueprints = null;
        buildingType = MainGameLogic.CardTypes.DEFAULT;
        spell = null;
        isBuilding = false;
    }
}
