using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TypingManager : MonoBehaviour {
    public List<Word> words;
    public TextMeshProUGUI display;

    // Update is called once per frame
    void Update() {
        string input = Input.inputString;
        if (input.Equals("")) return;

        if (Input.GetKeyDown(KeyCode.Backspace)) display.text = "";

        char c = input[0];
        bool match = false;
        foreach (Word w in words) {
            // if the current input matches a word
            if (w.ContinueText(c)) {
                match = true;
                string typed = w.GetTyped();
                // set display text 
                display.text = typed;
                // if user typed the whole word
                if (typed.Equals(w.text)) {
                    Debug.Log("TYPED: " + w.text);
                    // clear current typing
                    display.text = "";
                    break;
                }
            }
        }
        if (!match) {
            display.text = "";
        }
    }
}

[System.Serializable]
public class Word {
    public string text;
    public UnityEvent onTyped;
    [SerializeField] private string hasTyped;
    private int curChar;

    public Word(string t) {
        text = t;
        hasTyped = "";
        curChar = 0;
    }

    public bool ContinueText(char c) {
        // if c matches
        if (c.Equals(text[curChar])) {
            curChar++;
            hasTyped = text.Substring(0, curChar);

            // if we typed the whole word
            if (curChar >= text.Length) {
                onTyped?.Invoke();
                curChar = 0;
            }
            return true;
        }
        // if c doesn't match
        curChar = 0;
        hasTyped = "";
        return false;
    }

    public string GetTyped() {
        return hasTyped;
    }
}
