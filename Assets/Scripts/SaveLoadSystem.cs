using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SaveLoadSystem : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] public PlayerProfleScriptableObject playerProfileScriptableObject;
    [SerializeField] public  TowerScriptableObject towerScriptableObject;
    [SerializeField] public  TrapScriptableObject basicTrapScriptableObject;
    [SerializeField] public  TrapScriptableObject iceTrapScriptableObject;
    [SerializeField] public  TrapScriptableObject poisonTrapScriptableObject;
    [SerializeField] public  SpellScriptableObject attackSpeedBuffScriptableObject;
    [SerializeField] public  SpellScriptableObject rangeBuffScriptableObject;
    [SerializeField] public  SpellScriptableObject damageBuffScriptableObject;
    [SerializeField] public  SpellScriptableObject baseHealSciptableObject;
    [SerializeField] public  CardTexturesScriptableObject cardTexturesScriptableObject;
    
    [SerializeField] private EnemyScriptableObject enemyScriptableObject;
    
    private Dictionary<MainGameLogic.CardTypes, Texture2D> cardTextures = new Dictionary<MainGameLogic.CardTypes, Texture2D>
    {
        {MainGameLogic.CardTypes.TOWER, null},
        {MainGameLogic.CardTypes.BASIC_TRAP, null},
        {MainGameLogic.CardTypes.ICE_TRAP, null},
        {MainGameLogic.CardTypes.POISON_TRAP, null},
        {MainGameLogic.CardTypes.ATTACK_SPEED_BUFF, null},
        {MainGameLogic.CardTypes.DAMAGE_BUFF, null},
        {MainGameLogic.CardTypes.RANGE_BUFF, null},
        {MainGameLogic.CardTypes.BASE_HEAL, null}
    };

    public Profile playerProfile;
    private MainMenuHandler.Profiles currentProfile;
    string savePath;
    
    public class EnemyData
    {
        public int speed;
        public int health;
        public int weakness;
    }
    
    public class TowerData
    {
        public Dictionary<string, string> title;
        public Dictionary<string, string> description;
        public Dictionary<string, TowerStats> stats;
    }
    
    public class TrapData
    {
        public Dictionary<string, string> title;
        public Dictionary<string, string> description;
        public Dictionary<string, TrapStats> stats;
    }
    
    public class SpellData
    {
        public Dictionary<string, string> title;
        public Dictionary<string, string> description;
        public Dictionary<string, SpellStats> stats;
    }
    
    public class TowerStats
    {
        public int damage;
        public int range;
        public float attackspeed;
    }
    
    public class TrapStats
    {
        public float damage;
        public int health;
        public float effectstrength;
        public float effectduration;
    }
    
    public class SpellStats
    {
        public float effectstrength;
        public float effectduration;
    }
    
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
        
        updateScriptableObjectData();
        loadCardTextures();
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
    
    private void updateScriptableObjectData()
    {
        Dictionary<MainGameLogic.CardTypes, TextAsset> cardTextAssets = loadCardTextAssets();
    
        TowerData towerData = JsonConvert.DeserializeObject<TowerData>(cardTextAssets[MainGameLogic.CardTypes.TOWER].text);
        TrapData basicTrapData = JsonConvert.DeserializeObject<TrapData>(cardTextAssets[MainGameLogic.CardTypes.BASIC_TRAP].text);
        TrapData iceTrapData = JsonConvert.DeserializeObject<TrapData>(cardTextAssets[MainGameLogic.CardTypes.ICE_TRAP].text);
        TrapData poisonTrapData = JsonConvert.DeserializeObject<TrapData>(cardTextAssets[MainGameLogic.CardTypes.POISON_TRAP].text);
        SpellData baseHealSpellData = JsonConvert.DeserializeObject<SpellData>(cardTextAssets[MainGameLogic.CardTypes.BASE_HEAL].text);
        SpellData rangeBuffSpellData = JsonConvert.DeserializeObject<SpellData>(cardTextAssets[MainGameLogic.CardTypes.RANGE_BUFF].text);
        SpellData damageBuffSpellData = JsonConvert.DeserializeObject<SpellData>(cardTextAssets[MainGameLogic.CardTypes.DAMAGE_BUFF].text);
        SpellData attackSpeedBuffSpellData = JsonConvert.DeserializeObject<SpellData>(cardTextAssets[MainGameLogic.CardTypes.ATTACK_SPEED_BUFF].text);

        TextAsset enemyJson = Resources.Load<TextAsset>("TextAssets/Enemy");
        EnemyData enemyData = JsonConvert.DeserializeObject<EnemyData>(enemyJson.text);
        
        int towerLevel = playerProfileScriptableObject.cards[MainGameLogic.CardTypes.TOWER].level;
        TowerStats towerStats = towerData.stats[$"level{towerLevel}"];
        towerScriptableObject.Init(towerData, towerLevel);

        int basicTrapLevel = playerProfileScriptableObject.cards[MainGameLogic.CardTypes.BASIC_TRAP].level;
        TrapStats basicTrapStats = basicTrapData.stats[$"level{basicTrapLevel}"];
        basicTrapScriptableObject.Init(basicTrapData, basicTrapLevel);
        
        int iceTrapLevel = playerProfileScriptableObject.cards[MainGameLogic.CardTypes.ICE_TRAP].level;
        TrapStats iceTrapStats = iceTrapData.stats[$"level{iceTrapLevel}"];
        iceTrapScriptableObject.Init(iceTrapData, iceTrapLevel);

        int poisonTrapLevel = playerProfileScriptableObject.cards[MainGameLogic.CardTypes.POISON_TRAP].level;
        TrapStats poisonTrapStats = poisonTrapData.stats[$"level{poisonTrapLevel}"];
        poisonTrapScriptableObject.Init(poisonTrapData, poisonTrapLevel);
        
        int attackSpeedBuffLevel = playerProfileScriptableObject.cards[MainGameLogic.CardTypes.ATTACK_SPEED_BUFF].level;
        SpellStats attackSpeedBuffStats = attackSpeedBuffSpellData.stats[$"level{attackSpeedBuffLevel}"];
        attackSpeedBuffScriptableObject.Init(attackSpeedBuffSpellData, attackSpeedBuffLevel);

        int rangeBuffLevel = playerProfileScriptableObject.cards[MainGameLogic.CardTypes.RANGE_BUFF].level;
        SpellStats rangeBuffStats = rangeBuffSpellData.stats[$"level{rangeBuffLevel}"];
        rangeBuffScriptableObject.Init(rangeBuffSpellData, rangeBuffLevel);

        int damageBuffLevel = playerProfileScriptableObject.cards[MainGameLogic.CardTypes.DAMAGE_BUFF].level;
        SpellStats damageBuffStats = damageBuffSpellData.stats[$"level{damageBuffLevel}"];
        damageBuffScriptableObject.Init(damageBuffSpellData, damageBuffLevel);
        
        Debug.Log(damageBuffScriptableObject.effectstrength);

        int baseHealLevel = playerProfileScriptableObject.cards[MainGameLogic.CardTypes.BASE_HEAL].level;
        SpellStats baseHealStats = baseHealSpellData.stats[$"level{baseHealLevel}"];
        baseHealSciptableObject.Init(baseHealSpellData, baseHealLevel);

        enemyScriptableObject.speed = enemyData.speed;
        enemyScriptableObject.health = enemyData.health;
        enemyScriptableObject.weakness = enemyData.weakness;
    }
    
    private Dictionary<MainGameLogic.CardTypes, TextAsset> loadCardTextAssets()
    {
        Dictionary<MainGameLogic.CardTypes, TextAsset> cardTextAssets = new Dictionary<MainGameLogic.CardTypes, TextAsset>();

        cardTextAssets.Add(MainGameLogic.CardTypes.TOWER, Resources.Load<TextAsset>("TextAssets/Tower"));
        cardTextAssets.Add(MainGameLogic.CardTypes.BASIC_TRAP, Resources.Load<TextAsset>("TextAssets/BasicTrap"));
        cardTextAssets.Add(MainGameLogic.CardTypes.ICE_TRAP, Resources.Load<TextAsset>("TextAssets/IceTrap"));
        cardTextAssets.Add(MainGameLogic.CardTypes.POISON_TRAP, Resources.Load<TextAsset>("TextAssets/PoisonTrap"));
        cardTextAssets.Add(MainGameLogic.CardTypes.BASE_HEAL, Resources.Load<TextAsset>("TextAssets/BaseHealSpell"));
        cardTextAssets.Add(MainGameLogic.CardTypes.RANGE_BUFF, Resources.Load<TextAsset>("TextAssets/RangeBuffSpell"));
        cardTextAssets.Add(MainGameLogic.CardTypes.DAMAGE_BUFF, Resources.Load<TextAsset>("TextAssets/DamageBuffSpell"));
        cardTextAssets.Add(MainGameLogic.CardTypes.ATTACK_SPEED_BUFF, Resources.Load<TextAsset>("TextAssets/AttackSpeedBuffSpell"));

        return cardTextAssets;
    }
    
    private void loadCardTextures()
    {
        cardTextures[MainGameLogic.CardTypes.TOWER] = Resources.Load<Texture2D>("Cards/CardTower");
        cardTextures[MainGameLogic.CardTypes.BASIC_TRAP] = Resources.Load<Texture2D>("Cards/CardTrapBasic");
        cardTextures[MainGameLogic.CardTypes.ICE_TRAP] = Resources.Load<Texture2D>("Cards/CardTrapIce");
        cardTextures[MainGameLogic.CardTypes.POISON_TRAP] = Resources.Load<Texture2D>("Cards/CardTrapPoison");
        cardTextures[MainGameLogic.CardTypes.ATTACK_SPEED_BUFF] = null;
        cardTextures[MainGameLogic.CardTypes.DAMAGE_BUFF] = Resources.Load<Texture2D>("Cards/CardDamageBuff");
        cardTextures[MainGameLogic.CardTypes.RANGE_BUFF] = Resources.Load<Texture2D>("Cards/CardRangeBuff");
        cardTextures[MainGameLogic.CardTypes.BASE_HEAL] = Resources.Load<Texture2D>("Cards/CardBaseHeal");

        cardTexturesScriptableObject.textures = new Dictionary<MainGameLogic.CardTypes, Texture2D>(cardTextures);
    }

    public void buyCard(MainGameLogic.CardTypes card)
    {
        playerProfileScriptableObject.cards[card].owned++;
    }

    public void upgradeCard(MainGameLogic.CardTypes card)
    {
        playerProfileScriptableObject.cards[card].level++;
        updateScriptableObjectData();
    }

    public void addCardToDeck(MainGameLogic.CardTypes card)
    {
        playerProfileScriptableObject.cards[card].deck++;
        updateScriptableObjectData();
    }

    public void removeCardFromDeck(MainGameLogic.CardTypes card)
    {
        playerProfileScriptableObject.cards[card].deck--;
        updateScriptableObjectData();
    }
}
