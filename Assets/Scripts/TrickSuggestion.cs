using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrickSuggestion : MonoBehaviour
{
    // private state
    private Dictionary<Trick, int> trickFrequency;
    
    // component stuff
    public TextMeshProUGUI suggestionText;

    // Start is called before the first frame update
    void Start() {
        trickFrequency = new Dictionary<Trick, int>();
        foreach (var word in TrickManager.Instance.allTricks) {
            if (word.Text == "push" || word.Text == "ollie" || word.Text == "grab" || word.Text == "drop") continue;
            if (word.trickScore > 0) trickFrequency[word] = 0;
        }

        suggestionText.text = "";

        // add callbacks
        TrickManager.Instance.onCompleteTrick += CountTrick;
        Player.Instance.onStateChange += SuggestTrick;
    }

    private void SuggestTrick(Player.State state) {
        if (state == Player.State.OnGround || state == Player.State.OnRamp) {
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
                leastUsedWords.Add(pair.Key.Text);
            }
            else if (pair.Value == minValue) {
                leastUsedWords.Add(pair.Key.Text);
            }
        }
        
        // select a random word
        int randomIndex = Random.Range(0, leastUsedWords.Count);
        string suggestedWord = leastUsedWords[randomIndex];
        // display suggested word
        suggestionText.text = suggestedWord;
    }

    private void CountTrick(Trick trick) {
        // ignore tricks with no score
        if (trick.Text.Equals("push") || trick.Text.Equals("ollie") || trick.Text.Equals("grab") || trick.Text.Equals("drop")) return;

        // increment frequency
        trickFrequency[trick]++;

        // if word is suggested, clear it
        if (trick.Text == suggestionText.text) {
            suggestionText.text = "";
        }
    }
}
