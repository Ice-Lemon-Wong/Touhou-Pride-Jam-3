using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PortraitAnimationManager : DialogueSystemCommandParser
{
    [Header("animator configs")]
    [SerializeField] string animationCommandSyntax = "animate";
    [SerializeField] string neutralAnimationName = "neutral";
    [SerializeField] PortraitsManager pm;


    [Header("shaking effect configs")]
    [SerializeField] string shakeAnimationName = "shake";
    [SerializeField] float shakeMagnitud;
    [SerializeField] float shakeDuration;

    int portraitNumber = 0;
    Image portraitsToAnimate;
     

    // Start is called before the first frame update
    void Start()
    {
        
        AddComand(animationCommandSyntax, PlayAnimation);
        InitCommands();
        ds.initDialogueEvents += ResetAnimations;
        ds.requiredEndEvent += ResetAnimations;
    }

    void ResetAnimations() { 
        Image[] allPortraits = pm.GetAllPortrait();

        foreach (var item in allPortraits)
        {
            item.GetComponent<Animator>().StopPlayback();
            item.GetComponent<Animator>().Play(neutralAnimationName);
        }
    }

    public void PlayAnimation(string[] commandLine) {
        portraitNumber = -1;

        if (commandLine.Length < 3) {
            Debug.LogError($"animation line '{commandLine}' has too few arguments");
        }


        if (! Int32.TryParse(commandLine[1], out portraitNumber))
        {
            Debug.LogError($"argument '{commandLine[1]}' at index 1 is not a number for protraits");
            return;
        }

        portraitsToAnimate = pm.GetPortrait(portraitNumber);

        if (portraitsToAnimate == null) return;

        if (commandLine[2].Substring(0, commandLine[2].Length - 1) == shakeAnimationName) {
            portraitsToAnimate.GetComponent<Animator>().StopPlayback();
            
            StopCoroutine(ShakePortrait(portraitsToAnimate.transform));
            StartCoroutine(ShakePortrait(portraitsToAnimate.transform));

        } else {
            portraitsToAnimate.GetComponent<Animator>().Play(commandLine[2].Substring(0, commandLine[2].Length - 1));
        }
       
        
            
        
    }

    IEnumerator ShakePortrait(Transform portrait) {
        portrait.GetComponent<Animator>().enabled = false;
        Vector3 originalPos = portrait.localPosition;

        float currentShakePower = shakeMagnitud;
        float duration = shakeDuration;
        float shakeFadeTime = currentShakePower / duration;
        
        while (duration > 0)
        {
            
            float offsetx = UnityEngine.Random.Range(-1f, 1f) * currentShakePower;
            float offsety = UnityEngine.Random.Range(-1f, 1f) * currentShakePower;

            portrait.localPosition = originalPos += new Vector3(offsetx, offsety);

            duration -= Time.deltaTime;

            currentShakePower = Mathf.MoveTowards(currentShakePower, 0f, shakeFadeTime * Time.deltaTime);

            yield return null;

        }
        portrait.GetComponent<Animator>().enabled = true;
        portrait.localPosition = originalPos;

    }

   
}
