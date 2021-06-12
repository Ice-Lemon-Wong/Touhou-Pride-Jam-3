using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeakerFieldManager : DialogueSystemCommandParser
{
    [Space]
    [Header("speaker command config")]
    [SerializeField] private TextMeshProUGUI speakerTextFeild;
    [SerializeField] private string speakerCommand;
	public DialogueLogger dialogueLogger;

	// Start is called before the first frame update
	void Start()
    {
        speakerTextFeild.text = "";
        
        AddComand(speakerCommand, SetSpeaker);
        InitCommands();
        ds.initDialogueEvents += () => { speakerTextFeild.text = ""; };
        //speakerTextFeild.text = "";

    }

    //public void CheckSpeaker(string textCommand) {
    //    currentTextStrings = textCommand.Split(' ');

    //    if (currentTextStrings[0].Substring(1, currentTextStrings[0].Length-1).ToUpper().Equals(command.ToUpper())) {

    //        Debug.Log("switching speaker");
    //        speakerTextFeild.text = currentTextStrings[1];
    //    }
    //}

    public void SetSpeaker(string[] textCommand)
    {
        speakerTextFeild.text = "";
        for (int i = 1; i < textCommand.Length; i++)
        {
            if (string.IsNullOrEmpty(textCommand[i])) {
                speakerTextFeild.text = "";
                return;
            }
            
            speakerTextFeild.text += textCommand[i];
            
            if (textCommand[i] != "\r")
			    dialogueLogger.AddToLog(textCommand[i], true);

		}

    }



}
