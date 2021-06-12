using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIEnabler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] textUI;
    [SerializeField] Image[] imageUI;
    [SerializeField] DialogueSystem ds;
    [SerializeField] float effectSpeed = 5;
    float disableDelayAtEnd;
    public bool isEnabled = false;
    private Color colourVar = new Color(1,1,1,1);
    private float targetAlpha = 0;

    private void Start()
    {
        disableDelayAtEnd = ds.endDelay;
        ds.initDialogueEvents += EnableUI;
        ds.requiredEndEvent += DisableUI;
        colourVar = new Color(1, 1, 1, 1);
        isEnabled = false;

        for (int i = 0; i < textUI.Length; i++)
        {
            colourVar = textUI[i].color;
            colourVar.a = 0;
            textUI[i].color = colourVar;
        }

        for (int i = 0; i < imageUI.Length; i++)
        {
            colourVar = Color.white;
            colourVar.a = 0;
            imageUI[i].color = colourVar;
        }
    }

    private void Update()
    {
        if (isEnabled) targetAlpha = 1; else targetAlpha = 0;

        for (int i = 0; i < textUI.Length; i++)
        {
            colourVar = textUI[i].color;
            colourVar.a = Mathf.Lerp(textUI[i].color.a, targetAlpha, effectSpeed * Time.deltaTime);
            textUI[i].color = colourVar;
        }

        for (int i = 0; i < imageUI.Length; i++)
        {
            colourVar = Color.white;
            colourVar.a = Mathf.Lerp(imageUI[i].color.a, targetAlpha, effectSpeed * Time.deltaTime);
            imageUI[i].color = colourVar;
        }
    }

    public void EnableUI(bool shouldEnable = true) {
        isEnabled = shouldEnable;
    }

    public void EnableUI()
    {
        isEnabled = true;
    }

    public void DisableUI()
    {
        disableDelayAtEnd = ds.endDelay;
        StartCoroutine(DisableUIRoutine());
    }

    IEnumerator DisableUIRoutine() {
        yield return new WaitForSeconds(disableDelayAtEnd);
        isEnabled = false;
    }

}
