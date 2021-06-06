using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardPlayerAI : MonoBehaviour
{


    private string[] knowledgeBase;
    [SerializeField] const string unknownLable = "unknown";
    public string desiredCard;
    [SerializeField] Vector2 thinkingTime;
    [SerializeField] CardsManager cm;
    [SerializeField] TextMeshProUGUI debugAIDecision;
    

    private enum Decisions { normal = 0, intentionalMiss, stuborn, cheat, random }
    private Decisions AIDecision;

    private int stubornCount = 0;



    // Start is called before the first frame update
    void Start()
    {
        cm = CardsManager.instanceCM;
    }

    // Update is called once per frame
    void Update()
    {
        debugAIDecision.text = AIDecision.ToString();
    }

    public void InitKnowledgeBase(int size)
    {
        knowledgeBase = new string[size];

        for (int i = 0; i < knowledgeBase.Length; i++)
        {
            knowledgeBase[i] = unknownLable;
        }
    }

    public void SetDesiredCard(string cardName)
    {
        if (stubornCount <= 0)
        {
            desiredCard = cardName;
        }

    }

    public void RevealCard(int index, string cardName)
    {
        knowledgeBase[index] = cardName;
    }

    public void BeStuborn()
    {
        AIDecision = Decisions.stuborn;

        if (stubornCount <= 0)
        {
            Debug.Log("changing desire");

            desiredCard = cm.getCardName(Random.Range(0, knowledgeBase.Length));
            stubornCount = 3;
        }

        //if there is such card
        for (int i = 0; i < knowledgeBase.Length; i++)
        {
            if (cm.getCardName(i) == desiredCard && cm.isCardValidToplay(i))
            {
                StartCoroutine(ThinkAndPlay(i));
                Debug.Log("stuborn :" + stubornCount);
                stubornCount -= 1;
                return;
            }
        }

        //if fails
        ChooseRandomCard();
        stubornCount = 0;


    }

    public void ResetStuborn()
    {
        stubornCount = 0;
    }

    public void PlayTurn()
    {
        if (stubornCount > 0)
        {
            BeStuborn();
            return;
        }

        AIDecision = (Decisions)Random.Range(0, 5);
        //AIDecision = Decisions.stuborn;

        if (AIDecision == Decisions.normal)
        {
            PlayNormally();
        }
        else if (AIDecision == Decisions.cheat)
        {
            Cheat();
        }
        else if (AIDecision == Decisions.stuborn)
        {
            BeStuborn();
        }
        else
        {
            //random
            ChooseRandomCard();
        }


    }

    private void PlayNormally()
    {
        AIDecision = Decisions.normal;

        //if there is such card
        for (int i = 0; i < knowledgeBase.Length; i++)
        {
            if (knowledgeBase[i] == desiredCard && cm.isCardValidToplay(i))
            {
                Debug.Log("found");
                StartCoroutine(ThinkAndPlay(i));
                return;
            }
        }

        //if not play random
        ChooseRandomCard();
    }

    private void ChooseRandomCard()
    {
        Debug.Log("random");
        AIDecision = Decisions.random;
        int decidedIndex = 0;
        do
        {
            decidedIndex = Random.Range(0, knowledgeBase.Length);
        } while (!cm.isCardValidToplay(decidedIndex));

        StartCoroutine(ThinkAndPlay(decidedIndex));
    }

    IEnumerator ThinkAndPlay(int choosenIndex)
    {
        yield return new WaitForSeconds(Random.Range(thinkingTime.x, thinkingTime.y));
        cm.PlayCard(choosenIndex);
    }

    void Cheat()
    {
        AIDecision = Decisions.cheat;

        //if there is such card
        for (int i = 0; i < knowledgeBase.Length; i++)
        {
            if (cm.getCardName(i) == desiredCard && cm.isCardValidToplay(i))
            {
                StartCoroutine(ThinkAndPlay(i));
                Debug.Log("cheated");
                return;
            }
        }

        //if fails
        ChooseRandomCard();

    }

}
