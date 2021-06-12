using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardDistributor : MonoBehaviour
{
    [SerializeField] bool initCardHere = false;
    [SerializeField] int currentStage = 1;

    [SerializeField] CardSO[] allCards;
    [SerializeField] CardSO[] closingCards;
    [SerializeField] CardSO[] finalCards;
    [SerializeField] CardSO[] rareCards;
    static private StoredCard[] cardStorage;

    private List<DistricutedCard> cardstoDistribute;
    

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
    public List<DistricutedCard> DistributeCards(int pairOfCards, bool isAct4 = false)
    {
        Debug.Log(pairOfCards);

        cardstoDistribute = new List<DistricutedCard>();
        //cardstoDistribute = new CardSO[ammountOfCards];

        for (int i = 0; i < cardStorage.Length; i++)
        {
            if (cardStorage[i].IsCardAvailable(currentStage,isAct4)) {
                cardstoDistribute.Add(cardStorage[i].retriveCard(currentStage,isAct4));
            }
        }

        Debug.Log(cardstoDistribute.Count);
        while (cardstoDistribute.Count >  pairOfCards)
        {
            cardstoDistribute.RemoveAt(UnityEngine.Random.Range(0, cardstoDistribute.Count));
        }

        if (!isAct4) {
            if (cardstoDistribute.Count >= pairOfCards) cardstoDistribute.RemoveAt(UnityEngine.Random.Range(0, cardstoDistribute.Count));

            cardstoDistribute.Add(new DistricutedCard(rareCards[currentStage - 1], currentStage));
        }
        Debug.Log(cardstoDistribute.Count);

        return cardstoDistribute;
    }


    public List<DistricutedCard> DistributeClosingCards()
    {
        cardstoDistribute = new List<DistricutedCard>();
        //cardstoDistribute = new CardSO[ammountOfCards];

        for (int i = 0; i < finalCards.Length; i++)
        {
            cardstoDistribute.Add(new DistricutedCard(closingCards[currentStage - 1], currentStage));
        }

        return cardstoDistribute;
    }

    public List<DistricutedCard> DistributeFinalCards()
    {
        cardstoDistribute = new List<DistricutedCard>();
        //cardstoDistribute = new CardSO[ammountOfCards];

        for (int i = 0; i < finalCards.Length; i++)
        {
            cardstoDistribute.Add(new DistricutedCard(finalCards[i], 5)); ;
        }

        

        return cardstoDistribute;
    }

    public struct DistricutedCard {
        public CardSO cardData;
        public int currentTimes;

        public DistricutedCard(CardSO cardData, int currentUsedTime) {
            this.cardData = cardData;
            this.currentTimes = currentUsedTime;
        }
    }

    [Serializable]
    private struct StoredCard {
        public CardSO cardData;
        public int usedTimes;
        public int minimumStartingStage;

        public StoredCard(CardSO newCard) {
            cardData = newCard;
            
            usedTimes = 0;
            minimumStartingStage = cardData.minActApperance;
        }

        public DistricutedCard retriveCard(int currentStage,bool isAct4 = false) {

            counter += 1;
            Debug.Log("counter: " + counter);
            //usedTimes++;
            DistricutedCard card = new DistricutedCard(cardData, currentStage);
            if (isAct4) card.currentTimes = 4;

            return card;


        }

        static int counter = 0;
        public bool IsCardAvailable(int currentStage, bool isAct4 = false)
        {

            if (isAct4)
            {
                if (cardData.isAppearAct4 == true) return true;
                else return false;

            }
            else {
                Debug.Log(minimumStartingStage +(cardData.appearTimes - 1));
                Debug.Log(currentStage <= (minimumStartingStage + cardData.appearTimes - 1));
                if (minimumStartingStage <= currentStage && currentStage <= minimumStartingStage + (cardData.appearTimes-1))
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
}
