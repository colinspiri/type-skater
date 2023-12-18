using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrickManager : MonoBehaviour {
    public static TrickManager Instance;

    // component stuff
    private Animator playerAnimator;

    // public constants
    // public TextMeshProUGUI predictiveText;
    public GameObject completedTextStartTransform;
    public GameObject completedTextPrefab;
    public List<Trick> allTricks;

    // state
    private List<Word> availableTricks = new List<Word>();

    // UI
    // public GameObject errorTextPrefab;

    // callbacks
    public event Action<Trick> OnCompleteTrick;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        playerAnimator = Player.Instance.GetComponent<Animator>();

        Player.Instance.onStateChange += _ => TypingManager.Instance.Clear();
        Player.Instance.onStateChange += UpdateAvailableTricks;
        UpdateAvailableTricks(Player.Instance.state);
    }

    private void OnEnable() {
        TypingManager.OnTypeChar += OnTypeChar;
        TypingManager.OnTypeWord += OnTypeWord;
    }
    private void OnDisable() {
        TypingManager.OnTypeChar -= OnTypeChar;
        TypingManager.OnTypeWord -= OnTypeWord;
    }

    private void OnTypeChar(bool charIsCorrect) {
        if (charIsCorrect) {
            if(AudioManager.Instance) AudioManager.Instance.PlayTypingSound();
        }
        else {
            if(AudioManager.Instance) AudioManager.Instance.PlayTypingWrongSound();
        }
        
        if (Player.Instance.state == Player.State.Midair || Player.Instance.state == Player.State.OnRail) {
            TimeManager.Instance.StartAirTimeByTrick();
        }
    }

    private void OnTypeWord(Word word) {
        // animate completed text
        TextMeshProUGUI completedText =
            Instantiate(completedTextPrefab, completedTextStartTransform.transform.parent, false)
                .GetComponent<TextMeshProUGUI>();
        completedText.text = word.Text;

        if (word is Trick trick) {
            if (trick.Text.Equals("push")) Player.Instance.Push();
            else if (trick.Text.Equals("ollie")) Player.Instance.Jump();
            else if (trick.Text.Equals("drop")) {
                Player.Instance.Drop();
                availableTricks.Remove(word);
                TimeManager.Instance.EndAirTime();
            }
            else if (trick.Text.Equals("grab")) {
                Player.Instance.Grab();
                availableTricks.Remove(word);
            }

            // if gains score
            if (trick.trickScore > 0) {
                playerAnimator.SetTrigger("trick");
            }

            OnCompleteTrick?.Invoke(trick);
        }
    }

    private void UpdateAvailableTricks(Player.State state) {
        if (GameManager.Instance.GameIsOver) return;
        
        availableTricks.Clear();
        foreach (var trick in allTricks) {
            if (trick.availableInStates.Contains(state)) {
                availableTricks.Add(trick);
            }
        }

        TypingManager.Instance.SetWordList(availableTricks);
    }

    public List<Word> GetAvailableTricks() {
        return availableTricks;
    }
}