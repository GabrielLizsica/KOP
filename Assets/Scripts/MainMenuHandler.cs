using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.Rendering;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject sceneHandlerObject;
    [SerializeField] private VisualTreeAsset cardRowTemplate;

    private TowerScriptableObject towerScriptableObject;
    private Dictionary<MainGameLogic.CardTypes, TrapScriptableObject> trapScriptableObjects;
    private Dictionary<MainGameLogic.CardTypes, SpellScriptableObject> spellScriptableObjects;
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
    private ListView cardListView;
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
    
    public interface ICardStats
    {
    }

    private class CardData : ICardStats
    {
        public CardData(MainGameLogic.CardTypes _type, string _name, string _description, ICardStats _stats, CardStats _saveStats, Texture2D _texture)
        {
            type = _type;
            name = _name;
            description = _description;
            stats = _stats;
            saveStats = _saveStats;
            texture = _texture;
        }
        
        public MainGameLogic.CardTypes type { get; }
        public string name { get; }
        public string description { get; }
        public ICardStats stats { get; }
        public CardStats saveStats { get; }
        public Texture2D texture;
    }
    
    private struct CardStats
    {
        public CardStats(int _inDeck, int _level, int _owned)
        {
            inDeck = _inDeck;
            level = _level;
            owned = _owned;
        }
        public int inDeck { get; }
        public int level { get; }
        public int owned { get; }
    }
    
    private class TowerData : ICardStats
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
    
    private class TrapData : ICardStats
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
    
    private class SpellData : ICardStats
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
        });
    }
    
    private List<CardData> createCardDataList()
    {
        List<CardData> test = new List<CardData>();
        MainGameLogic.CardTypes cardType = MainGameLogic.CardTypes.DEFAULT;
        string cardName = "";
        string cardDesc = "";
        SpellData spellStats = null;
        TrapData trapStats = null;
        TowerData towerStats = null;
        Texture2D cardTexture;

        for (int i = 1; i < Enum.GetValues(typeof(MainGameLogic.CardTypes)).Length; i++)
        {
            cardType = (MainGameLogic.CardTypes)i;
            spellStats = null;
            trapStats = null;
            towerStats = null;

            if (BuildingHandler.spells.Contains(cardType))
            {
                SpellScriptableObject spellObject = spellScriptableObjects[cardType];
                cardName = spellObject.title["en"];
                cardDesc = spellObject.description["en"];
                spellStats = new SpellData(spellObject.effectstrength, spellObject.effectduration);
            }
            else if (BuildingHandler.traps.Contains(cardType))
            {
                TrapScriptableObject trapObject = trapScriptableObjects[cardType];
                cardName = trapObject.title["en"];
                cardDesc = trapObject.description["en"];      
                trapStats = new TrapData(trapObject.damage, trapObject.health, trapObject.effectstrength, trapObject.effectduration);  
            }
            else if (cardType == MainGameLogic.CardTypes.TOWER)
            {
                TowerScriptableObject towerObject = towerScriptableObject;
                cardName = towerScriptableObject.title["en"];
                cardDesc = towerScriptableObject.description["en"];
                towerStats = new TowerData(towerObject.damage, towerObject.range, towerObject.attackspeed);
            }

            PlayerProfleScriptableObject profile = saveLoadSystem.playerProfileScriptableObject;
            SaveLoadSystem.Card card = profile.cards[cardType];
            CardStats cardSaveStats = new CardStats(card.deck, card.level, card.owned);
            cardTexture = saveLoadSystem.cardTexturesScriptableObject.textures[cardType];

            if (spellStats != null)
            {
                test.Add(new CardData(cardType, cardName, cardDesc, spellStats, cardSaveStats, cardTexture));
            } 
            else if (trapStats != null)
            {
                test.Add(new CardData(cardType, cardName, cardDesc, trapStats, cardSaveStats, cardTexture));
            }
            else if (towerStats != null)
            {
                test.Add(new CardData(cardType, cardName, cardDesc, towerStats, cardSaveStats, cardTexture));
            }
        }

        return test;
    }
    
    private void bindCardElements(VisualElement ve, CardData data)
    {
        VisualElement texture = ve.Q<VisualElement>("Texture");
        VisualElement nameBox = ve.Q<VisualElement>("NameBox");

        VisualElement statsBox = ve.Q<VisualElement>("StatsBox");
        VisualElement statLabelBox = statsBox.Q<VisualElement>("StatLabels");
        VisualElement statValueLabelBox = statsBox.Q<VisualElement>("StatValueLabels");

        VisualElement profileStatsBox = ve.Q<VisualElement>("ProfileStatsBox");
        VisualElement profileStatLabelBox = profileStatsBox.Q<VisualElement>("ProfileStatLabels");
        VisualElement profileStatValueLabelBox = profileStatsBox.Q<VisualElement>("ProfileStatValueLabels");
        
        Label name = nameBox.Q<Label>("Name");
        Label description = nameBox.Q<Label>("Description");

        List<Label> statLabels = new List<Label>();
        List<Label> statValueLabels = new List<Label>();

        List<Label> profileStatLabels = new List<Label>();
        List<Label> profileStatValueLabels = new List<Label>();
        
        for (int i = 0; i < 4; i++)
        {
            statLabels.Add(statLabelBox.Q<Label>($"Stat{i}"));
            statValueLabels.Add(statValueLabelBox.Q<Label>($"StatValue{i}"));
        }

        for (int i = 0; i < 3; i++)
        {
            profileStatLabels.Add(profileStatLabelBox.Q<Label>($"ProfileStat{i}"));
            profileStatValueLabels.Add(profileStatValueLabelBox.Q<Label>($"ProfileStatValue{i}"));
        }

        Debug.Log("Number of profileStatLabels: " + profileStatLabels.Count);

        if (data.stats is TowerData)
        {
            statLabels[0].text = "Damage";
            statLabels[1].text = "Range";
            statLabels[2].text = "Attack Speed";
            statLabels[3].style.display = DisplayStyle.None;

            TowerData stats = data.stats as TowerData;

            statValueLabels[0].text = $"{stats.damage}";
            statValueLabels[1].text = $"{stats.range}";
            statValueLabels[2].text = $"{stats.attackSpeed}";
            statValueLabels[3].style.display = DisplayStyle.None;
        }
        else if (data.stats is TrapData)
        {
            statLabels[0].text = "Damage";
            statLabels[1].text = "Health";
            statLabels[2].text = "Effect Strength";
            statLabels[3].text = "Effect Duration";

            TrapData stats = data.stats as TrapData;

            statValueLabels[0].text = $"{stats.damage}";
            statValueLabels[1].text = $"{stats.health}";
            statValueLabels[2].text = $"{stats.effectStrength}";
            statValueLabels[3].text = $"{stats.effectDuration}";
        }
        else if (data.stats is SpellData)
        {
            statLabels[0].text = "Effect Strength";
            statLabels[1].text = "Effect Duration";
            statLabels[2].style.display = DisplayStyle.None;
            statLabels[3].style.display = DisplayStyle.None;

            SpellData stats = data.stats as SpellData;

            statValueLabels[0].text = $"{stats.effectStrength}";
            statValueLabels[1].text = $"{stats.effectDuration}";
            statValueLabels[2].style.display = DisplayStyle.None;
            statValueLabels[3].style.display = DisplayStyle.None;
        }

        profileStatLabels[0].text = "Owned";
        profileStatLabels[1].text = "Level";
        profileStatLabels[2].text = "In Deck";

        profileStatValueLabels[0].text = $"{data.saveStats.owned}";
        profileStatValueLabels[1].text = $"{data.saveStats.level}";
        profileStatValueLabels[2].text = $"{data.saveStats.inDeck}";

        texture.style.backgroundImage = data.texture;
        name.text = data.name;
        description.text = data.description;
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
        
        towerScriptableObject = saveLoadSystem.towerScriptableObject;
        trapScriptableObjects = new Dictionary<MainGameLogic.CardTypes, TrapScriptableObject>
        {
            {MainGameLogic.CardTypes.BASIC_TRAP, saveLoadSystem.basicTrapScriptableObject},
            {MainGameLogic.CardTypes.ICE_TRAP, saveLoadSystem.iceTrapScriptableObject},
            {MainGameLogic.CardTypes.POISON_TRAP, saveLoadSystem.poisonTrapScriptableObject}
        };

        spellScriptableObjects = new Dictionary<MainGameLogic.CardTypes, SpellScriptableObject>
        {
            {MainGameLogic.CardTypes.ATTACK_SPEED_BUFF, saveLoadSystem.attackSpeedBuffScriptableObject},
            {MainGameLogic.CardTypes.BASE_HEAL, saveLoadSystem.baseHealSciptableObject},
            {MainGameLogic.CardTypes.DAMAGE_BUFF, saveLoadSystem.damageBuffScriptableObject},
            {MainGameLogic.CardTypes.RANGE_BUFF, saveLoadSystem.rangeBuffScriptableObject}
        };

        profileSelector.style.display = DisplayStyle.None;
        profileDeleter.style.display = DisplayStyle.None;
        cardsMenu.style.display = DisplayStyle.None;
        preBattleMenu.style.display = DisplayStyle.Flex;
        
        List<CardData> testList = createCardDataList();

        cardListView = cardsMenu.Q<ListView>("Cards");
        cardListView.fixedItemHeight = Screen.height * 0.1777f;
        cardListView.itemsSource = testList;
        cardListView.makeItem = () => cardRowTemplate.CloneTree();
        cardListView.bindItem = (ve, index) => bindCardElements(ve, cardListView.itemsSource[index] as CardData);
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