using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour {
    public KeyCode resetKey;
    public KeyCode quitKey;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(resetKey)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKey(quitKey)) {
            Debug.Log("quit game");
            Application.Quit();
        }
    }
}
