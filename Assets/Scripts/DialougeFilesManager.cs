using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialougeFilesManager : MonoBehaviour
{
    [SerializeField] DialogueSystem ds;
    [SerializeField] bool isComponentInSameObject;
    [SerializeField] char lineStartPrefix = '-';
    [SerializeField] private bool loadImmediantly;
    [SerializeField] private DialogueUIEnabler UiEnabler;

    public string txtFileName;
    public string startingLine;
    string[] txtContent;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (isComponentInSameObject)
        {
            ds = GetComponent<DialogueSystem>();
        }

        yield return null;
        if (loadImmediantly)
        {
            LoadDialogueFromFile(true);

            //testing only 
            ds.endDialougeEvents += TestEndingEvent;
        }
    }

    public void LoadDialogueFromFile(bool loadImmediantly  = true)
    {

        TextAsset txtAsset = (TextAsset)Resources.Load(txtFileName);
        txtContent = txtAsset.text.Split('\n');


        ds.LoadDialouge(SearchDesiredText(), loadImmediantly);
        UiEnabler.EnableUI(true);
    }




    //call this to load dialogue 
    public void LoadDialogueFromFile(string fileNameToLoad, string startingLineToUse = "", bool loadImmediantly = true) {
        txtFileName = fileNameToLoad;
        startingLine = startingLineToUse;

        TextAsset txtAsset = (TextAsset)Resources.Load(txtFileName);
        txtContent = txtAsset.text.Split('\n');

        ds.LoadDialouge(SearchDesiredText(), loadImmediantly);
        UiEnabler.EnableUI(true);
    }




    public string[] SearchDesiredText(){
        if (string.IsNullOrEmpty(startingLine)) {
            return txtContent;
        }


        bool isStartingPointFound = false;
        List<string> desiredText = new List<string>();


        for (int i = 0; i < txtContent.Length; i++)
        {
            if (txtContent[i][0].Equals(lineStartPrefix))
            {
                //Debug.Log(txtContent[i].Substring(1));
                //Debug.Log($"{txtContent[i].Substring(1).GetType()} and { startingLine.ToUpper().GetType()} is the same? : {txtContent[i].Substring(1).ToUpper().GetType() == startingLine.ToUpper().GetType()}");
                
                if (isStartingPointFound == false && txtContent[i].Substring(1,startingLine.Length).ToUpper()  ==  startingLine.ToUpper() )
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

    public void TestEndingEvent() {
        LoadDialogueFromFile("Test");
        ds.endDialougeEvents -= TestEndingEvent; 
    }
}
