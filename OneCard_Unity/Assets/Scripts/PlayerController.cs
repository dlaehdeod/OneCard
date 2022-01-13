using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public CardManager cardManager;
    public TurnManager turnManager;
    public SevenCard sevenCard;
    public Color possibleColor;
    public Color impossibleColor;

    private List<RaycastResult> raycastResults;
    private PointerEventData pointer;

    private Transform card;
    private int originSiblingIndex;
    private Image selectedCardImage;
    private bool isCatching;
    private Vector3 originPosition;
    private Vector3 offset;

    private void Awake()
    {
        pointer = new PointerEventData(EventSystem.current);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseButtonDown();
        }

        if (Input.GetMouseButtonUp(0))
        {
            MouseButtonUp();
        }

        if (isCatching)
        {
            card.position = Input.mousePosition - offset;
        }
    }

    private void MouseButtonDown ()
    {
        if (!turnManager.IsPlayerTurn())
        {
            return;
        }

        pointer.position = Input.mousePosition;
        raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            GameObject target = raycastResults[0].gameObject;

            switch (target.tag)
            {
                case "PlayerCard":
                    card = target.transform;
                    isCatching = true;
                    originSiblingIndex = card.GetSiblingIndex();
                    card.SetAsLastSibling();
                    originPosition = card.position;
                    offset = Input.mousePosition - card.position;

                    if (selectedCardImage != null)
                    {
                        selectedCardImage.color = Color.white;
                    }

                    if (cardManager.IsCanUseCard(target.transform))
                    {
                        selectedCardImage = target.GetComponent<Image>();
                        selectedCardImage.color = possibleColor;
                    }
                    else
                    {
                        selectedCardImage = target.GetComponent<Image>();
                        selectedCardImage.color = impossibleColor;
                    }

                    break;
                case "PassTurn":
                    turnManager.PlayerTurnEnd();
                    cardManager.PassTurnByNotDropCard(0);
                    break;
                case "SevenType":
                    sevenCard.SelectType(target.transform);
                    turnManager.PassMyTurn();
                    break;
                default:
                    break;
            }
        }
    }

    private void MouseButtonUp ()
    {
        if (isCatching)
        {
            bool dropSuccess = cardManager.PlayerCardDrop(card);

            if (!dropSuccess)
            {
                card.SetSiblingIndex(originSiblingIndex);
                card.position = originPosition;
            }
        }

        if (selectedCardImage != null)
        {
            selectedCardImage.color = Color.white;
            selectedCardImage = null;
        }
        
        isCatching = false;
    }
}