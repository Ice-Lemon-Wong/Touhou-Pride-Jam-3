using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PortraitsManager : DialogueSystemCommandParser
{
    [SerializeField] private string portraitCommandText = "portrait";
    [SerializeField] private string portraitDisableText = "disable";
    [SerializeField] private string portraitEnableText = "enable";
    [SerializeField] int numberOfPortraits = 2;
    [SerializeField] float portraitEffectSpeed = 10;
    [SerializeField] float fadingOffSpeed = 10;
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
            portraitDisplays[i] = new PortraitDisplayFeild(portraits[i], portraitParent, false);
            //StartCoroutine(portraitDisplays[i].EnableAnimator());
        }

        AddComand(portraitCommandText, PortraitCommandFunction);
        InitCommands();
        ds.initDialogue += DisablePortraits;
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < portraitDisplays.Length; i++)
        {
            portraitDisplays[i].UpdateProtraitDisplay(portraitEffectSpeed, fadingOffSpeed);
        }
    }

    public void PortraitCommandFunction(string[] commandLine) {

        portraitNumber = -1;
        bool isExpression = true;
        if (! Int32.TryParse(commandLine[1], out portraitNumber))
        {
            Debug.LogError($"argument '{commandLine[1]}' at index 1 is not a number for protraits");
            return;
        }

        if (portraitNumber < 0 || portraitNumber >= portraitDisplays.Length) {
            Debug.LogError($"argument '{commandLine[1]}' at index 1 is not a valid portrait image field index");
            return;
        }

        if (commandLine[2].Substring(0, commandLine[2].Length - 1).ToUpper().Equals(portraitDisableText.ToUpper()))
        {
            portraitDisplays[portraitNumber].Enable(false);
            return;
        }
        else if (commandLine[2].Substring(0, commandLine[2].Length - 1).ToUpper().Equals(portraitEnableText.ToUpper()))
        {
            portraitDisplays[portraitNumber].Enable(true);
            return;
        }

        if (!CheckCharacterExist(commandLine[2])) {
            Debug.LogError($"argument '{commandLine[2]}' at index 2 is not a valid character name");
            return;
        }



        if (!CheckPortraitExist(commandLine[2], commandLine[3]))
        {

            isExpression = false;
            if (!CheckPortraitExist(commandLine[2], commandLine[3],isExpression))
            {
                Debug.LogError($"argument '{commandLine[3]}' at index 3 is not a valid portrait name");
                return;
            }
        }

        portraitDisplays[portraitNumber].ChangePortrait( GetPortratSprite(commandLine[2], commandLine[3],isExpression));

    }

    public bool CheckCharacterExist(string name) {
        
        for (int i = 0; i < characterData.Length; i++)
        {
            if (characterData[i].characterName.ToUpper().Equals(name.ToUpper())) return true;
        }

        return false;
    }

    public bool CheckPortraitExist(string characterName, string expressionName, bool isExpression = true)
    {
        for (int i = 0; i < characterData.Length; i++)
        {
            if (characterData[i].characterName.ToUpper().Equals(characterName.ToUpper())) {

                if (isExpression)
                {
                    for (int j = 0; j < characterData[i].allSpritePortraitData.Length; j++)
                    {

                        //Debug.Log((characterData[i].allSpriteData[j].expressionName.ToUpper()) + "" );
                        //Debug.Log(expressionName.Substring(0, expressionName.Length - 1).ToUpper());
                        if (characterData[i].allSpritePortraitData[j].portraitName.ToUpper() == expressionName.Substring(0, expressionName.Length - 1).ToUpper())
                        {
                            return true;
                        }
                    }
                }
                else {
                    for (int j = 0; j < characterData[i].allSpriteBase.Length; j++)
                    {

                        //Debug.Log((characterData[i].allSpriteData[j].expressionName.ToUpper()) + "" );
                        //Debug.Log(expressionName.Substring(0, expressionName.Length - 1).ToUpper());
                        if (characterData[i].allSpriteBase[j].portraitName.ToUpper() == expressionName.Substring(0, expressionName.Length - 1).ToUpper())
                        {
                            return true;
                        }
                    }
                }

                
            }
        }

        return false;
    }

    public Sprite GetPortratSprite(string characterName, string expressionName, bool isExpression = true) {
        for (int i = 0; i < characterData.Length; i++)
        {
            if (characterData[i].characterName.ToUpper().Equals(characterName.ToUpper()))
            {

                if (isExpression)
                {
                    for (int j = 0; j < characterData[i].allSpritePortraitData.Length; j++)
                    {
                        if (characterData[i].allSpritePortraitData[j].portraitName.ToUpper() == expressionName.Substring(0, expressionName.Length - 1).ToUpper())
                        {
                            return characterData[i].allSpritePortraitData[j].portraitSprite;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < characterData[i].allSpriteBase.Length; j++)
                    { 
                        if (characterData[i].allSpriteBase[j].portraitName.ToUpper() == expressionName.Substring(0, expressionName.Length - 1).ToUpper())
                        {
                            return characterData[i].allSpriteBase[j].portraitSprite;
                        }
                    }
                }

                
            }
        }

        return null;
    }

    public Image GetPortrait(int index) {
        if (index >= 0 && index < portraitDisplays.Length)
        {
            return portraitDisplays[index].GetPortraitImages();
        }
        else {
            Debug.LogError($"index of {index} is not a valid portrait display index to get");
            return null;
        }
    }

    public Image[] GetAllPortrait()
    {
         Image[] returningImage = new Image[portraitDisplays.Length];
        for (int i = 0; i < portraitDisplays.Length; i++)
        {
            returningImage[i] = portraitDisplays[i].GetPortraitImages();
        }

        return returningImage;
    }

    public void EnablePortraits(bool enablePortraits = true) {
        for (int i = 0; i < portraitDisplays.Length; i++)
        {
            portraitDisplays[i].Enable(enablePortraits);
        }
    }
    public void DisablePortraits()
    {
        for (int i = 0; i < portraitDisplays.Length; i++)
        {
            portraitDisplays[i].Enable(false);
        }
    }


    [Serializable]
    struct PortraitCharacter {
        public string characterName;
        public ProtraitSpriteData[] allSpritePortraitData;
        public ProtraitSpriteData[] allSpriteBase;
    }

    [Serializable]
    struct ProtraitSpriteData {
        public string portraitName;
        public Sprite portraitSprite;
    }

    

    struct PortraitDisplayFeild
    {
        public Image expressionImage;
        public Image expressionImageBackup;
        public Image baseImage;
        public Image baseImageeBackup;
        public bool isUsingFrontExpression;
        public bool isUsingFrontBase;
        public bool isPortraitTransitioning;
        private Color colourVar;
        public bool isEnable;

        public PortraitDisplayFeild(Image newPortraitImage, Transform parentObject, bool isEnable = true) {

            baseImage = newPortraitImage;
            expressionImage = newPortraitImage.transform.GetChild(0).GetComponent<Image>();

            baseImageeBackup = Instantiate(baseImage, expressionImage.transform.position, expressionImage.transform.rotation);
            //expressionImageBackup = Instantiate(expressionImage, expressionImage.transform.position, expressionImage.transform.rotation);
            baseImageeBackup.transform.SetParent(parentObject);
            baseImageeBackup.transform.position = baseImage.transform.position;
            baseImageeBackup.transform.localScale = baseImage.transform.localScale;
            baseImageeBackup.transform.rotation = baseImage.transform.rotation;
            expressionImageBackup = baseImageeBackup.transform.GetChild(0).GetComponent<Image>();

            isUsingFrontExpression = true;
            isUsingFrontBase = true;
            isPortraitTransitioning = true;

            

            expressionImageBackup.color = new Color(0, 0, 0, 0);
            colourVar = new Color(1, 1, 1, 1);

            this.isEnable = isEnable;
            if (!isEnable) {
                colourVar.a = 0;
                expressionImage.color = colourVar;
                baseImage.color = colourVar;
                expressionImageBackup.color = colourVar;
                baseImageeBackup.color = colourVar;
            }

            //back up image Ui does not use animator, instead follow the main image UI every frame
            if (baseImageeBackup.GetComponent<Animator>() != null) baseImageeBackup.GetComponent<Animator>().enabled = false;
            
           
        }

        

        public void UpdateProtraitDisplay(float effectSpeed, float fadingOffSpeed) {

            baseImageeBackup.transform.position = baseImage.transform.position;
            baseImageeBackup.transform.localScale = baseImage.transform.localScale;
            baseImageeBackup.transform.rotation = baseImage.transform.rotation;

            if (!isEnable) {
                colourVar.a = Mathf.Lerp(expressionImage.color.a, 0, fadingOffSpeed * Time.deltaTime);
                expressionImage.color = colourVar;
                colourVar.a = Mathf.Lerp(baseImage.color.a, 0, fadingOffSpeed * Time.deltaTime);
                baseImage.color = colourVar;
                colourVar.a = Mathf.Lerp(expressionImageBackup.color.a, 0, fadingOffSpeed * Time.deltaTime);
                expressionImageBackup.color = colourVar;
                colourVar.a = Mathf.Lerp(baseImageeBackup.color.a, 0, fadingOffSpeed * Time.deltaTime);
                baseImageeBackup.color = colourVar;
                return;
            }

            

            if (isUsingFrontExpression)
            {
                colourVar.a = Mathf.Lerp(expressionImage.color.a, 1, effectSpeed * Time.deltaTime);
                expressionImage.color = colourVar;
                colourVar.a = Mathf.Lerp(baseImage.color.a, 1, effectSpeed * Time.deltaTime);
                baseImage.color = colourVar;

                colourVar.a = Mathf.Lerp(expressionImageBackup.color.a, 0, effectSpeed * Time.deltaTime);
                expressionImageBackup.color = colourVar;
                colourVar.a = Mathf.Lerp(baseImageeBackup.color.a, 0, effectSpeed * Time.deltaTime);
                baseImageeBackup.color = colourVar;
            }
            else {
                colourVar.a = Mathf.Lerp(expressionImage.color.a, 0, effectSpeed * Time.deltaTime);
                expressionImage.color = colourVar;
                colourVar.a = Mathf.Lerp(baseImage.color.a, 0, effectSpeed * Time.deltaTime);
                baseImage.color = colourVar;

                colourVar.a = Mathf.Lerp(expressionImageBackup.color.a, 1, effectSpeed * Time.deltaTime);
                expressionImageBackup.color = colourVar;
                colourVar.a = Mathf.Lerp(baseImageeBackup.color.a, 1, effectSpeed * Time.deltaTime);
                baseImageeBackup.color = colourVar;
            }

            if (isUsingFrontBase)
            {
                colourVar.a = Mathf.Lerp(baseImage.color.a, 1, effectSpeed * Time.deltaTime);
                baseImage.color = colourVar;
               
                colourVar.a = Mathf.Lerp(baseImageeBackup.color.a, 0, effectSpeed * Time.deltaTime);
                baseImageeBackup.color = colourVar;
            }
            else {
                colourVar.a = Mathf.Lerp(baseImage.color.a, 0, effectSpeed * Time.deltaTime);
                baseImage.color = colourVar;

                colourVar.a = Mathf.Lerp(baseImageeBackup.color.a, 1, effectSpeed * Time.deltaTime);
                baseImageeBackup.color = colourVar;
            }
        }

        

        public void Enable(bool shouldEnable = true) {
            isEnable = shouldEnable;

            //if (isEnable)
            //{
            //    if (isUsingFrontExpression)
            //    {
            //        colourVar.a = 1;
            //        expressionImage.color = colourVar;
            //        baseImage.color = colourVar;

            //        colourVar.a = 0;
            //        expressionImageBackup.color = colourVar;
            //        baseImageeBackup.color = colourVar;
            //    }
            //    else
            //    {
            //        colourVar.a = 0;
            //        expressionImage.color = colourVar;
            //        baseImage.color = colourVar;

            //        colourVar.a = 1;
            //        expressionImageBackup.color = colourVar;
            //        baseImageeBackup.color = colourVar;
            //    }
            //}
            //else {
            //    colourVar.a = 0;
            //    expressionImage.color = colourVar;
            //    baseImage.color = colourVar;
            //    expressionImageBackup.color = colourVar;
            //    baseImageeBackup.color = colourVar;
            //}
        }

        public void ChangePortrait(Sprite portraitSprite, bool isExpression = true)
        {
            if (isExpression)
            {
                if (isUsingFrontExpression)
                {
                    expressionImageBackup.sprite = portraitSprite;
                }
                else
                {
                    expressionImage.sprite = portraitSprite;
                }
                isUsingFrontExpression = !isUsingFrontExpression;
            }
            else {

                if (isUsingFrontBase)
                {
                    baseImageeBackup.sprite = portraitSprite;
                }
                else
                {
                    baseImage.sprite = portraitSprite;
                }
                isUsingFrontBase = !isUsingFrontBase;
            }
            

        }


        public Image GetPortraitImages() {
            return baseImage ;
        }

        
    }
}
