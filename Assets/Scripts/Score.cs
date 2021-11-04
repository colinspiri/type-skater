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
            GameObject unsecuredScoreObject = Instantiate(unsecuredScorePrefab, transform, false);
            unsecuredScoreText = unsecuredScoreObject.GetComponent<TextMeshProUGUI>();
            unsecuredScoreAnimator = unsecuredScoreObject.GetComponent<Animator>();
        };
        Player.Instance.onLand += () => {
            // if safe landing
            if (Player.Instance.safe) {
                // add score
                score += unsecuredScore;
                scoreText.text = score.ToString();
                // push
                float multiplier = Mathf.Lerp(0.7f, 2.0f, unsecuredScore / 10.0f);
                Player.Instance.Push(multiplier);
                // animate unsecured score
                if (unsecuredScoreAnimator != null) {
                    unsecuredScoreAnimator.SetBool("secured", true);
                    unsecuredScoreText = null;
                    unsecuredScoreAnimator = null;
                }
            }
            // if crash landing
            else {
                // screen shake
                StartCoroutine(CameraShake.Instance.Shake(0.2f + unsecuredScore * 0.1f));
                if (unsecuredScoreAnimator != null) {
                    Destroy(unsecuredScoreAnimator.gameObject);
                    unsecuredScoreAnimator = null;
                    unsecuredScoreText = null;
                }
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
}
