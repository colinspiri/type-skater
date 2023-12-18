using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PredictiveText : MonoBehaviour
{
    // components
    public TextMeshProUGUI predictiveText;
    
    // Start is called before the first frame update
    void Start() {
        predictiveText.text = "";
        
        Player.Instance.onStateChange += UpdatePredictiveText;
        TrickManager.Instance.OnTypeChar += OnTypeChar;
        TrickManager.Instance.OnCompleteTrick += trick => {
            TrickSuggestion.Instance.CountTrick(trick);
            UpdatePredictiveText(Player.Instance.state);
        };
    }

    private void OnTypeChar(bool charCorrect) {
        UpdatePredictiveText(Player.Instance.state);
    }

    private void UpdatePredictiveText(Player.State state) {
        if (state == Player.State.OnGround || state == Player.State.OnRamp) {
            predictiveText.text = "";
            return;
        }
        
        // if nothing typed, suggest a trick
        if (!TypingManager.Instance.CurrentlyTyping()) {
            var suggestedTrick = TrickSuggestion.Instance.SuggestTrick(state);
            predictiveText.text = suggestedTrick;
        }

        // if word started, suggest a trick that's among the possible words
        else {
            List<Word> possibleWords = TypingManager.Instance.PossibleWords;
            
            // if word started & no possible words, don't show anything
            if (possibleWords.Count == 0) predictiveText.text = "";
            else {
                var suggestedTrick = TrickSuggestion.Instance.SuggestTrick(state, possibleWords);
                predictiveText.text = suggestedTrick;
            }
        }
    }
}
