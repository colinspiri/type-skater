using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypingManager : MonoBehaviour {
    // components
    public static TypingManager Instance;
    [SerializeField] private TextMeshProUGUI displayText;
    
    // input
    private List<Word> _wordList = new List<Word>();
    // only for testing
    [SerializeField] private List<Word> defaultWordList = new List<Word>();

    // state
    private bool _clearOnWrongChar;
    private string _typedText;
    public string TypedText => _typedText;
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

    public void ClearWordList() {
        _wordList.Clear();
        Clear();
    }
    public void SetWordList(List<Word> wordList) {
        _wordList = new List<Word>(wordList);
        Clear();
    }
    public void SetClearOnWrongChar(bool value) {
        _clearOnWrongChar = value;
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
        /*else if (Input.GetKeyDown(KeyCode.Return)) {
            // ignore these keycodes
        }*/
        // new char typed
        else {
            char inputChar = input[0];
            _typedText += inputChar;
            checkWords = true;
        }

        if (checkWords) {
            CheckForWords();
        }
    }

    private void CheckForWords() {
        bool lastCharIsCorrect = false;
        Word wordCompleted = null;
        int minWordLength = int.MaxValue;
        foreach (var possibleWord in PossibleWords) {
            if (possibleWord.Text.Length < minWordLength) minWordLength = possibleWord.Text.Length;
        }
        PossibleWords.Clear();
        
        lastCharIsCorrect = UpdatePossibleWords(ref wordCompleted);

        if (_clearOnWrongChar && lastCharIsCorrect == false) {
            Clear();
            _typedText += Input.inputString[0];
            lastCharIsCorrect = UpdatePossibleWords(ref wordCompleted);
        }
        
        OnTypeChar?.Invoke(lastCharIsCorrect);
        // if(!lastCharIsCorrect && AudioManager.Instance) AudioManager.Instance.PlayTypingWrongSound();

        if(wordCompleted) OnTypeWord?.Invoke(wordCompleted);
    }

    private bool UpdatePossibleWords(ref Word wordCompleted) {
        // loop through all available words and check if input text matches
        bool lastCharIsCorrect = false;
        foreach (var word in _wordList) {
            if (word.Equals(_typedText)) {
                word.Complete();
                Clear();

                wordCompleted = word;
                return true;
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
        return lastCharIsCorrect;
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