using System;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;

public class MultiplierDisplay : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI displayText;

    [SerializeField] private FloatReference currentMultiplier;

    [SerializeField] private float colorChangePeriod;
    [SerializeField] private List<Color> colors;

    private void Start() {
        currentMultiplier.AddListener(UpdateDisplay);
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        if (currentMultiplier.Value <= 1) {
            displayText.text = "";
            return;
        }

        // set color
        int index = Mathf.FloorToInt(currentMultiplier.Value / colorChangePeriod) - 1;
        if (index < 0) index = 0;
        if (index >= colors.Count) {
            index = colors.Count - 1;
        }
        displayText.color = colors[index];

        // set value
        displayText.text = "x" + currentMultiplier.Value.ToString("F1");
    }
}