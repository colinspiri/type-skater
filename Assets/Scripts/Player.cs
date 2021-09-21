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

    public bool onGround;
    public float slowTimeScale;

    public delegate void OnJump();
    public OnJump onJump;

    public delegate void OnLand();
    public OnLand onLand;


    private Rigidbody2D rb;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Push()
    {
        if (rb.velocity.x < maxVelocity && onGround)
        {
            rb.AddForce(new Vector2(pushForce, 0));
        }
    }

    public void Jump()
    {
        if (!onGround) return;

        float t = rb.velocity.x / maxVelocity;
        rb.AddForce(new Vector2(0, Mathf.Lerp(minJumpForce, maxJumpForce, t)));
        onGround = false;
        Time.timeScale = slowTimeScale;
        onJump?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = true;
            Time.timeScale = 1.0f;
            onLand?.Invoke();
        }
    }
}
