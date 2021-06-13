using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
	public DialogueUIEnabler mainDialogueUIEnabler;
	public DialogueUIEnabler subDialogueUIEnabler;
    public DialogueSystem dialogueSystem;
	public DialogueSystem subDialogueSystem;
	public GameObject logPanel;
	private float alphaNew;
	private float alphaNewButtons;
	private CanvasGroup canvasGroup;
	public GameObject buttonPanel;
	public CanvasGroup buttonCanvasGroup;
	private float targetAlpha = 0;
	private float targetAlphaButtons = 0;
	private bool isEnabled = false;
	public float toggleSpeed = 7f;
	private bool isHidden = false;
	private bool canHide = false;
	private bool isTransitioning = false;
	// Start is called before the first frame update
	void Start()
    {
		canvasGroup = logPanel.GetComponent<CanvasGroup>();
		DisableLogger();

		dialogueSystem.initDialogueEvents += showButtonsAndPanels;
		dialogueSystem.requiredEndEvent += canHideFunc;

		//subDialogueSystem.initDialogueEvents += showButtonsAndPanelsSub;
		//subDialogueSystem.requiredEndEvent += canHideFunc;

		buttonCanvasGroup.alpha = 0;
		buttonCanvasGroup.blocksRaycasts = false;


	}

    void HandleInputs() {
        if (Input.GetMouseButton(0) && isHidden && !isTransitioning) {
			if (DialougeFilesManager.activeDSIndex == 0) {
                showButtonsAndPanels();
            } else if (DialougeFilesManager.activeDSIndex == 1) {
				showButtonsAndPanelsSub();
			}
		}
    }

    // Update is called once per frame
    void Update()
    {
        alphaNew = Mathf.Lerp(canvasGroup.alpha, targetAlpha, toggleSpeed * Time.deltaTime);
		canvasGroup.alpha = alphaNew;

        if (canvasGroup.alpha <= 0.001f) {
			logPanel.SetActive(false);
		}

        alphaNewButtons = Mathf.Lerp(buttonCanvasGroup.alpha, targetAlphaButtons, toggleSpeed * Time.deltaTime);
		buttonCanvasGroup.alpha = alphaNewButtons;

        if (canHide) {
            hideButtons();
        }

		HandleInputs();
	}

    void showButtonsAndPanels() {
		buttonCanvasGroup.blocksRaycasts = true;
		buttonPanel.SetActive(true);
        mainDialogueUIEnabler.EnableUI();
		targetAlphaButtons = 1;
		isHidden = false;
		isTransitioning = false;
		canHide = false;
	}

    void showButtonsAndPanelsSub() {
		buttonCanvasGroup.blocksRaycasts = true;
		buttonPanel.SetActive(true);
		subDialogueUIEnabler.EnableUI();
		targetAlphaButtons = 1;
		isHidden = false;
		isTransitioning = false;
		canHide = false;
	}

    void hideButtons() {
		buttonCanvasGroup.blocksRaycasts = false;
		targetAlphaButtons = 0;
		if (buttonCanvasGroup.alpha <= 0.001f) {
			buttonPanel.SetActive(false);
			isHidden = true;
		}
		isTransitioning = true;
	}

    void canHideFunc() {
		canHide = true;
	}

    public void toggleUI() {
        Debug.LogWarning(DialougeFilesManager.activeDSIndex);
        if (DialougeFilesManager.activeDSIndex == 0) {
			if (mainDialogueUIEnabler.isEnabled) {
				mainDialogueUIEnabler.DisableUI();
				targetAlphaButtons = 0;
				isHidden = true;
			} else {
				mainDialogueUIEnabler.EnableUI();
				targetAlphaButtons = 1;
			}
		} else if (DialougeFilesManager.activeDSIndex == 1) {
            if (subDialogueUIEnabler.isEnabled) {
				subDialogueUIEnabler.DisableUI();
				targetAlphaButtons = 0;
				isHidden = true;
			} else {
				subDialogueUIEnabler.EnableUI();
				targetAlphaButtons = 1;
			}
        }
    }

    public void DisableLogger() {
		targetAlpha = 0.0f;
	}

    public void EnableLogger() {
		targetAlpha = 1.0f;
        logPanel.SetActive(true);
	}
}
