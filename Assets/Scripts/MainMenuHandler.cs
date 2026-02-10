using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using NUnit.Framework;

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
    private VisualElement mainMenu;
    private VisualElement resetConfirmMenu;
    private Label resetConfirmLabel;

    private SceneHandler sceneHandler; 
    private SaveLoadSystem saveLoadSystem;

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

    

    private void Start()
    {
        mainMenu = GetComponent<UIDocument>().rootVisualElement;
        sceneHandler = sceneHandlerObject.GetComponent<SceneHandler>();
        saveLoadSystem = sceneHandlerObject.GetComponent<SaveLoadSystem>();

        profileSelector = mainMenu.Q<VisualElement>("ProfileSelector");
        profileDeleter = mainMenu.Q<VisualElement>("ProfileDeleter");
        preBattleMenu = mainMenu.Q<VisualElement>("PreBattleMenu");
        resetConfirmMenu = mainMenu.Q<VisualElement>("ProfileResetConfirm");
        quitButton = profileSelector.Q<Button>("QuitButton");
        quitButton.clicked += Application.Quit;

        resetConfirmLabel = resetConfirmMenu.Q<Label>("ConfirmLabel");
        
        preBattleMenu.style.display = DisplayStyle.None;
        resetConfirmMenu.style.display = DisplayStyle.None;

        setConfirmButtons();
        setProfileButtons();
        setProfileDeleterButtons();
        setPreBattleMenuButtons();

        setConfirmButtonEvents();
        setProfileButtonEvents();
        setProfileDeleterButtonEvents();
        setPreBattleMenuButtonEvents();

        Debug.Log(Application.persistentDataPath);
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
    
    private void toggleProfileMenu(bool enabled)
    {
        SetInteractableRecursive(profileSelector, enabled);
        SetInteractableRecursive(profileDeleter, enabled);
    }
    
    private void SetInteractableRecursive(VisualElement root, bool enabled)
    {
        root.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
        root.style.opacity = enabled ? 1f : 0.5f;

        foreach (var child in root.Children())
        {
            SetInteractableRecursive(child, enabled);
        }
    }
}
