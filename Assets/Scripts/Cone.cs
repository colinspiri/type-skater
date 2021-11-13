using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cone : MonoBehaviour {
    public int scorePenalty;
    public bool resetGameOnCollide = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            // wipe out
            Player.Instance.WipeOut(0.4f);
            // score penalty
            Score.Instance.Penalty(scorePenalty);
            // destroy cone
            Destroy(gameObject);
            
            // reset the player
            if (resetGameOnCollide && !Score.Instance.gameOver) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
