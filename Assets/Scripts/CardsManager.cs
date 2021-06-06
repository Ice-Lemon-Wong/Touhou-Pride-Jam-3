using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Space]
    [Header("card game config")]
    [SerializeField] private bool InitAfterCreate;
    [SerializeField] private int  cardsRequiredPerMatch = 2;
    [SerializeField] private float cardFlipDelay = 0.75f;
    [SerializeField] private Vector2 cardHidingOffset;
    [SerializeField] private float transitionTime = 0.75f;

    [Space]
    [Header("card distribution config")]
    [SerializeField] CardSO[] cardSOPool;
    [SerializeField] CardDistributor cardDistributor;

    [Space]
    [Header("debug text stuff")]
    [SerializeField] TextMeshProUGUI debugStateText;
    
    

    private int[] matchedCardsID;
    private int currentMatchIndex = 0;
    private float currentTransitionTime = 0;
    private bool isPlayerTurn = true;
    private CardPlayerAI playerAI;
    

    public enum BoardState { playerTurn, enemyTurn, boardProcess};
    public BoardState currentBoardState;

    public struct CardOnBoard{
        public int ID;
        public Vector2 boardPosition;
        public GameObject cardObject;
        public CardSO cardData;
        public bool isFlipped;
        public bool isMatched;
        

        public CardOnBoard(int ID, Vector2 boardPosition, GameObject cardObject, CardSO cardData) {
            this.ID = ID;
            this.boardPosition = boardPosition;
            this.cardObject = cardObject;
            this.isFlipped = false;
            this.isMatched = false;
            this.cardData = cardData;
            
        }

        public void Matched(bool matched = true) {
            isMatched = matched;
            cardObject.GetComponent<CardScript>().HideCard();
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

        yield return null;
        //create cards
        CreatBoard();

        
        

    }

    private void CreatBoard()
    {
        float cardX = 0;
        float cardY = 0;
        float width = xBounds.y - xBounds.x;
        float height = yBounds.y - yBounds.x;
        GameObject cardObject;

        List<CardSO> decidedCards = new List<CardSO>();
        decidedCards = cardDistributor.DistributeCards((cardLayout.x * cardLayout.y) / cardsRequiredPerMatch);
        CardSO selectedCardData = new CardSO();
        List<CardSO> boardCards = new List<CardSO>();
        int selectedIndex = 0;

        //create the appropriate ammount of cards
        for (int i = 0; i < cardLayout.x * cardLayout.y; i++)
        {
            if (i % cardsRequiredPerMatch == 0)
            {
                selectedIndex = Random.Range(0, decidedCards.Count);
                selectedCardData = decidedCards[selectedIndex];
                decidedCards.RemoveAt(selectedIndex);

            }
            boardCards.Add(selectedCardData);

        }
        
        for (int i = 0; i < cardLayout.x * cardLayout.y; i++)
        {

            //randomize cards
            selectedIndex = Random.Range(0, boardCards.Count);
            selectedCardData = boardCards[selectedIndex];
            boardCards.RemoveAt(selectedIndex);


            //selectedCardData = cardSOPool[Random.Range(0, cardSOPool.Length)];

            //figure out where to spawn card object
            cardX = ((i % cardLayout.x) * (width / (cardLayout.x - 1))) + xBounds.x;
            cardY = ((i / cardLayout.x) * (height / (cardLayout.y - 1))) + yBounds.x;

            //spawn card object
            cardObject = Instantiate(cardobject, new Vector2(cardX, cardY) + cardHidingOffset, Quaternion.identity);
            cardObject.GetComponent<CardScript>().InitCard(i, selectedCardData.icon, new Vector2(cardX, cardY), new Vector2(cardX, cardY) + cardHidingOffset);
            cardObject.name = selectedCardData.name + "-" + (i % cardsRequiredPerMatch);
            BoadCards[i] = new CardOnBoard(i, new Vector2(cardX, cardY), cardObject, selectedCardData);
            BoadCards[i].cardObject.GetComponent<CardScript>().SetPosition();

        }

        playerAI = FindObjectOfType<CardPlayerAI>();
        playerAI.InitKnowledgeBase(cardLayout.x * cardLayout.y);

        if (InitAfterCreate) InitBoardCards(true);
    }

    private void InitBoardCards(bool refreshBoard = false)
    {
        StartCoroutine(InitBoardCardsRoutine(refreshBoard));
    }

    IEnumerator InitBoardCardsRoutine(bool refreshBoard = false) {
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
        currentBoardState = BoardState.playerTurn;
    }

    private void CleanBoardCards(bool isMatched = false)
    {
        StartCoroutine(CleanBoardCardsRoutine(isMatched));
        
    }

    IEnumerator CleanBoardCardsRoutine(bool isMatched = false) {

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
            Debug.Log("closing");
            yield return new WaitForSeconds(transitionTime);
        }
        else {
            for (int i = 0; i < BoadCards.Length; i++)
            {
                BoadCards[i].cardObject.GetComponent<CardScript>().UnflipCard();
                BoadCards[i].isFlipped = false;
            }
            yield return new WaitForSeconds(cardFlipDelay);
        }

        InitBoardCards(isMatched);
    }

    // Update is called once per frame
    void Update()
    {
        debugStateText.text = currentBoardState.ToString();
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
                
                CleanBoardCards();
                return;
            }
        }

        //a match
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
