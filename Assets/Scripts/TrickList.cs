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
        foreach (Word w in TypingManager.Instance.words) {
            // skip tricks that you've already done, but not grind
            if (TypingManager.Instance.GetCurrentTricks().Contains(w) && w.text != "grind") continue;
            // skip if not in state
            if (!w.availableInStates.Contains(Player.Instance.state)) continue;
            trickListText.text += w.text + '\n';
        }
    }
}
