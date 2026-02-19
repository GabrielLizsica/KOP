using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR;
using System.Collections;

public class InBattleMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject mainGameObject;
    [SerializeField] private CardTexturesScriptableObject cardTexturesScriptableObject;
    private MainGameLogic mainGameLogic;
    private SaveLoadSystem saveLoadSystem;
    private Dictionary<MainGameLogic.CardTypes, Texture2D> cardTextures;
    private DeckHandler deckHandler;
    private VisualElement battleUI;
    private VisualElement handUI;
    private VisualElement pauseMenu;
    private Button continueButton;
    private Button quitButton;
    private Label pauseLabel;
    private Label energyWarningLabel;
    private Label healthLabel;
    private Label energyLabel;

    private bool isPaused;
    private bool isPauseMenuOpen;

    public enum displayLabels
    {
        HEALTH,
        ENERGY
    }

    public Dictionary<string, Button> cardButtons = new Dictionary<string, Button>
    {
        {"card0", null},
        {"card1", null},
        {"card2", null},
        {"card3", null},
        {"card4", null}
    };
    
    private void Awake()
    {
        deckHandler = mainGameObject.GetComponent<DeckHandler>();
        mainGameLogic = mainGameObject.GetComponent<MainGameLogic>();
        saveLoadSystem = SaveLoadSystem.Instance;

        battleUI = GetComponent<UIDocument>().rootVisualElement;
        handUI = battleUI.Q<VisualElement>("hand");
        pauseLabel = battleUI.Q<VisualElement>("PauseElement").Q<Label>("PauseLabel");
        energyWarningLabel = battleUI.Q<VisualElement>("EnergyWarningElement").Q<Label>("EnergyWarningLabel");

        VisualElement playerStatDisplay = battleUI.Q<VisualElement>("PlayerStatDisplay");
        healthLabel = playerStatDisplay.Q<Label>("HealthLabel");
        energyLabel = playerStatDisplay.Q<Label>("EnergyLabel");

        pauseMenu = battleUI.Q<VisualElement>("PauseMenu");
        continueButton = pauseMenu.Q<Button>("ContinueButton");
        quitButton = pauseMenu.Q<Button>("QuitButton");

        for (int i = 0; i < cardButtons.Count; i++)
        {
            cardButtons[$"card{i}"] = handUI.Q<Button>($"card{i}");
        }

        cardTextures = new Dictionary<MainGameLogic.CardTypes, Texture2D>(cardTexturesScriptableObject.textures);

        pauseLabel.style.display = DisplayStyle.None;
        pauseMenu.style.display = DisplayStyle.None;
        energyWarningLabel.style.display = DisplayStyle.None;

        continueButton.clicked += OnContinueButtonClicked;
        quitButton.clicked += OnQuitButtonClicked;
        deckHandler.OnCardDraw += Deck_OnCardDraw;
    }

    private void Start()
    {
        updateLabel(displayLabels.HEALTH);
        updateLabel(displayLabels.ENERGY);
    }

    public void updateLabel(displayLabels label)
    {
        switch (label)
        {
            case displayLabels.HEALTH:
                healthLabel.text = $"Health: {mainGameLogic.currentHealth}";
                break;
            case displayLabels.ENERGY:
                energyLabel.text = $"Energy: {mainGameLogic.currentEnergy}";
                break;
        }
    }

    public IEnumerator displayEnergyWarning()
    {
        energyWarningLabel.style.display = DisplayStyle.Flex;

        yield return new WaitForSeconds(2);

        energyWarningLabel.style.display = DisplayStyle.None;
    }

    private void Deck_OnCardDraw(object sender, DeckHandler.OnCardDrawEventArgs e)
    {
        int cardCost;
        cardButtons[$"card{e.cardID}"].style.backgroundImage = new StyleBackground(cardTextures[e.cardType]);

        if (BuildingHandler.traps.Contains(e.cardType))
        {
            cardCost = saveLoadSystem.trapScriptableObjects[e.cardType].costs["energy"];
        }
        else if (BuildingHandler.spells.Contains(e.cardType))
        {
            cardCost = saveLoadSystem.spellScriptableObjects[e.cardType].costs["energy"];
        }
        else
        {
            cardCost = saveLoadSystem.towerScriptableObject.costs["energy"];
        }

        cardButtons[$"card{e.cardID}"].userData = cardCost;
        cardButtons[$"card{e.cardID}"].text = $"-{(int)cardButtons[$"card{e.cardID}"].userData}";
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
