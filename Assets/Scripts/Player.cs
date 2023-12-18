using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance;
    
    // public constants
    [Header("Speed Constants")]
    public float minSpeed;
    public float maxSpeed;
    public float midairSpeed;
    public float landingSpeed;
    public float pushSpeed;

    [Header("Jump Constants")] 
    public float minJumpInitialVelocity;
    public float maxJumpInitialVelocity;
    public float unsafeRotationZ;
    
    [Header("Drop & Grab")]
    public float dropForce;
    public float grabJumpVelocity;
    public float grabPushForce;

    [Header("Ramp Constants")]
    public float rampSpeed;

    [Header("Rail Constants")]
    public float railSpeed;

    // private state
    public enum State { Midair, OnGround, OnRail, OnRamp, }
    [HideInInspector] public State state = State.OnGround;
    private bool _stopped;
    private bool _safe;
    
    // callbacks TODO - turn into Action events
    public delegate void OnJump();
    public OnJump onJump;

    public delegate void OnUnsafeLanding();
    public OnUnsafeLanding onUnsafeLanding;

    public delegate void OnSafeLanding(float score);
    public OnSafeLanding onSafeLanding;

    public delegate void OnWipeOut();
    public OnWipeOut onWipeOut;
    
    public delegate void OnStateChange(State newState);
    public OnStateChange onStateChange;

    // component stuff
    private Rigidbody2D rb;
    private Animator animator;
    [Header("Component Constants")] // TODO: turn trail into separate script that references multiplier
    public TrailRenderer trail;
    public Color slowTrailColor;
    public Color mediumTrailColor;
    public Color fastTrailColor;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        onJump += () => transform.eulerAngles = new Vector3(0, 0, unsafeRotationZ);
    }

    private void Start() {
        _stopped = true;
        
        if (GameManager.Instance != null) {
            GameManager.Instance.OnGameOver += () => {
                _stopped = true;
            };
        }
    }

    private void Update() {
        if (!GameManager.Instance.GameStopped) {
            // safe
            if (state == State.OnGround || state == State.OnRamp) SetSafe(true);
            else {
                SetSafe(!TypingManager.Instance.CurrentlyTyping());
            }
        }
        else SetSafe(true);

        // check max & min speed
        if (state == State.OnGround) {
            float currentSpeed = GetSpeed();
            if (currentSpeed > maxSpeed) {
                rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            }
            if (currentSpeed < minSpeed && !_stopped) {
                rb.velocity = new Vector2(minSpeed, rb.velocity.y);
            }
        }

        // trail color
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, GetSpeed());
        if (t <= 0.33) {
            trail.startColor = slowTrailColor;
            trail.endColor = slowTrailColor;
        }
        else if (t <= 0.67) {
            trail.startColor = mediumTrailColor;
            trail.endColor = mediumTrailColor;
        }
        else {
            trail.startColor = fastTrailColor;
            trail.endColor = fastTrailColor;
        }
    }

    private void SetSafe(bool value) {
        if (_safe == value) return;

        _safe = value;
        animator.SetBool("safe", state == State.Midair ? _safe : true);
        if (_safe) {
            AudioManager.Instance.PlaySafeSound();
        }
        else {
            AudioManager.Instance.PlayUnsafeSound();
        }
    }

    private void ChangeState(State newState) {
        state = newState;
        if (state == State.Midair) {
            TimeManager.Instance.StartAirTimeByJump();
        }
        else if (state == State.OnGround) {
            TimeManager.Instance.EndAirTime();
        }
        else if (state == State.OnRail) {
            TimeManager.Instance.StartAirTimeByJump();
            SetSpeed(railSpeed);
        }
        else if (state == State.OnRamp) {
            TimeManager.Instance.EndAirTime();
            SetSpeed(rampSpeed);
        }
        onStateChange?.Invoke(state);
    }

    public float GetSpeed() {
        return rb.velocity.x;
    }

    public void Push() {
        SetSpeed(pushSpeed);

        _stopped = false;
        
        PlayerAnimator.Instance.PushParticles();
    }

    public void Jump()
    {
        // only do callback if starting new jump
        bool newJump = state == State.OnGround || (state == State.OnRamp && !Score.Instance.scoreIsUnsecured);
        if(state == State.OnRail) Instructions.Instance.FinishInstruction("rail_ollie");
        
        // determine initial jump vel
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, GetSpeed());
        float jumpInitialVelocity = Mathf.Lerp(minJumpInitialVelocity, maxJumpInitialVelocity, t);

        rb.velocity = new Vector2(midairSpeed, jumpInitialVelocity);
        
        /*// use jump force based on speed
        float jumpForce = (jumpType != Speed.Stopped ? jumpType : currentSpeed) switch {
            Speed.Slow => slowJumpForce,
            Speed.Medium => mediumJumpForce,
            Speed.Fast => fastJumpForce,
            _ => 0
        };
        rb.AddForce(new Vector2(0, jumpForce / TimeManager.Instance.midairTimeScale));
        // set speed
        rb.velocity = new Vector2(mediumSpeed, rb.velocity.y);*/
        
        
        // change state
        ChangeState(State.Midair);
        Skateboard.Instance.SetAnimation(Skateboard.Animation.Ollie);

        if(newJump) onJump?.Invoke();
    }

    public void Grab() {
        rb.velocity = new Vector2(rb.velocity.x,grabJumpVelocity);
        rb.AddForce(new Vector2(grabPushForce, 0));
        
        Skateboard.Instance.SetAnimation(Skateboard.Animation.Ollie);
    }

    public void Drop() {
        if(rb.velocity.y > 0) rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, -1 * dropForce));
    }

    private void SetSpeed(float newSpeed) {
        rb.velocity = new Vector2(newSpeed, rb.velocity.y);
    }

    private void SafeLanding() {
        SetSpeed(landingSpeed);
        onSafeLanding?.Invoke(Score.Instance.GetUnsecuredScore());
    }
    private void UnsafeLanding() {
        WipeOut();
        onUnsafeLanding?.Invoke();
    }

    public void WipeOut() {
        SetSpeed(minSpeed);

        // if player landed with unsecured score, screen shake magnitude is proportional to the score
        float magnitude = (Score.Instance.GetUnsecuredScore() > 0) ? (0.2f + Score.Instance.GetUnsecuredScore() * 0.1f) : 1f;
        StartCoroutine(CameraShake.Instance.Shake(magnitude));

        // particles?
        
        // callback
        onWipeOut?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // land on ground
        if (other.gameObject.CompareTag("Ground") && state != State.OnGround) {
            ChangeState(State.OnGround);
            Skateboard.Instance.SetAnimation(Skateboard.Animation.None);

            if(_safe) SafeLanding();
            else UnsafeLanding();
        }
        
        // land on rail
        if (other.gameObject.CompareTag("Rail") && state != State.OnRail) {
            if (_safe) {
                Skateboard.Instance.SetAnimation(Skateboard.Animation.None);

                ChangeState(State.OnRail);
                Instructions.Instance.FinishInstruction("rail_land");
            }
            else {
                ChangeState(State.Midair);
                WipeOut();
                Grab();
                other.gameObject.GetComponent<Rail>().disableCollider = true;
            }
        }
        
        // land on ramp
        if (other.gameObject.CompareTag("Ramp") && state != State.OnRamp) {
            // if on ground
            if (state == State.OnGround) {
                ChangeState(State.OnRamp);
            }
            // safe landing from midair
            else if (state == State.Midair && _safe) {
                ChangeState(State.OnRamp);
                Instructions.Instance.FinishInstruction("ramp_land");
            }
            // unsafe landing from midair, bounce off
            else if (state == State.Midair && !_safe) {
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                WipeOut();
                Grab();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // end of rail
        if (other.CompareTag("RailEnd") && state == State.OnRail) {
            // set to midair
            ChangeState(State.Midair);
            Jump();
        }
        // end of ramp
        if (other.CompareTag("RampEnd") && state == State.OnRamp) {
            Jump();
            Instructions.Instance.FinishInstruction("ramp_launch");
        }
    }
}
