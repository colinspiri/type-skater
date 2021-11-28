using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance;
    
    // public constants
    [Header("Speed Constants")]
    public float slowSpeed;
    public float mediumSpeed;
    public float fastSpeed;
    public float maxSpeed;
    public float pushForce;
    public TextMeshProUGUI speedText;

    [Header("Jump Constants")]
    public float slowJumpForce;
    public float mediumJumpForce;
    public float fastJumpForce;
    public float midairTimeScale;
    public float unsafeRotationZ;
    public float dropForce;

    [Header("Ramp Constants")] 
    public Speed rampSpeed;
    public Speed rampJump;
    public float bounceForce;

    [Header("Rail Constants")] 
    public Speed railSpeed;
    public Speed railJump;
    public float railTimeScale;

    // private state
    public enum State {
        Midair,
        OnGround,
        OnRail,
        OnRamp,
    }
    [HideInInspector] public State state = State.OnGround;
    public enum Speed {
        Stopped,
        Slow,
        Medium,
        Fast,
    }
    [HideInInspector] public Speed currentSpeed = Speed.Stopped;
    
    [NonSerialized] public int grindCount;
    [HideInInspector] public bool safe;
    
    // callbacks
    public delegate void OnJump();
    public OnJump onJump;

    public delegate void OnLand();
    public OnLand onLand;

    public delegate void OnUnsafeLanding();
    public OnUnsafeLanding onUnsafeLanding;

    public delegate void OnSafeLanding(float score);
    public OnSafeLanding onSafeLanding;

    public delegate void OnWipeOut();
    public OnWipeOut onWipeOut;
    
    public delegate void OnStateChange(State newState);
    public OnStateChange onStateChange;
    
    // particle effects
    // public GameObject pushParticles;

    // component stuff
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        onJump += () => transform.eulerAngles = new Vector3(0, 0, unsafeRotationZ);
    }

    private void Update() {
        // safe
        safe = Input.GetKey(KeyCode.Return) || state != State.Midair;
        animator.SetBool("safe", state == State.Midair ? safe : true);

        
        // speed up time when holding ENTER (makes the physics wonky idk why)
        // if (state == State.Midair) {
        //     Time.timeScale = safe ? 1.5f : midairTimeScale;
        // }

        // set current speed state
        if (state == State.OnRail) {
            SetSpeed(railSpeed);
        }
        else if (state == State.OnRamp) {
            SetSpeed(rampSpeed);
        }
        else {
            float speed = rb.velocity.x;
            if (speed >= fastSpeed) {
                currentSpeed = Speed.Fast;
                if(speed > maxSpeed) rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            }
            else if (speed >= mediumSpeed) {
                currentSpeed = Speed.Medium;
            }
            else if (currentSpeed != Speed.Stopped) {
                currentSpeed = Speed.Slow;
                if(speed < slowSpeed) rb.velocity = new Vector2(slowSpeed, rb.velocity.y);
            }
        }

        // speed text
        Debug.Log("current speed = " + currentSpeed + " " + Mathf.RoundToInt(rb.velocity.x));
        speedText.text = currentSpeed + " ";
    }

    private void ChangeState(State newState) {
        state = newState;
        if (state == State.Midair) {
            Time.timeScale = midairTimeScale;
        }
        else if (state == State.OnGround) {
            Time.timeScale = 1;
        }
        else if (state == State.OnRail) {
            Time.timeScale = railTimeScale;
            SetSpeed(railSpeed);
        }
        else if (state == State.OnRamp) {
            Time.timeScale = 1;
            SetSpeed(rampSpeed);
        }
        onStateChange?.Invoke(state);
    }

    public float GetSpeed() {
        return rb.velocity.x;
    }

    public void Push()
    {
        if (rb.velocity.x < maxSpeed)
        {
            rb.AddForce(new Vector2(pushForce, 0));
        }
        if (currentSpeed == Speed.Stopped) currentSpeed = Speed.Slow;
        // var particles = Instantiate(pushParticles);
        // particles.transform.position = transform.position;
    }

    public void Jump(Speed jumpType = Speed.Stopped)
    {
        // only do callback if starting new jump
        bool newJump = state == State.OnGround || state == State.OnRamp;
        
        // use jump force based on speed
        float jumpForce = (jumpType != Speed.Stopped ? jumpType : currentSpeed) switch {
            Speed.Slow => slowJumpForce,
            Speed.Medium => mediumJumpForce,
            Speed.Fast => fastJumpForce,
            _ => 0
        };
        rb.AddForce(new Vector2(0, jumpForce));
        // set speed
        float newSpeed = currentSpeed switch {
            Speed.Slow => mediumSpeed,
            Speed.Medium => mediumSpeed,
            Speed.Fast => fastSpeed,
            _ => 0
        };
        rb.velocity = new Vector2(newSpeed, rb.velocity.y);
        
        // change state
        ChangeState(State.Midair);
        Skateboard.Instance.SetAnimation(Skateboard.Animation.Ollie);

        if(newJump) onJump?.Invoke();
    }

    private void Bounce() {
        rb.AddForce(new Vector2(0, bounceForce));
    }

    public void Drop() {
        if(rb.velocity.y > 0) rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, -1 * dropForce));
    }

    private void SetSpeed(Speed speed) {
        currentSpeed = speed;
        float newSpeed = currentSpeed switch {
            Speed.Stopped => 0,
            Speed.Slow => slowSpeed,
            Speed.Medium => mediumSpeed,
            Speed.Fast => fastSpeed,
            _ => -1
        };
        rb.velocity = new Vector2(newSpeed, rb.velocity.y);
    }

    private void SafeLanding() {
        // speed boost
        // SetSpeed(Speed.Medium);
        // callbacks
        onLand?.Invoke();
        onSafeLanding?.Invoke(Score.Instance.GetUnsecuredScore());
    }
    private void UnsafeLanding() {
        // wipe out
        WipeOut();
        // callbacks
        onLand?.Invoke();
        onUnsafeLanding?.Invoke();
    }

    public void WipeOut() {
        // slow
        SetSpeed(Speed.Slow);

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

            if(safe) SafeLanding();
            else UnsafeLanding();
        }
        
        // land on rail
        if (other.gameObject.CompareTag("Rail") && state != State.OnRail) {
            if (safe) {
                ChangeState(State.OnRail);
                grindCount = 0;
            }
            else {
                ChangeState(State.Midair);
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                WipeOut();
                Bounce();
            }
        }
        
        // land on ramp
        if (other.gameObject.CompareTag("Ramp") && state != State.OnRamp) {
            // if on ground
            if (state == State.OnGround) {
                ChangeState(State.OnRamp);
            }
            // safe landing from midair
            else if (state == State.Midair && safe) {
                ChangeState(State.OnRamp);
                SafeLanding();
            }
            // unsafe landing from midair, bounce off
            else if (state == State.Midair && !safe) {
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                WipeOut();
                Bounce();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // end of rail
        if (other.CompareTag("RailEnd") && state == State.OnRail) {
            // set to midair
            ChangeState(State.Midair);
            Jump(railJump);
        }
        // end of ramp
        if (other.CompareTag("RampEnd") && state == State.OnRamp) {
            Jump(rampJump);
        }
    }
}
