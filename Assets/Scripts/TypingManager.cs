using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TypingManager : MonoBehaviour {
    public static TypingManager Instance;
    
    // component stuff
    private Animator playerAnimator;
    
    // public data
    public TextMeshProUGUI typingText;
    public List<Word> words;
    public GameObject completedTextPrefab;

    // state
    private List<Word> currentTricks = new List<Word>();

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        playerAnimator = Player.Instance.GetComponent<Animator>();

        Player.Instance.onLand += () => currentTricks.Clear();

        typingText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        string input = Input.inputString;
        if (input.Equals("")) return;
        
        char c = input[0];
        Word currentWord = null;
        foreach (Word w in words) {
            // skip tricks that you've already done, but not grind
            if (currentTricks.Contains(w) && w.text != "grind" && w.text != "drop") continue;
            // skip trick if not in correct state
            if (!w.availableInStates.Contains(Player.Instance.state)) continue;

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
                        currentTricks.Add(w);
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
                    break;
                }
            }
        }
        // update typing text
        typingText.text = currentWord == null ? "" : currentWord.GetTyped();
    }

    public List<Word> GetCurrentTricks() {
        return currentTricks;
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
