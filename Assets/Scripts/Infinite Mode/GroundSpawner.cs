using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour {
    public GameObject groundPrefab;
    public GameObject mostRecentGround;

    // Update is called once per frame
    void Update()
    {
        if (Player.Instance.transform.position.x > mostRecentGround.transform.position.x - mostRecentGround.transform.localScale.x / 2) {
            mostRecentGround = Instantiate(groundPrefab, new Vector3(mostRecentGround.transform.position.x + mostRecentGround.transform.localScale.x / 2, mostRecentGround.transform.position.y, 0), Quaternion.identity);
        }
    }
}
