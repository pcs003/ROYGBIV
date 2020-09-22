using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public GameObject gamePage;
    public Text scoreText;

    public AudioSource gameAudio;
    public AudioSource bossAudio;
    public AudioSource evilLaughAudio;

    public GameObject boss1;

    public GameObject pauseButton;
    public GameObject unpauseButton;
    private float currentTimeScale;

    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown
    }

    int score = 0;
    public static bool gameOver = true;
    public static bool inBossFight = false;
    public static bool isPaused = false;

    public bool GameOver { get { return gameOver; } }
    public bool InBossFight { get { return inBossFight; } }
    public bool IsPaused { get { return isPaused; } }
    public int Score {  get { return score; } }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        boss1.SetActive(false);

        pauseButton.SetActive(true);
        unpauseButton.SetActive(false);

        startPage.SetActive(true);
        gameOverPage.SetActive(false);
        countdownPage.SetActive(false);
        gamePage.SetActive(false);

    }

    void Update()
    {
        if ( score == 50)
        {
            StopCoroutine(SpeedUp());
            StartCoroutine(BossFight());

        }
    }

    void OnEnable()
    {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerScored += OnPlayerScored;
        TapController.OnPlayerDied += OnPlayerDied;

    }

    void OnDisable()
    {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerScored -= OnPlayerScored;
        TapController.OnPlayerDied -= OnPlayerDied;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        score = 0;
        gameOver = false;
        OnGameStarted(); //event sent to TapController
        StartCoroutine(ScoreKeeper());
        StartCoroutine(SpeedUp());

    }

    void OnPlayerScored()
    {
        
    }

    void OnPlayerDied()
    {
        gameOver = true;

        boss1.SetActive(false);

        int savedScore = PlayerPrefs.GetInt("Highscore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("Highscore", score);
        }

        SetPageState(PageState.GameOver);
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                gamePage.SetActive(true);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                gamePage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                gamePage.SetActive(true);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                gamePage.SetActive(true);
                break;
        }
    }

    public void ConfirmGameOver()
    {
        //activated when replay button pressed
        OnGameOverConfirmed(); //event sent to TapController
        scoreText.text = "0";
        StopCoroutine(ScoreKeeper());
        StopCoroutine(SpeedUp());
        SetPageState(PageState.Start);

        boss1.SetActive(false);
        inBossFight = false;
        bossAudio.Stop();
        gameAudio.Play();
    }

    public void StartGame()
    {
        //acivated when play button pressed
        SetPageState(PageState.Countdown);
        Time.timeScale = 1.0f;
    }

    public void PauseGame()
    {
        if (!isPaused)
        {
            currentTimeScale = Time.timeScale;
            //Debug.Log("Current Time Scale: " + currentTimeScale.ToString());
            Time.timeScale = 0;
            pauseButton.SetActive(false);
            unpauseButton.SetActive(true);
            isPaused = true;
        }
    }

    public void UnpauseGame()
    {
        if (isPaused)
        {
            //Debug.Log("Tried to unpause");
            Time.timeScale = currentTimeScale;
            pauseButton.SetActive(true);
            unpauseButton.SetActive(false);
            isPaused = false;
        }
    }

    IEnumerator ScoreKeeper()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(0.1f);
            score++;
            scoreText.text = score.ToString();
        }
    }

    IEnumerator SpeedUp()
    {
        float scale = 1.0f;
        while (!gameOver)
        {
            yield return new WaitForSeconds(10);
            scale += 0.1f;
            Time.timeScale = scale;
        }
        scale = 1.0f;
        Time.timeScale = scale;
    }

    IEnumerator BossFight()
    {
        inBossFight = true;
        boss1.SetActive(true);
        yield return new WaitForSeconds(3);
        Time.timeScale = 1.0f;
        yield return new WaitForSeconds(2.2f);
        bossAudio.Play();
        gameAudio.Stop();
        yield return new WaitForSeconds(1);
        evilLaughAudio.Play();
        yield return new WaitForSeconds(11.8f);
        inBossFight = false;
        bossAudio.Stop();
        gameAudio.Play();
        yield return new WaitForSeconds(2);
        boss1.SetActive(false);
    }
}
