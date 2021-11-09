using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TypingMenu : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject levelSelect;
    public GameObject credits;
    public GameObject options;

    public List<MenuOption> menuOptions;
    
    public enum MenuState {
        MainMenu,
        LevelSelect,
        Options,
        Credits
    }
    private MenuState currentMenu;
    
    // Start is called before the first frame update
    void Start() {
        mainMenu.SetActive(false);
        levelSelect.SetActive(false);
        options.SetActive(false);
        credits.SetActive(false);
        ChangeMenu(MenuState.MainMenu);
        foreach (MenuOption menuOption in menuOptions) {
            menuOption.GetText();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // escape to go back to main menu
        if (Input.GetKeyDown(KeyCode.Escape)) {
            BackToMainMenu();
        }
        
        string input = Input.inputString;
        if (input.Equals("")) return;
        
        char c = input[0];
        MenuOption currentOption = null;
        foreach (MenuOption option in menuOptions) {
            if (option.menuState != currentMenu) continue;
            // if the current input matches a word
            if (option.ContinueText(c)) {
                if (currentOption == null) {
                    currentOption = option;
                }
                else if(option.GetTyped().Length > currentOption.GetTyped().Length) {
                    currentOption.Clear();
                    currentOption = option;
                }
            }
        }
    }
    
    public void Play() {
        // Debug.Log("play");
        ChangeMenu(MenuState.LevelSelect);
    }

    public void Options() {
        // Debug.Log("options");
        ChangeMenu(MenuState.Options);
    }
    public void Credits() {
        // Debug.Log("credits");
        ChangeMenu(MenuState.Credits);
    }
    public void Quit() {
        // Debug.Log("quit");
        Application.Quit();
    }

    public void City() {
        // Debug.Log("city");
        // load city scene
        SceneManager.LoadScene("Level1");
    }
    public void Park() {
        Debug.Log("park");
        // load park scene
    }
    public void Infinite() {
        // Debug.Log("infinite");
        // load infinite mode
        SceneManager.LoadScene("Infinite");
    }
    public void BackToMainMenu() {
        // Debug.Log("back to main menu");
        ChangeMenu(MenuState.MainMenu);
    }

    private void ChangeMenu(MenuState newMenu) {
        // disable current menu
        if(currentMenu == MenuState.MainMenu) mainMenu.SetActive(false);
        else if(currentMenu == MenuState.LevelSelect) levelSelect.SetActive(false);
        else if (currentMenu == MenuState.Options) options.SetActive(false);
        else if(currentMenu == MenuState.Credits) credits.SetActive(false);
        
        // enable new menu
        currentMenu = newMenu;
        if(currentMenu == MenuState.MainMenu) mainMenu.SetActive(true);
        else if(currentMenu == MenuState.LevelSelect) levelSelect.SetActive(true);
        else if (currentMenu == MenuState.Options) options.SetActive(true);
        else if(currentMenu == MenuState.Credits) credits.SetActive(true);
    }
}

[Serializable]
public class MenuOption {
    public TextMeshProUGUI menuText;
    public TypingMenu.MenuState menuState;
    public UnityEvent onSelect;
    
    private TextMeshProUGUI highlightedText;

    [HideInInspector] public string text;
    private string hasTyped;
    private int curChar;

    public void GetText() {
        text = menuText.text;
        highlightedText = menuText.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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
