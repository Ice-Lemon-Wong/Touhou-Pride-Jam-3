using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

//note
//dialouge cannot be started in the start() call because it needs to wait for events

public class DialogueSystem : MonoBehaviour
{
    [Header("dialogue configs")]
    [SerializeField] private string[] dialougeTexts;
    [SerializeField] private TextMeshProUGUI dialogueTextFeild;
    [SerializeField] private float typingSpeed;
    [SerializeField] Optional<string> interuptInput;
    [SerializeField] Optional<string> advancingInput;
    [SerializeField] private bool canUseExternalControls = true;
    [SerializeField] public Color defaulrColour;

    [Space]
    [Header("timing configs")]
    [SerializeField] private bool startImmediantly;
    [SerializeField] public float startDelay;
    [SerializeField] public float endDelay;

    [Space]
    [Header("command parsing configs")]
    [SerializeField] private char commandPrefix;
    [SerializeField] private bool skipEmpty;
    [SerializeField] private char[] ignorePrefix;

    [Space]
    [Header("extra config ")]
    [SerializeField] Optional<GameObject> continueButton;
    



    public Action<string> dialogueCommandEvents;
    public Action dialogueLineEvent;
    public Action endDialougeEvents;
    public Action dialogueStartEvents;

    public Action initDialogueEvents;
    public Action requiredEndEvent;

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
            StartCoroutine(StartDialogueRoutine());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!canUseExternalControls) return;

        HandleInputs();
    }

    private void HandleInputs()
    {
        if (interuptInput.Enabled)
        {
            if (Input.GetButtonDown(interuptInput.Value))
            {
                if (!isInterupt)
                {
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
        dialogueStartEvents = null;
        endDialougeEvents = null;
        if (startImmediantly) {
            StartDialogue();
        }
    }

    public void StartDialogue() {
        if (dialougeTexts == null) return;

        StartCoroutine(StartDialogueRoutine());
    }

    IEnumerator StartDialogueRoutine() {
        initDialogueEvents?.Invoke();
        dialogueTextFeild.color = defaulrColour;
        yield return new WaitForSeconds(startDelay);
        dialogueStartEvents?.Invoke();
       
        currentDialougeIndex = 0;
        TypeDialouge();
    }

    public void AdvanceDialogue() {
        if (!canUseExternalControls) return;
        StopCoroutine("TypeDialougeRoutine");
        if (dialougeTexts == null) return;

        if (continueButton.Enabled)
        {
            continueButton.Value.SetActive(false);
        }


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

            StartCoroutine(EndDialogueRoutine());            
        }
    }

    public void SkipDialogue() {
        currentDialougeIndex = dialougeTexts.Length;
        AdvanceDialogue();
    }

    IEnumerator EndDialogueRoutine() {
        requiredEndEvent?.Invoke();
        dialogueTextFeild.text = "";
        yield return new WaitForSeconds(endDelay);
        
        //fire events
        endDialougeEvents?.Invoke();
        
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


    public void SetStartEvents(Action[] events) {
        dialogueStartEvents = null;
        foreach (var item in events)
        {
            dialogueStartEvents += item;
        }
        
    }

    public void SetEndEvents(Action[] events)
    {

        endDialougeEvents = null;
        foreach (var item in events)
        {
            endDialougeEvents += item;
        }

    }

    //create a wait time vaiable that is public

    IEnumerator  TypeDialougeRoutine(string dialougeToType) {


        float t = 0;
        int charIndex = 0;
        isInterupt = false;
        
        isTyping = true;

        if (continueButton.Enabled)
        {
            continueButton.Value.SetActive(false);
        }

        Debug.Log(dialougeToType);
        if (skipEmpty && (string.IsNullOrWhiteSpace(dialougeToType)  )) {
            SkipDialougeTyping();
        }else {

           
            Debug.Log("made it to comands");
            dialogueCommandEvents?.Invoke(dialougeToType);
            //make sure you can change wait time here woth the command fucntion

            bool isSkipPrefix = false;

            for (int i = 0; i < ignorePrefix.Length; i++)
            {
                if (dialougeToType[0].Equals(ignorePrefix[i])){
                    isSkipPrefix = true;
                }
            }

            Debug.Log($"shoud skip?: { dialougeToType[0] } {isSkipPrefix} ");
            if (dialougeToType[0].Equals(commandPrefix) || isSkipPrefix)
            {
               //wait even if wait time is 0
               //reset wait time after wait
                SkipDialougeTyping();
            } 
            else
            {
				//dialougeToType = dialougeTextEffects.UpdateText(dialougeToType);
				dialogueTextFeild.text = dialougeToType;
				//dialougeTextEffects.UpdateText(dialougeToType);

				//typing effect
				while (charIndex < dialougeToType.Length)
                {

                    //for whatever reason this needs to be in the loop to not show
                    if (continueButton.Enabled)
                    {
                        continueButton.Value.SetActive(false);
                    }

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

					//process here

					dialogueTextFeild.maxVisibleCharacters = charIndex;
					//dialogueTextFeild.text = dialougeToType.Substring(0, charIndex);

					yield return null;
                }

                

                //dialogueTextFeild.text = dialougeToType;

                if (continueButton.Enabled)
                {
                    continueButton.Value.SetActive(true);
                }
            }
        }

        
        isTyping = false;



    }

    public void TestEvent(string testString) {
        
        Debug.Log(testString);
        Debug.Log("Event is working as intended");
    }
    

}
