using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class TestSequenceManager : MonoBehaviour
{

    [SerializeField] private int currentSequenceIndex = 0;
    [SerializeField] private bool loopEvent;
    [SerializeField] private BackgroundManager bgm;
    [SerializeField] private BackgroundSequencedEvent[] bgEvents;
    public UnityEvent[] events;

    // Start is called before the first frame update
    void Start()
    {
        currentBGCount = 0;
        FireSequence();
    }

    public void FireSequence() {
        if (currentSequenceIndex < 0 || currentSequenceIndex >= events.Length) return;

        events[currentSequenceIndex]?.Invoke();
        currentSequenceIndex++;
        if (loopEvent) currentSequenceIndex %= events.Length;

    }

    IEnumerator FireNextEvent(float duration) {
        yield return new WaitForSeconds(duration);
        FireSequence();
    }

    public void WaitEvent(float waitTime)
    { 

        if (waitTime >= 0)
        {
            StartCoroutine(FireNextEvent(waitTime));
        }

    }

    public void sayHi()
    {
        Debug.Log("Hi");
        
        
    }
    public void sayHowdy()
    {
        Debug.Log("Howdy");

    }

    int currentBGCount = 0;
    public void FireBGEvent(float waitTime)
    {
        Debug.Log("changing BG "+ bgEvents[currentBGCount].bgIndex);
        bgm.SetActiveBG(bgEvents[currentBGCount].bgIndex);
        currentBGCount++;

        if (waitTime >= 0) {
            StartCoroutine(FireNextEvent(waitTime));
        }

    }
    



    [Serializable]
    public struct BackgroundSequencedEvent {
       
        public int bgIndex;

       
        
        
    }
}
