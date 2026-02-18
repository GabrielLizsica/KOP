using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance { get; private set; }
    public bool isProfileLoaded = false;
    private int currentScene;
    public enum Scenes
    {
        MAIN_MENU = 0,
        IN_GAME = 1
    } 
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    public void changeScene(Scenes scene)
    {
        SceneManager.LoadScene((int)scene);
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }
}
