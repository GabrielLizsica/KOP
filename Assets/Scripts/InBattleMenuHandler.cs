using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class InBattleMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject mainGameObject;
    private DeckHandler deckHandler;
    private VisualElement battleUI;
    private VisualElement handUI;
    public Dictionary<string, Button> cardButtons = new Dictionary<string, Button>
    {
        {"card0", null},
        {"card1", null},
        {"card2", null},
        {"card3", null},
        {"card4", null}
    };

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

    private void Start()
    {
        deckHandler = mainGameObject.GetComponent<DeckHandler>();

        battleUI = GetComponent<UIDocument>().rootVisualElement;
        handUI = battleUI.Q<VisualElement>("hand");

        cardButtons["card0"] = handUI.Q<Button>("card0");
        cardButtons["card1"] = handUI.Q<Button>("card1");
        cardButtons["card2"] = handUI.Q<Button>("card2");
        cardButtons["card3"] = handUI.Q<Button>("card3");
        cardButtons["card4"] = handUI.Q<Button>("card4");

        deckHandler.OnCardDraw += Deck_OnCardDraw;

        cardTextures[MainGameLogic.CardTypes.TOWER] = Resources.Load<Texture2D>("Cards/CardTower");
        cardTextures[MainGameLogic.CardTypes.BASIC_TRAP] = Resources.Load<Texture2D>("Cards/CardTrapBasic");
        cardTextures[MainGameLogic.CardTypes.ICE_TRAP] = Resources.Load<Texture2D>("Cards/CardTrapIce");
        cardTextures[MainGameLogic.CardTypes.POISON_TRAP] = Resources.Load<Texture2D>("Cards/CardTrapPoison");
        cardTextures[MainGameLogic.CardTypes.ATTACK_SPEED_BUFF] = null;
        cardTextures[MainGameLogic.CardTypes.DAMAGE_BUFF] = Resources.Load<Texture2D>("Cards/CardDamageBuff");
        cardTextures[MainGameLogic.CardTypes.RANGE_BUFF] = Resources.Load<Texture2D>("Cards/CardRangeBuff");
        cardTextures[MainGameLogic.CardTypes.BASE_HEAL] = Resources.Load<Texture2D>("Cards/CardBaseHeal");
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
}
