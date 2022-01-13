using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Option : MonoBehaviour
{
    public static bool isFirstSetting = true;
    public static int playerCount = 4;
    public static int startCardCount = 5;
    public static int defeatCardCount = 14;

    public int basePlayerCount = 2;
    public int baseStartCardCount = 1;
    public int baseDefeatCardCount = 10;

    public Scrollbar playerCountScroll;
    public Scrollbar startCardCountScroll;
    public Scrollbar defeatCardCountScroll;

    public Text playerCountText;
    public Text startCardCountText;
    public Text defeatCardCountText;

    public Sprite musicOn;
    public Sprite musicOff;
    public Image musicImage;

    private void Awake()
    {
        if (!isFirstSetting)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

        playerCountScroll.onValueChanged.AddListener(delegate { PlayerCountScrollChange(); });
        startCardCountScroll.onValueChanged.AddListener(delegate { StartCardCountScrollChange(); });
        defeatCardCountScroll.onValueChanged.AddListener(delegate { DefeatCardCountScrollChange(); });

        playerCountScroll.value = (float)(playerCount - basePlayerCount) / (playerCountScroll.numberOfSteps - 1);
        startCardCountScroll.value = (float)(startCardCount - baseStartCardCount) / (startCardCountScroll.numberOfSteps - 1);
        defeatCardCountScroll.value = (float)(defeatCardCount - baseDefeatCardCount) / (defeatCardCountScroll.numberOfSteps - 1);

        playerCountText.text = playerCount.ToString();
        startCardCountText.text = startCardCount.ToString();
        defeatCardCountText.text = defeatCardCount.ToString();
    }

    #region Call By Button

    public void OptionButtonDown()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void MusicButtonDown()
    {
        if (musicImage.sprite == musicOn)
        {
            musicImage.sprite = musicOff;
            BackgroundMusic.instance.MusicOff();
        }
        else
        {
            musicImage.sprite = musicOn;
            BackgroundMusic.instance.MusicOn();
        }
    }

    public void PlayerCountScrollChange ()
    {
        int value = (int)(playerCountScroll.value * 1000);
        int step = 1000 / (playerCountScroll.numberOfSteps - 1);

        playerCount = value / step + basePlayerCount;
        playerCountText.text = playerCount.ToString();
    }

    public void StartCardCountScrollChange ()
    {
        int value = (int)(startCardCountScroll.value * 1000);
        int step = 1000 / (startCardCountScroll.numberOfSteps - 1);

        startCardCount = value / step + baseStartCardCount;
        startCardCountText.text = startCardCount.ToString();
    }

    public void DefeatCardCountScrollChange ()
    {
        int value = (int)(defeatCardCountScroll.value * 1000);
        int step = 1000 / (defeatCardCountScroll.numberOfSteps - 1);

        defeatCardCount = value / step + baseDefeatCardCount;
        defeatCardCountText.text = defeatCardCount.ToString();
    }

    public void GameStartButtonDown()
    {
        isFirstSetting = false;

        SceneManager.LoadScene("Play");
    }

    public void GameQuit ()
    {
        Application.Quit();
    }

    #endregion
}