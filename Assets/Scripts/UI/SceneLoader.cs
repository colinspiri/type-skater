using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "SceneLoader")]
public class SceneLoader : ScriptableObject {
    public SceneReference mainMenuScene;
    public SceneReference gameScene;
    public SceneReference loadingScene;
    
    public void LoadGameScene() {
        Time.timeScale = 1;
        LoadWithLoadingSceen(gameScene.ScenePath);
    }

    public void LoadWithLoadingSceen(string scenePath)
    {
        PlayerPrefs.SetString("loadscene", scenePath);
        SceneManager.LoadScene(loadingScene.ScenePath);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene.ScenePath);
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}