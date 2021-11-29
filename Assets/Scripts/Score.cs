using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Score : MonoBehaviour {
    public static Score Instance;

    // score-keeping data
    [HideInInspector] public int score;
    private int unsecuredScore;
    private int multiplier;
    [HideInInspector] public int wipeouts;
    
    // unsecured score
    public GameObject unsecuredScorePrefab;
    private TextMeshProUGUI unsecuredScoreText;
    private Animator unsecuredScoreAnimator;
    public GameObject unsecuredScoreLocation;
    private Color multiplierColor;
    
    // floating score
    public GameObject flyingScorePrefab;
    
    // component stuff
    private TextMeshProUGUI scoreText;
    
    // pause menu + game over
    public GameObject gameOverPrefab;
    public List<GameObject> objectsToDisable;
    public PauseMenu pauseMenu;
    public delegate void OnGameOver();
    public OnGameOver onGameOver;

    private void Awake() {
        if (Instance == null) Instance = this;
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        scoreText.text = score.ToString();

        Player.Instance.onJump += () => {
            
            // instantiate unsecured score
            GameObject unsecuredScoreObject = Instantiate(unsecuredScorePrefab, unsecuredScoreLocation.transform, false);
            unsecuredScoreText = unsecuredScoreObject.GetComponent<TextMeshProUGUI>();
            unsecuredScoreAnimator = unsecuredScoreObject.GetComponent<Animator>();
            var multiplierText = unsecuredScoreObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            // set multiplier based on player speed
            if (Player.Instance.currentSpeed == Player.Speed.Fast) {
                multiplier = 3;
                multiplierColor = Player.Instance.fastTrailColor;
            }
            else if (Player.Instance.currentSpeed == Player.Speed.Medium) {
                multiplier = 2;
                multiplierColor = Player.Instance.mediumTrailColor;
            }
            else if (Player.Instance.currentSpeed == Player.Speed.Slow) {
                multiplier = 1;
                multiplierColor = Player.Instance.slowTrailColor;
            }
            else multiplier = 0;
            multiplierText.text = "x" + multiplier;
            multiplierText.color = multiplierColor;
        };
        Player.Instance.onSafeLanding += s => {
            SecureScore();
        };
        Player.Instance.onUnsafeLanding += () => {
            unsecuredScore = 0;
            // destroy unsecured animator
            if (unsecuredScoreAnimator != null) {
                unsecuredScoreAnimator.SetTrigger("lost");
                unsecuredScoreAnimator = null;
                unsecuredScoreText = null;
            }
        };
        Player.Instance.onWipeOut += () => wipeouts++;
    }

    private void SecureScore() {
        // add score
        score += unsecuredScore;
        unsecuredScore = 0;
        multiplier = 1;
        scoreText.text = score.ToString();
        // animate unsecured score
        if (unsecuredScoreAnimator != null) {
            unsecuredScoreAnimator.SetBool("secured", true);
            unsecuredScoreAnimator = null;
            unsecuredScoreText = null;
        }
    }

    public void AddScore(int addition) {
        int addedScore = addition * multiplier;
        // if on ground
        if (Player.Instance.state == Player.State.OnGround) {
            score += addedScore;
            scoreText.text = score.ToString();
        }
        // if in the air, score is unsecured
        else {
            unsecuredScore += addedScore;
            unsecuredScoreText.text = unsecuredScore.ToString();
            SoundManager.Instance.PlayTrickSound();
            var flyingScoreText = Instantiate(flyingScorePrefab, unsecuredScoreText.transform, false)
                .GetComponent<TextMeshProUGUI>();
            flyingScoreText.text = "+" + addedScore.ToString();
            flyingScoreText.color = multiplierColor;
        }
    }

    public int GetUnsecuredScore() {
        return unsecuredScore;
    }

    public void GameOver() {
        SecureScore();
        // display game over
        TextMeshProUGUI gameOverText = Instantiate(gameOverPrefab, transform.parent, false).GetComponent<TextMeshProUGUI>();
        gameOverText.text = "your score: " + score;
        string wipedouttext = "wiped out " + wipeouts + " time";
        if (wipeouts != 1) wipedouttext += "s";
        gameOverText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = wipedouttext;
        gameOverText.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            Mathf.RoundToInt(TypingManager.Instance.GetWordsPerMinute()) + " words/min";
        // disable other objects
        Player.Instance.SetSpeed(Player.Speed.Slow);
        Player.Instance.currentSpeed = Player.Speed.Stopped;
        Time.timeScale = 1.0f;
        foreach (GameObject o in objectsToDisable) {
            o.SetActive(false);
        }
        pauseMenu.enabled = false;
        scoreText.enabled = false;
        onGameOver?.Invoke();
    }
}
