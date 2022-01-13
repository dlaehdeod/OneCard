using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public CardManager cardManager;
    public Transform[] arrowPositions;
    public Color[] arrowColor;
    public Text gameStateText;

    private bool[] playing;

    private bool isPlayerTurn;
    private bool gameStart;
    private bool forwardDirection;
    private int turn;
    private int grade;

    private Image arrow;

    private void Awake()
    {
        playing = new bool[4];

        gameStart = false;
        isPlayerTurn = true;
        forwardDirection = true;
        grade = 1;

        arrow = transform.GetChild(0).GetComponent<Image>();

        arrow.color = arrowColor[0];
        transform.position = arrowPositions[0].position;
        transform.rotation = arrowPositions[0].rotation;
    }

    public void SetPlayerCount (int count)
    {
        playing[0] = true;
        playing[2] = true;

        if (count == 3)
        {
            playing[1] = true;
        }
        else if (count == 4)
        {
            playing[1] = true;
            playing[3] = true;
        }
    }

    public void GameStart ()
    {
        gameStart = true;
        gameStateText.text = "카드를 선택하세요!";
        gameStateText.color = arrowColor[0];
        arrow.gameObject.SetActive(true);
    }

    public void PassMyTurn ()
    {
        if (!gameStart)
        {
            return;
        }

        SetNextTurn();

        arrow.color = arrowColor[turn];
        gameStateText.color = arrow.color;
        transform.rotation = arrowPositions[turn].rotation;
        transform.position = arrowPositions[turn].position;

        if (turn == 0)
        {
            gameStateText.text = "카드를 선택하세요!";
            isPlayerTurn = true;
        }
        else
        {
            gameStateText.text = "다른 플레이어가 선택 중입니다.";
            cardManager.ComputerTurn(turn);
        }
    }

    public void SetNextTurn ()
    {
        if (forwardDirection)
        {
            do
            {
                turn = (turn + 1) % 4;
            } while (!playing[turn]);
        }
        else
        {
            do
            {
                turn = (turn - 1 + 4) % 4;
            } while (!playing[turn]);
        }
    }

    public void ReverseOrder ()
    {
        forwardDirection = !forwardDirection;
    }

    public void PlayerTurnEnd ()
    {
        isPlayerTurn = false;
    }

    public bool IsPlayerTurn ()
    {
        return gameStart && turn == 0 && isPlayerTurn;
    }

    public void GameOverPlayer (int playerIndex)
    {
        if (playerIndex == 0)
        {
            PlayerDefeat();
        }

        playing[playerIndex] = false;

        for (int i = 1; i < 4; ++i)
        {
            if (playing[i])
            {
                return;
            }
        }

        PlayerWin();
    }

    public void ComputerWin(int computerIndex)
    {
        playing[computerIndex] = false;
        grade++;

        bool playerDefeat = true;

        for (int i = 1; i < 4; ++i)
        {
            if (playing[i])
            {
                playerDefeat = false;
            }
        }

        if (playerDefeat)
        {
            PlayerDefeat();
        }
    }

    public bool GetComputerPlaying (int computerIndex)
    {
        return playing[computerIndex];
    }

    public bool GameOver ()
    {
        return !gameStart;
    }

    public void PlayerWin ()
    {
        gameStart = false;
        arrow.gameObject.SetActive(false);

        if (grade == 1)
        {
            gameStateText.text = "이겼습니다!";
        }
        else
        {
            gameStateText.text = "아쉽네요 " + grade + "등 입니다.";
        }
        
        StartCoroutine(ReGame());
    }

    private IEnumerator ReGame ()
    {
        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene("Play");
    }

    private void PlayerDefeat ()
    {
        gameStart = false;

        arrow.gameObject.SetActive(false);
        gameStateText.text = "졌습니다..";

        StartCoroutine(ReGame());
    }
}