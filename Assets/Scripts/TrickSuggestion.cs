using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrickSuggestion : MonoBehaviour
{
    // components
    public static TrickSuggestion Instance;
    
    // private state
    private Dictionary<Trick, int> _trickFrequency;
    [HideInInspector] public string suggestedText;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        _trickFrequency = new Dictionary<Trick, int>();
        foreach (var word in TrickManager.Instance.allTricks) {
            if (word.Text == "push" || word.Text == "ollie" || word.Text == "grab" || word.Text == "drop") continue;
            if (word.trickScore > 0) _trickFrequency[word] = 0;
        }

        suggestedText = "";

        // add callbacks
        TrickManager.Instance.OnCompleteTrick += CountTrick;
    }

    public string SuggestTrick(Player.State state, List<Word> wordList = null) {
        // get a list of the least used words
        int minValue = Int32.MaxValue;
        List<string> leastUsedWords = new List<string>();
        foreach (var pair in _trickFrequency) {
            if (!pair.Key.availableInStates.Contains(state)) continue;
            if (wordList != null && !wordList.Contains(pair.Key)) continue;
            
            if (pair.Value < minValue) {
                minValue = pair.Value;
                leastUsedWords.Clear();
                leastUsedWords.Add(pair.Key.Text);
            }
            else if (pair.Value == minValue) {
                leastUsedWords.Add(pair.Key.Text);
            }
        }
        
        // select the word with the most points
        if (leastUsedWords.Count > 0) {
            if (!leastUsedWords.Contains(suggestedText)) {
                int randomIndex = Random.Range(0, leastUsedWords.Count);
                suggestedText = leastUsedWords[randomIndex];
            }
        }
        else suggestedText = "";
        
        return suggestedText;
    }

    private void CountTrick(Trick trick) {
        // ignore tricks with no score
        if (trick.Text.Equals("push") || trick.Text.Equals("ollie") || trick.Text.Equals("grab") || trick.Text.Equals("drop")) return;

        // increment frequency
        _trickFrequency[trick]++;

        // if word is suggested, clear it
        if (trick.Text == suggestedText) {
            suggestedText = "";
        }
    }
}
