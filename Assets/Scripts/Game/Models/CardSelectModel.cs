using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameConfig;

public class CardSelectModel : VMInventory<CardSelectModel>
{
    public VM<int> card1 = 1;
    public VM<int> card2 = 2;
    public VM<int> card3 = 3;
    public VMAdapter<int, Sprite> card1Image = new VMAdapter<int, Sprite>(
        (id) =>
        {
            Debug.Log(Config.CardsConfig.Get(id));
            return Resources.Load<Sprite>("Images/cards/" + Config.CardsConfig.Get(id).Image);
        }
    );
    public VMAdapter<int, Sprite> card2Image = new VMAdapter<int, Sprite>(
        (id) =>
        {
            Debug.Log(Config.CardsConfig.Get(id));
            return Resources.Load<Sprite>("Images/cards/" + Config.CardsConfig.Get(id).Image);
        }
    );
    public VMAdapter<int, Sprite> card3Image = new VMAdapter<int, Sprite>(
        (id) =>
        {
            Debug.Log(Config.CardsConfig.Get(id));
            return Resources.Load<Sprite>("Images/cards/" + Config.CardsConfig.Get(id).Image);
        }
    );
}
