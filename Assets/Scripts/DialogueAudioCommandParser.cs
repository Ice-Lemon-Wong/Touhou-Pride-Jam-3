using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueAudioCommandParser : DialogueSystemCommandParser
{
    [SerializeField] string audioCommandText = "audio";
    [SerializeField] string playCommandText = "play";
    [SerializeField] string stopCommandText = "stop";
    [SerializeField] string playFadeCommandText = "playFade";
    [SerializeField] string stopFadeCommandText = "stopFade";

    private AudioManager2D audioManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager2D.audioManager2DInstance;
        AddComand(audioCommandText, AudioCommandFunction);
        InitCommands();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AudioCommandFunction(string[] commandLine) {
        if (commandLine[1].ToUpper().Equals(playCommandText.ToUpper()))
        {
            audioManager.Play(commandLine[2].Substring(0, commandLine[2].Length - 1));


        }
        else if (commandLine[1].ToUpper().Equals(stopCommandText.ToUpper()))
        {
            if (commandLine[2].Substring(0, commandLine[2].Length - 1).ToUpper().Equals("ALL")) {
                audioManager.StopAllAudio(); 
                return;
            }

            int audioTypeIndex = 0;
            if (!Int32.TryParse(commandLine[2], out audioTypeIndex))
            {
                Debug.LogError($"argument '{commandLine[2]}' at index 1 is not a number at {commandLine}");
                return;
            }
            else {
                audioManager.Stop(audioTypeIndex);
            }


        }
        else if (commandLine[1].ToUpper().Equals(playFadeCommandText.ToUpper()))
        {
            audioManager.PlayFade(commandLine[2].Substring(0, commandLine[2].Length - 1));


        }
        else if (commandLine[1].ToUpper().Equals(stopFadeCommandText.ToUpper()))
        {
            if (commandLine.Length <= 2)
            {
                Debug.LogError($"too few arguments for play fade command at {commandLine}");
                return;
            }

            int audioTypeIndex = 0;
            if (!Int32.TryParse(commandLine[2], out audioTypeIndex))
            {
                Debug.LogError($"argument '{commandLine[2]}' at index 1 is not a number at {commandLine}");
                return;
            }
            else
            {
                audioManager.StopFade(audioTypeIndex);
            }


        }
        else {
            Debug.LogError($"{commandLine[1]} is not a valid command for audio in {commandLine}");
        }
        
    }
}
