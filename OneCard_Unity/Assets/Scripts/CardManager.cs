using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public TurnManager turnManager;
    public SevenCard sevenCard;

    public Transform[] players;
    public Transform[] imageIntervalTransforms;
    public Transform cardMoveParent;
    public Transform dropCard;

    public Text attackCountText;

    public GameObject cardObject;
    public GameObject cardBackfaceObject;
    public Sprite[] cardSprites;

    [Header("Options")]
    public float startDrawDelay = 0.1f;
    public float drawCardSpeed = 2.0f;
    public float playerGetCardDelay = 0.5f;
    public float computerSelectDelay = 1.5f;
    public float sortWidthBoundaryFactor = 2.5f;
    public float sortHeightBoundaryFactor = 1.5f;

    private float imageInterval;
    private int attackCount;

    private void Awake()
    {
        if (Option.isFirstSetting)
        {
            return;
        }

        int length = cardSprites.Length;

        for (int i = 0; i < length; ++i)
        {
            GameObject newObject = Instantiate(cardObject, transform.position, Quaternion.identity, transform);
            GameObject backfaceObject = Instantiate(cardBackfaceObject, transform.position, Quaternion.identity, newObject.transform);

            newObject.GetComponent<Image>().sprite = cardSprites[i];
            newObject.name = cardSprites[i].name;
        }

        imageInterval = imageIntervalTransforms[1].position.x - imageIntervalTransforms[0].position.x;

        for (int cardCount = 0; cardCount < Option.startCardCount; ++cardCount)
        {
            for (int playerIndex = 0; playerIndex < Option.playerCount; ++playerIndex)
            {
                StartCoroutine(DelayDrawCard(playerIndex, cardCount * Option.playerCount + playerIndex));
            }
        }

        StartCoroutine(DrawFirstTopCard(Option.startCardCount * Option.playerCount + 1));
    }

    private void Start()
    {
        if (Option.isFirstSetting)
        {
            return;
        }

        turnManager.SetPlayerCount(Option.playerCount);
    }

    public bool PlayerCardDrop (Transform card)
    {
        float dropAceptDistance = (imageInterval * 1.5f) * (imageInterval * 1.5f);

        bool dropAcept = Vector3.SqrMagnitude(card.position - dropCard.position) < dropAceptDistance;
        bool isCanUseCard = IsCanUseCard(card);

        if (dropAcept && isCanUseCard)
        {
            Vector3 startPosition = card.position;
            Vector3 endPosition = dropCard.position;

            card.SetParent(dropCard);
            card.tag = "Untagged";
            SortCard(0);

            StartCoroutine(CardMove(card, startPosition, endPosition, dropCard.rotation));

            CheckGameOver(0);

            if (!turnManager.GameOver())
            {
                PassTurnByDropCardAbility(card, 0);
            }

            return true;
        }

        return false;
    }

    public bool IsCanUseCard (Transform card)
    {
        Transform topCard = dropCard.GetChild(dropCard.childCount - 1);

        char topCardType = topCard.name[0];
        char topCardNumber = topCard.name[2];

        char dropCardType = card.name[0];
        char dropCardNumber = card.name[2];

        if (dropCardType == 'Y' || dropCardType == 'B')
        {
            return true;
        }
        else if (attackCount > 0)
        {
            if (topCardType == 'B' && dropCardType == 'S' && dropCardNumber == 'A')
            {
                return true;
            }
            else if (topCardType == dropCardType && (dropCardNumber == 'A' || dropCardNumber == '2'))
            {
                return true;
            }
            else if (topCardNumber == dropCardNumber)
            {
                return true;
            }

            return false;
        }
        else if (topCardType == 'Y' || topCardType == 'B')
        {
            return true;
        }
        else if (sevenCard.isUsingSevenCard)
        {
            return (sevenCard.sevenType == dropCardType) || (topCardNumber == dropCardNumber);
        }

        return (topCardType == dropCardType) || (topCardNumber == dropCardNumber);
    }

    private void PassTurnByDropCardAbility (Transform card, int playerIndex)
    {
        char cardType = card.name[0];
        char cardNumber = card.name[2];

        if (cardType == 'Y')
        {
            AddAttackCount(7);
        }
        else if (cardType == 'B')
        {
            AddAttackCount(5);
        }
        else if (cardNumber == 'A')
        {
            AddAttackCount(3);
        }
        else if (cardNumber == '2')
        {
            AddAttackCount(2);
        }
        else if (cardNumber == '7')
        {
            sevenCard.ShowSelectView(playerIndex);

            if (playerIndex != 0)
            {
                StartCoroutine(ComputerSelectSevenCardType(playerIndex));
            }
            
            return;
        }
        else if (cardNumber == 'K')
        {
            if (playerIndex != 0)
            {
                ComputerTurn(playerIndex);
            }

            return;
        }
        else if (cardNumber == 'Q')
        {
            turnManager.ReverseOrder();
        }
        else if (cardNumber == 'J')
        {
            turnManager.SetNextTurn();
        }

        sevenCard.DisableType();
        turnManager.PassMyTurn();
    }

    private void AddAttackCount (int count)
    {
        attackCount += count;

        if (attackCount == 0)
        {
            attackCountText.text = "";
        }
        else
        {
            attackCountText.text = string.Format("x{0}", attackCount.ToString());
        }
    }

    public void PassTurnByNotDropCard (int playerIndex)
    {
        StartCoroutine(PlayerEatCard(playerIndex));
    }

    private IEnumerator PlayerEatCard (int playerIndex)
    {
        do
        {
            if (attackCount > 0)
            {
                AddAttackCount(-1);
            }

            yield return StartCoroutine(DrawCard(playerIndex));
            if (players[playerIndex].childCount >= Option.defeatCardCount)
            {
                AddAttackCount(-attackCount);
                DefeatPlayerReturnAllCard(playerIndex);
                turnManager.GameOverPlayer(playerIndex);
                break;
            }
        } while (attackCount > 0);

        yield return null;
        turnManager.PassMyTurn();
    }

    private void DefeatPlayerReturnAllCard (int playerIndex)
    {
        Transform player = players[playerIndex];

        while (player.childCount > 0)
        {
            Transform card = player.GetChild(0);
            card.SetParent(transform);
            card.position = transform.position;
            card.rotation = transform.rotation;
        }
    }

    public void ComputerTurn (int computerIndex)
    {
        if (turnManager.GetComputerPlaying(computerIndex) == false)
        {
            turnManager.PassMyTurn();
        }

        Transform computerCard = ComputerSelectCard(computerIndex);

        if (computerCard == null)
        {
            StartCoroutine(ComputerPassTurn(computerIndex));
        }
        else
        {
            StartCoroutine(ComputerCardDrop(computerCard, computerIndex));
        }
    }

    private IEnumerator ComputerPassTurn (int computerIndex)
    {
        yield return new WaitForSeconds(computerSelectDelay);

        StartCoroutine(PlayerEatCard(computerIndex));
    }

    private IEnumerator ComputerCardDrop(Transform card, int computerIndex)
    {
        yield return new WaitForSeconds(computerSelectDelay);

        Vector3 startPosition = card.position;
        Vector3 endPosition = dropCard.position;

        card.SetParent(dropCard);
        SortCard(computerIndex);

        yield return StartCoroutine(CardMove(card, startPosition, endPosition, dropCard.rotation));
        card.GetChild(0).gameObject.SetActive(false);

        CheckGameOver(computerIndex);

        if (!turnManager.GameOver())
        {
            PassTurnByDropCardAbility(card, computerIndex);
        }
    }

    private void CheckGameOver (int playerIndex)
    {
        if (playerIndex == 0)
        {
            if (players[playerIndex].childCount == 0)
            {
                sevenCard.DisableType();
                turnManager.PlayerWin();
            }
        }
        else
        {
            if (players[playerIndex].childCount == 0)
            {
                turnManager.ComputerWin(playerIndex);
            }
        }
    }

    private Transform ComputerSelectCard (int playerIndex)
    {
        List<Transform> canUseCardList = new List<Transform>();
        int childCount = players[playerIndex].childCount;

        for (int index = 0; index < childCount; ++index)
        {
            Transform card = players[playerIndex].GetChild(index);

            if (IsCanUseCard(card))
            {
                canUseCardList.Add(card);
            }
        }

        if (canUseCardList.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, canUseCardList.Count);

        return canUseCardList[randomIndex];
    }

    private IEnumerator ComputerSelectSevenCardType(int playerIndex)
    {
        yield return new WaitForSeconds(computerSelectDelay);

        Transform card = ComputerSelectCard(playerIndex);

        sevenCard.SelectType(card);

        yield return null;

        turnManager.PassMyTurn();
    }

    private IEnumerator DrawFirstTopCard (int delay)
    {
        yield return new WaitForSeconds(delay * startDrawDelay);

        int cardIndex = Random.Range(0, transform.childCount);
        Transform card = transform.GetChild(cardIndex);
        card.SetParent(dropCard);

        Vector3 startPosition = transform.position;
        Vector3 endPosition = dropCard.position;

        yield return StartCoroutine(CardMove(card, startPosition, endPosition, dropCard.rotation));

        card.GetChild(0).gameObject.SetActive(false);

        turnManager.GameStart();
    }

    private IEnumerator DelayDrawCard(int playerIndex, int delay)
    {
        yield return new WaitForSeconds(delay * startDrawDelay);
        StartCoroutine(DrawCard(playerIndex));
    }

    private IEnumerator DrawCard(int playerIndex)
    {
        if (transform.childCount == 0)
        {
            ShuffleDropCard();
        }

        int cardIndex = Random.Range(0, transform.childCount);
        Transform card = transform.GetChild(cardIndex);
        card.SetParent(cardMoveParent);
        card.rotation = players[playerIndex].rotation;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = players[playerIndex].position;

        yield return StartCoroutine(CardMove(card, startPosition, endPosition, card.rotation));

        if (playerIndex == 0)
        {
            card.GetChild(0).gameObject.SetActive(false);
            card.tag = "PlayerCard";
        }

        card.SetParent(players[playerIndex]);
        SortCard(playerIndex);

        yield return null;
    }

    private void ShuffleDropCard ()
    {
        while (dropCard.childCount > 1)
        {
            Transform card = dropCard.GetChild(0);
            card.GetChild(0).gameObject.SetActive(true);
            card.position = transform.position;
            card.SetParent(transform);
        }
    }

    private IEnumerator CardMove (Transform card, Vector3 startPosition, Vector3 endPosition, Quaternion rotation)
    {
        float time = 0.0f;

        while (time < 1.0f)
        {
            time += Time.deltaTime * drawCardSpeed;
            card.position = Vector3.Lerp(startPosition, endPosition, time);

            yield return null;
        }

        card.position = endPosition;
        card.rotation = rotation;
        yield return null;
    }

    private void SortCard(int playerIndex)
    {
        if (playerIndex % 2 == 0)
        {
            SortByWidth(playerIndex);
        }
        else
        {
            SortByHeight(playerIndex);
        }
    }

    private void SortByWidth(int playerIndex)
    {
        Transform player = players[playerIndex];
        int childCount = player.childCount;

        if (childCount == 0)
        {
            return;
        }

        Vector3 cardPosition;
        cardPosition.x = Screen.width / 2 - imageInterval / 2 * (childCount - 1);
        cardPosition.y = player.position.y;
        cardPosition.z = 0.0f;

        float leftBoundary = imageInterval * sortWidthBoundaryFactor;
        float rightBoundary = Screen.width - imageInterval * sortWidthBoundaryFactor;

        if (cardPosition.x <= leftBoundary)
        {
            float width = rightBoundary - leftBoundary;
            float interval = width / (childCount - 1);

            cardPosition.x = leftBoundary;
            player.GetChild(0).position = cardPosition;

            for (int i = 1; i < childCount; ++i)
            {
                cardPosition.x += interval;
                player.GetChild(i).position = cardPosition;
            }
        }
        else
        {
            player.GetChild(0).position = cardPosition;

            for (int i = 1; i < childCount; ++i)
            {
                cardPosition.x += imageInterval;
                player.GetChild(i).position = cardPosition;
            }
        }
    }

    private void SortByHeight (int playerIndex)
    {
        Transform player = players[playerIndex];
        int childCount = player.childCount;

        if (childCount == 0)
        {
            return;
        }

        Vector3 cardPosition;
        cardPosition.x = player.position.x;
        cardPosition.y = Screen.height / 2 - imageInterval / 2 * (childCount - 1);
        cardPosition.z = 0.0f;

        float bottomBoundary = imageInterval * sortHeightBoundaryFactor;
        float topBoundary = Screen.height - imageInterval * sortHeightBoundaryFactor;

        if (cardPosition.y <= bottomBoundary)
        {
            float height = topBoundary - bottomBoundary;
            float interval = height / (childCount - 1);

            cardPosition.y = bottomBoundary;
            player.GetChild(0).position = cardPosition;

            for (int i = 1; i < childCount; ++i)
            {
                cardPosition.y += interval;
                player.GetChild(i).position = cardPosition;
            }
        }
        else
        {
            player.GetChild(0).position = cardPosition;

            for (int i = 1; i < childCount; ++i)
            {
                cardPosition.y += imageInterval;
                player.GetChild(i).position = cardPosition;
            }
        }
    }
}