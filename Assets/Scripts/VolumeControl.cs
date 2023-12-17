using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeControl : MonoBehaviour {
    public string volumeParameter = "MasterVolume";

    public bool showValue;
    public TextMeshProUGUI valueDisplay;

    private Slider _slider;

    private void Awake() {
        _slider = GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start() {
        _slider.value = PlayerPrefs.GetFloat(volumeParameter, _slider.value);
        _slider.onValueChanged.AddListener(value => {
            AudioManager.Instance.SetVolume(volumeParameter, value);
            CheckUpdateValueDisplay();
        });
    }

    private void OnEnable() {
        _slider.value = PlayerPrefs.GetFloat(volumeParameter, _slider.value);
        CheckUpdateValueDisplay();
    }

    private void OnDisable() {
        PlayerPrefs.SetFloat(volumeParameter, _slider.value);
    }

    private void CheckUpdateValueDisplay() {
        if (showValue)
        {
            valueDisplay.text = (_slider.value).ToString("P0");
        }
    }
}
