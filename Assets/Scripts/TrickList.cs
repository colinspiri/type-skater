using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrickList : MonoBehaviour
{
    // Start is called before the first frame update
    public TypingManager typingManager;
    public TextMeshProUGUI trickDisplay;
    
    
    void Start()
    {
        trickDisplay.text = "Tricks\n";
        foreach (Word w in typingManager.words) {
            if (w.trickScore == 0) continue;
            trickDisplay.text += w.trickScore + " " + '\t' + w.text + '\n';
        }
    }
}
