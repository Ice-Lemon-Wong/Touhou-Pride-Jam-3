using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TransitionManager : DialogueSystemCommandParser
{
    public DialougeFilesManager dialogueManager;
    public DialogueUIEnabler dialogueUIEnabler;
    public CardsManager cardManager;
    public bool cardGameDialogueFinished = false;
	private int rounds = 1;

	private Action[] actions;

	private IEnumerator Start() 
    {
        yield return null;
		dialogueManager.LoadDialogueFromFile("Test", "Start");
		ds.SetEndEvents(new Action[] {CardGameEvent});
	}

    public void CardGameEvent() {
        Debug.LogWarning("Starting card game");
		dialogueUIEnabler.DisableUI();
		cardManager.InitBoardCards(true);
		cardManager.action = () =>
		{
			dialogueManager.LoadDialogueFromFile("Test", "Test");
			ds.endDialougeEvents -= cardManager.endGame;
		};
	}

    public void cardsFoundTransition() {
		string dialoguesToLoad = "CardMatched_";
		cardGameDialogueFinished = false;
		dialogueUIEnabler.EnableUI();
        
        dialogueManager.LoadDialogueFromFile("Test", dialoguesToLoad += rounds.ToString());
		rounds++;
	}

    public void cardsFoundEndTransition() {
        dialogueUIEnabler.DisableUI();
		cardGameDialogueFinished = true;
    }

    public void cardGameEndTransition() {
        dialogueUIEnabler.EnableUI();
        dialogueManager.LoadDialogueFromFile("Test", "CardGameEnd");
		//calls the end game function
		ds.SetEndEvents(new Action[] { cardManager.endGame });
		//ds.endDialougeEvents -= cardManager.endGame;
	}
}
