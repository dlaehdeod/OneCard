using UnityEngine;
using UnityEngine.UI;

public class SevenCard : MonoBehaviour
{
    public Sprite[] typeSprites;
    public Text selectText;
    public Image sevenTypeImage;
    public char sevenType;
    public bool isUsingSevenCard;

    private readonly char[] cardTypes = { 'S', 'D', 'C', 'H' };

    public void ShowSelectView (int playerIndex)
    {
        if (playerIndex == 0)
        {
            selectText.text = "문양을 선택하세요!";
        }
        else
        {
            selectText.text = "문양을 선택 중입니다.";
        }

        gameObject.SetActive(true);
    }

    public void SelectType (Transform card)
    {
        char cardType;

        if (card == null || card.name[0] == 'Y' || card.name[0] == 'B')
        {
            int index = Random.Range(0, cardTypes.Length);

            cardType = cardTypes[index];
        }
        else
        {
            cardType = card.name[0];
        }

        switch (cardType)
        {
            case 'S':
                 sevenTypeImage.sprite = typeSprites[0];
                break;
            case 'D':
                sevenTypeImage.sprite = typeSprites[1];
                break;
            case 'C':
                sevenTypeImage.sprite = typeSprites[2];
                break;
            case 'H':
                sevenTypeImage.sprite = typeSprites[3];
                break;
            default:
                break;
        }

        isUsingSevenCard = true;
        sevenType = cardType;
        sevenTypeImage.enabled = true;
        gameObject.SetActive(false);
    }

    public void DisableType ()
    {
        isUsingSevenCard = false;
        sevenTypeImage.enabled = false;
    }
}