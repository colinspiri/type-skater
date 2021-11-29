using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Score : MonoBehaviour {
    public static Score Instance;

    [HideInInspector] public int score;
    private int unsecuredScore;

    [HideInInspector] public int wipeouts;
    
    public GameObject unsecuredScorePrefab;
    private TextMeshProUGUI unsecuredScoreText;
    private Animator unsecuredScoreAnimator;
    public GameObject unsecuredScoreLocation;
    
    private TextMeshProUGUI scoreText;
    
    // game over
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
        };
        Player.Instance.onSafeLanding += (float s) => {
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
        scoreText.text = score.ToString();
        // animate unsecured score
        if (unsecuredScoreAnimator != null) {
            unsecuredScoreAnimator.SetBool("secured", true);
            unsecuredScoreAnimator = null;
            unsecuredScoreText = null;
        }
    }

    public void AddScore(int addition) {
        // if on ground
        if (Player.Instance.state == Player.State.OnGround) {
            score += addition;
            scoreText.text = score.ToString();
        }
        // if in the air, score is unsecured
        else {
            unsecuredScore += addition;
            unsecuredScoreText.text = unsecuredScore.ToString();
            SoundManager.Instance.PlayTrickSound();
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
