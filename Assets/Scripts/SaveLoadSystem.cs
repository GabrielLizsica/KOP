using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

public class SaveLoadSystem : MonoBehaviour
{
    [SerializeField] private PlayerProfleScriptableObject playerProfileScriptableObject;

    public Profile playerProfile;
    
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
    }

    public void loadProfile(MainMenuHandler.Profiles profile)
    {
        TextAsset profileJson = Resources.Load<TextAsset>($"TextAssets/Profile{(int)profile}");

        ProfileRaw rawProfileData = JsonConvert.DeserializeObject<ProfileRaw>(profileJson.text);
        playerProfile = new Profile(rawProfileData);
        playerProfileScriptableObject.gold = playerProfile.gold;
        playerProfileScriptableObject.cards = new Dictionary<MainGameLogic.CardTypes, Card>(playerProfile.cards);
    }
}
