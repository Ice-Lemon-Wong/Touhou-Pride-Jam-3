using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashTimer : MonoBehaviour
{

    [SerializeField] float interval = 1.5f;
    [SerializeField] SquashAndStretch sns;
    public bool interupt = false;

    TickCounter tick = new TickCounter();
    
    private bool isSquash = false;
    


    // Start is called before the first frame update
    void Start()
    {
        tick = new TickCounter(interval, SquashingCycle);
        //tick.addAction(interval, SquashingCycle);
    }

    // Update is called once per frame
    void Update()
    {
        tick.AdvanceTick();
    }

    void SquashingCycle() {
        if (isSquash)
        {
            if (!interupt) sns.SetToSquash();
            isSquash = false;
        }
        else {
            if (!interupt) sns.SetToStretch();
            isSquash = true;
        }
    }


    public void InteruptTimer(float duration) {
        StartCoroutine(InteruptTimerCooutine(duration));
    }

    IEnumerator InteruptTimerCooutine(float duration) {
        interupt = true;
        yield return new WaitForSeconds(duration);
        interupt = false;
    }
}
