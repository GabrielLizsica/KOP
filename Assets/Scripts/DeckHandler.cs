using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeckHandler : MonoBehaviour
{
    [SerializeField] private GameObject battleUIObject;
    private InBattleMenuHandler battleUI;

    private MainGameLogic mainGameLogic;
    private BuildingHandler cardHandler;
    private List<MainGameLogic.CardTypes> deck;
    private List<MainGameLogic.CardTypes> remainingDeck;
    private List<MainGameLogic.CardTypes> hand  = new List<MainGameLogic.CardTypes>( new MainGameLogic.CardTypes[5] );
    private int selectedIndex = -1;

    public event EventHandler<OnCardDrawEventArgs> OnCardDraw;
    public class OnCardDrawEventArgs : EventArgs
    {
        public int cardID;
        public MainGameLogic.CardTypes cardType;
    }

    private void Start()
    {
        mainGameLogic = GetComponent<MainGameLogic>();
        cardHandler = GetComponent<BuildingHandler>();
        battleUI = battleUIObject.GetComponent<InBattleMenuHandler>();

        battleUI.cardButtons["card0"].clicked += () => OnButtonClicked("card0");
        battleUI.cardButtons["card1"].clicked += () => OnButtonClicked("card1");
        battleUI.cardButtons["card2"].clicked += () => OnButtonClicked("card2");
        battleUI.cardButtons["card3"].clicked += () => OnButtonClicked("card3");
        battleUI.cardButtons["card4"].clicked += () => OnButtonClicked("card4");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectCard(0);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectCard(1);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectCard(2);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectCard(3);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectCard(4);
        }
    }
    
    public void setDeck(List<MainGameLogic.CardTypes> _deck)
    {
        deck = _deck;
        initialize();
    }
    
    private void initialize()
    {
        setRemainingDeck();
        drawInitialCards();
    }
    
    private void setRemainingDeck()
    {
        remainingDeck = deck;
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
        remainingDeck.RemoveAt(cardIndex);

        OnCardDraw?.Invoke(this, new OnCardDrawEventArgs {cardID = index, cardType = hand[index]});

        Debug.Log("The " + index + " card is: " + hand[index]);
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
            selectedIndex = -1;
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
