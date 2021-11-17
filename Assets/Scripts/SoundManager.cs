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
    public AudioSource railland;
    public AudioSource railgrind;
    public AudioSource trick;
    public AudioSource slide; // ramp
    public AudioSource safeLanding;
    public AudioSource skateboardFalling;
    public AudioSource skid;

    public AudioSource scratchAscending;
    public AudioSource scratchDescending;

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
            Player.Instance.onUnsafeLanding += () => {
                skateboardFalling.Play();
            };
            Player.Instance.onWipeOut += () => {
                recordScratch.Play();
                skid.Play();
            };
            Player.Instance.onStateChange += state => {
                if (state == Player.State.OnRail) {
                    railland.Play();
                    railgrind.Play();
                }
                else railgrind.Stop();

                if (state == Player.State.OnRamp) {
                    slide.Play();
                }
                else slide.Stop();
            };
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

            // enter
            if (player.state == Player.State.Midair) {
                if (Input.GetKeyDown(KeyCode.Return)) {
                    scratchAscending.Play();
                }
                if (Input.GetKeyUp(KeyCode.Return)) {
                    scratchDescending.Play();
                }
            }
        }
    }

    public void PlayTypingSound() {
        key.Play();
    }

    public void PlayTrickSound() {
        trick.Play();
    }
}
