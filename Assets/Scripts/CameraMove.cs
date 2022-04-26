using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
    public GameObject objectToFollow;
    private float followSpeed = 5f;

    private Vector3 offset;

    private bool panningOverLevel;
    private int direction = -1;
    private float levelStartX;
    private float levelEndX;
    private float panVelocity;
    private float panAcceleration = 2f;

    // Start is called before the first frame update
    void Start() {
        offset = transform.position - objectToFollow.transform.position;
        levelStartX = transform.position.x;

        if (Score.Instance != null) {
            Score.Instance.onGameOver += () => {
                levelEndX = Player.Instance.transform.position.x;
                panningOverLevel = true;
            };
        }
    }

    void FixedUpdate() {
        // panning over level
        if (panningOverLevel) {
            if (direction == -1) {
                if (panVelocity > -followSpeed) {
                    panVelocity -= panAcceleration * Time.deltaTime;
                }
                transform.Translate(panVelocity * Time.deltaTime, 0, 0);
                if (transform.position.x < levelStartX) direction = 1;
            }
            else if (direction == 1) {
                if (panVelocity < followSpeed) {
                    panVelocity += panAcceleration * Time.deltaTime;
                }
                transform.Translate(panVelocity * Time.deltaTime, 0, 0 );
                if (transform.position.x > levelEndX) direction = -1;
            }
        }
        else {
            // following player
            float x = Mathf.Lerp(transform.position.x, objectToFollow.transform.position.x + offset.x, followSpeed * Time.deltaTime);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}
