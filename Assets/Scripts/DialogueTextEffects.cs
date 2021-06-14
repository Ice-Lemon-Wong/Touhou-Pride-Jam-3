using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DialogueTextEffects : DialogueSystemCommandParser
{
    public TextMeshProUGUI dialogueTextFeild;
	private bool shouldWaveText = false;
	private bool shouldShakeText = false;
	private bool shouldSpeedText = false;

	[Header("Wave Effect Configurations")]
	[SerializeField] private float waveSpeed = 1f;
	[SerializeField] private float waveWavelength = 0.01f;
	[SerializeField] private float waveHeight = 5f;

	[Header("Shake Effect Configurations")]
	[SerializeField] private float shakePower = 0.5f;

	private List<int> waveList = new List<int>();
	private List<bool> waveLettersList = new List<bool>();
	private List<int> shakeList = new List<int>();
	private List<bool> shakeLettersList = new List<bool>();
	private List<int> speedList = new List<int>();
	private List<bool> speedLettersList = new List<bool>();

	public static bool isSpeedAltered = false;
	public SkipBar skipBar;

	// Start is called before the first frame update
	void Start()
    {
        AddComand("text", TextEffect);
		//ds.dialogueLineEvent += () => { dialogueTextFeild.color = Color.black; };
		InitCommands();
    }

    // This needs to be lateupdate or else the text will jiggle when waving
    void LateUpdate()
    {
        dialogueTextFeild.ForceMeshUpdate();
        if (shouldWaveText) {
			waveText();
		}

		if (shouldShakeText) {
			shakeText();
		}

		if (shouldSpeedText) {
			speedText();
		}
	}

	//Handles all the text effect parameters for 'text' command
	//Supported params: wave, shake, typespeed, wait, reset
    void TextEffect(string[] commandLine) {
		int effectStartIndex = -1;
		int effectEndIndex = -1;

		if (commandLine[1].Substring(0, commandLine[1].Length).ToUpper().Equals("WAVE")) {
			shouldWaveText = true;

			if (commandLine.Length % 2 == 0) {
				for (int i = 2; i < commandLine.Length - 1; i+=2)
				{
					Int32.TryParse(commandLine[i], out effectStartIndex);
					Int32.TryParse(commandLine[i + 1], out effectEndIndex);

					int[] indexArray = {effectStartIndex, effectEndIndex};

					waveList.AddRange(indexArray);
				}
			} 
			

			int counter = 0;
			for (int i = 0; i < waveList[waveList.Count - 1]; i++) {
				int currentBeginningIndex = waveList[counter];
				int currentEndIndex = waveList[counter + 1];

				if (i >= currentBeginningIndex && i <= currentEndIndex) {
					waveLettersList.Add(true);
				} else {
					waveLettersList.Add(false);
				}

				if (i > currentEndIndex && waveList.Count > 2) {
					counter += 2;
				}
			}
		} else if (commandLine[1].Substring(0, commandLine[1].Length).ToUpper().Equals("SHAKE")) {
			shouldShakeText = true;

			if (commandLine.Length % 2 == 0) {
				for (int i = 2; i < commandLine.Length - 1; i+=2)
				{
					Int32.TryParse(commandLine[i], out effectStartIndex);
					Int32.TryParse(commandLine[i + 1], out effectEndIndex);

					int[] indexArray = {effectStartIndex, effectEndIndex};

					shakeList.AddRange(indexArray);
				}
			} 

			int counter = 0;
			for (int i = 0; i < shakeList[shakeList.Count - 1]; i++) {
				int currentBeginningIndex = shakeList[counter];
				int currentEndIndex = shakeList[counter + 1];

				if (i >= currentBeginningIndex && i <= currentEndIndex) {
					shakeLettersList.Add(true);
				} else {
					shakeLettersList.Add(false);
				}

				if (i > currentEndIndex && shakeList.Count > 2) {
					counter += 2;
				}
			}
		} else if (commandLine[1].Substring(0, commandLine[1].Length).ToUpper().Equals("TYPESPEED")) {
			isSpeedAltered = true;
			shouldSpeedText = true;
			int speedIndex = 0;

			if (commandLine.Length % 3 == 2) {
				for (int i = 2; i < commandLine.Length - 1; i += 3)
				{
					Int32.TryParse(commandLine[i], out speedIndex);
					Int32.TryParse(commandLine[i + 1], out effectStartIndex);
					Int32.TryParse(commandLine[i + 2], out effectEndIndex);
					

					int[] indexArray = {speedIndex, effectStartIndex, effectEndIndex};

					speedList.AddRange(indexArray);
				}
			} 

			int counter = 0;
			for (int i = 0; i < speedList[speedList.Count - 1]; i++) {
				int currentBeginningIndex = speedList[counter + 1];
				int currentEndIndex = speedList[counter + 2];

				if (i >= currentBeginningIndex && i <= currentEndIndex) {
					speedLettersList.Add(true);
				} else {
					speedLettersList.Add(false);
				}
			}
		} else if (commandLine[1].Substring(0, commandLine[1].Length).ToUpper().Equals("WAIT")) {
			int waitTimeLocal = 0;
			Int32.TryParse(commandLine[2], out waitTimeLocal);

			ds.waitTime = waitTimeLocal;
		} else if (commandLine[1].Substring(0, commandLine[1].Length).ToUpper().Equals("RESET")) {
			Debug.LogWarning("Resetting");
			if (commandLine[2].Substring(0, commandLine[2].Length - 1).ToUpper().Equals("ALL")) {
				shouldShakeText = false;
				shouldWaveText = false;
				shouldSpeedText = false;
				isSpeedAltered = false;

				waveList.Clear();
				waveLettersList.Clear();
				shakeList.Clear();
				shakeLettersList.Clear();
				speedList.Clear();
				speedLettersList.Clear();

				if (!skipBar.getSkipStatus()) {
					DialogueSystem.setDefaultTypingSpeed();
				}
			} else if (commandLine[2].Substring(0, commandLine[2].Length - 1).ToUpper().Equals("SHAKE")) {
				shouldShakeText = false;

				shakeList.Clear();
				shakeLettersList.Clear();
			} else if (commandLine[2].Substring(0, commandLine[2].Length - 1).ToUpper().Equals("WAVE")) {
				shouldWaveText = false;

				waveList.Clear();
				waveLettersList.Clear();
			} else if (commandLine[2].Substring(0, commandLine[2].Length - 1).ToUpper().Equals("TYPESPEED")) {
				shouldSpeedText = false;
				isSpeedAltered = false;

				speedList.Clear();
				speedLettersList.Clear();

				DialogueSystem.setDefaultTypingSpeed();
			}
			
		}
	}

    void waveText() {
		var textInfo = dialogueTextFeild.textInfo;
		for (int i = 0; i < waveLettersList.Count - 1; ++i)
        {
			if (waveLettersList[i] == true) {
				var charInfo = textInfo.characterInfo[i];

            	if (!charInfo.isVisible) {
					continue;
				}

				var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            	for (int j = 0; j < 4; ++j)
            	{
					var orig = verts[charInfo.vertexIndex + j];
					verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * waveSpeed + orig.x * waveWavelength) * waveHeight, 0);
				}
			}
			
		}

        for (int i = 0; i < textInfo.meshInfo.Length; ++i)
        {
			var meshInfo = textInfo.meshInfo[i];
			meshInfo.mesh.vertices = meshInfo.vertices;
			dialogueTextFeild.UpdateGeometry(meshInfo.mesh, i);
		}
    }

	void shakeText() {
		var textInfo = dialogueTextFeild.textInfo;

		for (int i = 0; i < shakeLettersList.Count - 1; ++i)
        {
			if (shakeLettersList[i] == true) {
				var charInfo = textInfo.characterInfo[i];

            	if (!charInfo.isVisible) {
					continue;
				}

				var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
				Vector3 shakeVector = new Vector3(UnityEngine.Random.Range(-shakePower, shakePower), UnityEngine.Random.Range(-shakePower, shakePower));

				for (int j = 0; j < 4; ++j)
            	{
					var orig = verts[charInfo.vertexIndex + j];
					verts[charInfo.vertexIndex + j] = orig + shakeVector;
				}
			}
			
		}

        for (int i = 0; i < textInfo.meshInfo.Length; ++i)
        {
			var meshInfo = textInfo.meshInfo[i];
			meshInfo.mesh.vertices = meshInfo.vertices;
			dialogueTextFeild.UpdateGeometry(meshInfo.mesh, i);
		}
	}

	void speedText() {
		if (dialogueTextFeild.maxVisibleCharacters < speedLettersList.Count) {
			if (speedLettersList[dialogueTextFeild.maxVisibleCharacters] == true) {
				DialogueSystem.setTypingSpeed(speedList[0]);
			} 
		} else {
			DialogueSystem.setDefaultTypingSpeed();
		}
		
	}
}