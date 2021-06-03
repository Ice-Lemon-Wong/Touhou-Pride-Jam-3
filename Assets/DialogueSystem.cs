using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

//note
//dialouge cannot be started in the start() call because it needs to wait for events

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private string[] dialougeTexts;
    [SerializeField] private TextMeshProUGUI dialogueTextFeild;
    [SerializeField] private float typingSpeed;
    [SerializeField] Optional<string> interuptInput;
    [SerializeField] Optional<string> advancingInput;
    [SerializeField] private bool startImmediantly;
    [SerializeField] private float startDelay;
    [SerializeField] private char commandPrefix;
    [SerializeField] private bool skipEmpty;
    [SerializeField] Optional<GameObject> continueButton;
    

    

    public Action<string> dialougeLineEvents;
    public Action endDialougeEvents;

    private int currentDialougeIndex = 0;
    private bool isInterupt = false;
    private bool isTyping = false;

    

    // Start is called before the first frame update
    IEnumerator Start()
    {
        dialogueTextFeild.text = "";
        if (continueButton.Enabled) {
            continueButton.Value.SetActive(false);
        }

        //dialougeLineEvent += TestEvent;

        if (startImmediantly) {
            yield return null;
            yield return new WaitForSecondsRealtime(startDelay);
            StartDialogue();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (interuptInput.Enabled) {
            if (Input.GetButtonDown(interuptInput.Value)) {
                if (!isInterupt) {
                    isInterupt = true;
                }
            }
        }

        if (advancingInput.Enabled)
        {
            if (Input.GetButtonDown(advancingInput.Value))
            {
                if (!isTyping)
                {
                    AdvanceDialogue();
                }
            }
        }
    }

    public void LoadDialouge(string[] dialoguesToLoad, bool startImmediantly = true)
    {
        dialougeTexts = dialoguesToLoad;
        if (startImmediantly) {
            StartDialogue();
        }
    }

    public void StartDialogue() {
        if (dialougeTexts == null) return;
        currentDialougeIndex = 0;
        TypeDialouge();
    }

    public void AdvanceDialogue() {
        StopCoroutine("TypeDialougeRoutine");
        if (dialougeTexts == null) return;

        currentDialougeIndex++;
        if (currentDialougeIndex < dialougeTexts.Length)
        {
            TypeDialouge(dialougeTexts[currentDialougeIndex]);
        }
        else {
            
            if (continueButton.Enabled)
            {
                continueButton.Value.SetActive(false);
            }
            dialogueTextFeild.text = "";

            //fire events
            endDialougeEvents?.Invoke();
        }
    }


    public void SkipDialougeTyping() {
        StopCoroutine("TypeDialougeRoutine");
        dialogueTextFeild.text = "";
        AdvanceDialogue();
    }


    public void TypeDialouge()
    {
        StopCoroutine("typeDialougeRoutine");
        StartCoroutine(TypeDialougeRoutine(dialougeTexts[currentDialougeIndex]));
    }

    public void TypeDialouge(string dialougeToType) {
        StopCoroutine("typeDialougeRoutine");
        StartCoroutine(TypeDialougeRoutine(dialougeToType));
    }

    public void InteruptDialouge() {
        isInterupt = true;
    }


    IEnumerator  TypeDialougeRoutine(string dialougeToType) {


        float t = 0;
        int charIndex = 0;
        isInterupt = false;
        dialogueTextFeild.text = "";
        isTyping = true;

        Debug.Log(dialougeToType);
        if (skipEmpty && string.IsNullOrWhiteSpace(dialougeToType) ) {
            SkipDialougeTyping();
        }else {

            Debug.Log("made it to comands");
            dialougeLineEvents?.Invoke(dialougeToType);

            
            if (dialougeToType[0].Equals(commandPrefix))
            {
                SkipDialougeTyping();
            }
            else
            {
                if (continueButton.Enabled)
                {
                    continueButton.Value.SetActive(false);
                }

                while (charIndex < dialougeToType.Length)
                {
                    if (isInterupt)
                    {
                        charIndex = dialougeToType.Length;
                    }
                    else
                    {
                        t += Time.deltaTime * typingSpeed;
                        charIndex = Mathf.FloorToInt(t);
                        charIndex = Mathf.Clamp(charIndex, 0, dialougeToType.Length);
                    }

                    
                    dialogueTextFeild.text = dialougeToType.Substring(0, charIndex);

                    yield return null;
                }

                
                dialogueTextFeild.text = dialougeToType;
            }
        }

        if (continueButton.Enabled)
        {
            continueButton.Value.SetActive(true);
        }
        isTyping = false;

    }

    public void TestEvent(string testString) {
        Debug.Log(testString);
        Debug.Log("Event is working as intended");
    }
    

}
