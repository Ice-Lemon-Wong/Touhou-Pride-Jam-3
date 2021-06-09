using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialougeTextColourManager : DialogueSystemCommandParser
{
    public TextMeshProUGUI dialogueTextFeild;

    // Start is called before the first frame update
    void Start()
    {
        AddComand("colour", ChangeColour);
        //ds.dialogueLineEvent += () => { dialogueTextFeild.color = Color.black; };
        InitCommands();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeColour(string[] commandLine) {
        if (commandLine[1].Substring(0, commandLine[1].Length - 1).ToUpper().Equals("RED")) {
            dialogueTextFeild.color = Color.red;
        }
    }
}
