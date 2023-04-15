using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TileBoard board;
    public CanvasGroup gameOver;
    public CanvasGroup winGame;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;

    public bool isContinue;

    private int score;

    private void Start()
    {
        NewGame();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Debug.Log("Found more than one Game_Manager in the scene");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void NewGame()
    {
        // reset score
        SetScore(0);
        hiscoreText.text = LoadHiscore().ToString();
        isContinue = false;

        // hide game over and win screen
        KhanhHidden(gameOver);
        KhanhHidden(winGame);

        // update board state
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
        SoundSetting.Instance.OnBackGroundMusic();
    }

    public void KhanhGameOver()
    {
        End(gameOver, winGame);
        SoundSetting.Instance.OnSFXSound(SoundSetting.Instance.SfxLose);
    }

    public void KhanhWinTheGame()
    {
        End(winGame, gameOver);
        SoundSetting.Instance.OnSFXSound(SoundSetting.Instance.sfxWin);
    }

    public void End(CanvasGroup canvasGroup1, CanvasGroup canvasGroup2)
    {
        board.enabled = false;

        canvasGroup1.interactable = true;
        canvasGroup1.blocksRaycasts = true;

        canvasGroup2.interactable = false;
        canvasGroup2.blocksRaycasts = false;

        SoundSetting.Instance.OffBackGroundMusic();
        StartCoroutine(Fade(canvasGroup1, 1f, 1f));
    }

    public void CountinueGame()
    {
        board.enabled = true;
        KhanhHidden(winGame);

        isContinue = true;

        SoundSetting.Instance.OnBackGroundMusic();
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();

        SaveHiscore();
    }

    private void SaveHiscore()
    {
        int hiscore = LoadHiscore();

        if (score > hiscore) {
            PlayerPrefs.SetInt("hiscore", score);
        }
    }

    private int LoadHiscore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }
    private void KhanhHidden(CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0f;
    }

}
