using System;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;

public class TypingManager : MonoBehaviour {
    // components
    public static TypingManager Instance;
    
    // input
    private List<Word> _wordList = new List<Word>();
    // only for testing
    [SerializeField] private List<Word> defaultWordList = new List<Word>();

    // state
    private bool _clearOnWrongChar;
    public string TypedText { get; private set; }

    private int CorrectLength { get; set; }

    private bool TypedAllCorrect => CorrectLength == TypedText.Length;
    public List<Word> PossibleWords { get; } = new List<Word>();
    public StringVariable correctText;
    public StringVariable wrongText;

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
        return TypedText != "";
    }
    
    public void Clear() {
        TypedText = "";
        CorrectLength = 0;
    }

    private void Update() {
        HandleInput();

        if (correctText && wrongText) {
            UpdateDisplay();
        }
    }

    private void HandleInput() {
        string input = Input.inputString;
        if (input.Equals("")) return;

        // backspace
        bool checkWords = false;
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            bool controlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)
                                                                    || Input.GetKey(KeyCode.LeftShift) ||
                                                                    Input.GetKey(KeyCode.RightShift);
            if (TypedText.Length > 1) {
                if (TypedAllCorrect) {
                    if (controlPressed) {
                        Clear();
                    }
                    else {
                        CorrectLength--;
                        TypedText = TypedText.Substring(0, TypedText.Length - 1);
                    }
                }
                else {
                    if (controlPressed) {
                        TypedText = TypedText.Substring(0, CorrectLength);
                    }
                    else {
                        TypedText = TypedText.Substring(0, TypedText.Length - 1);
                    }
                }

                checkWords = true;
            }
            else if (TypedText.Length == 1) {
                Clear();
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
            TypedText += inputChar;
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
            TypedText += Input.inputString[0];
            lastCharIsCorrect = UpdatePossibleWords(ref wordCompleted);
        }
        
        OnTypeChar?.Invoke(lastCharIsCorrect);
        if(wordCompleted) OnTypeWord?.Invoke(wordCompleted);
    }

    private bool UpdatePossibleWords(ref Word wordCompleted) {
        // loop through all available words and check if input text matches
        bool lastCharIsCorrect = false;
        foreach (var word in _wordList) {
            if (word.Equals(TypedText)) {
                word.Complete();
                Clear();

                wordCompleted = word;
                return true;
            }
            
            int correctLength = word.StartsWith(TypedText);
            
            if (correctLength == 0) continue;
            // contains wrong characters
            if (correctLength < TypedText.Length) continue;

            lastCharIsCorrect = true;
            
            if (correctLength > CorrectLength) {
                CorrectLength = correctLength;
                PossibleWords.Clear();
                PossibleWords.Add(word);
            }
            else if (correctLength == CorrectLength) {
                CorrectLength = correctLength;
                PossibleWords.Add(word);
            }
        }
        return lastCharIsCorrect;
    }
    private void UpdateDisplay() {
        //Debug.Log("UpdateDisplay()");

        /*string correctString = "hel";
        correctText.SetValue(correctString);

        string wrongString = "lo";
        wrongText.SetValue(wrongString);*/

        if (TypedText.Length == 0) {
            correctText.Value = "";
            wrongText.Value = "";
        }
        else if (TypedAllCorrect) {
            correctText.Value = TypedText;
            wrongText.Value = "";
        }
        else if(CorrectLength > 0) {
            correctText.Value = TypedText.Substring(0, CorrectLength);
            wrongText.Value = TypedText.Substring(CorrectLength);
        }
        else {
            correctText.Value = "";
            wrongText.Value = TypedText;
        }
    }
    
   
}