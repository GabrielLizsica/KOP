using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class BuildingHandler : MonoBehaviour
{
    [SerializeField] private GameObject building;
    private MainGameLogic mainGameLogic;
    private GameObject newBuilding;


    private void Start()
    {
        mainGameLogic = GetComponent<MainGameLogic>();
    }
    
    private void Update()
    {
        if(newBuilding != null)
        {
            newBuilding.transform.position = new Vector3(mainGameLogic.MousePosTile.x + 0.5f, mainGameLogic.MousePosTile.y + 0.5f, 0);
        }
    }
    
    public void placeNewBuilding(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            newBuilding = Instantiate(building, new Vector3Int(0, 0, 0), Quaternion.identity);
        }
    }
}
