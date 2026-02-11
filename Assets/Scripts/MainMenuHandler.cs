using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Mono.Cecil.Cil;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject sceneHandlerObject;

    private Dictionary<Profiles, Button> profileButtons = new Dictionary<Profiles, Button>();
    private Dictionary<Profiles, Button> profileDeleteButtons = new Dictionary<Profiles, Button>();
    private Dictionary<PreGameButtons, Button> preBattleButtons = new Dictionary<PreGameButtons, Button>();
    private Dictionary<ConfirmButtons, Button> confirmButtons = new Dictionary<ConfirmButtons,Button>();
    private Button quitButton;
    private Profiles profileToReset;
    private VisualElement profileSelector;
    private VisualElement profileDeleter;
    private VisualElement preBattleMenu;
    private VisualElement root;
    private VisualElement mainMenu;
    private VisualElement resetConfirmMenu;
    private VisualElement cardsMenu;
    private Label resetConfirmLabel;

    private SceneHandler sceneHandler; 
    private SaveLoadSystem saveLoadSystem;

    private bool cardsMenuOpen;

    public enum Profiles
    {
        PROFILE_0,
        PROFILE_1,
        PROFILE_2,
        PROFILE_3
    }  

    private enum PreGameButtons
    {
        START,
        CARDS,
        DECK,
        SAVE_EXIT
    }  
    
    private enum ConfirmButtons
    {
        CONFIRM,
        CANCEL
    }

    private struct CardData<T>
    {
        public CardData(MainGameLogic.CardTypes _type, string _name, string _description, T _stats, CardStats _saveStats)
        {
            type = _type;
            name = _name;
            description = _description;
            stats = _stats;
            saveStats = _saveStats;
        }
        
        public MainGameLogic.CardTypes type { get; }
        public string name { get; }
        public string description { get; }
        public T stats { get; }
        public CardStats saveStats { get; }
    }
    
    private struct CardStats
    {
        public int inDeck { get; }
        public int level { get; }
        public int owned { get; }
    }
    
    private struct TowerData
    {
        public TowerData(int _damage, int _range, float _attackSpeed)
        {
            damage = _damage;
            range = _range;
            attackSpeed = _attackSpeed;
        }
        
        public int damage { get; }
        public int range { get; }
        public float attackSpeed { get; }
    }
    
    private struct TrapData
    {
        public TrapData (float _damage, int _health, float _effectStrength, float _effectDuration)
        {
            damage = _damage;
            health = _health;
            effectStrength = _effectStrength;
            effectDuration = _effectDuration;
        }
    
        public float damage { get; }
        public int health { get; }
        public float effectStrength { get; }
        public float effectDuration { get; }
    }
    
    private struct SpellData
    {
        public SpellData(float _effectStrength, float _effectDuration)
        {
            effectStrength = _effectStrength;
            effectDuration = _effectDuration;
        }
    
        public float effectStrength { get; }
        public float effectDuration { get; }
    }

    private void Start()
    {
        sceneHandler = sceneHandlerObject.GetComponent<SceneHandler>();
        saveLoadSystem = sceneHandlerObject.GetComponent<SaveLoadSystem>();

        

        root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Root");

        root.schedule.Execute(() =>
        {
            mainMenu = root.Q<VisualElement>("MainMenu");
            resetConfirmMenu = root.Q<VisualElement>("ProfileResetConfirm");
            cardsMenu = root.Q<VisualElement>("CardsMenu");
            
            profileSelector = mainMenu.Q<VisualElement>("ProfileSelector");
            profileDeleter = mainMenu.Q<VisualElement>("ProfileDeleter");
            preBattleMenu = mainMenu.Q<VisualElement>("PreBattleMenu");
            
            quitButton = profileSelector.Q<Button>("QuitButton");
            quitButton.clicked += Application.Quit;

            resetConfirmLabel = resetConfirmMenu.Q<Label>("ConfirmLabel");
            
            preBattleMenu.style.display = DisplayStyle.None;
            resetConfirmMenu.style.display = DisplayStyle.None;
            cardsMenu.style.display = DisplayStyle.None;

            cardsMenuOpen = false;

            setConfirmButtons();
            setProfileButtons();
            setProfileDeleterButtons();
            setPreBattleMenuButtons();

            setConfirmButtonEvents();
            setProfileButtonEvents();
            setProfileDeleterButtonEvents();
            setPreBattleMenuButtonEvents();

            Debug.Log(Application.persistentDataPath);
            
            if (cardsMenu == null)
            {
                Debug.LogError("Cards menu not found!");
            }
        });
    }
    
    private void setConfirmButtons()
    {
        confirmButtons.Add(ConfirmButtons.CANCEL, resetConfirmMenu.Q<Button>("CancelButton"));
        confirmButtons.Add(ConfirmButtons.CONFIRM, resetConfirmMenu.Q<Button>("ConfirmButton"));
    }
    
    private void setConfirmButtonEvents()
    {
        confirmButtons[ConfirmButtons.CANCEL].clicked += OnCancelButtonClicked;
        confirmButtons[ConfirmButtons.CONFIRM].clicked += OnConfirmButtonClicked;
    }

    private void setProfileButtons()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Profiles)).Length; i++)
        {
            profileButtons.Add((Profiles)i, profileSelector.Q<Button>($"ProfileButton{i}"));
        }
    }

    private void setProfileDeleterButtons()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Profiles)).Length; i++)
        {
            profileDeleteButtons.Add((Profiles)i, profileDeleter.Q<Button>($"ProfileDeleteButton{i}"));
        }
    }

    private void setProfileButtonEvents()
    {
        foreach (var pair in profileButtons)
        {
            pair.Value.clicked += () => OnProfileButtonClicked(pair.Key);
        }
    }
    
    private void setProfileDeleterButtonEvents()
    {
        foreach (var pair in profileDeleteButtons)
        {
            pair.Value.clicked += () => OnProfileDeleteButtonClicked(pair.Key);
        }
    }

    private void setPreBattleMenuButtons()
    {
        preBattleButtons[PreGameButtons.START] = preBattleMenu.Q<Button>("StartButton");
        preBattleButtons[PreGameButtons.DECK] = preBattleMenu.Q<Button>("DeckButton");
        preBattleButtons[PreGameButtons.CARDS] = preBattleMenu.Q<Button>("CardsButton");
        preBattleButtons[PreGameButtons.SAVE_EXIT] = preBattleMenu.Q<Button>("SaveExitButton");
    }

    private void setPreBattleMenuButtonEvents()
    {
        preBattleButtons[PreGameButtons.START].clicked += OnStartButtonClicked;
        preBattleButtons[PreGameButtons.SAVE_EXIT].clicked += OnSaveButtonClicked;
        preBattleButtons[PreGameButtons.CARDS].clicked += OnCardsMenuButtonClicked;
    }
    
    private void OnStartButtonClicked()
    {
        sceneHandler.changeScene();
    }

    private void OnProfileButtonClicked(Profiles profile)
    {
        saveLoadSystem.loadProfile(profile);
        
        profileSelector.style.display = DisplayStyle.None;
        profileDeleter.style.display = DisplayStyle.None;
        cardsMenu.style.display = DisplayStyle.None;
        preBattleMenu.style.display = DisplayStyle.Flex;
    }
    
    private void OnSaveButtonClicked()
    {
        saveLoadSystem.saveProfile();
        
        profileSelector.style.display = DisplayStyle.Flex;
        profileDeleter.style.display = DisplayStyle.Flex;
        preBattleMenu.style.display = DisplayStyle.None;
    }
    
    private void OnProfileDeleteButtonClicked(Profiles profile)
    {
        toggleProfileMenu(false);
        resetConfirmMenu.style.display = DisplayStyle.Flex;
        resetConfirmLabel.text = $"Are you sure you want to reset profile {(int)profile + 1}?";
        profileToReset = profile;
    }
    
    private void OnCancelButtonClicked()
    {
        resetConfirmMenu.style.display = DisplayStyle.None;
        toggleProfileMenu(true);
    }
    
    private void OnConfirmButtonClicked()
    {
        saveLoadSystem.resetProfile(profileToReset);
        resetConfirmMenu.style.display = DisplayStyle.None;
        toggleProfileMenu(true);
    }
    
    private void OnCardsMenuButtonClicked()
    {
        if (cardsMenu.style.display == DisplayStyle.None)
        {
            setVisibleRecursive(cardsMenu, true);
        }
        else if (cardsMenu.style.display == DisplayStyle.Flex)
        {
            setVisibleRecursive(cardsMenu, false);
        }
    }
    
    private void toggleProfileMenu(bool enabled)
    {
        setInteractableRecursive(profileSelector, enabled, true);
        setInteractableRecursive(profileDeleter, enabled, true);
    }
    
    private void setInteractableRecursive(VisualElement _root, bool enabled, bool touchOpacity)
    {
        _root.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
        
        if (touchOpacity)
        {
            _root.style.opacity = enabled ? 1f : 0.5f;
        }

        foreach (var child in _root.Children())
        {
            setInteractableRecursive(child, enabled, touchOpacity);
        }
    }
    
    private void setVisibleRecursive(VisualElement _root, bool enabled)
    {
        _root.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
        
        foreach (var child in _root.Children())
        {
            setVisibleRecursive(child, enabled);
        }
    }
}
