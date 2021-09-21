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

    [NonSerialized]
    public bool onGround = true;
    public float slowTimeScale;

    public bool safe;
    public float unsafeRotationZ;

    public delegate void OnJump();
    public OnJump onJump;

    public delegate void OnLand();
    public OnLand onLand;
    

    private Rigidbody2D rb;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        onJump += () => transform.eulerAngles = new Vector3(0, 0, unsafeRotationZ);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        bool oldSafe = safe;
        safe = Input.GetKey(KeyCode.Return);
        if (!onGround && oldSafe != safe) {
            transform.eulerAngles = new Vector3(0, 0, safe ? 0 : unsafeRotationZ);
        }
    }

    public void Push(float multiplier = 1.0f)
    {
        if (rb.velocity.x < maxVelocity && onGround)
        {
            rb.AddForce(new Vector2(pushForce * multiplier, 0));
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
        if (!onGround && (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Rail")))
        {
            onGround = true;
            Time.timeScale = 1.0f;
            onLand?.Invoke();
        }
    }
}
