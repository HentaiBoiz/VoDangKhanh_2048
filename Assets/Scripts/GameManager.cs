using System.Collections;
using TMPro;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TileBoard board = TileBoard.Instance;
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
        ResetKey();
    }

    public void KhanhGameOver()
    {
        End(gameOver, winGame);
        SoundSetting.Instance.OnSFXSound(SoundSetting.Instance.SfxLose);
        ResetKey();
    }

    public void KhanhWinTheGame()
    {
        End(winGame, gameOver);
        SoundSetting.Instance.OnSFXSound(SoundSetting.Instance.sfxWin);
        ResetKey();
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

    //public void SaveGame()
    //{
    //    TileCell[] cells = board.grid.cells;
    //    foreach (TileCell cell in cells)
    //    {
    //        if (!cell.empty)
    //            //saveTiles += cell.tile.number.ToString() + "," + cell.coordinates.x.ToString() + "," + cell.coordinates.y.ToString() + ",";

    //    }
            //PlayerPrefs.SetString("Tile", saveTiles);
            //PlayerPrefs.SetInt("score", score);
            //Debug.Log(saveTiles);
    //}
    public void SaveGame()
    {
        if (board.enabled)
        {
            string saveTiles = "";
            for (int i = 0; i < board.grid.rows.Length; i++)
            {
                for (int j = 0; j < board.grid.rows[i].cells.Length; j++)
                {
                    if(board.grid.rows[i].cells[j].tile != null) {
                        saveTiles += board.grid.rows[i].cells[j].tile.number.ToString() + ",";
                    }
                    else
                    {
                        saveTiles += " " + ",";
                    }
                    
                }
            }
            
            PlayerPrefs.SetString("Tile", saveTiles);
            PlayerPrefs.SetInt("score", score);
            Debug.Log(saveTiles);
        }
    }

    //public void Load()
    //{
    //    if (PlayerPrefs.HasKey("Tile"))
    //    {
    //        board.ClearBoard();
    //        string loadString = PlayerPrefs.GetString("Tile");

    //        string[] saveSplit = loadString.Split(',');

    //        int number = 0;
    //        int posX = 0;
    //        int posY = 0;

    //        for (int i = 0; i < saveSplit.Length; i++)
    //        {
    //            if (saveSplit[i].Length != 0)
    //            {
    //                if (i % 3 == 0)
    //                {
    //                    number = int.Parse(saveSplit[i]);
    //                }
    //                else if (i % 3 == 1)
    //                {
    //                    posX = int.Parse(saveSplit[i]);
    //                }
    //                else if (i % 3 == 2)
    //                {
    //                    posY = int.Parse(saveSplit[i]);
    //                    board.LoadTile(number, board.grid.GetCell(posX, posY));
    //                }
    //                if (i % 3 == 0)
    //                {
    //                    number = int.Parse(saveSplit[i]);
    //                    board.LoadTile(number, board.grid.GetCell(posx, posy));
    //                }
    //            }
    //        }
    //        score = PlayerPrefs.GetInt("score");
    //        scoreText.text = score.ToString();
    //    }
    //}
    public void Load()
    {
        
        string loadString = PlayerPrefs.GetString("Tile","");
        Debug.Log(loadString);
        if (loadString != "")
        {
            for (int i = 0; i < loadString.Length; i++)
            {
                string[] tilesArray = loadString.Split(',');
                for (int j = 0; j < tilesArray.Length; j++)
                {
                    if(tilesArray[j] != " ")
                    {
                        board.grid.rows[i].cells[j].tile.number = int.Parse(tilesArray[j]);
                        Debug.Log(board.grid.rows[i].cells[j].tile.number);
                    }
                }
                    
            }
        }
    }

    public void ResetKey()
    {
        PlayerPrefs.DeleteKey("Tile");
        PlayerPrefs.DeleteKey("score");
    }

}
