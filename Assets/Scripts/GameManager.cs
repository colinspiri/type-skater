using System;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // game state
    public bool gameStopped;
    
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
        gameStopped = true;
        TimeManager.Instance.PauseTime();
        if (pauseAudio) {
            AudioListener.pause = true;
        }
    }

    public void Resume(bool resumeAudio = true)
    {
        gameStopped = false;
        TimeManager.Instance.ResumeTime();
        if (resumeAudio) {
            AudioListener.pause = false;
        }
    }

    public void GameOver()
    {
        if (gameStopped) return;

        Player.Instance.onWipeOut -= GameOver;

        gameStopped = true;
        TimeManager.Instance.EndAirTime();

        OnGameOver?.Invoke();
        if(gameOverEvent) gameOverEvent.Raise();
    }
}