using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Timer : MonoBehaviour
{
    public float timeRemaining;

    private TextMeshProUGUI timeDisplay;

    public GameObject levelEnd;

    private void Awake() {
        timeDisplay = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRemaining > 0) {
            if(Time.timeScale != 0) timeRemaining -= Time.unscaledDeltaTime;
            DisplayTime(timeRemaining);
            // move level end so player never hits it
            levelEnd.transform.position = Player.Instance.transform.position + new Vector3(5, 0, 0);
            if (timeRemaining <= 0) {
                timeRemaining = 0;
                timeDisplay.enabled = false;
            }
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
