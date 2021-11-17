using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelSelect;
    public GameObject options;
    public GameObject credits;

    public enum MenuState {
        MainMenu,
        LevelSelect,
        Options,
        Credits
    }
    private MenuState currentMenu;

    private void Start() {
        mainMenu.SetActive(false);
        levelSelect.SetActive(false);
        options.SetActive(false);
        credits.SetActive(false);
        ChangeMenu(MenuState.MainMenu);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        // escape to go back to main menu
        if (Input.GetKeyDown(KeyCode.Escape)) {
            BackToMainMenu();
        }
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

    public void Level0() {
        SceneManager.LoadScene("Level0");
    }
    public void Level1() {
        SceneManager.LoadScene("Level1");
    }
    public void Level2() {
        SceneManager.LoadScene("Level2");
    }
    public void Infinite() {
        // load infinite mode
        SceneManager.LoadScene("Infinite");
    }
    public void BackToMainMenu() {
        // Debug.Log("back to main menu");
        ChangeMenu(MenuState.MainMenu);
    }
}
