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
    private float multiplier;
    public float multiplierIncrement;
    [HideInInspector] public int wipeouts;
    private List<string> staleTricks = new List<string>();
    public int maxStaleTricks;
    
    // unsecured score
    public bool scoreIsUnsecured;
    public GameObject unsecuredScorePrefab;
    private TextMeshProUGUI unsecuredScoreText;
    private Animator unsecuredScoreAnimator;
    public GameObject unsecuredScoreLocation;
    private TextMeshProUGUI multiplierText;
    
    // floating score
    public GameObject flyingScorePrefab;
    
    // component stuff
    private TextMeshProUGUI scoreText;
    public GameObject completedTrickTextPrefab;
    public Color completedTrickTextColor;

    // pause menu + game over
    public bool gameOverOnWipeOut;
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

        TrickManager.Instance.onCompleteWord += CountTrick;

        Player.Instance.onJump += () => {
            // instantiate unsecured score
            GameObject unsecuredScoreObject = Instantiate(unsecuredScorePrefab, unsecuredScoreLocation.transform, false);
            unsecuredScoreText = unsecuredScoreObject.GetComponent<TextMeshProUGUI>();
            unsecuredScoreAnimator = unsecuredScoreObject.GetComponent<Animator>();
            scoreIsUnsecured = true;
            multiplierText = unsecuredScoreObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            // set multiplier based on player speed
            if (Player.Instance.currentSpeed == Player.Speed.Fast) {
                multiplier = 2.0f;
                multiplierText.color = Player.Instance.fastTrailColor;
            }
            else if (Player.Instance.currentSpeed == Player.Speed.Medium) {
                multiplier = 1.5f;
                multiplierText.color = Player.Instance.mediumTrailColor;
            }
            else if (Player.Instance.currentSpeed == Player.Speed.Slow) {
                multiplier = 1.0f;
                multiplierText.color = Player.Instance.slowTrailColor;
            }
            else multiplier = 0;
            multiplierText.text = "x" + multiplier.ToString("F1");
        };
        Player.Instance.onSafeLanding += s => {
            SecureScore();
        };
        Player.Instance.onUnsafeLanding += () => {
            unsecuredScore = 0;
            scoreIsUnsecured = false;
            // destroy unsecured animator
            if (unsecuredScoreAnimator != null) {
                unsecuredScoreAnimator.SetTrigger("lost");
                unsecuredScoreAnimator = null;
                unsecuredScoreText = null;
            }
        };
        Player.Instance.onWipeOut += () => wipeouts++;
        if (gameOverOnWipeOut) Player.Instance.onWipeOut += GameOver;
    }

    private void SecureScore() {
        // add score
        score += unsecuredScore;
        unsecuredScore = 0;
        scoreIsUnsecured = false;
        multiplier = 1;
        scoreText.text = score.ToString();
        // animate unsecured score
        if (unsecuredScoreAnimator != null) {
            unsecuredScoreAnimator.SetBool("secured", true);
            unsecuredScoreAnimator = null;
            unsecuredScoreText = null;
        }
    }

    private void CountTrick(Trick trick) {
        int score = 0;
        if (trick.trickScore > 0) {
            // count how many times the trick appears in the stale list
            int appearing = 0;
            foreach (var staleTrick in staleTricks) {
                if (trick.text == staleTrick) appearing++;
            }
            // calculate score
            float roughScore = trick.trickScore * multiplier;
            float staleMultiplier = 0.6f;
            for (int i = 0; i < appearing; i++) {
                var newRoughScore = roughScore * staleMultiplier;
                if (roughScore - newRoughScore < 1) {
                    roughScore--;
                }
                else roughScore = newRoughScore;
            }
            score = Mathf.FloorToInt(roughScore);
            if (score <= 0) score = 1;
            AddScore(score);
        
            // add to stale tricks
            staleTricks.Add(trick.text);
            // push a trick out if stale maximum reached
            if(staleTricks.Count > maxStaleTricks) staleTricks.RemoveAt(0);
        }
        
        // spawn completed trick text
        GameObject completedTrickText = Instantiate(completedTrickTextPrefab, Player.Instance.transform.position, Quaternion.identity);
        TextMeshProUGUI text = completedTrickText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = trick.trickScore > 0 ? trick.text + " " + score : trick.text;
        text.color = completedTrickTextColor;
    }

    private void AddScore(int addition) {
        // if on ground
        if (Player.Instance.state == Player.State.OnGround) {
            score += addition;
            scoreText.text = score.ToString();
        }
        // if in the air, score is unsecured
        else {
            unsecuredScore += addition;
            unsecuredScoreText.text = unsecuredScore.ToString();

            multiplier += multiplierIncrement;
            multiplierText.text = "x" + multiplier.ToString("F1");
            
            SoundManager.Instance.PlayTrickSound();
            
            var flyingScoreText = Instantiate(flyingScorePrefab, unsecuredScoreText.transform, false)
                .GetComponent<TextMeshProUGUI>();
            flyingScoreText.text = "+" + addition.ToString();
            flyingScoreText.color = multiplierText.color;
        }
    }

    public int GetUnsecuredScore() {
        return unsecuredScore;
    }

    public void GameOver() {
        // remove callback
        if(gameOverOnWipeOut) Player.Instance.onWipeOut -= GameOver;
        // secure unsecured store
        SecureScore();
        
        // compare with high score
        int highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "HighScore", 0);
        if (score > highScore) {
            highScore = score;
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", highScore);
        }

        // display game over
        TextMeshProUGUI gameOverText = Instantiate(gameOverPrefab, transform.parent, false).GetComponent<TextMeshProUGUI>();
        gameOverText.text = "your score: " + score;
        gameOverText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "high score: " + highScore;
        string wipedouttext = "wiped out " + wipeouts + " time";
        if (wipeouts != 1) wipedouttext += "s";
        gameOverText.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = wipedouttext;
        gameOverText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
            Mathf.RoundToInt(TrickManager.Instance.GetWordsPerMinute()) + " words/min";
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
