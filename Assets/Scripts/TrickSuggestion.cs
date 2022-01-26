using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TrickSuggestion : MonoBehaviour
{
    // private state
    private Dictionary<Word, int> trickFrequency;
    
    // component stuff
    private TextMeshProUGUI suggestionText;

    private void Awake() {
        suggestionText = GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start() {
        // TODO: load from PlayerPrefs
        trickFrequency = new Dictionary<Word, int>();
        foreach (var word in TypingManager.Instance.allWords) {
            if (word.text == "push" || word.text == "ollie" || word.text == "grab" || word.text == "drop") continue;
            if (word.trickScore > 0) trickFrequency[word] = 0;
        }

        suggestionText.text = "";

        // add callbacks
        TypingManager.Instance.onCompleteWord += CountWord;
        Player.Instance.onStateChange += SuggestTrick;
    }

    private void SuggestTrick(Player.State state) {
        if (state != Player.State.Midair && state != Player.State.OnRail) {
            suggestionText.text = "";
            return;
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
        // string debugstring = "min frequency = " + minValue + " ";
        // foreach (var word in leastUsedWords) {
        //     debugstring += " ";
        // }
        // Debug.Log(debugstring);
        
        // select a random word
        int randomIndex = Random.Range(0, leastUsedWords.Count);
        string suggestedWord = leastUsedWords[randomIndex];
        // display suggested word
        suggestionText.text = suggestedWord;
    }

    private void CountWord(string typed) {
        // ignore non-tricks
        if (typed.Equals("push") || typed.Equals("ollie") || typed.Equals("grab") || typed.Equals("drop")) return;

        // increment frequency
        Word key = null;
        foreach (var word in trickFrequency.Keys) {
            if (word.text.Equals(typed)) {
                key = word;
                break;
            }
        }
        trickFrequency[key]++;

        // if word is suggested, clear it and suggest another
        if (typed.Equals(suggestionText.text)) {
            suggestionText.text = "";
            SuggestTrick(Player.Instance.state);
        }
    }
}
