using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueSystemCommandParser : MonoBehaviour
{

    private List<Command> commands = new List<Command>();
    [SerializeField] protected DialogueSystem ds;
    [SerializeField] protected bool isComponentInObject;


    //automatically assigns dialogue system if the box is checked
    public void InitCommands()
    {
        if (isComponentInObject) {
            ds = GetComponent<DialogueSystem>();
        }

        foreach (var item in commands)
        {
            ds.dialougeLineEvents += item.CheckCommand;
        }
    }

    public void AddComand(string commandText, Action<string[]> commandFunction)
    {
       if (commands == null) commands = new List<Command>();

        commands.Add(new Command( commandText, commandFunction));
    }

    private struct Command {
        public string commandText;
        public Action<string[]> commandFunction;
        static private string[] currentTextStrings;

        public Command( string commandText, Action<string[]> commandFunction) {
            this.commandText = commandText;
            this.commandFunction = commandFunction;
            currentTextStrings = new string[1];
        }

        public void CheckCommand(string commandLine) {
            currentTextStrings = commandLine.Split(' ');

            if (currentTextStrings[0].Substring(1, currentTextStrings[0].Length - 1).ToUpper().Equals(commandText.ToUpper()))
            {
                commandFunction?.Invoke(currentTextStrings);
            }
        }
    
    }

}
