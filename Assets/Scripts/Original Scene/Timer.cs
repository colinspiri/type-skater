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
    public GameObject gameOverPrefab;

    public List<GameObject> objectsToDisable;
    public FollowPlayer cameraFollow;

    private TextMeshProUGUI timeDisplay;

    private void Awake() {
        timeDisplay = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRemaining > 0) {
            timeRemaining -= Time.unscaledDeltaTime;
            DisplayTime(timeRemaining);
            if (timeRemaining <= 0) {
                timeRemaining = 0;
                // display game over
                TextMeshProUGUI gameOverText = Instantiate(gameOverPrefab, transform.parent, false).GetComponent<TextMeshProUGUI>();
                gameOverText.text = "your score: " + Score.Instance.score;
                // disable other objects
                cameraFollow.enabled = false;
                Time.timeScale = 1.0f;
                timeDisplay.enabled = false;
                // Player.Instance.enabled = false;
                foreach (GameObject o in objectsToDisable) {
                    o.SetActive(false);
                }
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
