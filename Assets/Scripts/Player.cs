using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance;

    public float pushForce;
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
    public float railSpeed;
    // private GameObject currentRail;

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
    }

    public void Push(float multiplier = 1.0f)
    {
        if (rb.velocity.x < maxVelocity && state == State.OnGround)
        {
            rb.AddForce(new Vector2(pushForce * multiplier, 0));
        }
    }

    public void Jump()
    {
        if (state != State.OnGround) return;

        rb.AddForce(new Vector2(0, Mathf.Lerp(minJumpForce, maxJumpForce, rb.velocity.x / maxVelocity)));
        state = State.Midair;
        Time.timeScale = midairTimeScale;
        onJump?.Invoke();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") && state != State.OnGround) {
            state = State.OnGround;
            Time.timeScale = 1.0f;
            onLand?.Invoke();
        }
        
        if (other.gameObject.CompareTag("Rail")) {
            // if (safe) {
                state = State.OnRail;
                Time.timeScale = railTimeScale;
            // }
            // else {
            //     other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            //     StartCoroutine(CameraShake.Instance.Shake());
            //     state = State.Midair;
            //     Time.timeScale = midairTimeScale;
            // }
        }
    }
}
