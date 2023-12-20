using System;
using ScriptableObjectArchitecture;
using UnityEngine;

public class Multiplier : MonoBehaviour {
    // state
    [SerializeField] private FloatVariable currentMultiplier;
    private float _baseGroundMultiplier;
    
    // constants 
    [SerializeField] private float trickIncrease;
    [SerializeField] private float onGroundDecrease;
    [SerializeField] private float maxSpeedDelta;

    private void Start() {
        currentMultiplier.Value = 1;
        _baseGroundMultiplier = currentMultiplier.Value;

        TrickManager.Instance.OnCompleteTrick += trick => {
            if (trick.trickScore > 0) {
                currentMultiplier.Value += trickIncrease;
            }
        };
        Player.Instance.OnStateChange += state => {
            if (state == Player.State.OnGround) {
                _baseGroundMultiplier = currentMultiplier.Value;
            }
        };
    }

    private void Update() {
        if (Player.Instance.state == Player.State.OnGround) {
            // decrease when rolling
            _baseGroundMultiplier -= onGroundDecrease * Time.unscaledDeltaTime;
            if (_baseGroundMultiplier < 1) _baseGroundMultiplier = 1;
            
            // speed delta
            float speedFactor = Mathf.InverseLerp(Player.Instance.minSpeed, Player.Instance.maxSpeed,
                Player.Instance.GetSpeed());
            float speedDelta = Mathf.Lerp(-maxSpeedDelta, maxSpeedDelta, speedFactor);
            
            currentMultiplier.Value = _baseGroundMultiplier + speedDelta;

            if (currentMultiplier.Value < 1) currentMultiplier.Value = 1;
        }
    }
}