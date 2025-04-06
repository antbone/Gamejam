using System.Collections.Generic;
using UnityEngine;

public class CardInventory : VMInventory<CardInventory>
{
    public VMList<int> cards = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};//卡牌id数组
}