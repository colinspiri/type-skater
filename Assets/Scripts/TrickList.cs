using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrickList : MonoBehaviour
{
    // Start is called before the first frame update
    public TypingManager typingManager;
    public TextMeshProUGUI trickDisplay;

    private void Update() {
        trickDisplay.text = "Available Tricks\n";
        foreach (Word w in typingManager.words) {
            // skip tricks that you've already done, but not grind
            if (typingManager.currentTricks.Contains(w) && w.text != "grind") continue;
            // skip if not in state
            if (!w.availableInStates.Contains(Player.Instance.state)) continue;
            trickDisplay.text += w.trickScore + " " + '\t' + w.text + '\n';
        }
    }
}
