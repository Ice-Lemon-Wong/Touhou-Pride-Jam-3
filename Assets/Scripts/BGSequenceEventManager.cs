using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BGSequenceEventManager : MonoBehaviour
{

    [SerializeField] private BackgroundManager bgm;
    [SerializeField] private BackgroundSequencedEvent[] bgEvents;
    [SerializeField] private bool loopEvents = false;

    private int currentBGCount = 0;

    private void Start()
    {
        currentBGCount = 0;
    }

    public void FireBGEvent()
    {
        if (currentBGCount < 0 || currentBGCount >= bgEvents.Length) return;

        Debug.Log("changing BG " + bgEvents[currentBGCount].bgIndex);
        bgm.SetActiveBG(bgEvents[currentBGCount].bgIndex);
        currentBGCount++;
        if (loopEvents) currentBGCount %= bgEvents.Length;


    }


    [Serializable]
    private struct BackgroundSequencedEvent
    {

        public int bgIndex;

    }
}
