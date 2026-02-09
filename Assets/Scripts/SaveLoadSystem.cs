using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.IO;

public class SaveLoadSystem : MonoBehaviour
{
    [SerializeField] private PlayerProfleScriptableObject playerProfileScriptableObject;

    public Profile playerProfile;
    private MainMenuHandler.Profiles currentProfile;
    string savePath;
    
    public class Card
    {
        public int level;
        public int deck;
        public int owned;
    }

    public class ProfileRaw
    {
        public int gold;
        public Dictionary<string, Card> cards;
    }


    public class Profile
    {
        public int gold;
        public Dictionary<MainGameLogic.CardTypes, Card> cards;

        public Profile(ProfileRaw rawData)
        {
            gold = rawData.gold;
            cards = new Dictionary<MainGameLogic.CardTypes, Card>();

            foreach (var pair in rawData.cards)
            {
                if (Enum.TryParse(pair.Key, out MainGameLogic.CardTypes cardType))
                {
                    cards.Add(cardType, pair.Value);
                    Debug.Log($"Successfully parsed card into: {pair.Key}, {pair.Value}");
                }
                else
                {
                    Debug.LogError($"Unknown cardType in Json: {pair.Key}");
                }
            }
        }

        public Profile() { }
    }
    
    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "Saves");
        Debug.Log(savePath);

        #if SIMULATE_FIRST_RUN
            if (Directory.Exists(savePath))
                Directory.Delete(savePath, true);
        #endif
        
        if (!Directory.Exists(savePath))
        {
            Profile skeletonProfile = new Profile(JsonConvert.DeserializeObject<ProfileRaw>(Resources.Load<TextAsset>("TextAssets/SkeletonProfile").text));
            
            Directory.CreateDirectory(savePath);
            
            for (int i = 0; i < Enum.GetValues(typeof(MainMenuHandler.Profiles)).Length; i++)
            {
                string saveFilePath = Path.Combine(savePath, $"Profile{i}.json");
                File.WriteAllText(saveFilePath, Resources.Load<TextAsset>("TextAssets/SkeletonProfile").text);
            }
        }
    }

    public void loadProfile(MainMenuHandler.Profiles profile)
    {
        currentProfile = profile;
        ProfileRaw rawProfileData = JsonConvert.DeserializeObject<ProfileRaw>(File.ReadAllText(savePath + $"/Profile{(int)currentProfile}.json"));
        
        playerProfile = new Profile(rawProfileData);
        playerProfileScriptableObject.gold = playerProfile.gold;
        playerProfileScriptableObject.cards = new Dictionary<MainGameLogic.CardTypes, Card>(playerProfile.cards);
    }
    
    public void saveProfile()
    {
        Profile saveData = new Profile();
        saveData.gold = playerProfileScriptableObject.gold;
        saveData.cards = new Dictionary<MainGameLogic.CardTypes, Card>(playerProfileScriptableObject.cards);
        
        string saveJson = JsonConvert.SerializeObject(saveData);
        string filePath = Path.Combine(savePath, $"Profile{(int)currentProfile}.json");
        File.WriteAllText(filePath, saveJson);
    }
    
    public void resetProfile(MainMenuHandler.Profiles profile)
    {
        string saveFilePath = Path.Combine(savePath, $"Profile{(int)profile}.json");
        File.WriteAllText(saveFilePath, Resources.Load<TextAsset>("TextAssets/SkeletonProfile").text);
    }
}
