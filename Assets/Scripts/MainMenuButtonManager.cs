using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonManager : MonoBehaviour
{
	public GameObject mainTitle;
	public GameObject credits;
	private CanvasGroup mainTitleCanvas;
	private CanvasGroup creditsCanvas;
	private float alphaNew;
	private float targetAlphaMainTitle;
	private float targetAlphaCredits;
	private bool shouldShowMainMenu = true;
	private bool shouldShowCredits = false;
	public float toggleSpeed = 5f;
	public Scene_Navigator_Script scene;

	void Start() {
		mainTitleCanvas = mainTitle.GetComponent<CanvasGroup>();
		creditsCanvas = credits.GetComponent<CanvasGroup>();

		mainTitle.SetActive(true);
		credits.SetActive(false);

		targetAlphaMainTitle = 1;
		targetAlphaCredits = 0;
	}

    void Update() {
        if (shouldShowMainMenu) {
			showMainMenu();
		} else if (shouldShowCredits) {
			showCredits();
		}

        alphaNew = Mathf.Lerp(mainTitleCanvas.alpha, targetAlphaMainTitle, toggleSpeed * Time.deltaTime);
		mainTitleCanvas.alpha = alphaNew;

        alphaNew = Mathf.Lerp(creditsCanvas.alpha, targetAlphaCredits, toggleSpeed * Time.deltaTime);
		creditsCanvas.alpha = alphaNew;
    }

    private void showMainMenu() {
		mainTitle.SetActive(true);
        credits.SetActive(false);
	}

    private void showCredits() {
		credits.SetActive(true);
        mainTitle.SetActive(false);
	}

	public void PlayButtonPressed() {
		scene.AdvenceScene();
	}

    public void CreditButtonPressed() {
		targetAlphaMainTitle = 0;
		targetAlphaCredits = 1;

        shouldShowCredits = true;
		shouldShowMainMenu = false;
	}

    public void BackButtonPressed() {
        targetAlphaMainTitle = 1;
        targetAlphaCredits = 0;

		shouldShowCredits = false;
		shouldShowMainMenu = true;
	}

    public void ExitButtonPressed() {
		scene.QuitGame();
	}
}
