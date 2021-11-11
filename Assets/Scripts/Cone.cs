using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cone : MonoBehaviour {
    public int scorePenalty;

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            // wipe out
            Player.Instance.WipeOut();
            // score penalty
            Score.Instance.Penalty(scorePenalty);
            // destroy cone
            Destroy(gameObject);
            
            // if on level0, reset the player
            if (SceneManager.GetActiveScene().name.Equals("Level0")) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
