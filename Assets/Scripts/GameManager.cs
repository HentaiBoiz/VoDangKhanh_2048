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

        // hide game over screen
        gameOver.interactable = false;
        gameOver.alpha = 0f;


        // hide win screen
        winGame.interactable = false;
        winGame.alpha = 0f;

        // update board state
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
        SoundSetting.Instance.music.Play();
    }

    public void GameOver()
    {
        End(gameOver);
        winGame.blocksRaycasts = false;
    }

    public void WinTheGame()
    {
        End(winGame);
        gameOver.blocksRaycasts = false;
    }

    public void End(CanvasGroup canvasGroup)
    {
        board.enabled = false;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        SoundSetting.Instance.music.Stop();
        StartCoroutine(Fade(canvasGroup, 1f, 1f));
    }

    public void CountinueGame()
    {
        board.enabled = true;
        winGame.alpha = 0f;
        winGame.interactable = false;

        isContinue = true;

        SoundSetting.Instance.music.Play();
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

}
