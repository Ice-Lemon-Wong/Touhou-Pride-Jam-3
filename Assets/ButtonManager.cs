using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
	public DialogueUIEnabler mainDialogueUIEnabler;
	public DialogueUIEnabler subDialogueUIEnabler;
	public GameObject logPanel;
	private float alphaNew;
	private float alphaNewButtons;
	private CanvasGroup canvasGroup;
	public GameObject buttonPanel;
	public CanvasGroup buttonCanvasGroup;
	private float targetAlpha = 1;
	private float targetAlphaButtons = 1;
	private bool isEnabled = false;
	public float toggleSpeed = 7f;
	private bool isHidden = false;
	// Start is called before the first frame update
	void Start()
    {
		canvasGroup = logPanel.GetComponent<CanvasGroup>();
		DisableLogger();
	}

    void HandleInputs() {
        if (Input.GetMouseButton(0) && isHidden) {
            buttonPanel.SetActive(true);
			mainDialogueUIEnabler.EnableUI();
			targetAlphaButtons = 1;
			isHidden = false;
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

        if (buttonCanvasGroup.alpha <= 0.001f) {
			buttonPanel.SetActive(false);
			isHidden = true;
		}

		HandleInputs();
	}

    public void toggleUI() {
        if (DialougeFilesManager.activeDSIndex == 0) {
			if (mainDialogueUIEnabler.isEnabled) {
				mainDialogueUIEnabler.DisableUI();
				targetAlphaButtons = 0;
			} else {
				mainDialogueUIEnabler.EnableUI();
				targetAlphaButtons = 1;
			}
		} else if (DialougeFilesManager.activeDSIndex == 1) {
            if (subDialogueUIEnabler.isEnabled) {
				subDialogueUIEnabler.DisableUI();
				targetAlphaButtons = 0;
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
