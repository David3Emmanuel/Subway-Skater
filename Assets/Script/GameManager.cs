using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE_AMOUNT = 5;
    public static GameManager Instance;

    private bool isGameStarted = false;
    public bool IsDead { get; set; }
    private PlayerMotor player;

    // UI
    public TextMeshProUGUI scoreText, coinText, modifierText, highScoreText;
    public Animator gameMenuAnimator, menuAnimator, menuDiamondAnimator;
    private float score, coinScore, modifierScore;
    private int lastScore;

    // Death Menu
    public Animator deathMenuAnimator;
    public TextMeshProUGUI deadScoreText, deadCoinText;

    void Awake()
    {
        Instance = this;
        modifierScore = 1.0f;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();

        scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString("0");
        modifierText.text = "x" + modifierScore.ToString("0.0");
        highScoreText.text = "best: " + PlayerPrefs.GetFloat("High Score", 0).ToString("0");
    }

    void Start()
    {
        AudioManager.Instance.backgroundAudio.Play();
    }

    void Update()
    {
        if (MobileInput.Instance.Tap && !isGameStarted) OnStart();

        if (isGameStarted && !IsDead)
        {
            lastScore = (int)score;
            score += Time.deltaTime * modifierScore;
            if ((int)score > lastScore) scoreText.text = score.ToString("0");
        }
    }

    void OnStart()
    {
        isGameStarted = true;
        player.StartRunning();
        FindObjectOfType<GlacierSpawner>().IsScrolling = true;
        FindObjectOfType<CameraMotor>().IsMoving = true;
        gameMenuAnimator.SetTrigger("Show");
        menuAnimator.SetTrigger("Hide");
        AudioManager.Instance.clickAudio.Play();
    }

    public void CollectCoin()
    {
        menuDiamondAnimator.SetTrigger("Collect");
        AudioManager.Instance.coinAudio.Play();
        coinScore++;
        coinText.text = coinScore.ToString("0");

        score += COIN_SCORE_AMOUNT;
        scoreText.text = score.ToString("0");
    }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    public bool IsGameStarted
    {
        get
        {
            return isGameStarted;
        }
    }

    public void OnPlay()
    {
        AudioManager.Instance.clickAudio.Play();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game Scene");
    }

    public void OnDeath()
    {
        IsDead = true;
        deadScoreText.text = scoreText.text;
        deadCoinText.text = coinText.text;
        deathMenuAnimator.SetTrigger("Dead");
        gameMenuAnimator.SetTrigger("Hide");
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
        AudioManager.Instance.backgroundAudio.Stop();
        AudioManager.Instance.deathAudio.Play();

        if (score > PlayerPrefs.GetFloat("High Score", 0))
        {
            PlayerPrefs.SetFloat("High Score", score);
        }
    }
}
