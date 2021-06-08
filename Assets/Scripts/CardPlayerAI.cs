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
    [SerializeField] int minimumNormalAmount = 4;
    [SerializeField] int minimumCheatAmount = 4;
    [SerializeField] int minimumStuburnCount = 1;
    [SerializeField] TextMeshProUGUI debugAIDecision;
    [SerializeField] TextMeshProUGUI seedText;

    private enum Decisions { normal = 0, cheat, stuborn, random }
    private Decisions AIDecision;

    private int stubornCount = 0;
    private int[] decisionSeed;
    private int currentTurn;



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

    public void InitKnowledgeBase(int size, int turnCount)
    {
        knowledgeBase = new string[size];

        for (int i = 0; i < knowledgeBase.Length; i++)
        {
            knowledgeBase[i] = unknownLable;
        }

        //generate seed
        GenerateDecisionSeed(turnCount, true);

    }

    private void GenerateDecisionSeed(int turnCount, bool cheat = false)
    {
        decisionSeed = new int[turnCount];
        currentTurn = 0;
        seedText.text = "";

        if (cheat) {
			for (int i = 0; i < decisionSeed.Length; i++)
            {
				decisionSeed[i] = 1;
				seedText.text += 1;
			}
			return;
		} 

        //randomise seed
        for (int i = 0; i < decisionSeed.Length; i++)
        {
            decisionSeed[i] = Random.Range(0, 8);
        }

        int countedNumber = 0;
        countedNumber = countInSeed((int)Decisions.normal);

        //refine normal
        if (countedNumber < minimumNormalAmount)
        {
            for (int i = 0; i < decisionSeed.Length; i++)
            {
                if (decisionSeed[i] % 4 != (int)Decisions.normal && decisionSeed[i] % 4 != (int)Decisions.cheat && countedNumber < minimumNormalAmount)
                {
                    decisionSeed[i] = (int)Decisions.normal * Random.Range(1, 3);
                    countedNumber++;
                }
            }
        }

        //refine cheat 
        countedNumber = countInSeed((int)Decisions.cheat);
        if (countedNumber < minimumNormalAmount)
        {
            for (int i = decisionSeed.Length - 1; i > 0; i--)
            {
                if (decisionSeed[i] % 4 != (int)Decisions.normal && decisionSeed[i] % 4 != (int)Decisions.cheat && countedNumber < minimumNormalAmount)
                {
                    decisionSeed[i] = (int)Decisions.cheat * Random.Range(1, 3);
                    countedNumber++;
                }
            }
        }

        for (int i = 0; i < decisionSeed.Length; i++)
        {
            if (decisionSeed[i] % 4 == (int)Decisions.stuborn) {
                decisionSeed[i] = (int)Decisions.random;
            }
        }



        //hard coded
        for (int i = 0; i < minimumStuburnCount; i++)
        {
            decisionSeed[Random.Range(decisionSeed.Length - 4, decisionSeed.Length)] = (int)Decisions.stuborn;
        }

        for (int i = 0; i < decisionSeed.Length; i++)
        {
            seedText.text += decisionSeed[i];
        }
        
    }

    int countInSeed(int findThisNumber) {
        int count = 0;
        foreach (var number in decisionSeed)
        {
            if (number % 4 == findThisNumber) {
                count++;
            }
        }
        return count;
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
            int decidedIndex = 0;
            do
            {
                decidedIndex = Random.Range(0, knowledgeBase.Length);
            } while (!cm.isCardValidToplay(decidedIndex));
            desiredCard = cm.getCardName(decidedIndex);

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
        else {
            AIDecision = (Decisions) (decisionSeed[currentTurn]% 4);
            Debug.Log(decisionSeed[currentTurn] %4);
            currentTurn++;
        }

        //AIDecision = (Decisions)Random.Range(0, 5);
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
            AIDecision = Decisions.random;
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
