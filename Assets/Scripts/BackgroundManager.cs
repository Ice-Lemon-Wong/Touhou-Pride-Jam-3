using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{

    public static BackgroundManager insatnceBGM;

    //singleton pattern
    private void Awake()
    {
        if (insatnceBGM == null)
        {
            insatnceBGM = this;
        }

        if (insatnceBGM != this)
        {
            Destroy(gameObject);
        }

    }


    [SerializeField] Image[] backgrounds;
    [SerializeField] int activeBGIndex = 0;
    [SerializeField] float effectSpeed = 4;
    
    private Color colourVar = new Color(1, 1, 1, 1);
    

    // Start is called before the first frame update
    void Start()
    {
        colourVar = new Color(1, 1, 1, 1);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            colourVar = backgrounds[i].color;
            if (i == activeBGIndex)
            {
                colourVar.a = 1;
            }
            else
            {
                colourVar.a = 0;
            }

            backgrounds[i].color = colourVar;
        }

    }

    // Update is called once per frame
    void Update()
    {
        

        for (int i = 0; i < backgrounds.Length; i++)
        {
            colourVar = backgrounds[i].color;
            if (i == activeBGIndex)
            {
                colourVar.a = Mathf.Lerp(backgrounds[i].color.a, 1, effectSpeed * Time.deltaTime);
            }
            else {
                colourVar.a = Mathf.Lerp(backgrounds[i].color.a, 0, effectSpeed * Time.deltaTime);
            }

            backgrounds[i].color = colourVar;
        }
    }

    public void SetActiveBG(int bgIndex, bool instant = false) {
        if (bgIndex < 0 || bgIndex > backgrounds.Length) {
            Debug.LogError("background index out of range");
            return;
        }
       

        activeBGIndex = bgIndex;

        if (instant) {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                colourVar = backgrounds[i].color;
                if (i == activeBGIndex)
                {
                    colourVar.a = 1;
                }
                else
                {
                    colourVar.a = 0;
                }

                backgrounds[i].color = colourVar;
            }
        }
    }
}