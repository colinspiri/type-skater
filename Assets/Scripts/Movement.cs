using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour {
    public float pushForce;

    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) {
            rb.AddForce(new Vector2(pushForce, 0));
        }
        // transform.Translate(speed * Time.deltaTime, 0f, 0f);
    }
}
