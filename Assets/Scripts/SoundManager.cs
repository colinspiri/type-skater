using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance;
    
    // music
    public AudioSource music;
    // SFX
    public AudioSource recordScratch;
    public AudioSource digThis;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if (Player.Instance != null) {
            Player.Instance.onSafeLanding += (float score) => {
                SafeLanding();
            };
            Player.Instance.onUnsafeLanding += WipeOut;
        }
    }

    public void PlayTestSFX() {
        recordScratch.Play();
    }

    private void WipeOut() {
        recordScratch.Play();
    }

    private void SafeLanding() {
        digThis.Play();
    }
}
