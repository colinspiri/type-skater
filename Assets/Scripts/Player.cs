using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance;

    public float pushForce;
    public float minVelocity;
    public float maxVelocity;

    public float minJumpForce;
    public float maxJumpForce;
    
    public enum State {
        Midair,
        OnGround,
        OnRail,
    }
    public State state;
    public float midairTimeScale;
    public float railTimeScale;
    public float railMinSpeed;
    private float railSpeed;
    // private GameObject currentRail;
    [NonSerialized] public int grindCount;

    public bool safe;
    public float unsafeRotationZ;

    public delegate void OnJump();
    public OnJump onJump;

    public delegate void OnLand();
    public OnLand onLand;

    
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
        safe = Input.GetKey(KeyCode.Return) || state != State.Midair;
        animator.SetBool("safe", state == State.Midair ? safe : true);

        if (state == State.OnRail) {
            rb.velocity = new Vector2(railSpeed, rb.velocity.y);
        }

        if (rb.velocity.x < minVelocity) {
            Slow();
        }
    }

    public void Push(float multiplier = 1.0f)
    {
        if (rb.velocity.x < maxVelocity)
        {
            rb.AddForce(new Vector2(pushForce * multiplier, 0));
        }
    }

    public void Jump(float multiplier = 1.0f)
    {
        bool wasOnGround = state == State.OnGround;

        rb.AddForce(new Vector2(0, multiplier * Mathf.Lerp(minJumpForce, maxJumpForce, rb.velocity.x / maxVelocity)));
        state = State.Midair;
        Time.timeScale = midairTimeScale;
        
        if(wasOnGround) onJump?.Invoke();
    }

    public void Slow() {
        rb.velocity = new Vector2(minVelocity, rb.velocity.y);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") && state != State.OnGround) {
            state = State.OnGround;
            Time.timeScale = 1.0f;
            onLand?.Invoke();
        }
        
        if (other.gameObject.CompareTag("Rail") && state != State.OnRail) {
            if (safe) {
                state = State.OnRail;
                Time.timeScale = railTimeScale;
                railSpeed = rb.velocity.x > railMinSpeed ? rb.velocity.x : railMinSpeed;
                grindCount = 0;
            }
            else {
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                StartCoroutine(CameraShake.Instance.Shake());
                state = State.Midair;
                Time.timeScale = midairTimeScale;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Ramp")) {
            float multiplier = 0.1f + 0.2f * grindCount;
            Jump(multiplier);
            Debug.Log("jumped with multiplier " + multiplier);
        }
    }
}
