using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class InBattleMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject mainGameObject;
    [SerializeField] private CardTexturesScriptableObject cardTexturesScriptableObject;
    private MainGameLogic mainGameLogic;
    private Dictionary<MainGameLogic.CardTypes, Texture2D> cardTextures;
    private DeckHandler deckHandler;
    private VisualElement battleUI;
    private VisualElement handUI;
    private VisualElement pauseMenu;
    private Button continueButton;
    private Button quitButton;
    private Label pauseLabel;

    private bool isPaused;
    private bool isPauseMenuOpen;

    public Dictionary<string, Button> cardButtons = new Dictionary<string, Button>
    {
        {"card0", null},
        {"card1", null},
        {"card2", null},
        {"card3", null},
        {"card4", null}
    };

    private void Start()
    {
        deckHandler = mainGameObject.GetComponent<DeckHandler>();
        mainGameLogic = mainGameObject.GetComponent<MainGameLogic>();

        battleUI = GetComponent<UIDocument>().rootVisualElement;
        handUI = battleUI.Q<VisualElement>("hand");
        pauseLabel = battleUI.Q<VisualElement>("PauseElement").Q<Label>("PauseLabel");
        pauseLabel.style.display = DisplayStyle.None;

        pauseMenu = battleUI.Q<VisualElement>("PauseMenu");
        continueButton = pauseMenu.Q<Button>("ContinueButton");
        quitButton = pauseMenu.Q<Button>("QuitButton");
        pauseMenu.style.display = DisplayStyle.None;

        if (continueButton == null)
        {
            Debug.LogError("continueButton not found!");
        }

        continueButton.clicked += OnContinueButtonClicked;
        quitButton.clicked += OnQuitButtonClicked;

        cardButtons["card0"] = handUI.Q<Button>("card0");
        cardButtons["card1"] = handUI.Q<Button>("card1");
        cardButtons["card2"] = handUI.Q<Button>("card2");
        cardButtons["card3"] = handUI.Q<Button>("card3");
        cardButtons["card4"] = handUI.Q<Button>("card4");

        cardTextures = new Dictionary<MainGameLogic.CardTypes, Texture2D>(cardTexturesScriptableObject.textures);

        deckHandler.OnCardDraw += Deck_OnCardDraw;
        
    }

    private void Deck_OnCardDraw(object sender, DeckHandler.OnCardDrawEventArgs e)
    {
        switch (e.cardID)
        {
            case 0:
                cardButtons["card0"].style.backgroundImage = new StyleBackground(cardTextures[e.cardType]);
                break;
            
            case 1:
                cardButtons["card1"].style.backgroundImage = new StyleBackground(cardTextures[e.cardType]);
                break;
            
            case 2:
                cardButtons["card2"].style.backgroundImage = new StyleBackground(cardTextures[e.cardType]);
                break;

            case 3:
                cardButtons["card3"].style.backgroundImage = new StyleBackground(cardTextures[e.cardType]);
                break;

            case 4:
                cardButtons["card4"].style.backgroundImage = new StyleBackground(cardTextures[e.cardType]);
                break;
        }
    }

    public void OnPauseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed && !isPauseMenuOpen)
        {
            pauseLabel.style.display = (isPaused = mainGameLogic.togglePause(isPaused)) ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public void OnPauseMenuButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed && !isPaused)
        {
            pauseMenu.style.display = (isPauseMenuOpen = mainGameLogic.togglePause(isPauseMenuOpen)) ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void OnContinueButtonClicked()
    {
        pauseMenu.style.display = (isPauseMenuOpen = mainGameLogic.togglePause(isPauseMenuOpen)) ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void OnQuitButtonClicked()
    {
        SceneHandler.Instance.changeScene(SceneHandler.Scenes.MAIN_MENU);
    }
}
