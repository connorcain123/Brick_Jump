using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public GameObject gameStartPanel, countDownText, pausePanel, levelCompletePanel;
    public AudioClip select;
    public TMP_Text levelScoreText, scoreText, highScoreText, starsCollectedText;

    private bool gameOver;
    private bool gameRunning;
    private bool levelComplete;
    private bool countingDown;
    private int count = 3;
    private AudioSource audioSource;
    private int levelNum, score, starsCollected;
    private bool infiniteGameMode;
    private bool scoreUpdaterRunning;

    public bool LevelComplete { get => levelComplete; set => levelComplete = value; }
    public bool GameOver { get => gameOver; set => gameOver = value; }
    public bool GameRunning { get => gameRunning; set => gameRunning = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Infinite")
        {
            levelScoreText.text = "SCORE: " + score;
            infiniteGameMode = true;
        }
        else
        {
            levelNum = int.Parse(SceneManager.GetActiveScene().name.Replace("Level", ""));
            levelScoreText.text = "LEVEL: " + levelNum;
        }
        
        Time.timeScale = 0f;
        audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("Music"))
        {
            audioSource.volume = PlayerPrefs.GetFloat("Music");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver || levelComplete)
        {
            Time.timeScale = 0f;
            levelScoreText.color = Color.gray;
            if (infiniteGameMode)
            {
                int highscore = PlayerPrefs.GetInt("highscore");
                scoreText.text = "SCORE: " + score + " | HIGHSCORE: " + highscore;
                scoreText.enabled = true;
                scoreText.gameObject.SetActive(true);
                if (score >= highscore)
                {
                    highScoreText.gameObject.SetActive(true);
                    PlayerPrefs.SetInt("highscore", score);
                    PlayerPrefs.Save();
                }
            }
            else
            {
                starsCollectedText.color = Color.gray;
                for (int i = 1; i <= starsCollected; i++)
                {
                    levelCompletePanel.transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
                }
                if(PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Stars", 0) < starsCollected)
                {
                    PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "Stars", starsCollected);
                    PlayerPrefs.Save();
                }
            }
        }

        if(gameRunning && !scoreUpdaterRunning && infiniteGameMode)
        {
            StartCoroutine(ScoreUpdater());
        }

        //if (countingDown)
        //{
        //    if(count > 1)
        //    {
        //        countDownText.GetComponent<TMP_Text>().text = count.ToString("N0"); 
        //        //play countdown audio
        //    }
        //    else
        //    {
        //        countDownText.GetComponent<TMP_Text>().text = "GO!";
        //        //play go audio
        //    }

        //    count -= Time.deltaTime;


        //    if (count < 0)
        //    {
        //        countDownText.SetActive(false);
        //        countingDown = false;
        //        count = 3;
        //        gameRunning = true;
        //        //play backing track
        //        //audioSource.Play();
        //    }
        //}
    }

    private IEnumerator ScoreUpdater()
    {
        while (true)
        {
            scoreUpdaterRunning = true;
            score++;
            levelScoreText.text = "SCORE: " + score;
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator CountDown()
    {
        while (count >= 0)
        {
            if (count == 0)
            {
                countDownText.GetComponent<TMP_Text>().text = "GO!";
            }
            else
            {
                countDownText.GetComponent<TMP_Text>().text = count.ToString();
            }
            count--;
            yield return new WaitForSeconds(1f);
        }

        countDownText.SetActive(false);
        count = 3;
        gameRunning = true;
    }

    public void CollectStar(GameObject star)
    {
        Destroy(star);
        starsCollected++;
        starsCollectedText.text = "STARS: " + starsCollected + "/3";
    }

    public void StartGame()
    {
        if (!gameRunning)
        {
            gameStartPanel.SetActive(false);
            ResumeGame();
        }     
    }

    public void ResumeGame()
    {
        gameRunning = false;
        pausePanel.SetActive(false);
        
        if (!gameStartPanel.activeSelf || gameOver || levelComplete)
        {
            count = 3;
            Time.timeScale = 1f;
            countDownText.SetActive(true);
            StartCoroutine(CountDown());
            //countDownText.SetActive(true);
            //countingDown = true;
            //Time.timeScale = 1f;
        }     
    }

    //public void PlayButtonClick()
    //{
    //    audioSource.PlayOneShot(select);
    //}
}
