using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class SequenceManager : MonoBehaviour
{

    [SerializeField] private int currentSequenceIndex = 0;
    [SerializeField] private bool loopEvent;
    [SerializeField] private SequecedEvent[] sequecedEvents;

    // Start is called before the first frame update
    void Start()
    {
        
        currentSequenceIndex = 0;
        FireSequence();
    }

    public void FireSequence() {
        if (currentSequenceIndex < 0 || currentSequenceIndex >= sequecedEvents.Length) return;

        StopCoroutine(FireSequenceRoutine());
        StartCoroutine(FireSequenceRoutine());

    }

    IEnumerator FireSequenceRoutine() {
        yield return new WaitForSeconds(sequecedEvents[currentSequenceIndex].eventDelay);
        bool fireNextEvent = sequecedEvents[currentSequenceIndex].FireNextEvent;

        sequecedEvents[currentSequenceIndex].eventToFire?.Invoke();
        
        currentSequenceIndex++;
        if (loopEvent) currentSequenceIndex %= sequecedEvents.Length;

        Debug.Log("next event "+ currentSequenceIndex);
        if (fireNextEvent) FireSequence();

    }

    //IEnumerator FireNextEvent(float duration) {
    //    yield return new WaitForSeconds(duration);
    //    FireSequence();
    //}

    //public void WaitEvent(float waitTime)
    //{ 

    //    if (waitTime >= 0)
    //    {
    //        StartCoroutine(FireNextEvent(waitTime));
    //    }

    //}

  

    [Serializable]
    public struct SequecedEvent
    {
        public string name;
        public float eventDelay;
        public UnityEvent eventToFire;
        public bool FireNextEvent;
        
        

    }


   
}
