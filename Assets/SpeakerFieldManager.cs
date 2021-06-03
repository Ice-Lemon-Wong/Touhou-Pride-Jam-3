using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeakerFieldManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerTextFeild;
    [SerializeField] private bool isComponentInSameObject = false;
    [SerializeField] private DialogueSystem ds;
    [SerializeField] private string command;
    

    private string[] currentTextStrings;
    


    // Start is called before the first frame update
    void Start()
    {
        if (isComponentInSameObject) {
            ds=  GetComponent<DialogueSystem>();
        }

        if (ds != null) {
            ds.dialougeLineEvents += CheckSpeaker;
        }

        //speakerTextFeild.text = "";
        
    }

    public void CheckSpeaker(string textCommand) {
        currentTextStrings = textCommand.Split(' ');

        if (currentTextStrings[0].Substring(1, currentTextStrings[0].Length-1).ToUpper().Equals(command.ToUpper())) {

            Debug.Log("switching speaker");
            speakerTextFeild.text = currentTextStrings[1];
        }
    }


}
