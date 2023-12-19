using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrickManager : MonoBehaviour {
    public static TrickManager Instance;

    // component stuff
    private Animator _playerAnimator;

    // public constants
    public GameObject completedTextStartTransform;
    public GameObject completedTextPrefab; // TODO: separate completed text to other script
    public List<Trick> allTricks;

    // state
    private List<Word> availableTricks = new List<Word>();

    // callbacks
    public event Action<bool> OnTypeChar;
    public event Action<Trick> OnCompleteTrick;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _playerAnimator = Player.Instance.GetComponent<Animator>();

        Player.Instance.OnStateChange += _ => TypingManager.Instance.Clear();
        Player.Instance.OnStateChange += UpdateAvailableTricks;
        UpdateAvailableTricks(Player.Instance.state);
    }

    private void OnEnable() {
        TypingManager.OnTypeChar += OnTypeCharCallback;
        TypingManager.OnTypeWord += OnTypeWordCallback;
    }
    private void OnDisable() {
        TypingManager.OnTypeChar -= OnTypeCharCallback;
        TypingManager.OnTypeWord -= OnTypeWordCallback;
    }

    public void Bind() {
        UpdateAvailableTricks(Player.Instance.state);
    }

    private void OnTypeCharCallback(bool charIsCorrect) {
        if (GameManager.Instance.GameStopped) return;
        
        if (charIsCorrect) {
            if(AudioManager.Instance) AudioManager.Instance.PlayTypingSound();
        }
        else {
            if(AudioManager.Instance) AudioManager.Instance.PlayTypingWrongSound();
        }
        
        if (Player.Instance.state == Player.State.Midair || Player.Instance.state == Player.State.OnRail) {
            TimeManager.Instance.StartAirTimeByTrick();
        }
        
        OnTypeChar?.Invoke(charIsCorrect);
    }

    private void OnTypeWordCallback(Word word) {
        if (GameManager.Instance.GameStopped) return;
        
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
                TimeManager.Instance.EndAirTime();
                availableTricks.Remove(word);
                TypingManager.Instance.SetWordList(availableTricks);
            }
            else if (trick.Text.Equals("grab")) {
                Player.Instance.Grab();
                availableTricks.Remove(word);
                TypingManager.Instance.SetWordList(availableTricks);
            }

            // if gains score
            if (trick.trickScore > 0) {
                _playerAnimator.SetTrigger("trick"); // TODO: move to Player as callback
            }

            OnCompleteTrick?.Invoke(trick);
        }
    }

    private void UpdateAvailableTricks(Player.State state) {
        if (GameManager.Instance == null || TypingManager.Instance == null) return;
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