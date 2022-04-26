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
    private Dictionary<Trick, int> trickFrequency;
    
    // component stuff
    private TextMeshProUGUI suggestionText;

    private void Awake() {
        suggestionText = GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start() {
        // TODO: load from PlayerPrefs
        trickFrequency = new Dictionary<Trick, int>();
        foreach (var word in TrickManager.Instance.allWords) {
            if (word.text == "push" || word.text == "ollie" || word.text == "grab" || word.text == "drop") continue;
            if (word.trickScore > 0) trickFrequency[word] = 0;
        }

        suggestionText.text = "";

        // add callbacks
        TrickManager.Instance.onCompleteWord += CountWord;
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
        
        // select a random word
        int randomIndex = Random.Range(0, leastUsedWords.Count);
        string suggestedWord = leastUsedWords[randomIndex];
        // display suggested word
        suggestionText.text = suggestedWord;
    }

    private void CountWord(Trick trick) {
        // ignore non-tricks
        if (trick.text.Equals("push") || trick.text.Equals("ollie") || trick.text.Equals("grab") || trick.text.Equals("drop")) return;

        // increment frequency
        trickFrequency[trick]++;

        // if word is suggested, clear it
        if (trick.text == suggestionText.text) {
            suggestionText.text = "";
        }
    }
}
