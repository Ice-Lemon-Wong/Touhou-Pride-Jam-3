using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardGameSequenceEventManager : MonoBehaviour
{
    private CardsManager cm;
    [SerializeField] private SequenceManager sm;
    
    
    [SerializeField] private DialougeFilesManager dfm;
    [SerializeField] private bool loopEvents = false;
    [SerializeField] private int dialogueSystemIndex = 0;
    [SerializeField] private CardGameSequenceEvent[] cardGameSequenceEvents;
    

    private int cardSequenceIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        cm = CardsManager.instanceCM;
        cardSequenceIndex = 0;
    }

    public void FireCardGameSequenceEvent() {
        if (cardSequenceIndex < 0 || cardSequenceIndex >= cardGameSequenceEvents.Length) return;

        cardGameSequenceEvents[cardSequenceIndex].FireCardEvent(cm, EventForCards, sm.FireSequence);
        cardSequenceIndex++;

        if (loopEvents) cardSequenceIndex %= cardGameSequenceEvents.Length;
    }

    //not used
    public void EventForCards() {
        dfm.LoadDialogueFromFile(dialogueSystemIndex,"Test1", "Card", new Action[] { cm.endTurn });

    }

    public void EventForCards(string fileName, int timesIndex)
    {
        if (string.IsNullOrEmpty(fileName)) {
            cm.endTurn();
            return;
        }

        dfm.LoadDialogueFromFile(dialogueSystemIndex,fileName, "card" + "_" + timesIndex, new Action[] { cm.endTurn });

    }

    public void InitCards() {
        cm.InitBoardCards(true);
    }


    [Serializable]
    struct CardGameSequenceEvent {
        public Vector2 xBounds;
        public Vector2 yBounds;
        public Vector2Int cardLayout;
        public int turnAmount;
        public bool isCheat;
        public bool isClosing;
        public bool isFinal;
        public bool isAct4;
        public bool proceedNextEvent;

        public void FireCardEvent(CardsManager cm, Action<string, int> cardEvent,Action endEvent) {
            cm.CreatBoard(xBounds, yBounds, cardLayout, turnAmount, true, isCheat , isClosing, isFinal, isAct4);
            cm.matchingEvent = cardEvent;
            if (proceedNextEvent) cm.cardGameEndEvent = endEvent;

        }

    }
}
