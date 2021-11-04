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

    public void WipeOut() {
        recordScratch.Play();
    }

    public void SafeLanding() {
        digThis.Play();
    }
}
