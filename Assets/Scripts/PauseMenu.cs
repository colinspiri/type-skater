using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    public KeyCode pauseButton;
    public GameObject pauseMenu;
    public List<GameObject> objectsToDisable;
    
    private bool paused;
    private float previousTimeScale;
    
    // Start is called before the first frame update
    void Start()
    {
        Resume();
    }

    // Update is called once per frame
    void Update() {
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
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        
        paused = true;
    }

    public void Resume() {
        foreach (var o in objectsToDisable) {
            o.SetActive(true);
        }
        pauseMenu.SetActive(false);
        Time.timeScale = previousTimeScale;
        
        paused = false;
    }
}
