using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    private int currentScene;
    public enum Scenes
    {
        MAIN_MENU = 0,
        IN_GAME = 1
    } 
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && SceneManager.GetActiveScene().buildIndex == (int)Scenes.MAIN_MENU)
        {
            changeScene();
        }
    }

    public void changeScene()
    {
        SceneManager.LoadScene((int)Scenes.IN_GAME);
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }
}
