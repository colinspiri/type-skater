using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TypingManager : MonoBehaviour {
    public List<Word> words;
    public TextMeshProUGUI display;

    public List<Word> currentTricks = new List<Word>();
    private bool securedTricks;

    public GameObject completedTextPrefab;

    private void Start() {
        Player.Instance.onJump += () => securedTricks = false;
        Player.Instance.onLand += () => {
            if (securedTricks) {
                foreach (Word trick in currentTricks) {
                    Score.Instance.AddScore(trick.trickScore);
                }
            }
            currentTricks.Clear();
            display.text = "";
        };
    }

    // Update is called once per frame
    void Update() {
        string input = Input.inputString;
        if (input.Equals("")) return;
        if (securedTricks) {
            Debug.Log("secured");
            return;
        }
        
        Debug.Log(input);

        if (Input.GetKeyDown(KeyCode.Backspace)) {
            foreach (Word word in words) {
                word.Clear();
            }
            display.text = "";
        }
        if (Input.GetKeyDown(KeyCode.Return)) securedTricks = true;

        char c = input[0];
        Word currentWord = null;
        foreach (Word w in words) {
            // if the current input matches a word
            if (w.ContinueText(c)) {
                if (currentWord == null) {
                    currentWord = w;
                }
                else if(w.GetTyped().Length > currentWord.GetTyped().Length) {
                    currentWord.Clear();
                    currentWord = w;
                }
                // if user typed the whole word
                if (w.GetTyped().Equals(w.text)) {
                    // add to current tricks
                    if (!Player.Instance.onGround && !securedTricks) {
                        currentTricks.Add(w);
                    }
                    // animate completed text
                    TextMeshProUGUI completedText = Instantiate(completedTextPrefab, display.transform.parent, false).GetComponent<TextMeshProUGUI>();
                    completedText.text = w.text;
                    // clear current typing
                    display.text = "";
                    currentWord = null;
                    break;
                }
            }
        }
        if (currentWord == null) {
            display.text = "";
        }
        else {
            display.text = currentWord.GetTyped();
        }
    }
}

[System.Serializable]
public class Word {
    public string text;
    public int trickScore;
    public UnityEvent onTyped;

    private string hasTyped;
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
        Clear();
        return false;
    }

    public void Clear() {
        curChar = 0;
        hasTyped = "";
    }

    public string GetTyped() {
        return hasTyped;
    }
}
