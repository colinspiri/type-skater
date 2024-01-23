using System;
using System.Collections.Generic;
using DG.Tweening;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;

public class MultiplierDisplay : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI displayText;

    [SerializeField] private FloatReference currentMultiplier;

    [SerializeField] private float colorChangePeriod;
    [SerializeField] private List<Color> colors;
    private int _currentColorIndex;

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
        bool majorIncrease = index > _currentColorIndex;
        _currentColorIndex = index;
        
        // if changed, animate
        if (majorIncrease && displayText != null) {
            displayText.DOKill();
            displayText.transform.DOScale(1.5f, 0.2f).OnComplete((() => {
                displayText.transform.DOScale(1, 0.4f);
            }));
        }

        // set value
        displayText.text = "x" + currentMultiplier.Value.ToString("F1");
    }
}