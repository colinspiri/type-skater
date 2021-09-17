using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour {
    public GameObject groundPrefab;
    
    public GameObject mostRecentGround;
    private List<GameObject> grounds;
    
    public Transform player;

    private void Start() {
        grounds = new List<GameObject>();
        grounds.Add(mostRecentGround);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.x > mostRecentGround.transform.position.x - mostRecentGround.transform.localScale.x / 2) {
            mostRecentGround = Instantiate(groundPrefab, new Vector3(mostRecentGround.transform.position.x + mostRecentGround.transform.localScale.x / 2, mostRecentGround.transform.position.y, 0), Quaternion.identity);
            grounds.Add(mostRecentGround);
            // remove off-screen grounds
            if (grounds.Count >= 5) {
                Destroy(grounds[0]);
                grounds.RemoveAt(0);
            }
        }
    }
}
