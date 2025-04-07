using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card System/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    public Sprite cardImage;
    public GameObject card3DPrefab;
    public string cardName;
    [TextArea] public string cardDescription;
    public int cardQuality;
    public int score;

    [Header("Type Info")]
    public CardType cardType;
    public CardRarity cardRarity;
    public List<int> Ð§¹û;
}
