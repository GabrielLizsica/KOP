using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject sceneHandlerObject;

    private SceneHandler sceneHandler;
    private VisualElement mainMenu;
    private Button startButton;
    
    private void Start()
    {
        mainMenu = GetComponent<UIDocument>().rootVisualElement;
        sceneHandler = sceneHandlerObject.GetComponent<SceneHandler>();
        
        startButton = mainMenu.Q<Button>("StartButton");
        startButton.clicked += OnStartButtonClicked;
    }
    
    private void OnStartButtonClicked()
    {
        Debug.Log("Change Scene!");
        Debug.Log(sceneHandler);
        sceneHandler.changeScene();
    }
}
