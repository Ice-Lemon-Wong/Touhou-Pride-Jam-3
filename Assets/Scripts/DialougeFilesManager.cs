using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialougeFilesManager : MonoBehaviour
{
    [SerializeField] DialogueSystem[] ds;
    [SerializeField] char lineStartPrefix = '-';
    [SerializeField] private bool loadImmediantly;
    [SerializeField] private DialogueUIEnabler UiEnabler;
    

    public string txtFileName;
    public string startingLine;
    string[] txtContent;
	public static int activeDSIndex;

	// Start is called before the first frame update
	IEnumerator Start()
    {
        

        yield return null;
        if (loadImmediantly)
        {
            LoadDialogueFromFile();

            //testing only 
            //ds.endDialougeEvents += TestEndingEvent;


        }
    }

    public void LoadDialogueFromFile(int index = 0,bool loadImmediantly  = true)
    {

        TextAsset txtAsset = (TextAsset)Resources.Load(txtFileName);
        txtContent = txtAsset.text.Split('\n');


        ds[index].LoadDialouge(SearchDesiredText(), loadImmediantly);
		activeDSIndex = index;
		//UiEnabler.EnableUI(true);
	}




    //call this to load dialogue 
    public void LoadDialogueFromFile(int index,string fileNameToLoad, string startingLineToUse = "", bool loadImmediantly = true) {
        txtFileName = fileNameToLoad;
        startingLine = startingLineToUse;

        TextAsset txtAsset = (TextAsset)Resources.Load(txtFileName);
        txtContent = txtAsset.text.Split('\n');

        ds[index].LoadDialouge(SearchDesiredText(), loadImmediantly);
        activeDSIndex = index;
        //UiEnabler.EnableUI(true);
    }


    public void LoadDialogueFromFile(int index,string fileNameToLoad, string startingLineToUse , Action[] endDialogueEvent)
    {
        txtFileName = fileNameToLoad;
        startingLine = startingLineToUse;

        
        TextAsset txtAsset = (TextAsset)Resources.Load(txtFileName);
        txtContent = txtAsset.text.Split('\n');

        ds[index].LoadDialouge(SearchDesiredText(), true);
        ds[index].SetEndEvents(endDialogueEvent);
        activeDSIndex = index;
        //UiEnabler.EnableUI(true);
    }



    public string[] SearchDesiredText(){
        if (string.IsNullOrEmpty(startingLine)) {
            return txtContent;
        }


        bool isStartingPointFound = false;
        List<string> desiredText = new List<string>();
        int stringLength = startingLine.Length;

        for (int i = 0; i < txtContent.Length; i++)
        {
            if (txtContent[i][0].Equals(lineStartPrefix))
            {
                //Debug.Log(txtContent[i] + " " + txtContent[i].Length) ;
                //Debug.Log($"{txtContent[i].Substring(1).GetType()} and { startingLine.ToUpper().GetType()} is the same? : {txtContent[i].Substring(1).ToUpper().GetType() == startingLine.ToUpper().GetType()}");

                stringLength = txtContent[i].Length - 2;
                if (stringLength < 0) stringLength = 0;

                //if (txtContent[i].Length - 1 >= startingLine.Length)
                //{
                //    stringLength = startingLine.Length;
                //}
                //else {
                //    stringLength = txtContent[i].Length-1;
                //}
                //Debug.Log(stringLength);

                if (isStartingPointFound == false && txtContent[i].Substring(1, stringLength ).ToUpper()  ==  startingLine.ToUpper() )

                {
                    Debug.Log("made it");
                    isStartingPointFound = true;
                }
                else if (isStartingPointFound == true) {
                    return desiredText.ToArray();
                }

            } else if (isStartingPointFound == true) {
                desiredText.Add(txtContent[i]);
            }
        }

        if (isStartingPointFound)
        {
            Debug.LogError($"no end point found for {startingLine}, plase put a {lineStartPrefix} at the end of desired texts");
        }
        else {
            Debug.LogError($"the starting line ({startingLine}) does not exist in this file {txtFileName}");
        }
        
        return null;
       
    }


    public string[] SearchDesiredText(string[] textLines, string startingLine)
    {
        if (string.IsNullOrEmpty(startingLine))
        {
            return txtContent;
        }

        bool isStartingPointFound = false;
        List<string> desiredText = new List<string>();


        for (int i = 0; i < textLines.Length; i++)
        {
            if (textLines[i][0].Equals(lineStartPrefix))
            {

                if (textLines[i].Substring(1).ToUpper().Equals(startingLine.ToUpper()) && isStartingPointFound == false)
                {

                    isStartingPointFound = true;
                }
                else if (isStartingPointFound == true)
                {
                    return desiredText.ToArray();
                }

            }
            else if (isStartingPointFound == true)
            {
                desiredText.Add(textLines[i]);
            }
        }

        Debug.LogError($"the starting line ({startingLine}) does not exist in this file {txtFileName}");
        return null;

    }


}
