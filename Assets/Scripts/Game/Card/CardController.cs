using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using TMPro;

public class CardController : MonoBehaviour
{
    public CardData cardData;
    public Sprite cardImage;
    // public TextMeshProUGUI reward;

    private void Start()
    {

    }

    public void InitializeCardUI()
    {
        cardImage = cardData.cardImage;
        // reward.text = cardData.reward.ToString();
    }

    public void SelectCardFromOneOfThree()
    {

    }

    /*    IEnumerator SelectCardRoutine()
        {

        }*/

}
