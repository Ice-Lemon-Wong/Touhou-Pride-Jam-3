using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CardsManager : MonoBehaviour
{
	
	public static CardsManager instanceCM;
    //singleton pattern
    private void Awake()
    {
        if (instanceCM == null)
        {
            instanceCM = this;
        }

        if (instanceCM != this)
        {
            Destroy(gameObject);
        }

    }

    [Header("board layout config")]
    [SerializeField] private Vector2 xBounds;
    [SerializeField] private Vector2 yBounds;
    [SerializeField] private Vector2Int cardLayout;
    [SerializeField] private GameObject cardobject;
    [SerializeField] private GameObject blockingPanel;

    [Space]
    [Header("card game config")]
    [SerializeField] private bool InitAfterCreate;
    [SerializeField] private int  cardsRequiredPerMatch = 2;
    [SerializeField] private float cardFlipDelay = 0.75f;
    [SerializeField] private Vector2 cardHidingOffset;
    [SerializeField] private float transitionTime = 0.75f;
    [SerializeField] private int turnAmmount = 8;
    //[SerializeField] private bool createOnStart = false;

    [Space]
    [Header("card distribution config")]
    [SerializeField] CardSO[] cardSOPool;
    [SerializeField] CardDistributor cardDistributor;

    [Space]
    [Header("debug text stuff")]
    [SerializeField] TextMeshProUGUI debugStateText;
    [SerializeField] TextMeshProUGUI turnText;


    private int[] matchedCardsID;
    private int currentMatchIndex = 0;
    private float currentTransitionTime = 0;
    private bool isPlayerTurn = true;
    private CardPlayerAI playerAI;
    private int currentTurn;
	public Action cardGameEndEvent;
    private bool isGameReady = false;
    public Action<string, int> matchingEvent;
    public int matchedCardIndex= 0;


	public enum BoardState { playerTurn, enemyTurn, boardProcess};
    public BoardState currentBoardState;

    public struct CardOnBoard{
        public int ID;
        public Vector2 boardPosition;
        public GameObject cardObject;
        public CardSO cardData;
        public bool isFlipped;
        public bool isMatched;
        //store the current times it appear
        public int currentTimes;

        public CardOnBoard(int ID, Vector2 boardPosition, GameObject cardObject, CardSO cardData, int currentTimes) {
            this.ID = ID;
            this.boardPosition = boardPosition;
            this.cardObject = cardObject;
            this.isFlipped = false;
            this.isMatched = false;
            this.cardData = cardData;
            this.currentTimes = currentTimes;
        }

        public void Matched(bool matched = true) {
            isMatched = matched;
            cardObject.GetComponent<CardScript>().HideCard();
        }

        public void DestroyCard()
        {
            isMatched = true;
            Destroy(cardObject);
            
        }

    }
    public CardOnBoard[] BoadCards;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        BoadCards = new CardOnBoard[cardLayout.x * cardLayout.y];
        matchedCardsID = new int[cardsRequiredPerMatch];
        currentMatchIndex = 0;
        currentBoardState = BoardState.boardProcess;

        blockingPanel.SetActive(false);

        yield return null;
        //create cards

        //if (createOnStart) {
        //    CreatBoard();
        //}
        


    }

    //out dated function, do not use
    public void CreatBoard()
    {
        DestroyBoard();

        

        float cardX = 0;
        float cardY = 0;
        float width = xBounds.y - xBounds.x;
        float height = yBounds.y - yBounds.x;
        GameObject cardObject;

        BoadCards = new CardOnBoard[cardLayout.x * cardLayout.y];
        matchedCardsID = new int[cardsRequiredPerMatch];
        currentMatchIndex = 0;
        currentBoardState = BoardState.boardProcess;

        List<CardDistributor. DistricutedCard> decidedCards = new List<CardDistributor.DistricutedCard>();
        decidedCards = cardDistributor.DistributeCards((cardLayout.x * cardLayout.y) / cardsRequiredPerMatch);
        CardDistributor.DistricutedCard selectedCardData = new CardDistributor.DistricutedCard();
        List<CardDistributor.DistricutedCard> initialCardDeck = new List<CardDistributor.DistricutedCard>();
        int selectedIndex = 0;

        //create the appropriate ammount of cards
        for (int i = 0; i < cardLayout.x * cardLayout.y; i++)
        {
            if (i % cardsRequiredPerMatch == 0)
            {
                selectedIndex = UnityEngine.Random.Range(0, decidedCards.Count);
                selectedCardData = decidedCards[selectedIndex];
                decidedCards.RemoveAt(selectedIndex);

            }
            initialCardDeck.Add(selectedCardData);

        }
        
        for (int i = 0; i < cardLayout.x * cardLayout.y; i++)
        {

            //randomize cards
            selectedIndex = UnityEngine.Random.Range(0, initialCardDeck.Count);
            selectedCardData = initialCardDeck[selectedIndex];
            initialCardDeck.RemoveAt(selectedIndex);


            //selectedCardData = cardSOPool[Random.Range(0, cardSOPool.Length)];

            //figure out where to spawn card object
            cardX = ((i % cardLayout.x) * (width / (cardLayout.x - 1))) + xBounds.x;
            cardY = ((i / cardLayout.x) * (height / (cardLayout.y - 1))) + yBounds.x;

            //spawn card object
            cardObject = Instantiate(cardobject, new Vector2(cardX, cardY) + cardHidingOffset, Quaternion.identity);
            cardObject.GetComponent<CardScript>().InitCard(i, selectedCardData.cardData.icon, new Vector2(cardX, cardY), new Vector2(cardX, cardY) + cardHidingOffset);
            cardObject.name = selectedCardData.cardData.name + "-" + (i % cardsRequiredPerMatch);

            //add the card data structure ot the board
            BoadCards[i] = new CardOnBoard(i, new Vector2(cardX, cardY), cardObject, selectedCardData.cardData, selectedCardData.currentTimes);
            BoadCards[i].cardObject.GetComponent<CardScript>().SetPosition();
            Debug.Log(selectedCardData.currentTimes);

        }

        playerAI = FindObjectOfType<CardPlayerAI>();
        playerAI.InitKnowledgeBase(cardLayout.x * cardLayout.y, turnAmmount);
        currentTurn = turnAmmount;
        isGameReady = true;

        if (InitAfterCreate) InitBoardCards(true);
    }

    public void CreatBoard(Vector2 newXBounds, Vector2 newYBounds, Vector2Int newCardlayout,int turnAmount, bool initImmedaintly = false, bool isCheat = false, bool isClosing = false, bool isFinal = false, bool isAct4 = false)
    {
        DestroyBoard();

        

        xBounds = newXBounds;
        yBounds = newYBounds;
        cardLayout = newCardlayout;
        turnAmmount = turnAmount;

        float cardX = 0;
        float cardY = 0;
        float width = xBounds.y - xBounds.x;
        float height = yBounds.y - yBounds.x;
        GameObject cardObject;

        BoadCards = new CardOnBoard[cardLayout.x * cardLayout.y];
        matchedCardsID = new int[cardsRequiredPerMatch];
        //currentMatchIndex = 0;
        //currentBoardState = BoardState.boardProcess;

        //grab different cards later
        List<CardDistributor.DistricutedCard> decidedCards = new List<CardDistributor.DistricutedCard>();

        if (isClosing)
        {
            decidedCards = cardDistributor.DistributeClosingCards();
        }
        else if (isFinal)
        {
            decidedCards = cardDistributor.DistributeFinalCards();
        }
        else {
            decidedCards = cardDistributor.DistributeCards((cardLayout.x * cardLayout.y) / cardsRequiredPerMatch, isAct4);
        }
        

        CardDistributor.DistricutedCard selectedCardData = new CardDistributor.DistricutedCard();
        List<CardDistributor.DistricutedCard> initialDeck = new List<CardDistributor.DistricutedCard>();
        int selectedIndex = 0;

        //create the appropriate ammount of cards
        for (int i = 0; i < cardLayout.x * cardLayout.y; i++)
        {
            if (i % cardsRequiredPerMatch == 0)
            {
                selectedIndex = UnityEngine.Random.Range(0, decidedCards.Count);
                selectedCardData = decidedCards[selectedIndex];
                decidedCards.RemoveAt(selectedIndex);

            }
            initialDeck.Add(selectedCardData);

        }


        Debug.Log(BoadCards.Length);
        for (int i = 0; i < cardLayout.x * cardLayout.y; i++)
        {

            //randomize cards
            //shuffle the deck
            selectedIndex = UnityEngine.Random.Range(0, initialDeck.Count);
            selectedCardData = initialDeck[selectedIndex];
            initialDeck.RemoveAt(selectedIndex);


            //selectedCardData = cardSOPool[Random.Range(0, cardSOPool.Length)];

            //figure out where to spawn card object
            if (cardLayout.x == 1)
            {
                cardX = ((i % cardLayout.x) * (width / 2)) + xBounds.x;
            }
            else {
                cardX = ((i % cardLayout.x) * (width / (cardLayout.x - 1))) + xBounds.x;
            }


            if (cardLayout.y == 1)
            {
                cardY = ((i / cardLayout.x) * (height /2)) + yBounds.x;
            }
            else
            {
                cardY = ((i / cardLayout.x) * (height / (cardLayout.y - 1))) + yBounds.x;
            }


           

            //spawn card object
            cardObject = Instantiate(cardobject, new Vector2(cardX, cardY) + cardHidingOffset, Quaternion.identity);
            cardObject.GetComponent<CardScript>().InitCard(i, selectedCardData.cardData.icon, new Vector2(cardX, cardY), new Vector2(cardX, cardY) + cardHidingOffset);
            cardObject.name = selectedCardData.cardData.name + "-" + (i % cardsRequiredPerMatch);
            BoadCards[i] = new CardOnBoard(i, new Vector2(cardX, cardY), cardObject, selectedCardData.cardData, selectedCardData.currentTimes);
            BoadCards[i].cardObject.GetComponent<CardScript>().SetPosition();

        }

        Debug.Log("created card");

        playerAI = FindObjectOfType<CardPlayerAI>();
        playerAI.InitKnowledgeBase(cardLayout.x * cardLayout.y, turnAmmount, isCheat);
        currentTurn = turnAmmount;
        isGameReady = true;

        Debug.Log(initImmedaintly);
        if (initImmedaintly) InitBoardCards(true);
    }

    public void DestroyBoard() {
        currentBoardState = BoardState.boardProcess;

        for (int i = 0; i < BoadCards.Length; i++)
        {
            BoadCards[i].DestroyCard();
        }
        cardGameEndEvent = null;
        matchingEvent = null;
        isGameReady = false;
    }

    //start the card game
    public void InitBoardCards(bool refreshBoard = false)
    {
        if (!isGameReady) return;
        StartCoroutine(InitBoardCardsRoutine(refreshBoard));
    }

    IEnumerator InitBoardCardsRoutine(bool refreshBoard = false) {

        blockingPanel.SetActive(true);

        //init cards
        for (int i = 0; i < BoadCards.Length; i++)
        {
            if (!BoadCards[i].isMatched)
            {
                //Debug.Log("unhide cards");
                BoadCards[i].isFlipped = false;
                BoadCards[i].cardObject.GetComponent<CardScript>().UnflipCard();

                if (refreshBoard) {
                    BoadCards[i].cardObject.GetComponent<CardScript>().SetPosition();
                    BoadCards[i].cardObject.GetComponent<CardScript>().FadeCard(false);
                    BoadCards[i].cardObject.GetComponent<CardScript>().HideCard(false);
                }
               
            }
        }

        if (refreshBoard) yield return new WaitForSeconds(transitionTime);

        isPlayerTurn = true;
        currentMatchIndex = 0;
        matchedCardIndex = 0;
        currentBoardState = BoardState.playerTurn;
    }

    private void CleanBoardCards(bool isMatched = false)
    {
        StartCoroutine(CleanBoardCardsRoutine(isMatched));
        
    }

    IEnumerator CleanBoardCardsRoutine(bool isMatched = false) {

        blockingPanel.SetActive(false);

        if (isMatched)
        {
            yield return new WaitForSeconds(transitionTime);
            //init cards
            for (int i = 0; i < BoadCards.Length; i++)
            {
                if (!BoadCards[i].isMatched)
                {
                    BoadCards[i].isFlipped = false;
                    BoadCards[i].cardObject.GetComponent<CardScript>().UnflipCard();
                    BoadCards[i].cardObject.GetComponent<CardScript>().FadeCard(true, false);
                }
            }
            yield return new WaitForSeconds(transitionTime);

            //currentTurn--;
            matchingEvent?.Invoke(BoadCards[matchedCardIndex].cardData.name, BoadCards[matchedCardIndex].currentTimes);
            //if (GetUnmatchedCardsCount() != 0) 
            //{
            //    transitionManager.cardsFoundTransition();
            //    yield return new WaitUntil(() => transitionManager.cardGameDialogueFinished == true);
            //}
            //else 
            //{
            //    transitionManager.cardGameEndTransition();
            //    //yield return new WaitUntil(() => transitionManager.cardGameDialogueFinished == true);
            //}
            //yield return new WaitForSeconds(transitionTime);
        }
        else {

            for (int i = 0; i < BoadCards.Length; i++)
            {
                BoadCards[i].cardObject.GetComponent<CardScript>().UnflipCard();
                BoadCards[i].isFlipped = false;
            }
            yield return new WaitForSeconds(cardFlipDelay);
            //currentTurn--;
            endTurn(isMatched);
        }
       

        //if (currentTurn > 0 && GetUnmatchedCardsCount() > 0)
        //      {
        //          InitBoardCards(isMatched);
        //      }
        //      else {

        //          yield return new WaitForSeconds(transitionTime);
        //          for (int i = 0; i < BoadCards.Length; i++)
        //          {
        //              if (!BoadCards[i].isMatched)
        //              {
        //                  BoadCards[i].isFlipped = false;
        //                  BoadCards[i].cardObject.GetComponent<CardScript>().UnflipCard();
        //                  BoadCards[i].cardObject.GetComponent<CardScript>().FadeCard(true, false);
        //              }
        //          }
        //          yield return new WaitForSeconds(transitionTime);

        //          //fire event here;
        //          Debug.Log("game is over");
        //      }


    }

    public void endTurn() {
		StartCoroutine(CheckTurnAmount(true));
	}

    public void endTurn(bool isMatched = false)
    {
        StartCoroutine(CheckTurnAmount(isMatched));
    }

    IEnumerator CheckTurnAmount(bool isMatched = false)
    {
        if (GetUnmatchedCardsCount() > 0)
        {
            InitBoardCards(isMatched);
        }
        else {
           
            yield return new WaitForSeconds(transitionTime);
            for (int i = 0; i < BoadCards.Length; i++)
            {
                if (!BoadCards[i].isMatched)
                {
                    BoadCards[i].isFlipped = false;
                    BoadCards[i].cardObject.GetComponent<CardScript>().UnflipCard();
                    BoadCards[i].cardObject.GetComponent<CardScript>().FadeCard(true, false);
                }
            }
            yield return new WaitForSeconds(transitionTime);

			//fire event here;
			cardGameEndEvent?.Invoke();

			Debug.Log("game is over (action)");
        }
    }

    // Update is called once per frame
    void Update()
    {
        debugStateText.text = currentBoardState.ToString();
        turnText.text = "turns left: " + currentTurn;
    }

    int GetUnmatchedCardsCount() {
        int cardCount = 0;
        for (int i = 0; i < BoadCards.Length; i++)
        {
            if (!BoadCards[i].isMatched) {
                cardCount++;
            }
        }

        return cardCount;
    }

    public string getCardName(int ID)
    {
        return BoadCards[ID].cardData.name;
    }

    public bool isCardValidToplay(int ID) {
        if (BoadCards[ID].isFlipped) return false;
        if (BoadCards[ID].isMatched) return false;
        return true;


    }

    public void PlayCard(int ID) {
        BoadCards[ID].cardObject.GetComponent<CardScript>().FlipCard();
    }
    

    public void flipCard(int ID) {

        StartCoroutine(flipCardRoutine(ID));
    }

    public int GetBoardWidth() {
        return cardLayout.y;
    }

     void CheckMatchingCards() {
        string matchedName = BoadCards[matchedCardsID[0]].cardData.name;
        for (int i = 0; i < matchedCardsID.Length; i++)
        {
            //not a match
            if (BoadCards[matchedCardsID[i]].cardData.name != matchedName)
            {
                matchedCardIndex = 0;
                CleanBoardCards();
                return;
            }
        }

        //a match
        matchedCardIndex = matchedCardsID[0];
        for (int i = 0; i < matchedCardsID.Length; i++)
        {
            
            BoadCards[matchedCardsID[i]].Matched();

        }
        playerAI.ResetStuborn();
        CleanBoardCards(true);
        
        
    }

 

    IEnumerator flipCardRoutine(int ID) {
        currentBoardState = BoardState.boardProcess;
        matchedCardsID[currentMatchIndex] = ID;
        currentMatchIndex++;

        BoadCards[ID].isFlipped = true;
        playerAI.RevealCard(ID, getCardName(ID));
        playerAI.SetDesiredCard(getCardName(ID));
        
        yield return new WaitForSeconds(cardFlipDelay);

        //need to checking cards
        if (currentMatchIndex >= cardsRequiredPerMatch)
        {
            CheckMatchingCards();
        }
        else {
            isPlayerTurn = !isPlayerTurn;
            Debug.Log("player turn: " + isPlayerTurn);
            if (!isPlayerTurn)
            {
                currentBoardState = BoardState.enemyTurn;
                playerAI.PlayTurn();
            }
            else {
                currentBoardState = BoardState.playerTurn;
            }
            
        }
    }

    //IEnumerator DeactivateHidingCards(int cardID )
    //{
    //    yield return new WaitForSeconds(transitionTime);
    //    BoadCards[cardID].boardPosition = BoadCards[cardID].targetPosition;
    //    cardObject.SetActive(false);
    //}


}
