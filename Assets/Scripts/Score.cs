using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Score : MonoBehaviour {
    public static Score Instance;

    [HideInInspector] public int score;
    private int unsecuredScore;
    
    public GameObject unsecuredScorePrefab;
    private TextMeshProUGUI unsecuredScoreText;
    private Animator unsecuredScoreAnimator;
    
    private TextMeshProUGUI scoreText;

    private void Awake() {
        if (Instance == null) Instance = this;
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        scoreText.text = score.ToString();

        Player.Instance.onJump += () => {
            // instantiate unsecured score
            GameObject unsecuredScoreObject = Instantiate(unsecuredScorePrefab, transform, false);
            unsecuredScoreText = unsecuredScoreObject.GetComponent<TextMeshProUGUI>();
            unsecuredScoreAnimator = unsecuredScoreObject.GetComponent<Animator>();
        };
        Player.Instance.onSafeLanding += () => {
            // add score
            score += unsecuredScore;
            unsecuredScore = 0;
            scoreText.text = score.ToString();
            // animate unsecured score
            if (unsecuredScoreAnimator != null) {
                unsecuredScoreAnimator.SetBool("secured", true);
                unsecuredScoreText = null;
                unsecuredScoreAnimator = null;
            }
        };
        Player.Instance.onUnsafeLanding += () => {
            // destroy unsecured animator
            if (unsecuredScoreAnimator != null) {
                Destroy(unsecuredScoreAnimator.gameObject);
                unsecuredScoreAnimator = null;
                unsecuredScoreText = null;
            }
        };
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
        }
    }

    public int GetUnsecuredScore() {
        return unsecuredScore;
    }
}
