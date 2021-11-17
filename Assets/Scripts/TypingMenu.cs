using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TypingMenu : MonoBehaviour {
    
    public List<MenuOption> menuOptions;

    private int currentOption = -1;
    
    private const float SliderAdjustment = 0.15f;

    // Start is called before the first frame update
    void Start() {
        foreach (var menuOption in menuOptions) {
            menuOption.Initialize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // arrow keys to navigate between options
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Space)) {
            int newCurrentOption = currentOption + 1;
            newCurrentOption %= menuOptions.Count;
            SetCurrentOption(newCurrentOption);
            HighlightCurrentOption();
            return;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            int newCurrentOption = currentOption - 1;
            if (newCurrentOption < 0) newCurrentOption = menuOptions.Count - 1;
            SetCurrentOption(newCurrentOption);
            HighlightCurrentOption();
            return;
        }
        if (currentOption != -1) {
            // adjust slider with right and left
            if (menuOptions[currentOption].slider != null) {
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.J)) {
                    // increase slider value
                    menuOptions[currentOption].slider.value += SliderAdjustment;
                    return;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow) | Input.GetKeyDown(KeyCode.F)) {
                    // decrease slider value
                    menuOptions[currentOption].slider.value -= SliderAdjustment;
                    return;
                }
            }
            // select current option
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.RightArrow)) {
                menuOptions[currentOption].onSelect?.Invoke();
                return;
            }
        }
        // type to select
        string input = Input.inputString;
        if (input.Equals("")) return;
        
        char c = input[0];
        int typedOption = -1;
        for (int i = 0; i < menuOptions.Count; i++) {
            MenuOption option = menuOptions[i];
            // if (option.slider != null) continue; // skip sliders
            // if the current input matches a word
            if (option.ContinueText(c)) {
                SoundManager.Instance.PlayTypingSound();
                if (typedOption == -1) {
                    typedOption = i;
                }
                else if(option.GetTyped().Length > menuOptions[typedOption].GetTyped().Length) {
                    menuOptions[typedOption].Clear();
                    typedOption = i;
                }
            }
            else option.Clear();
        }
        SetCurrentOption(typedOption);
    }

    private void SetCurrentOption(int newCurrent) {
        currentOption = newCurrent;
        // clear all other options
        for(int i = 0; i < menuOptions.Count; i++) {
            if (i == currentOption) continue;
            menuOptions[i].Unhighlight();
        }
    }
    private void HighlightCurrentOption() {
        // clear all options
        foreach (var menuOption in menuOptions) {
            menuOption.Clear();
        }
        // highlight current
        if (currentOption != -1) {
            menuOptions[currentOption].Highlight();
        }
    }
}

[Serializable]
public class MenuOption {
    public TextMeshProUGUI menuText;
    public Slider slider;
    
    public UnityEvent onSelect;
    
    private TextMeshProUGUI highlightedText;

    [HideInInspector] public string text;
    private string hasTyped;
    private int curChar;

    public void Initialize() {
        text = menuText.text;
        highlightedText = menuText.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        onSelect.AddListener(Clear);
    }
    
    public bool ContinueText(char c)
    {
        // if c matches
        if (c.Equals(text[curChar]))
        {
            curChar++;
            hasTyped = text.Substring(0, curChar);
            highlightedText.text = hasTyped;

            // if we typed the whole word
            if (curChar >= text.Length)
            {
                Clear();
                onSelect?.Invoke();
            }
            return true;
        }
        // if c doesn't match
        Clear();
        return false;
    }

    public void Highlight() {
        highlightedText.text = text;
    }
    public void Unhighlight() {
        highlightedText.text = "";
    }
    
    public void Clear()
    {
        curChar = 0;
        hasTyped = "";
        highlightedText.text = "";
    }
    
    public string GetTyped()
    {
        return hasTyped;
    }
}
