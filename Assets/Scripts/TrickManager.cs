using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

public class TrickManager : MonoBehaviour {
    public static TrickManager Instance;
    
    // component stuff
    private Animator playerAnimator;
    
    // public constants
    public TextMeshProUGUI typingText;
    public TextMeshProUGUI predictiveText;
    public GameObject completedTextPrefab;
    public List<Trick> allWords;
    
    // state
    private List<Trick> availableWords = new List<Trick>();
    private float timeTyping;
    private int wordsTyped;
    private Trick currentTrick;
    
    // UI
    public GameObject errorTextPrefab;
    
    // callbacks
    public delegate void OnCompleteWord(Trick trick);
    public OnCompleteWord onCompleteWord;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        playerAnimator = Player.Instance.GetComponent<Animator>();

        typingText.text = "";
        predictiveText.text = "";
        
        Player.Instance.onStateChange += UpdateAvailableWords;
        UpdateAvailableWords(Player.Instance.state);

        Player.Instance.onWipeOut += ClearCurrentTyping;
    }

    // Update is called once per frame
    void Update() {
        if (Player.Instance.state == Player.State.Midair || Player.Instance.state == Player.State.OnRail) {
            timeTyping += Time.unscaledDeltaTime;
        }

        string input = Input.inputString;
        if (input.Equals("") || Time.timeScale == 0.0f) return;
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            ClearCurrentTyping();
        }
        
        char c = input[0];
        Trick newCurrentTrick = null;
        for (var i = 0; i < availableWords.Count; i++) {
            Trick w = availableWords[i];
            // if the current input matches a word
            if (w.ContinueText(c)) {
                // play typing sound
                SoundManager.Instance.PlayTypingSound();
                // check if this is the current word the player is typing
                if (newCurrentTrick == null) {
                    newCurrentTrick = w;
                }
                else if (w.GetTyped().Length > newCurrentTrick.GetTyped().Length) {
                    newCurrentTrick.Clear();
                    newCurrentTrick = w;
                }

                // if user typed the whole word
                if (w.GetTyped().Equals(w.text)) {
                    // push
                    if (w.text.Equals("push")) Player.Instance.Push();
                    // ollie
                    else if (w.text.Equals("ollie")) Player.Instance.Jump();
                    // drop
                    else if (w.text.Equals("drop")) {
                        Player.Instance.Drop();
                        availableWords.Remove(w);
                    }
                    // grab
                    else if (w.text.Equals("grab")) {
                        Player.Instance.Grab();
                        availableWords.Remove(w);
                    }

                    // if it's a trick
                    if (w.trickScore > 0) {
                        // do player animation
                        playerAnimator.SetTrigger("trick");
                    }

                    // animate completed text
                    TextMeshProUGUI completedText = Instantiate(completedTextPrefab, typingText.transform.parent, false)
                        .GetComponent<TextMeshProUGUI>();
                    completedText.text = w.text;
                    // clear current typing
                    ClearCurrentTyping();
                    newCurrentTrick = null;
                    // count words
                    wordsTyped++;
                    // call callback
                    onCompleteWord?.Invoke(w);
                    break;
                }
            }
        }

        // check if player made a mistake
        if (currentTrick != null && newCurrentTrick == null && currentTrick.text.Length >= 2) {
            var errorText = Instantiate(errorTextPrefab, typingText.transform, false).GetComponent<TextMeshProUGUI>();
            string wrongCharacter = input.Trim();
            if (wrongCharacter.Length == 0) wrongCharacter = "_";
            errorText.text = typingText.text + "<color=#EC7357>" + wrongCharacter + "</color>";
        }
        // update typing text
        currentTrick = newCurrentTrick;
        typingText.text = currentTrick == null ? "" : currentTrick.GetTyped();
        predictiveText.text = currentTrick == null ? "" : currentTrick.text;
    }

    private void ClearCurrentTyping() {
        currentTrick?.Clear();
        currentTrick = null;
        typingText.text = "";
        predictiveText.text = "";
    }

    public bool IsCurrentlyTyping() {
        return typingText.text != "";
    }

    public void UpdateAvailableWords(Player.State state) {
        availableWords.Clear();
        foreach (var word in allWords) {
            if (word.availableInStates.Contains(state)) {
                availableWords.Add(word);
            }
        }
    }

    public List<Trick> GetAvailableWords() {
        return availableWords;
    }

    public float GetWordsPerMinute() {
        return 60f * wordsTyped / timeTyping;
    }
}

[Serializable]
public class Trick
{
    public string text;
    public int trickScore;
    public List<Player.State> availableInStates;

    private string hasTyped;
    private int curChar;

    public Trick(string t)
    {
        text = t;
        hasTyped = "";
        curChar = 0;
    }

    public bool ContinueText(char c)
    {
        // if c matches
        if (c.Equals(text[curChar]))
        {
            curChar++;
            hasTyped = text.Substring(0, curChar);

            // if we typed the whole word
            if (curChar >= text.Length)
            {
                curChar = 0;
            }
            return true;
        }
        // if c doesn't match
        Clear();
        return false;
    }

    public void Clear()
    {
        curChar = 0;
        hasTyped = "";
    }

    public string GetTyped()
    {
        return hasTyped;
    }
}
