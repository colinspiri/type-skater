using UnityEngine;

public class TimeManager : MonoBehaviour {
    public static TimeManager Instance;
    
    private float fixedDeltaTime;

    private void Awake() {
        Instance = this;
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }

    public void SetTimeScale(float newTimeScale) {
        Time.timeScale = newTimeScale;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
}