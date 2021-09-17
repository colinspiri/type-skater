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

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Alpha1)) AddScore(1);
    }

    public void AddScore(int addition) {
        score += addition;
        scoreDisplay.text = score.ToString();
    }
}