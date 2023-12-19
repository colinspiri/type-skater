using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class DisplayTypingText : MonoBehaviour {
    public TextMeshProUGUI displayText;
    public StringReference correctText;
    public StringReference wrongText;
    [Space] 
    public float cursorBlinkTime;

    private string _typedText;
    private bool _cursorVisible;
    private float _cursorBlinkTimer;
    
    // Start is called before the first frame update
    void Start() {
        ResetBlinkTimer();
    }

    private void OnEnable() {
        TypingManager.OnTypeChar += OnTypeChar;
    }
    private void OnDisable() {
        TypingManager.OnTypeChar -= OnTypeChar;
    }

    // Update is called once per frame
    void Update()
    {
        if (_cursorBlinkTimer > 0) {
            _cursorBlinkTimer -= Time.deltaTime;
            if (_cursorBlinkTimer <= 0) {
                ToggleCursor();
                _cursorBlinkTimer = cursorBlinkTime;
            }
        }
        
        UpdateDisplayText();
    }

    private void ResetBlinkTimer() {
        _cursorBlinkTimer = cursorBlinkTime;
        _cursorVisible = true;
    }

    private void ToggleCursor() {
        _cursorVisible = !_cursorVisible;
        
        UpdateDisplayText();
    }

    private void OnTypeChar(bool b) {
        ResetBlinkTimer();
        UpdateDisplayText();
    }

    private void UpdateDisplayText() {
        _typedText = correctText + "<color=#EC7357>" + wrongText + "</color>";
        if (_cursorVisible) _typedText += "_";
        
        displayText.text = _typedText;
    }
}
