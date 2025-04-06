using System.Collections.Generic;
using UnityEngine;

public class CardInventory : VMInventory<CardInventory>
{
    public VMList<int> cards = new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };//卡牌id数组
}