using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverDisplay : MonoBehaviour {
    public TextMeshProUGUI yourScoreText;
    public TextMeshProUGUI highScoreText;
    
    private void OnEnable() {
        yourScoreText.text = "your score: " + Score.Instance.score;
        int highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "HighScore", 0);
        highScoreText.text = "high score: " + highScore;
    }
}
