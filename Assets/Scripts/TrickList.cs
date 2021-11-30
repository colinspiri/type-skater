using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TrickList : MonoBehaviour
{
    private TextMeshProUGUI trickListText;

    [HideInInspector] public bool fakieEnabled;
    [HideInInspector] public bool grabEnabled;
    [HideInInspector] public bool dropEnabled;

    private void Awake() {
        trickListText = GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        trickListText.text = "";
    }

    private void Update() {
        trickListText.text = "";
        bool insertExtraLine = false;
        foreach (Word w in TypingManager.Instance.GetAvailableWords()) {
            if (SceneManager.GetActiveScene().name == "Level0") {
                if (w.text == "fakie" && !fakieEnabled) continue;
                if (w.text == "grab" && !grabEnabled) continue;
                if (w.text == "drop" && !dropEnabled) continue;
            }
            bool lineAfter = w.text == "grab" || w.text == "drop" || w.text == "ollie";
            if (insertExtraLine && !lineAfter) {
                trickListText.text += '\n';
                insertExtraLine = false;
            }
            trickListText.text += w.text + '\n';
            if (lineAfter) insertExtraLine = true;
        }
    }
}
