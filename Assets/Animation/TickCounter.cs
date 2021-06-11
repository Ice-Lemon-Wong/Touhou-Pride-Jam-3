using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//todo
//use deligates

public class TickCounter 
{
    private float counter = 0;
    public float increment = 1;
    public bool debugtText = false;
    public string debugString = "default counter";
    Action callbackAction;

    public TickCounter() {
        increment = 1;
        counter = 0;
    }

    public TickCounter(float incrementValue)
    {
        increment = incrementValue;
        counter = 0;
    }

    public TickCounter(float incrementValue, Action initalAction )
    {
        increment = incrementValue;
        callbackAction += initalAction;
        counter = 0;
    }




    //call this in update
    public void AdvanceTick() {
        if (Time.time > counter) {
            if (debugtText) Debug.Log("Tick: " + debugString);
            //do something
            callbackAction();
            counter = Time.time + increment;
        }
    
    }

    public void addAction(Action newAction) {
        callbackAction += newAction;
    }

    public void removeAction(Action removedAction)
    {
        callbackAction -= removedAction;
    }



}
