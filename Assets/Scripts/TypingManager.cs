using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TypingManager : MonoBehaviour
{
    public List<Word> words;
    public TextMeshProUGUI typingText;

    public List<Word> currentTricks = new List<Word>();
    
    public GameObject unsecuredScorePrefab;
    private TextMeshProUGUI unsecuredScoreText;
    private Animator unsecuredScoreAnimator;

    public GameObject completedTextPrefab;
    
    private void Start() {
        typingText.text = "";
        Player.Instance.onJump += () => {
            // Debug.Log("onJump");
            GameObject unsecuredScore = Instantiate(unsecuredScorePrefab, Score.Instance.scoreDisplay.transform, false);
            unsecuredScoreText = unsecuredScore.GetComponent<TextMeshProUGUI>();
            unsecuredScoreAnimator = unsecuredScore.GetComponent<Animator>();
        };
        Player.Instance.onLand += () => {
            // calculate score
            int scoreAdded = 0;
            foreach (Word trick in currentTricks) {
                scoreAdded += trick.trickScore;
            }
            // if safe landing
            if (Player.Instance.safe) {
                // add score
                Score.Instance.AddScore(scoreAdded);
                // push
                float multiplier = Mathf.Lerp(0.7f, 2.0f, scoreAdded/10.0f);
                Player.Instance.Push(multiplier);
                // animate score
                unsecuredScoreAnimator.SetBool("secured", true);
                unsecuredScoreText = null;
                unsecuredScoreAnimator = null;
            }
            // if crash landing
            else {
                // screen shake
                StartCoroutine(CameraShake.Instance.Shake(0.2f + scoreAdded*0.1f));
                Destroy(unsecuredScoreAnimator.gameObject);
                unsecuredScoreAnimator = null;
                unsecuredScoreText = null;
            }
            // securedTricks = false;
            currentTricks.Clear();
            typingText.text = "";
        };
    }

    // Update is called once per frame
    void Update()
    {
        string input = Input.inputString;
        if (input.Equals("")) return;
        
        char c = input[0];
        Word currentWord = null;
        foreach (Word w in words) {
            // skip tricks that you've already done
            if (currentTricks.Contains(w)) continue;
            
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
                    // add to current tricks
                    if (!Player.Instance.onGround)
                    {
                        currentTricks.Add(w);
                        // update unsecured score text
                        int score = 0;
                        foreach (Word trick in currentTricks) {
                            score += trick.trickScore;
                        }
                        unsecuredScoreText.text = score.ToString();
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
        if (currentWord == null)
        {
            typingText.text = "";
        }
        else
        {
            typingText.text = currentWord.GetTyped();
        }
    }
}

[Serializable]
public class Word
{
    public string text;
    public int trickScore;
    public UnityEvent onTyped;

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
                onTyped?.Invoke();
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
