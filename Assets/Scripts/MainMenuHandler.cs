using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEditor.Tilemaps;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject sceneHandlerObject;

    private Dictionary<Profiles, Button> profileButtons = new Dictionary<Profiles, Button>();
    private Dictionary<Profiles, Button> profileDeleteButtons = new Dictionary<Profiles, Button>();
    private Dictionary<PreGameButtons, Button> preBattleButtons = new Dictionary<PreGameButtons, Button>();
    private VisualElement profileSelector;
    private VisualElement profileDeleter;
    private VisualElement preBattleMenu;
    private VisualElement mainMenu;

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

    

    private void Start()
    {
        mainMenu = GetComponent<UIDocument>().rootVisualElement;
        sceneHandler = sceneHandlerObject.GetComponent<SceneHandler>();
        saveLoadSystem = sceneHandlerObject.GetComponent<SaveLoadSystem>();

        profileSelector = mainMenu.Q<VisualElement>("ProfileSelector");
        profileDeleter = mainMenu.Q<VisualElement>("ProfileDeleter");
        preBattleMenu = mainMenu.Q<VisualElement>("PreBattleMenu");
        preBattleMenu.style.display = DisplayStyle.None;

        setProfileButtons();
        setProfileDeleterButtons();
        setPreBattleMenuButtons();

        setProfileButtonEvents();
        setPreBattleMenuButtonEvents();
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
    }
    
    private void OnStartButtonClicked()
    {
        sceneHandler.changeScene();
    }

    private void OnProfileButtonClicked(Profiles profile)
    {
        saveLoadSystem.loadProfile(profile);
        Debug.Log("Loading profile: " + profile);
        profileSelector.style.display = DisplayStyle.None;
        profileDeleter.style.display = DisplayStyle.None;
        preBattleMenu.style.display = DisplayStyle.Flex;
    }
}
