using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        TimeManager.Instance.SetTimeScale(1);
    }

    public void BackToMainMenu() {
        SceneManager.LoadScene("Menu");
        TimeManager.Instance.SetTimeScale(1);
    }
}
