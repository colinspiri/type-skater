using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    private List<Word> words = new List<Word>();
    private List<Word> doneTricks = new List<Word>();
    
    // callbacks
    public delegate void OnCompleteWord(string word);
    public OnCompleteWord onCompleteWord;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        playerAnimator = Player.Instance.GetComponent<Animator>();

        Player.Instance.onLand += () => doneTricks.Clear();

        typingText.text = "";

        // words.AddRange(basicWords);
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Equals("Level0")) {
            words.AddRange(level0Words);
        }
        if (sceneName.Equals("Level1")) {
            // Debug.Log("scene is " + sceneName + ", adding level 1");
            words.AddRange(level0Words);
            words.AddRange(level1Words);
        }
        else if (sceneName.Equals("Level2")) {
            // Debug.Log("scene is " + sceneName + ", adding level 2");
            words.AddRange(level0Words);
            words.AddRange(level1Words);
            words.AddRange(level2Words);
        }
        else if (sceneName.Equals("Infinite")) {
            words.AddRange(level0Words);
            words.AddRange(level1Words);
            words.AddRange(level2Words);
        }
    }

    // Update is called once per frame
    void Update()
    {
        string input = Input.inputString;
        if (input.Equals("")) return;
        
        char c = input[0];
        Word currentWord = null;
        foreach (Word w in GetAvailableWords()) {
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

                    // if not OnGround, add to current tricks
                    if (w.trickScore > 0 && Player.Instance.state != Player.State.OnGround)
                    {
                        doneTricks.Add(w);
                        // add to score
                        Score.Instance.AddScore(w.trickScore);
                        // do player animation
                        playerAnimator.SetTrigger("trick");
                    }
                    // animate completed text
                    TextMeshProUGUI completedText = Instantiate(completedTextPrefab, typingText.transform.parent, false).GetComponent<TextMeshProUGUI>();
                    completedText.text = w.text;
                    // clear current typing
                    typingText.text = "";
                    currentWord = null;
                    // spawn completed trick text
                    GameObject completedTrickText = Instantiate(completedTrickTextPrefab, Player.Instance.transform.position, Quaternion.identity);
                    completedTrickText.transform.SetParent(transform);
                    TextMeshProUGUI text = completedTrickText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    text.text = w.text;
                    text.color = completedTrickTextColor;
                    // call callback
                    onCompleteWord?.Invoke(w.text);
                    break;
                }
            }
        }
        // update typing text
        typingText.text = currentWord == null ? "" : currentWord.GetTyped();
    }

    public List<Word> GetAvailableWords() {
        List<Word> availableWords = new List<Word>();
        foreach (var w in words) {
            // skip tricks that you've already done, but not grind
            if (doneTricks.Contains(w) && w.text != "grind" && w.text != "drop") continue;
            // skip if not in state
            if (!w.availableInStates.Contains(Player.Instance.state)) continue;
            // add to available words
            availableWords.Add(w);
        }
        return availableWords;
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
