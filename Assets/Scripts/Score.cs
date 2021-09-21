using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour {
    public static Score Instance;

    public int score;
    public TextMeshProUGUI scoreDisplay;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    private void Start() {
        scoreDisplay.text = score.ToString();
    }

    public void AddScore(int addition) {
        score += addition;
        scoreDisplay.text = score.ToString();
    }
}
