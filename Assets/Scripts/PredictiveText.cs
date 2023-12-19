using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PredictiveText : MonoBehaviour
{
    // components
    public TextMeshProUGUI predictiveText;
    
    // public constants
    public float pushSuggestionTime;
    public float ollieSuggestionTime;
    
    // state
    private float _timeWaitingForPush;
    private float _timeWaitingForOllie;


    // Start is called before the first frame update
    void Start() {
        predictiveText.text = "";
        
        Player.Instance.OnStateChange += OnStateChange;
        TrickManager.Instance.OnTypeChar += OnTypeChar;
        TrickManager.Instance.OnCompleteTrick += trick => {
            TrickSuggestion.Instance.CountTrick(trick);
            if(Player.Instance.state == Player.State.Midair || Player.Instance.state == Player.State.OnRail) SuggestNewTrick();
            if (trick.Equals("push")) _timeWaitingForPush = 0;
            if (trick.Equals("ollie")) _timeWaitingForOllie = 0;
        };
    }

    private void Update() {
        if (GameManager.Instance.GameStopped) return;
        
        if (Player.Instance.state == Player.State.OnGround) {
            CountGroundTimers();
            if(!TypingManager.Instance.CurrentlyTyping()) CheckGroundSuggestions();
        }
        else {
            _timeWaitingForPush = 0;
            _timeWaitingForOllie = 0;
        }
    }
    
    private void CountGroundTimers() {
        // push
        _timeWaitingForPush += Time.unscaledDeltaTime;

        // ollie
        _timeWaitingForOllie += Time.unscaledDeltaTime;
    }

    private void CheckGroundSuggestions() {
        // suggest ground tricks
        if (_timeWaitingForPush >= pushSuggestionTime) {
            predictiveText.text = "push";
        }
        else if (_timeWaitingForOllie >= ollieSuggestionTime) {
            predictiveText.text = "ollie";
        }
        else predictiveText.text = "";
    }

    

    private void OnTypeChar(bool charCorrect) {
        if (TypingManager.Instance.CurrentlyTyping() == false) return;

        // try to predict what player is typing
        List<Word> possibleWords = TypingManager.Instance.PossibleWords;
        if (possibleWords.Count == 0) predictiveText.text = "";
        else {
            // if one of the words is already predicted, don't do anything
            foreach (var word in possibleWords.Where(word => word.Equals(predictiveText.text))) {
                return;
            }

            // if on ground, just choose first
            if (Player.Instance.state == Player.State.OnGround || Player.Instance.state == Player.State.OnRamp) {
                predictiveText.text = possibleWords[0].Text;
            }
            // if midair, choose based on points/frequency
            else if (Player.Instance.state == Player.State.Midair || Player.Instance.state == Player.State.OnRail) {
                var suggestedTrick = TrickSuggestion.Instance.SuggestTrick(Player.Instance.state, possibleWords);
                predictiveText.text = suggestedTrick;
            }
        }
    }

    private void OnStateChange(Player.State state) {
        predictiveText.text = "";
        if(Player.Instance.state == Player.State.Midair || Player.Instance.state == Player.State.OnRail) SuggestNewTrick();
    }

    private void SuggestNewTrick() {
        var suggestedTrick = TrickSuggestion.Instance.SuggestTrick(Player.Instance.state);
        predictiveText.text = suggestedTrick;
    }
}
