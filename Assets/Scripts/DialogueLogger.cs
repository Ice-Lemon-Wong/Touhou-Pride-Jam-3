using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueLogger : MonoBehaviour
{
	public GameObject content;
	public GameObject text;
	public GameObject speakerText;
	private GameObject textObj;
	private bool setInLog = false;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToLog(string incomingText, bool isSpeaker) {
		//GameObject textObj = (GameObject)Instantiate(text);
        if (isSpeaker) {
			GameObject speakerObj = (GameObject)Instantiate(speakerText);
			speakerObj.GetComponentInChildren<TextMeshProUGUI>().text = incomingText;
			speakerObj.transform.SetParent(content.transform, false);
			textObj = null;
			setInLog = false;
		} else {
            if (textObj == null) {
                textObj = (GameObject)Instantiate(text);
            }
			textObj.GetComponentInChildren<TextMeshProUGUI>().text += incomingText;
			textObj.GetComponentInChildren<TextMeshProUGUI>().text += "\n";
			if (!setInLog) {
                textObj.transform.SetParent(content.transform, false);
				setInLog = true;
			}
			
        }
		
	}
}
