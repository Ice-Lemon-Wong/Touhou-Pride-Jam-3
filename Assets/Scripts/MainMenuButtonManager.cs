using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonManager : MonoBehaviour
{
	public Scene_Navigator_Script scene;
	public int creditSceneIndex;

	void Start() {

	}

    void Update() {

    }

	public void PlayButtonPressed() {
		scene.AdvenceScene();
	}

    public void CreditButtonPressed() {
		scene.GoToScene(creditSceneIndex);
	}

    public void BackButtonPressed() {
		//go back to main menu
		scene.GoToScene(0);
	}

    public void ExitButtonPressed() {
		scene.QuitGame();
	}
}
