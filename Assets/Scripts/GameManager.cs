using System;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // game state
    private bool _gameStopped;
    private bool _gameOver;
    public bool GameStopped => _gameStopped;
    public bool GameIsOver => _gameOver;
    
    // callbacks
    public event Action OnGameOver;
    public GameEvent gameOverEvent;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start() {
        Player.Instance.onWipeOut += GameOver;
    }

    public void Pause(bool pauseAudio = true)
    {
        _gameStopped = true;
        TimeManager.Instance.PauseTime();
        if (pauseAudio) {
            AudioListener.pause = true;
        }
    }

    public void Resume(bool resumeAudio = true)
    {
        _gameStopped = false;
        TimeManager.Instance.ResumeTime();
        if (resumeAudio) {
            AudioListener.pause = false;
        }
    }

    public void GameOver()
    {
        if (_gameOver) return;
        
        _gameStopped = true;
        _gameOver = true;
        
        TimeManager.Instance.EndAirTime();
        TypingManager.Instance.ClearWordList();

        OnGameOver?.Invoke();
        if(gameOverEvent) gameOverEvent.Raise();
    }
}