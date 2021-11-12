using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateboardGroundCheck : MonoBehaviour
{
    private Skateboard skateboard;
    private Player player;

    private void Awake()
    {
        skateboard = FindObjectOfType<Skateboard>();
        player = Player.Instance;

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            skateboard.grounded = true;
            skateboard.SetAnimation(Skateboard.Animation.None);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            skateboard.grounded = false;
        }
    }
}
