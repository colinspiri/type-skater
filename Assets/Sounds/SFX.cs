using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public AudioSource scratch;
    public AudioSource voice;

    public void playScratch(){
        scratch.Play();
    }

    public void playVoice(){
        voice.Play();
    }

}
