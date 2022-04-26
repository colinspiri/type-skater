using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TrickSuggestion : MonoBehaviour
{
    // constants 
    private float suggestionWaitTime = 0.5f;
    
    // private state
    private Dictionary<Word, int> trickFrequency;
    private Coroutine suggestionCoroutine;
    
    // component stuff
    public TextMeshProUGUI suggestionText;

    private void Awake() {
        suggestionText = GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start() {
        trickFrequency = new Dictionary<Word, int>();
        foreach (var word in TrickManager.Instance.allWords) {
            if (word.text == "push" || word.text == "ollie" || word.text == "grab" || word.text == "drop") continue;
            if (word.trickScore > 0) trickFrequency[word] = 0;
        }

        suggestionText.text = "";

        // add callbacks
        TrickManager.Instance.onCompleteWord += CountWord;
        Player.Instance.onStateChange += state => {
            suggestionText.text = "";
            if(suggestionCoroutine != null) StopCoroutine(suggestionCoroutine);
            suggestionCoroutine = StartCoroutine(SuggestTrick(state));
        };
    }

    private IEnumerator SuggestTrick(Player.State state) {
        if (state != Player.State.Midair && state != Player.State.OnRail) {
            yield break;
        }
        
        // wait some time
        yield return new WaitForSeconds(suggestionWaitTime);
        
        // wait until done typing to suggest trick
        if (TrickManager.Instance.IsCurrentlyTyping()) {
            yield break;
        }
        
        // get a list of the least used words
        int minValue = Int32.MaxValue;
        List<string> leastUsedWords = new List<string>();
        foreach (var pair in trickFrequency) {
            if (!pair.Key.availableInStates.Contains(state)) continue;
            
            if (pair.Value < minValue) {
                minValue = pair.Value;
                leastUsedWords.Clear();
                leastUsedWords.Add(pair.Key.text);
            }
            else if (pair.Value == minValue) {
                leastUsedWords.Add(pair.Key.text);
            }
        }
        
        // select a random word
        int randomIndex = Random.Range(0, leastUsedWords.Count);
        string suggestedWord = leastUsedWords[randomIndex];
        // display suggested word
        suggestionText.text = suggestedWord;
    }

    private void CountWord(Word word) {
        // ignore non-tricks
        if (word.text.Equals("push") || word.text.Equals("ollie") || word.text.Equals("grab") || word.text.Equals("drop")) return;

        // increment frequency
        trickFrequency[word]++;

        // if word is suggested, clear it
        if (word.text == suggestionText.text) {
            suggestionText.text = "";
        }
    }
}
