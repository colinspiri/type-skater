using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TypingManager : MonoBehaviour {
    // components
    public static TypingManager Instance;
    [SerializeField] private TextMeshProUGUI displayText;
    
    // input
    private List<Word> _wordList = new List<Word>();
    // only for testing
    [SerializeField] private List<Word> defaultWordList = new List<Word>();

    // state
    private string _typedText;
    private int _correctLength;
    private bool TypedAllCorrect => _correctLength == _typedText.Length;
    public List<Word> PossibleWords { get; } = new List<Word>();

    // callbacks
    public static event Action<bool> OnTypeChar;
    public static event Action<Word> OnTypeWord;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if(defaultWordList.Count != 0) SetWordList(defaultWordList);
        Clear();
    }

    public void SetWordList(List<Word> wordList) {
        _wordList = wordList;
        Clear();
    }

    public bool CurrentlyTyping() {
        return _typedText != "";
    }
    
    public void Clear() {
        _typedText = "";
        _correctLength = 0;    
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
                if (TypedAllCorrect) {
                    _correctLength--;
                    _typedText = _typedText.Substring(0, _typedText.Length - 1);
                }
                else {
                    _typedText = _typedText.Substring(0, _correctLength);
                }

                checkWords = true;
            }
            else if (_typedText.Length == 1) {
                _typedText = "";
                _correctLength = 0;
                PossibleWords.Clear();
            }

            // on backspace
        }
        // new char typed
        else {
            char inputChar = input[0];
            _typedText += inputChar;
            checkWords = true;
            
            if(AudioManager.Instance) AudioManager.Instance.PlayTypingSound();
        }

        if (checkWords) {
            CheckForWords();
        }
    }

    private void CheckForWords() {
        bool lastCharIsCorrect = false;
        Word wordCompleted = null;
        PossibleWords.Clear();
        
        // loop through all available words and check if input text matches
        foreach (var word in _wordList) {
            if (word.Equals(_typedText)) {
                word.Complete();
                lastCharIsCorrect = true;
                Clear();

                wordCompleted = word;
                break;
            }
            
            int correctLength = word.StartsWith(_typedText);
            
            if (correctLength == 0) continue;
            // contains wrong characters
            if (correctLength < _typedText.Length) continue;

            lastCharIsCorrect = true;
            
            if (correctLength > _correctLength) {
                _correctLength = correctLength;
                PossibleWords.Clear();
                PossibleWords.Add(word);
            }
            else if (correctLength == _correctLength) {
                _correctLength = correctLength;
                PossibleWords.Add(word);
            }
        }
        
        OnTypeChar?.Invoke(lastCharIsCorrect);
        if(lastCharIsCorrect && AudioManager.Instance) AudioManager.Instance.PlayTypingWrongSound();

        if(wordCompleted) OnTypeWord?.Invoke(wordCompleted);
    }

    private void UpdateDisplay() {
        if (_typedText.Length == 0) {
            displayText.text = "";
            return;
        }

        displayText.text = "";
        // displayText.text += _correctLength + " ";

        if (TypedAllCorrect) {
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

        /*foreach (var possibleWord in _possibleWords) {
            displayText.text += "\n" + possibleWord;
        }*/
    }
    
   
}