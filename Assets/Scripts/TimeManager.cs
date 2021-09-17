using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    public static TimeManager Instance;

    public Movement player;

    public float slowTimeScale;
    
    private void Awake() {
        if (Instance == null) Instance = this;
    }

    private void Update() {
        Time.timeScale = player.onGround ? 1.0f : slowTimeScale;
    }
}
