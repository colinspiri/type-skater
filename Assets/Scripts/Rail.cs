using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Rail : MonoBehaviour
{
    public Transform thresholdAbove;
    public Transform thresholdBelow;

    public bool disableCollider;

    private BoxCollider2D boxCollider;
    private bool playerAboveRail;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        playerAboveRail = false;
    }

    // Update is called once per frame
    void Update() {
        if (disableCollider) {
            boxCollider.enabled = false;
            return;
        }
        
        var playerTransform = Player.Instance.transform;

        var right_side = playerTransform.position.x + (playerTransform.localScale.x / 2);
        var left_pole = this.gameObject.transform.GetChild(4);

        var left_side = left_pole.transform.position.x - (left_pole.transform.localScale.x / 2);
        if (right_side < left_side)
        {
            playerAboveRail = false;
        }

        if (playerAboveRail)
        {
            boxCollider.enabled = true;

            if (playerTransform.position.y + playerTransform.localScale.y / 2 < thresholdBelow.position.y)
                playerAboveRail = false;
        }
        else
        {
            boxCollider.enabled = false;

            if (playerTransform.position.y - playerTransform.localScale.y / 2 > thresholdAbove.position.y) playerAboveRail = true;
        }
    }
}
