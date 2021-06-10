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

    public void EventForCards() {
        dfm.LoadDialogueFromFile("Test1", "Card", new Action[] { cm.endTurn });

    }

    public void InitCards() {
        cm.InitBoardCards(true);
    }


    [Serializable]
    struct CardGameSequenceEvent {
        public Vector2 xBounds;
        public Vector2 yBounds;
        public Vector2Int cardLayout;
        public bool isClosing;
        public bool isFinal;
        public bool proceedNextEvent;

        public void FireCardEvent(CardsManager cm, Action cardEvent,Action endEvent) {
            cm.CreatBoard(xBounds, yBounds, cardLayout, true, isClosing, isFinal);
            cm.matchingEvent = cardEvent;
            if (proceedNextEvent) cm.cardGameEndEvent = endEvent;

        }

    }
}
