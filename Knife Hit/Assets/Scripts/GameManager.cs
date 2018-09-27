using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public delegate int ScoreNeeded();
    public static event ScoreNeeded levelScore;

    public delegate void RestartGame();
    public static event RestartGame ResetKnifesParameters;

    public delegate void NextKnifeinPool();
    public static event NextKnifeinPool NextKnife;

    public delegate void ResetKnifeValues();
    public static event ResetKnifeValues ResetKnife;

    private int level;
    private int score;
    private int apples;
    private string appleKey = "apple";
    private int scoreNeededToNextLevel;

    [SerializeField]
    private GameObject level1;
    [SerializeField]
    private GameObject[] level2 = new GameObject[2];
    [SerializeField]
    private GameObject[] level3 = new GameObject[3];
    [SerializeField]
    private GameObject level4;
    [SerializeField]
    private GameObject boss;
    [SerializeField]
    private GameObject targetPosition;
    [SerializeField]
    private GameObject knifeHandler;
    [SerializeField]
    private GameObject mainCanvas;
    [SerializeField]
    private GameObject winCanvas;
    [SerializeField]
    private GameObject loseCanvas;
    [SerializeField]
    private GameObject RetryButton;
    [SerializeField]
    private GameObject Hud;
    [SerializeField]
    private GameObject scoreText;
    [SerializeField]
    private GameObject appleCounterText;
    [SerializeField]
    private GameObject levelName;
    [SerializeField]
    private GameObject HudKnifePrefab;
    private GameObject[] hudKnifes = new GameObject[10];
    [SerializeField]
    private GameObject HudKnifePosition;

    private void Awake()
    {
        Time.timeScale = 1;
        InstantiateLevels();
    }    

    void Start () {
        if (PlayerPrefs.GetInt(appleKey) == 0)
        {
            apples = 8;
        }
        else
        {
            apples = PlayerPrefs.GetInt(appleKey);
        }
        appleCounterText.GetComponent<Text>().text = apples.ToString();
        score = 0;
        level = -1;
        KnifeBehaviour.Hit += HitTarget;
        KnifeBehaviour.HitKnife += Lose;
        KnifeBehaviour.AppleUp += IncreaseAppleAmount;
	}

    private void OnDestroy()
    {
        KnifeBehaviour.Hit -= HitTarget;
        KnifeBehaviour.HitKnife -= Lose;
        KnifeBehaviour.AppleUp -= IncreaseAppleAmount;
    }
    
    public void NextLevel()
    {
        level++;
        switch (level)
        {
            case 0:
                {                    
                    level1.gameObject.SetActive(true);
                    levelName.GetComponent<Text>().text = "Level 1";
                    scoreNeededToNextLevel = levelScore();
                    SpawnHudKnifes(scoreNeededToNextLevel);
                    break;
                }
            case 1:
                {
                    level1.gameObject.SetActive(false);
                    level2[Random.Range(0,2)].gameObject.SetActive(true);
                    levelName.GetComponent<Text>().text = "Level 2";
                    scoreNeededToNextLevel = levelScore();
                    SpawnHudKnifes(scoreNeededToNextLevel);
                    break;
                }
            case 2:
                {
                    level2[0].gameObject.SetActive(false);
                    level2[1].gameObject.SetActive(false);
                    level3[Random.Range(0, 3)].gameObject.SetActive(true);
                    levelName.GetComponent<Text>().text = "Level 3";
                    scoreNeededToNextLevel = levelScore();
                    SpawnHudKnifes(scoreNeededToNextLevel);
                    break;
                }
            case 3:
                {
                    level3[0].gameObject.SetActive(false);
                    level3[1].gameObject.SetActive(false);
                    level3[2].gameObject.SetActive(false);
                    level4.gameObject.SetActive(true);
                    levelName.GetComponent<Text>().text = "Level 4";
                    scoreNeededToNextLevel = levelScore();
                    SpawnHudKnifes(scoreNeededToNextLevel);
                    break;
                }
            case 4:
                {
                    level4.gameObject.SetActive(false);
                    boss.gameObject.SetActive(true);
                    levelName.GetComponent<Text>().text = "Boss: The Shield";
                    scoreNeededToNextLevel = levelScore();
                    SpawnHudKnifes(scoreNeededToNextLevel);
                    break;
                }
            case 5:
                {
                    Win();
                    break;
                }
        }
    } //Passa para a próxima fase

    void Win()
    {
        ActivateWinCanvas();
        Time.timeScale = 0;
    }

    void Lose()
    {
        ActivateLoseCanvas();
        if (apples >= 10)
        {
            RetryButton.SetActive(true);
        }
    }

    public void Retry()
    {
        DeactivateLoseCanvas();
        RetryButton.SetActive(false);
        apples -= 10;
        PlayerPrefs.SetInt(appleKey, apples);
        appleCounterText.GetComponent<Text>().text = apples.ToString();
        ResetKnife();
        NextKnife();
    }

    void HitTarget()
    {
        score++;
        scoreText.GetComponent<Text>().text = score.ToString();
        scoreNeededToNextLevel--;
        Destroy(hudKnifes[scoreNeededToNextLevel]);
        if (scoreNeededToNextLevel == 0)
        {
            NextLevel();
        }
    }     

    void IncreaseAppleAmount()
    {
        apples++;
        PlayerPrefs.SetInt(appleKey, apples);
        appleCounterText.GetComponent<Text>().text = apples.ToString();
    }

    void InstantiateLevels()
    {
        level1 = Instantiate(level1, targetPosition.transform.position, Quaternion.identity) as GameObject;
        level2[0] = Instantiate(level2[0], targetPosition.transform.position, Quaternion.identity) as GameObject;
        level2[1] = Instantiate(level2[1], targetPosition.transform.position, Quaternion.identity) as GameObject;
        level3[0] = Instantiate(level3[0], targetPosition.transform.position, Quaternion.identity) as GameObject;
        level3[1] = Instantiate(level3[1], targetPosition.transform.position, Quaternion.identity) as GameObject;
        level3[2] = Instantiate(level3[2], targetPosition.transform.position, Quaternion.identity) as GameObject;
        level4 = Instantiate(level4, targetPosition.transform.position, Quaternion.identity) as GameObject;
        boss = Instantiate(boss, targetPosition.transform.position, Quaternion.identity) as GameObject;
    }

    public void ActivateKnifeHandler()
    {
        knifeHandler.SetActive(true);
    }

    public void DeactivateMainCanvas()
    {
        mainCanvas.SetActive(false);
    }

    private void ActivateWinCanvas()
    {
        winCanvas.SetActive(true);
    }

    private void DeactivateWinCanvas()
    {
        winCanvas.SetActive(false);
    }

    private void ActivateLoseCanvas()
    {
        loseCanvas.SetActive(true);
    }

    private void DeactivateLoseCanvas()
    {
        loseCanvas.SetActive(false);
    }

    public void Restart_Game()
    {
        Time.timeScale = 1;
        level = -1;
        score = 0;
        DeactivateWinCanvas();
        DeactivateLoseCanvas();        
        level1.gameObject.SetActive(false);
        level2[0].gameObject.SetActive(false);
        level2[1].gameObject.SetActive(false);
        level3[0].gameObject.SetActive(false);
        level3[1].gameObject.SetActive(false);
        level3[2].gameObject.SetActive(false);
        level4.gameObject.SetActive(false);
        boss.gameObject.SetActive(false);
        ResetKnifesParameters();
        NextKnife();
        DestroyHudKnifes(scoreNeededToNextLevel);
        NextLevel();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void EnableHud()
    {
        Hud.SetActive(true);
    }

    private void SpawnHudKnifes(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float hudHeight = Hud.GetComponent<RectTransform>().rect.height;
            Vector2 pos = new Vector2(
                HudKnifePosition.transform.position.x,
                HudKnifePosition.transform.position.y + (hudHeight*8/100) * i);
            hudKnifes[i] = Instantiate(HudKnifePrefab, pos, Quaternion.identity) as GameObject;
            hudKnifes[i].transform.SetParent(Hud.transform);
            hudKnifes[i].transform.localScale = HudKnifePosition.transform.localScale;
            hudKnifes[i].SetActive(true);
        }
    }

    private void DestroyHudKnifes(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Destroy(hudKnifes[i]);
        }
    } //Usado ao reiniciar o jogo para destruir as facas restantes no hud

}
