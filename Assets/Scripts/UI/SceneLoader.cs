using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "SceneLoader")]
public class SceneLoader : ScriptableObject {
    public SceneReference mainMenuScene;

    public SceneReference level0;
    public SceneReference level1;
    public SceneReference level2;
    public SceneReference infinite;

    public void Restart()
    {
        ResetTimeScale();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void Level0() {
        ResetTimeScale();
        SceneManager.LoadScene(level0.ScenePath);
    }
    public void Level1() {
        ResetTimeScale();
        SceneManager.LoadScene(level1.ScenePath);
    }
    public void Level2() {
        ResetTimeScale();
        SceneManager.LoadScene(level2.ScenePath);
    }
    public void Infinite() {
        ResetTimeScale();
        SceneManager.LoadScene(infinite.ScenePath);
    }

    public void MainMenu() {
        ResetTimeScale();
        SceneManager.LoadScene(mainMenuScene.ScenePath);
    }

    private void ResetTimeScale() {
        Time.timeScale = 1;
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}