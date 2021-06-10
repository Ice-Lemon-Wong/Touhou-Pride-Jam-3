using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardDistributor : MonoBehaviour
{
    [SerializeField] bool initCardHere = false;
    [SerializeField] int currentStage = 1;

    [SerializeField] CardSO[] allCards;
    static private StoredCard[] cardStorage;

    private List<CardSO> cardstoDistribute;
    //private CardSO[] cardstoDistribute;


    private void Start()
    {
        if (initCardHere) {
            InitCardStorage();
        }
    }


    void InitCardStorage() {
        cardStorage = new StoredCard[allCards.Length];

        for (int i = 0; i < allCards.Length; i++)
        {
            cardStorage[i] = new StoredCard(allCards[i]);
        }
    }


    //do not know the current times we get this card
    //get the number of times
    public List<CardSO> DistributeCards(int pairOfCards)
    {
        cardstoDistribute = new List<CardSO>();
        //cardstoDistribute = new CardSO[ammountOfCards];

        for (int i = 0; i < cardStorage.Length; i++)
        {
            if (cardStorage[i].IsCardAvailable(currentStage)) {
                cardstoDistribute.Add(cardStorage[i].retriveCard());
            }
        }

        while (cardstoDistribute.Count >  pairOfCards)
        {
            cardstoDistribute.RemoveAt(UnityEngine.Random.Range(0, cardstoDistribute.Count));
        }

        return cardstoDistribute;
    }

    [Serializable]
    private struct StoredCard {
        public CardSO cardData;
        public int amountLeft;
        public int minimumStartingStage;

        public StoredCard(CardSO newCard) {
            cardData = newCard;
            amountLeft = cardData.appearTimes;
            minimumStartingStage = cardData.minActApperance;
        }

        public CardSO retriveCard() {

            amountLeft--;
            return cardData;
           
        }
        public bool IsCardAvailable(int currentStage)
        {
            if (minimumStartingStage <= currentStage && amountLeft > 0)
            {
                
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
