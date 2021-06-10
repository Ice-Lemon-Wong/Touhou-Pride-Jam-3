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
		ds.SetEndEvents(new Action[] {startCardGameEvent});
	}

    public void startCardGameEvent() {
        Debug.LogWarning("Starting card game");
		dialogueUIEnabler.DisableUI();
		cardManager.InitBoardCards(true);
		cardManager.cardGameEndEvent = () =>
		{
			dialogueManager.LoadDialogueFromFile("Test", "Test");
			ds.SetEndEvents(new Action[] { CardGameEvent2 });
			cardManager.DestroyBoard();
			//ds.endDialougeEvents -= cardManager.endGame;
		};
	}

	public void ContinueCardGameEvent() {
		dialogueUIEnabler.DisableUI();
		cardManager.InitBoardCards(true);
	}

	public void CardGameEvent2() {
		
		Debug.LogWarning("creating card game 2");
		dialogueUIEnabler.DisableUI();
		cardManager.CreatBoard(new Vector2(-4, 4), new Vector2(-4, 4), new Vector2Int(2, 2), true);
		cardManager.cardGameEndEvent = () =>
		{
			dialogueManager.LoadDialogueFromFile("Test", "Test2");

			
			cardManager.DestroyBoard();
			//ds.endDialougeEvents -= cardManager.endGame;
		};
		ds.endDialougeEvents -= CardGameEvent2;
		rounds = 1;
	}


	public void cardsFoundTransition() {
		string dialoguesToLoad = "CardMatched_";
		cardGameDialogueFinished = false;
		dialogueUIEnabler.EnableUI();
        
        dialogueManager.LoadDialogueFromFile("Test", dialoguesToLoad += rounds.ToString());
		//ds.SetEndEvents(new Action[] { CardGameEvent });
		ds.SetEndEvents(new Action[] { ContinueCardGameEvent } );
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
		ds.SetEndEvents(new Action[] { cardManager.endTurn });
		//ds.endDialougeEvents -= cardManager.endGame;
	}
}
