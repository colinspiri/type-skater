using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance;
    
    // public constants
    public float pushForce;
    public float minVelocity;
    public float maxVelocity;

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

    public delegate void OnJump();
    public OnJump onJump;

    public delegate void OnLand();
    public OnLand onLand;

    public delegate void OnUnsafeLanding();
    public OnUnsafeLanding onUnsafeLanding;

    public delegate void OnSafeLanding(float score);
    public OnSafeLanding onSafeLanding;

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

        if (rolling && rb.velocity.x < minVelocity) {
            SlowToMinSpeed();
        }
    }

    public void Push(float multiplier = 1.0f)
    {
        if (rb.velocity.x < maxVelocity)
        {
            rb.AddForce(new Vector2(pushForce * multiplier, 0));
        }
        rolling = true;
    }

    public void Jump(float multiplier = 1.0f)
    {
        // if player jumped from the rail, jump proportional to grind count
        if (state == State.OnRail) {
            multiplier = Mathf.Lerp(0.1f, 1.5f, grindCount / 4.0f);
        }
        
        bool newJump = state == State.OnGround || state == State.OnRamp;
        rb.AddForce(new Vector2(0, multiplier * Mathf.Lerp(minJumpForce, maxJumpForce, rb.velocity.x / maxVelocity)));
        state = State.Midair;
        Time.timeScale = midairTimeScale;
        
        if(newJump) onJump?.Invoke();
    }

    public void Drop() {
        if(rb.velocity.y > 0) rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, -1 * dropForce));
    }
    
    private void SlowToMinSpeed() {
        rb.velocity = new Vector2(minVelocity, rb.velocity.y);
    }

    private void SafeLanding() {
        Time.timeScale = 1;
        // speed boost
        float score = Score.Instance.GetUnsecuredScore();
        if (score == 0) {
            Push(0.5f);
        }
        else {
            float multiplier = Mathf.Lerp(1f, 2.5f, score / 10.0f);
            Push(multiplier);
        }
        onLand?.Invoke();
        onSafeLanding?.Invoke(score);
    }
    private void UnsafeLanding() {
        Time.timeScale = 1;
        // wipe out
        WipeOut();
        onLand?.Invoke();
        onUnsafeLanding?.Invoke();
    }

    public void WipeOut(float slowFactor = 0f) {
        // slow
        if(slowFactor == 0) SlowToMinSpeed();
        else rb.velocity = new Vector2(Mathf.Lerp(minVelocity, rb.velocity.x, slowFactor), rb.velocity.y);

        // if player landed with unsecured score, screen shake magnitude is proportional to the score
        float magnitude = (Score.Instance.GetUnsecuredScore() > 0) ? (0.2f + Score.Instance.GetUnsecuredScore() * 0.1f) : 1f;
        StartCoroutine(CameraShake.Instance.Shake(magnitude));

        // particles?
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // land on ground
        if (other.gameObject.CompareTag("Ground") && state != State.OnGround) {
            state = State.OnGround;
            if(safe) SafeLanding();
            else UnsafeLanding();
        }
        
        // land on rail
        if (other.gameObject.CompareTag("Rail") && state != State.OnRail) {
            if (safe) {
                state = State.OnRail;
                Time.timeScale = railTimeScale;
                railSpeed = rb.velocity.x > railMinSpeed ? rb.velocity.x : railMinSpeed;
                grindCount = 0;
            }
            else {
                state = State.Midair;
                Time.timeScale = midairTimeScale;
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                WipeOut();
                Jump(0.2f);
            }
        }
        
        // land on ramp
        if (other.gameObject.CompareTag("Ramp") && state != State.OnRamp) {
            // if on ground
            if (state == State.OnGround) {
                state = State.OnRamp;
                rampSpeed = rb.velocity.x > rampMinSpeed ? rb.velocity.x : rampMinSpeed;
            }
            // safe landing from midair
            else if (state == State.Midair && safe) {
                state = State.OnRamp;
                rampSpeed = rb.velocity.x > rampMinSpeed ? rb.velocity.x : rampMinSpeed;
                SafeLanding();
            }
            // unsafe landing from midair, bounce off
            else if (state == State.Midair && !safe) {
                Time.timeScale = midairTimeScale;
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
            state = State.Midair;
            Jump(0.1f);
        }
        // end of ramp
        if (other.CompareTag("RampEnd") && state == State.OnRamp) {
            Jump(0.5f);
        }
    }
}
