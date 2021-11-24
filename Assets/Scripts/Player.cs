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
    public float pushForce;
    public float minRollingSpeed;
    public float maxRollingSpeed;

    public float minJumpForce;
    public float maxJumpForce;
    
    public float midairTimeScale;
    public float dropForce;
    
    public float railTimeScale;
    public float railMinSpeed;

    public float rampMinSpeed;
    
    public float unsafeRotationZ;

    // private state
    public enum State {
        Midair,
        OnGround,
        OnRail,
        OnRamp,
    }
    public State state;
    private float railSpeed;
    [NonSerialized] public int grindCount;
    private bool rolling;
    public bool safe;

    private float rampSpeed;

    public TextMeshProUGUI speedText; 

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
        // Debug.Log("Player.Update() " + Time.deltaTime + " " + Time.unscaledDeltaTime + " ");
        safe = Input.GetKey(KeyCode.Return) || state != State.Midair;
        animator.SetBool("safe", state == State.Midair ? safe : true);

        if (state == State.OnRail) {
            if(rb.velocity.x < railSpeed) rb.velocity = new Vector2(railSpeed, rb.velocity.y);
        }
        else if (state == State.OnRamp) {
            if(rb.velocity.x < rampSpeed) rb.velocity = new Vector2(rampSpeed, rb.velocity.y);
        }

        if (rb.velocity.x < minRollingSpeed && rolling && !Score.Instance.gameOver) {
            SlowToMinSpeed();
        }

        speedText.text = "" + Mathf.RoundToInt(rb.velocity.x);
    }

    private void ChangeState(State newState) {
        state = newState;
        if (state == State.Midair) {
            Time.timeScale = midairTimeScale;
        }
        else if (state == State.OnRail) {
            Time.timeScale = railTimeScale;
        }
        else if (state == State.OnGround) {
            Time.timeScale = 1;
        }
        else if (state == State.OnRamp) {
            Time.timeScale = 1;
        }
        onStateChange?.Invoke(state);
    }

    public float GetSpeed() {
        return rb.velocity.x;
    }

    public void Push(float multiplier = 1.0f)
    {
        if (rb.velocity.x < maxRollingSpeed)
        {
            rb.AddForce(new Vector2(pushForce * multiplier, 0));
        }
        rolling = true;
        // var particles = Instantiate(pushParticles);
        // particles.transform.position = transform.position;
    }

    public void Jump(float multiplier = 1.0f)
    {
        // if player jumped from the rail, jump proportional to grind count
        if (state == State.OnRail) {
            multiplier = Mathf.Lerp(0.1f, 1.5f, grindCount / 4.0f);
        }
        
        bool newJump = state == State.OnGround || state == State.OnRamp;
        if(rb.velocity.x > maxRollingSpeed) rb.velocity = new Vector2(maxRollingSpeed, rb.velocity.y);
        rb.AddForce(new Vector2(0, multiplier * Mathf.Lerp(minJumpForce, maxJumpForce, rb.velocity.x / maxRollingSpeed)));
        ChangeState(State.Midair);
        Skateboard.Instance.SetAnimation(Skateboard.Animation.Ollie);

        if(newJump) onJump?.Invoke();
    }

    public void Drop() {
        if(rb.velocity.y > 0) rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, -1 * dropForce));
    }
    
    private void SlowToMinSpeed() {
        rb.velocity = new Vector2(minRollingSpeed, rb.velocity.y);
    }

    private void SafeLanding() {
        // speed boost
        float score = Score.Instance.GetUnsecuredScore();
        float multiplier = Mathf.Lerp(0.5f, 1f, score / 10.0f);
        Push(multiplier);
        // callbacks
        onLand?.Invoke();
        onSafeLanding?.Invoke(score);
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
        SlowToMinSpeed();

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
                Skateboard.Instance.SetAnimation(Skateboard.Animation.None);

                ChangeState(State.OnRail);
                railSpeed = rb.velocity.x > railMinSpeed ? rb.velocity.x : railMinSpeed;
                grindCount = 0;
            }
            else {
                ChangeState(State.Midair);
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                WipeOut();
                Jump(0.2f);
            }
        }
        
        // land on ramp
        if (other.gameObject.CompareTag("Ramp") && state != State.OnRamp) {
            // if on ground
            if (state == State.OnGround) {
                ChangeState(State.OnRamp);
                rampSpeed = rb.velocity.x > rampMinSpeed ? rb.velocity.x : rampMinSpeed;
            }
            // safe landing from midair
            else if (state == State.Midair && safe) {
                ChangeState(State.OnRamp);
                rampSpeed = rb.velocity.x > rampMinSpeed ? rb.velocity.x : rampMinSpeed;
                SafeLanding();
            }
            // unsafe landing from midair, bounce off
            else if (state == State.Midair && !safe) {
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                WipeOut();
                Jump(0.2f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // end of rail
        if (other.CompareTag("RailEnd")) {
            // set to midair
            ChangeState(State.Midair);
            Jump(0.1f);
        }
        // end of ramp
        if (other.CompareTag("RampEnd") && state == State.OnRamp) {
            Jump(0.5f);
        }
    }
}
