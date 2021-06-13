using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BackgroundCommandParser : DialogueSystemCommandParser
{
    [SerializeField] BackgroundManager bgm;
    [SerializeField] string backgroundCommand = "background";

    // Start is called before the first frame update
    void Start()
    {
        AddComand(backgroundCommand, ChangeBGCommandFunction);
        InitCommands();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeBGCommandFunction(string[] commandLine) {
        int bgIndex = -1;

        
        //bool isExpression = true;
        if (!Int32.TryParse(commandLine[1], out bgIndex))
        {
            Debug.LogError($"argument '{commandLine[1]}' at index 1 is not a valid index for portrait");
            return;
        }

        if (commandLine.Length > 2) {

            //can only read int for now
            int transitionTime = 0;
            if (!Int32.TryParse(commandLine[2], out transitionTime))
            {
                Debug.LogError($"argument '{commandLine[2]}' at index 2 is not a valid number to specify transtion time");
                return;
            }
            bgm.SetActiveBG(bgIndex, transitionTime);
        }
        else {
            bgm.SetActiveBG(bgIndex);
        }

           
    }
}
