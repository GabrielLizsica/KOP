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

    public void changeScene()
    {
        SceneManager.LoadScene((int)Scenes.IN_GAME);
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }
}
