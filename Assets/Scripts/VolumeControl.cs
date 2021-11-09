using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeControl : MonoBehaviour {
    public AudioMixer mixer;
    public string volumeParameter = "MasterVolume";

    private Slider slider;
    private float multiplier = 30f;

    private void Awake() {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(ChangeVolume);
    }

    private void OnDisable() {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }

    private void ChangeVolume(float value) {
        mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiplier);
    }

    // Start is called before the first frame update
    void Start() {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
    }
}
