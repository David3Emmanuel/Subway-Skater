using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private bool isGameStarted = false;
    private PlayerMotor player;

    // UI
    public TextMeshProUGUI scoreText, coinText, modifierText;
    private float score, coinScore, modifierScore;

    void Awake() {
        Instance = this;
        UpdateScores();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
    }

    void Update() {
        if (MobileInput.Instance.Tap && !isGameStarted) {
            isGameStarted = true;
            player.StartRunning();
        }
    }

    public void UpdateScores() {
        scoreText.text = score.ToString();
        coinText.text = coinScore.ToString();
        modifierText.text = modifierScore.ToString();
    }

    public bool IsGameStarted {
        get {
            return isGameStarted;
        }
    }
}
