using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
    // constants
    [SerializeField] UIConstants uiConstants;
    public List<MenuScreen> menuScreens;
    public UnityEvent defaultBackAction;

    // state
    private List<MenuScreen> _menuScreenStack = new List<MenuScreen>();

    private void OnEnable()
    {
        if (menuScreens.Count > 0)
        {
            // hide all menu screens
            foreach (var screen in menuScreens)
            {
                screen.gameObject.SetActive(true);
                screen.StartOffscreen();
            }

            // show starting menu screen
            var startingScreen = menuScreens[0];
            startingScreen.StartOnscreen();

            _menuScreenStack.Clear();
            _menuScreenStack.Add(startingScreen);
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        _menuScreenStack.Clear();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToPreviousMenuScreen();

            if (AudioManager.Instance) AudioManager.Instance.PlayBackSound();
        }

        if (Input.anyKeyDown) {
            if(_menuScreenStack.Count > 0) _menuScreenStack[_menuScreenStack.Count - 1].CheckSelectable();
        }
    }

    public void OpenMenuScreen(MenuScreen screen)
    {
        // disable current menu screen
        _menuScreenStack[_menuScreenStack.Count - 1].PutAway();

        // push new menu screen to stack & enable
        _menuScreenStack.Add(screen);
        screen.Push();
    }

    public void BackToPreviousMenuScreen()
    {
        // if at root menu screen, do default back action instead
        if (_menuScreenStack.Count <= 1)
        {
            defaultBackAction?.Invoke();
            return;
        }
        
        // Set the last button so it comes back for it
        // _menuScreenStack[^1].SetLastButton(_menuScreenStack[^1].GetCurrentSelectable());

        // disable current menu screen & pop from stack
        _menuScreenStack[_menuScreenStack.Count - 1].Pop();
        _menuScreenStack.RemoveAt(_menuScreenStack.Count - 1);

        // enable previous menu screen
        _menuScreenStack[_menuScreenStack.Count - 1].BringBack();
    }

    public void Quit() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}