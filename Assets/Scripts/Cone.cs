using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
    }
}
