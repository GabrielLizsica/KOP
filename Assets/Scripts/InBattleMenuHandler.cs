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
    private VisualElement finishMenu;
    private Button continueButton;
    private Button quitButton;
    private Button finishButton;
    private Label pauseLabel;
    private Label energyWarningLabel;
    private Label healthLabel;
    private Label energyLabel;
    private Label finishMenuLabel;
    private Label rewardLabel;

    private bool isPaused;
    private bool isPauseMenuOpen;
    public bool isfinishMenuOpen { private set; get; }

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

        finishMenu = battleUI.Q<VisualElement>("FinishMenu");
        finishButton = finishMenu.Q<Button>("FinishButton");
        finishMenuLabel = finishMenu.Q<Label>("FinishMenuLabel");
        rewardLabel = finishMenu.Q<Label>("RewardLabel");

        for (int i = 0; i < cardButtons.Count; i++)
        {
            cardButtons[$"card{i}"] = handUI.Q<Button>($"card{i}");
        }

        cardTextures = new Dictionary<MainGameLogic.CardTypes, Texture2D>(cardTexturesScriptableObject.textures);

        pauseLabel.style.display = DisplayStyle.None;
        pauseMenu.style.display = DisplayStyle.None;
        energyWarningLabel.style.display = DisplayStyle.None;
        finishMenu.style.display = DisplayStyle.None;

        continueButton.clicked += OnContinueButtonClicked;
        quitButton.clicked += OnQuitButtonClicked;
        finishButton.clicked += OnQuitButtonClicked;
        deckHandler.OnCardDraw += Deck_OnCardDraw;

        isPaused = false;
        isPauseMenuOpen = false;
        isfinishMenuOpen = false;
    }

    private void Start()
    {
        updateLabel(displayLabels.HEALTH);
        updateLabel(displayLabels.ENERGY);
        Time.timeScale = 1f;
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

    public void displayFinishMenu(bool victory, int reward)
    {
        isfinishMenuOpen = true;

        if (victory)
        {
            finishMenuLabel.text = "VICTORY!";
        }
        else
        {
            finishMenuLabel.text = "DEFEAT!";
        }

        rewardLabel.text = $"Gold: +{reward}";
        finishMenu.style.display = DisplayStyle.Flex;
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
        if (context.performed && !isPauseMenuOpen && !isfinishMenuOpen)
        {
            pauseLabel.style.display = (isPaused = mainGameLogic.togglePause(isPaused)) ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public void OnPauseMenuButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed && !isPaused && !isfinishMenuOpen)
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
