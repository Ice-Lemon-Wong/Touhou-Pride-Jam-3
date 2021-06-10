using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueSequenceEventManager : MonoBehaviour
{
    [SerializeField] SequenceManager sm;
    [SerializeField] DialougeFilesManager dfm;
    [SerializeField] DialogueEvent[] dialogueSequencedEvents;
    [SerializeField] private bool loopEvents = false;
    private int currentDialogueEventIndex = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        currentDialogueEventIndex = 0;
    }

    public void FireDialogueSequencedEvent() {
        if (currentDialogueEventIndex < 0 || currentDialogueEventIndex >= dialogueSequencedEvents.Length) return;

        dialogueSequencedEvents[currentDialogueEventIndex].InitiateDialogue(dfm, sm.FireSequence);
        currentDialogueEventIndex++;

        if (loopEvents) currentDialogueEventIndex %= dialogueSequencedEvents.Length;
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
