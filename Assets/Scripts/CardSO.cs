using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new card", menuName = "Project/Card")]
public class CardSO : ScriptableObject
{
    
    
    public new string name;
    public Sprite icon;

    public enum CardType { common, rare, closing, final}
    public CardType type;

    public int minActApperance;
    public int maxActApperance;
    public bool isAppearAct4;

}
