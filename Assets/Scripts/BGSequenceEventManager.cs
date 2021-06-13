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


        if (bgEvents[currentBGCount].OverideSpeed)
        {
            bgm.SetActiveBG(bgEvents[currentBGCount].bgIndex, bgEvents[currentBGCount].transitionSpeed);
        }
        else {
            if (bgEvents[currentBGCount].instantTransition) {
                bgm.SetActiveBG(bgEvents[currentBGCount].bgIndex,true);
            }
            else
            {
                bgm.SetActiveBG(bgEvents[currentBGCount].bgIndex, false);

            }
            
        }
        Debug.Log("changing BG " + bgEvents[currentBGCount].bgIndex);
        
        currentBGCount++;
        if (loopEvents) currentBGCount %= bgEvents.Length;


    }


    [Serializable]
    private struct BackgroundSequencedEvent
    {

        public int bgIndex;
        public bool instantTransition;
        public bool OverideSpeed;
        public float transitionSpeed;
    }
}
