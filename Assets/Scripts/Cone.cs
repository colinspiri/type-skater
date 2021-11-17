using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cone : MonoBehaviour {
    public int scorePenalty;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            // wipe out
            Player.Instance.WipeOut();
            // destroy cone
            Destroy(gameObject);
        }
    }
}
