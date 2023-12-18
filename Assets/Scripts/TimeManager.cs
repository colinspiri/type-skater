using System;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    public static TimeManager Instance;

    public float midairTimeScale;
    public float airTimeOnTrick;
    public float airTimeOnJump;
    public float revertTime;
    
    private float _fixedDeltaTime;
    private float _maxAirTime;
    private float _currentAirTimeLeft;
    private float _previousTimeScale = 1;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        this._fixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Update() {
        if (Time.timeScale == 0) return;
        
        UpdateAirTime();
    }

    private void UpdateAirTime() {
        if (_currentAirTimeLeft > 0) {
            _currentAirTimeLeft -= Time.unscaledDeltaTime;

            if (_currentAirTimeLeft < revertTime) {
                float t = _currentAirTimeLeft / revertTime;
                SetTimeScale(Mathf.Lerp(1, midairTimeScale, t));
                
                if (_currentAirTimeLeft <= 0) {
                    _currentAirTimeLeft = 0;
                    SetTimeScale(1);
                }
            }
        }
    }

    public void SetTimeScale(float newTimeScale) {
        Time.timeScale = newTimeScale;
        Time.fixedDeltaTime = this._fixedDeltaTime * Time.timeScale;
    }

    public void StartAirTimeByTrick() {
        SetTimeScale(midairTimeScale);
        _maxAirTime = airTimeOnTrick + revertTime;
        _currentAirTimeLeft = _maxAirTime;
    }
    public void StartAirTimeByJump() {
        SetTimeScale(midairTimeScale);
        _maxAirTime = airTimeOnJump + revertTime;
        _currentAirTimeLeft = _maxAirTime;
    }
    public void EndAirTime() {
        SetTimeScale(1);
        _currentAirTimeLeft = 0;
    }

    public void PauseTime() {
        _previousTimeScale = Time.timeScale;
        SetTimeScale(0);
    }
    public void ResumeTime() {
        SetTimeScale(_previousTimeScale);
    }
}