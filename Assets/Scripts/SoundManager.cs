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

    public AudioSource rolling;
    public AudioSource push;
    public AudioSource jump;
    public AudioSource grind;
    public AudioSource safeLanding;
    
    public AudioSource key;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if (Player.Instance != null) {
            Player.Instance.onJump += () => {
                jump.Play();
            };
            Player.Instance.onSafeLanding += score => {
                safeLanding.Play();
                digThis.Play();
            };
            Player.Instance.onWipeOut += () => {
                recordScratch.Play();
            };;
        }

        if (TypingManager.Instance != null) {
            TypingManager.Instance.onCompleteWord += word => {
                if (word.Equals("push")) {
                    push.Play();
                }
            };
        }
    }

    private void Update() {
        var player = Player.Instance;
        if (player != null) {
            // play rolling sound
            if (player.GetSpeed() > 0 && (player.state == Player.State.OnGround || player.state == Player.State.OnRamp)) {
                if(!rolling.isPlaying) rolling.Play();
            }
            else if(rolling.isPlaying) rolling.Stop();
            // adjust volume based on player speed
            rolling.volume = Mathf.Lerp(0.01f, 0.5f, (player.GetSpeed() - player.minRollingSpeed) / 2*player.maxRollingSpeed);
        
            // grinding sound
            if (player.state == Player.State.OnRail) {
                if(!grind.isPlaying) grind.Play();
            }
            else if(grind.isPlaying) grind.Stop();
        }
    }

    public void PlayTypingSound() {
        key.Play();
    }
}
