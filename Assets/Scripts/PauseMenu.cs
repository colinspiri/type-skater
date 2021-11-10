using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    public KeyCode pauseButton;
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public List<GameObject> objectsToDisable;
    
    private bool paused;
    private float previousTimeScale;
    
    // Start is called before the first frame update
    void Start() {
        previousTimeScale = 1.0f;
        Resume();
    }

    // Update is called once per frame
    void Update() {
        // Debug.Log("paused = " + paused);
        if (Input.GetKeyDown(pauseButton)) {
            if (!paused) Pause();
            else Resume();
        }
    }

    private void Pause() {
        foreach (var o in objectsToDisable) {
            o.SetActive(false);
        }
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        
        paused = true;
    }

    public void Resume() {
        foreach (var o in objectsToDisable) {
            o.SetActive(true);
        }
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        Time.timeScale = previousTimeScale;
        
        paused = false;
    }

    public void Restart() {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Options() {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void BackToPauseMenu() {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void BackToMainMenu() {
        Resume();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
