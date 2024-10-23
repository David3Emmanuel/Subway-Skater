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
    public TextMeshProUGUI scoreText, coinText, modifierText;
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
    }

    void Update()
    {
        if (MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            player.StartRunning();
            FindObjectOfType<GlacierSpawner>().IsScrolling = true;
        }

        if (isGameStarted && !IsDead)
        {
            lastScore = (int)score;
            score += Time.deltaTime * modifierScore;
            if ((int)score > lastScore) scoreText.text = score.ToString("0");
        }
    }

    public void CollectCoin()
    {
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game Scene");
    }

    public void OnDeath()
    {
        IsDead = true;
        deadScoreText.text = scoreText.text;
        deadCoinText.text = coinText.text;
        deathMenuAnimator.SetTrigger("Dead");
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
    }
}
