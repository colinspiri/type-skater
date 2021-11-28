using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

public class TypingManager : MonoBehaviour {
    public static TypingManager Instance;
    
    // component stuff
    private Animator playerAnimator;
    
    // public data
    public TextMeshProUGUI typingText;
    public GameObject completedTextPrefab;
    public List<Word> level0Words;
    public List<Word> level1Words;
    public List<Word> level2Words;
    public GameObject completedTrickTextPrefab;
    public Color completedTrickTextColor;

    // state
    private List<Word> allWords = new List<Word>();
    private List<Word> availableWords = new List<Word>();
    private List<Word> tricksLeft = new List<Word>();
    private float timeTyping;
    private int wordsTyped;
    private Word currentWord;
    
    // UI
    public GameObject errorTextPrefab;
    
    // callbacks
    public delegate void OnCompleteWord(string word);
    public OnCompleteWord onCompleteWord;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        playerAnimator = Player.Instance.GetComponent<Animator>();

        typingText.text = "";

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Equals("Level0")) {
            allWords.AddRange(level0Words);
        }
        if (sceneName.Equals("Level1")) {
            allWords.AddRange(level0Words);
            allWords.AddRange(level1Words);
        }
        else if (sceneName.Equals("Level2")) {
            allWords.AddRange(level0Words);
            allWords.AddRange(level1Words);
            allWords.AddRange(level2Words);
        }
        else if (sceneName.Equals("Infinite")) {
            allWords.AddRange(level0Words);
            allWords.AddRange(level1Words);
            allWords.AddRange(level2Words);
        }
        Player.Instance.onStateChange += UpdateAvailableWords;
        UpdateAvailableWords(Player.Instance.state);
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.Instance.state == Player.State.Midair || Player.Instance.state == Player.State.OnRail) {
            timeTyping += Time.unscaledDeltaTime;
            if(availableWords.Count <= 2) AddRandomTrick();
        }

        string input = Input.inputString;
        if (input.Equals("")) return;
        
        char c = input[0];
        Word newCurrentWord = null;
        foreach (Word w in availableWords) {
            // if the current input matches a word
            if (w.ContinueText(c)) {
                // play typing sound
                SoundManager.Instance.PlayTypingSound();
                // check if this is the current word the player is typing
                if (newCurrentWord == null) {
                    newCurrentWord = w;
                }
                else if(w.GetTyped().Length > newCurrentWord.GetTyped().Length) {
                    newCurrentWord.Clear();
                    newCurrentWord = w;
                }
                // if user typed the whole word
                if (w.GetTyped().Equals(w.text))
                {
                    // push
                    if (w.text.Equals("push")) Player.Instance.Push();
                    // ollie
                    else if (w.text.Equals("ollie")) Player.Instance.Jump();
                    // drop
                    else if (w.text.Equals("drop")) Player.Instance.Drop();
                    // grind
                    else if (w.text.Equals("grind")) Player.Instance.grindCount++;

                    // if it's a trick
                    if (w.trickScore > 0) {
                        // add to score
                        Score.Instance.AddScore(w.trickScore);
                        // do player animation
                        playerAnimator.SetTrigger("trick");
                        // remove from available words
                        if (!w.text.Equals("drop") && !w.text.Equals("grind")) availableWords.Remove(w);
                    }
                    // animate completed text
                    TextMeshProUGUI completedText = Instantiate(completedTextPrefab, typingText.transform.parent, false).GetComponent<TextMeshProUGUI>();
                    completedText.text = w.text;
                    // spawn completed trick text
                    GameObject completedTrickText = Instantiate(completedTrickTextPrefab, Player.Instance.transform.position, Quaternion.identity);
                    TextMeshProUGUI text = completedTrickText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    text.text = w.text;
                    text.color = completedTrickTextColor;
                    // clear current typing
                    typingText.text = "";
                    newCurrentWord = null;
                    currentWord = null;
                    // count words
                    wordsTyped++;
                    // call callback
                    onCompleteWord?.Invoke(w.text);
                    break;
                }
            }
        }
        // check if player made a mistake
        if (currentWord != null && newCurrentWord == null && currentWord.text.Length >= 2) {
            var errorText = Instantiate(errorTextPrefab, typingText.transform, false).GetComponent<TextMeshProUGUI>();
            string wrongCharacter = input.Trim();
            if (wrongCharacter.Length == 0) wrongCharacter = "_";
            errorText.text = typingText.text + "<color=#EC7357>" + wrongCharacter + "</color>";
        }
        // update typing text
        currentWord = newCurrentWord;
        typingText.text = currentWord == null ? "" : currentWord.GetTyped();
    }

    private void UpdateAvailableWords(Player.State state) {
        availableWords.Clear();
        tricksLeft.Clear();
        foreach (var word in allWords) {
            if (word.availableInStates.Contains(state)) {
                tricksLeft.Add(word);
                if (state != Player.State.Midair && state != Player.State.OnRail) {
                    availableWords.Add(word);
                }
            }
        }
        if (state == Player.State.Midair || state == Player.State.OnRail) {
            // add "drop" if it exists
            if (state == Player.State.Midair) {
                int dropIndex = tricksLeft.FindIndex(word => word.text.Equals("drop"));
                if (dropIndex != -1) {
                    availableWords.Add(tricksLeft[dropIndex]);
                    tricksLeft.RemoveAt(dropIndex);
                }
            }
            // choose random tricks
            for (int i = 0; i < 2; i++) {
                AddRandomTrick();
            }
        }
    }

    public List<Word> GetAvailableWords() {
        return availableWords;
    }

    private void AddRandomTrick() {
        if (tricksLeft.Count == 0) return; // if no tricks left, do nothing
        int randomIndex = Random.Range(0, tricksLeft.Count);
        availableWords.Add(tricksLeft[randomIndex]);
        tricksLeft.RemoveAt(randomIndex);
    }

    public float GetWordsPerMinute() {
        return 60f * wordsTyped / timeTyping;
    }
}

[Serializable]
public class Word
{
    public string text;
    public int trickScore;
    public List<Player.State> availableInStates;

    private string hasTyped;
    private int curChar;

    public Word(string t)
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
