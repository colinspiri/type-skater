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
        foreach (var typedWord in TrickManager.Instance.GetAvailableTricks()) {
            var trick = (Trick)typedWord;
            if (SceneManager.GetActiveScene().name == "Level0") {
                if (trick.Text == "fakie" && !fakieEnabled) continue;
                if (trick.Text == "grab" && !grabEnabled) continue;
                if (trick.Text == "drop" && !dropEnabled) continue;
            }
            bool lineAfter = trick.Text == "grab" || trick.Text == "drop" || trick.Text == "ollie";
            if (insertExtraLine && !lineAfter) {
                trickListText.text += '\n';
                insertExtraLine = false;
            }
            trickListText.text += trick.Text + '\n';
            if (lineAfter) insertExtraLine = true;
        }
    }
}
