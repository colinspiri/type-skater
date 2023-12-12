using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypingManager : MonoBehaviour {
    // constants
    public List<TypedWord> _availableWords = new List<TypedWord>();

    // state
    private string _typedText;
    private int _correctLength;
    private bool _typedAllCorrect => _correctLength == _typedText.Length;
    private List<TypedWord> _possibleWords = new List<TypedWord>();

    // components
    public TextMeshProUGUI displayText;

    private void Start() {
        Clear();
    }

    private void Update() {
        HandleInput();

        if (displayText != null) {
            UpdateDisplay();
        }
    }

    private void HandleInput() {
        string input = Input.inputString;
        if (input.Equals("")) return;

        // backspace
        bool checkWords = false;
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            if (_typedText.Length > 1) {
                if (_typedAllCorrect) {
                    _correctLength--;
                }
                _typedText = _typedText.Substring(0, _typedText.Length - 1);

                checkWords = true;
            }
            else if (_typedText.Length == 1) {
                _typedText = "";
                _correctLength = 0;
                _possibleWords.Clear();
            }

            // on backspace
        }
        // new char typed
        else {
            char inputChar = input[0];
            _typedText += inputChar;
            checkWords = true;
            // onTyping?.Invoke(); each time a new character is typed
        }

        if (checkWords) {
            CheckForWords();
        }
    }

    private void CheckForWords() {
        bool lastCharIsCorrect = false;
        _possibleWords.Clear();
        
        // loop through all available words and check if input text matches
        foreach (var word in _availableWords) {
            if (word.Equals(_typedText)) {
                // on complete word
                lastCharIsCorrect = true;
                word.Complete();
                
                // clear 
                Clear();
                break;
            }
            
            int correctLength = word.StartsWith(_typedText);
            
            if (correctLength == 0) continue;
            // contains wrong characters
            if (correctLength < _typedText.Length) continue;

            lastCharIsCorrect = true;
            
            if (correctLength > _correctLength) {
                _correctLength = correctLength;
                _possibleWords.Clear();
                _possibleWords.Add(word);
            }
            else if (correctLength == _correctLength) {
                _correctLength = correctLength;
                _possibleWords.Add(word);
            }
        }

        if (lastCharIsCorrect) {
            // on type correct char
            // Debug.Log("correct char");
        }
        else {
            // on type wrong char
            // Debug.Log("wrong char");
        }
    }

    private void UpdateDisplay() {
        if (_typedText.Length == 0) {
            displayText.text = "";
            return;
        }

        displayText.text = _correctLength.ToString() + "\t";

        if (_typedAllCorrect) {
            displayText.text += _typedText;
        }
        else if(_correctLength > 0) {
            displayText.text += _typedText.Substring(0, _correctLength);
            displayText.text += "<color=#EC7357>";
            displayText.text += _typedText.Substring(_correctLength);
            displayText.text += "</color>";
        }
        else {
            displayText.text += "<color=#EC7357>";
            displayText.text += _typedText;
            displayText.text += "</color>";
        }

        foreach (var possibleWord in _possibleWords) {
            displayText.text += "\n" + possibleWord;
        }
    }
    
    private void Clear() {
        _typedText = "";
        _correctLength = 0;    
    }
}