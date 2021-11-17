using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Rail : MonoBehaviour {
    public Transform thresholdAbove;
    public Transform thresholdBelow;
    
    private BoxCollider2D boxCollider;
    private bool playerAboveRail;

    private void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update() {
        var playerTransform = Player.Instance.transform;
        
        if (playerAboveRail) {
            boxCollider.enabled = true;

            if (playerTransform.position.y + playerTransform.localScale.y / 2 < thresholdBelow.position.y)
                playerAboveRail = false;
        }
        else {
            boxCollider.enabled = false;

            if (playerTransform.position.y - playerTransform.localScale.y / 2 > thresholdAbove.position.y) playerAboveRail = true;
        }
    }
}
