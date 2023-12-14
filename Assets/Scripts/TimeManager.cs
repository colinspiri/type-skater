using System;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    public static TimeManager Instance;

    public float midairTimeScale;
    public float airTime;
    
    private float _fixedDeltaTime;
    private float _currentAirTimeLeft;

    private void Awake() {
        Instance = this;
        this._fixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Update() {
        if (_currentAirTimeLeft > 0) {
            _currentAirTimeLeft -= Time.unscaledDeltaTime;
            float t = _currentAirTimeLeft / airTime;
            SetTimeScale(Mathf.Lerp(1, midairTimeScale, t));

            if (_currentAirTimeLeft <= 0) {
                _currentAirTimeLeft = 0;
                SetTimeScale(1);
            }
        }
    }

    public void SetTimeScale(float newTimeScale) {
        Time.timeScale = newTimeScale;
        Time.fixedDeltaTime = this._fixedDeltaTime * Time.timeScale;
    }

    public void RefreshAirTime() {
        SetTimeScale(midairTimeScale);
        _currentAirTimeLeft = airTime;
    }

    public void EndAirTime() {
        SetTimeScale(1);
        _currentAirTimeLeft = 0;
    }
}