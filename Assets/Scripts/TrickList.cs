using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TrickList : MonoBehaviour
{
    private TextMeshProUGUI trickListText;

    private void Awake() {
        trickListText = GetComponent<TextMeshProUGUI>();
    }
    
    private void Update() {
        trickListText.text = "";
        foreach (Word w in TypingManager.Instance.GetAvailableWords()) {
            trickListText.text += w.text + '\n';
            if (w.text == "drop" || w.text == "grind") trickListText.text += "\n";
        }
    }
}
