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
    private List<bool> wereObjectsActive = new List<bool>();
    
    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < objectsToDisable.Count; i++) {
            wereObjectsActive.Add(objectsToDisable[i].activeSelf);
        }
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
        for (int i = 0; i < objectsToDisable.Count; i++) {
            wereObjectsActive[i] = objectsToDisable[i].activeSelf;
            objectsToDisable[i].SetActive(false);
        }
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
        previousTimeScale = Time.timeScale;
        TimeManager.Instance.SetTimeScale(0);
        
        paused = true;
    }

    public void Resume() {
        for (int i = 0; i < objectsToDisable.Count; i++) {
            objectsToDisable[i].SetActive(wereObjectsActive[i]);
        }
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        TimeManager.Instance.SetTimeScale(previousTimeScale);

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
        previousTimeScale = 1f;
        Resume();
        SceneManager.LoadScene("Menu");
    }
}
