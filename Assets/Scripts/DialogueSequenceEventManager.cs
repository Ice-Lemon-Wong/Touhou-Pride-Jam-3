using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueSequenceEventManager : MonoBehaviour
{
    [SerializeField] TestSequenceManager sm;
    [SerializeField] DialougeFilesManager dfm;
    [SerializeField] DialogueEvent[] dialogueSequencedEvents;
    private int currentDialogueEventIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentDialogueEventIndex = 0;
    }

    public void FireDialogueSequencedEvent() {
        dialogueSequencedEvents[currentDialogueEventIndex].InitiateDialogue(dfm, sm.FireSequence);
        currentDialogueEventIndex++;
    }
   

    [Serializable]
    struct DialogueEvent {
        public string fileName;
        public string startingLine;
        public bool proceedNextEvent;

        public void InitiateDialogue(DialougeFilesManager dfm, Action loadNextSequencedEventAction) {

            if (proceedNextEvent)
            {
                dfm.LoadDialogueFromFile(fileName, startingLine,new Action[] { loadNextSequencedEventAction});
            }
            else {
                dfm.LoadDialogueFromFile(fileName, startingLine);
            }
            
        }
    }
}
