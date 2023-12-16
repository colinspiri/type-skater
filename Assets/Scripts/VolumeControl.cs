using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeControl : MonoBehaviour {
    public string volumeParameter = "MasterVolume";

    private Slider slider;

    private void Awake() {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(value => {
            AudioManager.Instance.SetVolume(volumeParameter, value);
        });
    }

    // Start is called before the first frame update
    void Start() {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
    }

    private void OnEnable() {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
    }

    private void OnDisable() {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }
}
