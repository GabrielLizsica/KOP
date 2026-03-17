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
        //Set all of the card buttons to my click event with the corresponding value to the button as an argument
        battleUI.cardButtons["card0"].clicked += () => OnButtonClicked("card0");
        battleUI.cardButtons["card1"].clicked += () => OnButtonClicked("card1");
        battleUI.cardButtons["card2"].clicked += () => OnButtonClicked("card2");
        battleUI.cardButtons["card3"].clicked += () => OnButtonClicked("card3");
        battleUI.cardButtons["card4"].clicked += () => OnButtonClicked("card4");
    }
    
    //Creates the deck list
    public void setDeck(List<MainGameLogic.CardTypes> _deck)
    {
        deck = new List<MainGameLogic.CardTypes>(_deck);
        initialize();
    }
    
    //Sets the remaining deck based on the full loaded deck and draws the 5 initial cards at the start of the game
    private void initialize()
    {
        setRemainingDeck();
        drawInitialCards();
    }
    
    //Sets the remaining cards to the full loaded deck
    private void setRemainingDeck()
    {
        remainingDeck = new List<MainGameLogic.CardTypes>(deck);
    }
    
    //Draws the 5 initial cards at the start of the game
    private void drawInitialCards()
    {
        for (int i = 0; i < 5; i++)
        {
            drawCard(i);
        }
    }
    
    //Draws a new card and handles the changes coming with it in the remaining cards list
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
    
    //When a card is selected, it calls the corresponding function in the BuildingHandler
    private void selectCard(int index)
    {
        if (selectedIndex != -1)
        {
            cardHandler.finishBuilding();
        }
        
        cardHandler.cardSelected(hand[index]);
        selectedIndex = index;
    }
    
    //When a card is used calls the corresponding function in the BUildingHandler and handles changes related to the use of the card
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
    
    //Clears the selected card from memory
    public void cancelCast(InputAction.CallbackContext context)
    {
        if (context.performed && selectedIndex != -1)
        {
            cardHandler.finishBuilding();
            selectedIndex = -1;
        }
    }

    //When a card button is clicked, calls the selectCard function with the correct button ID
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
