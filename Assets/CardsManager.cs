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


    [SerializeField] private Vector2 xBounds;
    [SerializeField] private Vector2 yBounds;
    [SerializeField] private Vector2Int cardLayout;
    [SerializeField] private GameObject cardobject;
    [SerializeField] private int  cardsRequiredPerMatch = 2;
    [SerializeField] private float cardFlipDelay = 0.75f;
    [SerializeField] private Vector2 cardHidingOffset;
    [SerializeField] private float transitionTime = 0.75f;
    [SerializeField] CardSO[] cardSOPool;
    [SerializeField] TextMeshProUGUI debugStateText;

    private int[] matchedCardsID;
    private int currentMatchIndex = 0;
    private float currentTransitionTime = 0;

    public enum BoardState { playerTurn, enemyTurn, boardProcess};
    public BoardState currentBoardState;

    struct CardOnBoard{
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
    private CardOnBoard[] BoadCards;

    // Start is called before the first frame update
    void Start()
    {
        BoadCards = new CardOnBoard[cardLayout.x * cardLayout.y];
        matchedCardsID = new int[cardsRequiredPerMatch];
        currentMatchIndex = 0;
        currentBoardState = BoardState.boardProcess;

        //create cards
        float cardX = 0;
        float cardY = 0;
        float width = xBounds.y - xBounds.x;
        float height = yBounds.y - yBounds.x;
        GameObject cardObject;
        CardSO selectedCardData;
        for (int i = 0; i < cardLayout.x * cardLayout.y; i++)
        {
            selectedCardData = cardSOPool[Random.Range(0, cardSOPool.Length)];

            cardX = ((i % cardLayout.x) * (width / (cardLayout.x - 1))) + xBounds.x;
            cardY = ((i / cardLayout.x) * (height / (cardLayout.y - 1))) + yBounds.x;

            cardObject = Instantiate(cardobject, new Vector2(cardX, cardY) + cardHidingOffset, Quaternion.identity);
            cardObject.GetComponent<CardScript>().InitCard(i, selectedCardData.icon, new Vector2(cardX, cardY), new Vector2(cardX, cardY) + cardHidingOffset);


            BoadCards[i] = new CardOnBoard(i, new Vector2(cardX, cardY), cardObject, selectedCardData);
        }

        InitBoardCards(true);

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
                Debug.Log("unhide cards");
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


    

    public void flipCard(int ID) {

        StartCoroutine(flipCardRoutine(ID));
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
        
        CleanBoardCards(true);
        
        
    }

 

    IEnumerator flipCardRoutine(int ID) {
        currentBoardState = BoardState.boardProcess;
        matchedCardsID[currentMatchIndex] = ID;
        currentMatchIndex++;

        yield return new WaitForSeconds(cardFlipDelay);

        //need to checking cards
        if (currentMatchIndex >= cardsRequiredPerMatch)
        {
            CheckMatchingCards();
        }
        else {
            currentBoardState = BoardState.playerTurn;
        }
    }

    //IEnumerator DeactivateHidingCards(int cardID )
    //{
    //    yield return new WaitForSeconds(transitionTime);
    //    BoadCards[cardID].boardPosition = BoadCards[cardID].targetPosition;
    //    cardObject.SetActive(false);
    //}


}
