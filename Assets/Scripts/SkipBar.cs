using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkipBar : MonoBehaviour
{
	public DialogueSystem dialogueSystem;
	public DialogueSystem subDialogueSystem;
	public Image[] images;
	public TextMeshProUGUI textMeshProUGUI;
	public static bool keyHeld = false;
	public float sliderSpeed;
	public float skipSpeed = 0.5f;
	private Color tmp;
	private bool shouldSkip = false;
	private bool skipped = false;
	public static bool enableInput = false;
	private bool canHoldNow = false;
	// Start is called before the first frame update
	void Start()
    {
        foreach (var image in images)
        {
			tmp = image.color;
			tmp.a = 0;
			image.color = tmp;
		}

		tmp = textMeshProUGUI.color;
		tmp.a = 0;
		textMeshProUGUI.color = tmp;

		dialogueSystem.initDialogueEvents += EnableSkip;
		dialogueSystem.requiredEndEvent += DisableSkip;

		subDialogueSystem.initDialogueEvents += EnableSkip;
		subDialogueSystem.requiredEndEvent += DisableSkip;

		DisableSkip();
	}

    public void EnableSkip() {
		enableInput = true;
	}

    public void DisableSkip() {
		enableInput = false;
		canHoldNow = false;
	}

    // Update is called once per frame
    void Update()
    {
        if (enableInput) {
            handleInputs();
        } else {
            //This is stupid but it works.
			keyHeld = false;
			StopCoroutine(skip());
		}

		if (DialogueSystem.doneSkipping) {
			canHoldNow = true;
		}
        
        if (keyHeld && canHoldNow) {
			StartCoroutine(expandSliders());
		} else {
			StartCoroutine(contractSliders());
		}

        if (shouldSkip && !skipped) {
			DialogueSystem.setTypingSpeed(500);
			StartCoroutine(skip());
		}
	}

    void handleInputs() {
        if (Input.GetKey(KeyCode.Space)) {
			keyHeld = true;
		} else {
			keyHeld = false;
			if (!DialogueTextEffects.isSpeedAltered) {
				DialogueSystem.setDefaultTypingSpeed();
			}
		}
    }

    IEnumerator expandSliders() {
        foreach (var image in images)
        {
			image.fillAmount += 0.01f * sliderSpeed;
			tmp = image.color;
			tmp.a += 0.01f * sliderSpeed;

            //must be >=
		    if (image.fillAmount >= 1) {
			    image.fillAmount = 1;
			    shouldSkip = true;
		    } else {
			    shouldSkip = false;
		    }

            if (tmp.a >= 1) {
			    tmp.a = 1;
		    }

		    image.color = tmp;

			tmp = textMeshProUGUI.color;
			tmp.a = image.color.a;
			textMeshProUGUI.color = tmp;
		}

		yield return new WaitForSeconds(0.1f);
	}

    IEnumerator skip() {
		if (shouldSkip && !skipped) {
			if (DialougeFilesManager.activeDSIndex == 0) {
				dialogueSystem.SkipDialogue();
			} else if (DialougeFilesManager.activeDSIndex == 1) {
				subDialogueSystem.SkipDialogue();
			}
			skipped = true;
		}
		//yield return new WaitForSeconds(skipSpeed);
		yield return new WaitUntil(() => DialogueSystem.doneSkipping);
		DialogueSystem.doneSkipping = false;

		skipped = false;
	}

    IEnumerator contractSliders() {
        foreach (var image in images)
        {
			image.fillAmount -= 0.01f * sliderSpeed;
            tmp = image.color;
            tmp.a -= 0.01f * sliderSpeed;
            shouldSkip = false;

		    if (image.fillAmount <= 0) {
			    image.fillAmount = 0;
			    
		    } 

            if (tmp.a <= 0) {
			    tmp.a = 0;
		    }

		    image.color = tmp;
            tmp = textMeshProUGUI.color;
			tmp.a = image.color.a;
            textMeshProUGUI.color = tmp;
		}

		yield return new WaitForSeconds(0.1f);
    }

	public bool getSkipStatus() {
		return shouldSkip && !skipped;
	}
}
