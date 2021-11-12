using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cone : MonoBehaviour {
    public int scorePenalty;
    public bool resetGameOnCollide = false;

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            // wipe out
            Player.Instance.WipeOut();
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
