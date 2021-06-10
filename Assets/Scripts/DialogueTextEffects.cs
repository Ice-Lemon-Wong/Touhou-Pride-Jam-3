using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DialogueTextEffects : DialogueSystemCommandParser
{
    public TextMeshProUGUI dialogueTextFeild;
	private bool shouldWiggleText = false;
	private bool shouldShakeText = false;

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

	// Start is called before the first frame update
	void Start()
    {
        AddComand("txtEffect", TextEffect);
		//ds.dialogueLineEvent += () => { dialogueTextFeild.color = Color.black; };
		InitCommands();
    }

    // This needs to be lateupdate or else the text will jiggle when waving
    void LateUpdate()
    {
        dialogueTextFeild.ForceMeshUpdate();
        if (shouldWiggleText) {
			wiggleText();
		}

		if (shouldShakeText) {
			shakeText();
		}
	}

    void TextEffect(string[] commandLine) {
		int effectStartIndex = -1;
		int effectEndIndex = -1;

		if (commandLine[1].Substring(0, commandLine[1].Length).ToUpper().Equals("WAVY")) {
			shouldWiggleText = true;

			if (commandLine.Length % 2 == 0) {
				Debug.LogWarning("its pairs");

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
				Debug.LogWarning("its pairs");

				for (int i = 2; i < commandLine.Length - 1; i+=2)
				{
					Int32.TryParse(commandLine[i], out effectStartIndex);
					Int32.TryParse(commandLine[i + 1], out effectEndIndex);

					int[] indexArray = {effectStartIndex, effectEndIndex};

					shakeList.AddRange(indexArray);
				}

			/*if (!Int32.TryParse(commandLine[], out effectStartIndex))
        	{
            	Debug.LogError($"argument '{commandLine[2]}' at index 1 is not a number for effects");
            	return;
        	}*/
			} else {
				Debug.LogWarning("its odd");
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
		} else if (commandLine[1].Substring(0, commandLine[1].Length - 1).ToUpper().Equals("RESET")) {
			Debug.LogWarning("Resetting");
			shouldShakeText = false;
			shouldWiggleText = false;

			waveList.Clear();
			waveLettersList.Clear();
			shakeList.Clear();
			shakeLettersList.Clear();
		}
	}

    void wiggleText() {
        //dialogueTextFeild.ForceMeshUpdate();
		var textInfo = dialogueTextFeild.textInfo;
		for (int i = 0; i < waveLettersList.Count - 1 /*textInfo.characterCount*/; ++i)
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

		for (int i = 0; i < shakeLettersList.Count - 1 /*textInfo.characterCount*/; ++i)
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
}