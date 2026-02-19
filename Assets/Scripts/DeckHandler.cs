using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DeckHandler : MonoBehaviour
{
    [SerializeField] private GameObject battleUIObject;
    private InBattleMenuHandler battleUI;

    private MainGameLogic mainGameLogic;
    private BuildingHandler cardHandler;
    private List<MainGameLogic.CardTypes> deck;
    private List<MainGameLogic.CardTypes> remainingDeck;
    private List<MainGameLogic.CardTypes> hand  = new List<MainGameLogic.CardTypes>( new MainGameLogic.CardTypes[5] );
    private Dictionary<UnityEngine.UIElements.Button, Action> buttonActions = new Dictionary<UnityEngine.UIElements.Button, Action>();
    private int selectedIndex = -1;

    public event EventHandler<OnCardDrawEventArgs> OnCardDraw;
    public class OnCardDrawEventArgs : EventArgs
    {
        public int cardID;
        public MainGameLogic.CardTypes cardType;
    }

    private void Awake()
    {
        mainGameLogic = GetComponent<MainGameLogic>();
        cardHandler = GetComponent<BuildingHandler>();
        battleUI = battleUIObject.GetComponent<InBattleMenuHandler>();
    }

    private void Start()
    {
        battleUI.cardButtons["card0"].clicked += () => OnButtonClicked("card0");
        battleUI.cardButtons["card1"].clicked += () => OnButtonClicked("card1");
        battleUI.cardButtons["card2"].clicked += () => OnButtonClicked("card2");
        battleUI.cardButtons["card3"].clicked += () => OnButtonClicked("card3");
        battleUI.cardButtons["card4"].clicked += () => OnButtonClicked("card4");
    }
    
    public void setDeck(List<MainGameLogic.CardTypes> _deck)
    {
        deck = new List<MainGameLogic.CardTypes>(_deck);
        initialize();
    }
    
    private void initialize()
    {
        setRemainingDeck();
        drawInitialCards();
    }
    
    private void setRemainingDeck()
    {
        remainingDeck = new List<MainGameLogic.CardTypes>(deck);
    }
    
    private void drawInitialCards()
    {
        for (int i = 0; i < 5; i++)
        {
            drawCard(i);
        }
    }
    
    private void drawCard(int index)
    {   
        if (remainingDeck.Count == 0)
        {
            setRemainingDeck();
        }

        int cardIndex = UnityEngine.Random.Range(0, remainingDeck.Count);
        hand[index] = remainingDeck[cardIndex];

        OnCardDraw?.Invoke(this, new OnCardDrawEventArgs {cardID = index, cardType = hand[index]});
        remainingDeck.RemoveAt(cardIndex);
    }
    
    private void selectCard(int index)
    {
        if (selectedIndex != -1)
        {
            cardHandler.finishBuilding();
        }
        
        cardHandler.cardSelected(hand[index]);
        selectedIndex = index;
    }
    
    public void castCard(InputAction.CallbackContext context)
    {
        if (context.performed && selectedIndex != -1)
        {
            cardHandler.placeNewBuilding();
            drawCard(selectedIndex);
            mainGameLogic.currentEnergy -= (int)battleUI.cardButtons[$"card{selectedIndex}"].userData;
            selectedIndex = -1;
            battleUI.updateLabel(InBattleMenuHandler.displayLabels.ENERGY);
        }
    }
    
    public void cancelCast(InputAction.CallbackContext context)
    {
        if (context.performed && selectedIndex != -1)
        {
            cardHandler.finishBuilding();
            selectedIndex = -1;
        }
    }

    private void OnButtonClicked(string buttonID)
    {
        if ((int)battleUI.cardButtons[buttonID].userData > mainGameLogic.currentEnergy)
        {
            Debug.Log("Not enough energy!");
            StartCoroutine(battleUI.displayEnergyWarning());
        }
        else
        {
            switch (buttonID)
            {
                case "card0":
                    selectCard(0);
                    break;
                
                case "card1":
                    selectCard(1);
                    break;
                
                case "card2":
                    selectCard(2);
                    break;
                
                case "card3":
                    selectCard(3);
                    break;
                
                case "card4":
                    selectCard(4);
                    break;
            }
        }
    }
}
