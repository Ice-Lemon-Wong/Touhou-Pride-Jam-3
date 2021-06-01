using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private string[] dialougeTexts;
    [SerializeField] private TextMeshProUGUI dialogueTextFeild;
    [SerializeField] private TextMeshProUGUI speakerTextFeild;
    [SerializeField] private float textSpeed;

    

    private int currentDialougeIndex = 0;
    private bool isInterupt = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdvanceDialouge() {
        currentDialougeIndex++;
        if (currentDialougeIndex < dialougeTexts.Length)
        {
            ReadDialouge(dialougeTexts[currentDialougeIndex]);
        }
        else {
            dialogueTextFeild.text = "";
            //fire events
        }
    }

    public void ReadDialouge(string dialougeToType) {
        StopCoroutine("typeDialougeRoutine");
        StartCoroutine(TypeDialougeRoutine(dialougeToType));
    }

    public void InteruptDialouge() {
        isInterupt = true;
    }


    IEnumerator  TypeDialougeRoutine(string dialougeToType) {

        float t = 0;
        int charIndex = 0;
        isInterupt = false;
        dialogueTextFeild.text = "";

        while (charIndex < dialougeToType.Length)
        {
            if (!isInterupt)
            {
                charIndex = dialougeToType.Length;
            }
            else {
                t += Time.deltaTime;
                charIndex = Mathf.FloorToInt(t);
                charIndex = Mathf.Clamp(charIndex, 0, dialougeToType.Length);
            }


            dialogueTextFeild.text = dialougeToType.Substring(0, charIndex);

            yield return null;
        }

    }

}
