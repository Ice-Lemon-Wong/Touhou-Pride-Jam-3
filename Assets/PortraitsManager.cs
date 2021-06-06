using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PortraitsManager : DialogueSystemCommandParser
{
    [SerializeField] private string portraitCommandText = "portrait";
    [SerializeField] int numberOfPortraits = 2;
    [SerializeField] float portraitEffectSpeed = 10;
    [SerializeField] Image[] portraits;
    [SerializeField] Transform portraitParent;

    [SerializeField] PortraitCharacter[] characterData;
    private PortraitDisplayFeild[] portraitDisplays;
    

    int portraitNumber = -1;


    // Start is called before the first frame update
    void Start()
    {
        portraitDisplays = new PortraitDisplayFeild[portraits.Length];
        for (int i = 0; i < portraits.Length; i++)
        {
            portraitDisplays[i] = new PortraitDisplayFeild(portraits[i], portraitParent);
        }

        AddComand(portraitCommandText, PortraitCommandFunction);
        InitCommands();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < portraitDisplays.Length; i++)
        {
            portraitDisplays[i].UpdateProtraitDisplay(portraitEffectSpeed);
        }
    }

    public void PortraitCommandFunction(string[] commandLine) {

        portraitNumber = -1;
        if (! Int32.TryParse(commandLine[1], out portraitNumber))
        {
            Debug.LogError($"argument '{commandLine[1]}' at index 1 is not a number for protraits");
            return;
        }

        if (portraitNumber < 0 || portraitNumber >= portraitDisplays.Length) {
            Debug.LogError($"argument '{commandLine[1]}' at index 1 is not a valid portrait image field index");
            return;
        }

        if (!CheckCharacterExist(commandLine[2])) {
            Debug.LogError($"argument '{commandLine[2]}' at index 2 is not a valid character name");
            return;
        }

        if (!CheckCharacterExist(commandLine[2]))
        {
            Debug.LogError($"argument '{commandLine[2]}' at index 2 is not a valid character name");
            return;
        }

        if (!CheckPortraitExist(commandLine[2], commandLine[3]))
        {
            Debug.LogError($"argument '{commandLine[3]}' at index 3 is not a valid expression name");
            return;
        }

        portraitDisplays[portraitNumber].ChangePortrait( GetPortratSprite(commandLine[2], commandLine[3]));

    }

    public bool CheckCharacterExist(string name) {
        
        for (int i = 0; i < characterData.Length; i++)
        {
            if (characterData[i].characterName.ToUpper().Equals(name.ToUpper())) return true;
        }

        return false;
    }

    public bool CheckPortraitExist(string characterName, string expressionName)
    {
        for (int i = 0; i < characterData.Length; i++)
        {
            if (characterData[i].characterName.ToUpper().Equals(characterName.ToUpper())) {


                for (int j = 0; j < characterData[i].allSpriteData.Length; j++)
                {
                    
                    //Debug.Log((characterData[i].allSpriteData[j].expressionName.ToUpper()) + "" );
                    //Debug.Log(expressionName.Substring(0, expressionName.Length - 1).ToUpper());
                    if (characterData[i].allSpriteData[j].expressionName.ToUpper() == expressionName.Substring(0, expressionName.Length - 1).ToUpper()) { 
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public Sprite GetPortratSprite(string characterName, string expressionName) {
        for (int i = 0; i < characterData.Length; i++)
        {
            if (characterData[i].characterName.ToUpper().Equals(characterName.ToUpper()))
            {

                for (int j = 0; j < characterData[i].allSpriteData.Length; j++)
                {
                    //Debug.Log(characterData[i].allSpriteData[j].portraitSprite.name);
                    //Debug.Log(expressionName.Substring(0, expressionName.Length - 1).ToUpper());
                    if (characterData[i].allSpriteData[j].expressionName.ToUpper() == expressionName.Substring(0, expressionName.Length - 1).ToUpper())
                    {
                        return characterData[i].allSpriteData[j].portraitSprite;
                    }
                   
                }
            }
        }

        return null;
    }

    [Serializable]
    struct PortraitCharacter {
        public string characterName;
        public ProtraitSpriteData[] allSpriteData;
    }

    [Serializable]
    struct ProtraitSpriteData {
        public string expressionName;
        public Sprite portraitSprite;
    }

    struct PortraitDisplayFeild
    {
        public Image portraitImage;
        public Image backUpPortraitImage;
        public bool isUsingFront;
        private Color colourVar;

        public PortraitDisplayFeild(Image newPortraitImage, Transform parentObject) {
            portraitImage = newPortraitImage;
            backUpPortraitImage = Instantiate(portraitImage, portraitImage.transform.position, portraitImage.transform.rotation);
            backUpPortraitImage.transform.parent = parentObject;
            backUpPortraitImage.transform.position = portraitImage.transform.position;
            backUpPortraitImage.transform.localScale = portraitImage.transform.localScale;
            backUpPortraitImage.transform.rotation = portraitImage.transform.rotation;
            isUsingFront = true;
            backUpPortraitImage.color = new Color(0, 0, 0, 0);
            colourVar = new Color(1, 1, 1, 1);
        }

        public void UpdateProtraitDisplay(float effectSpeed) {
            if (isUsingFront)
            {
                colourVar.a = Mathf.Lerp(portraitImage.color.a, 1, effectSpeed * Time.deltaTime);
                portraitImage.color = colourVar;
                colourVar.a = Mathf.Lerp(backUpPortraitImage.color.a, 0, effectSpeed * Time.deltaTime);
                backUpPortraitImage.color = colourVar;
            }
            else {
                colourVar.a = Mathf.Lerp(portraitImage.color.a, 0, effectSpeed * Time.deltaTime);
                portraitImage.color = colourVar;
                colourVar.a = Mathf.Lerp(backUpPortraitImage.color.a, 1, effectSpeed * Time.deltaTime);
                backUpPortraitImage.color = colourVar;
            }
        }

        public void ChangePortrait(Sprite portraitSprite)
        {
            if (isUsingFront)
            {
                backUpPortraitImage.sprite = portraitSprite;
            }
            else
            {
                portraitImage.sprite = portraitSprite;
            }
            isUsingFront = !isUsingFront;

        }

    }
}
