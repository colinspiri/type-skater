using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour {
    
    private int randNum;
    private int count;

    // public variables
    public GameObject railPrefab;
    public GameObject rampPrefab;
    public GameObject conePrefab;
    public float railChance;
    public float rampChance;
    public float coneChance;
    public float firstSpawnLocation;
    public float minBlankSpace;
    public float maxBlankSpace;

    private float playerOffset = 10f;
    private float nextSpawnLocation;

    private void Start() {
        nextSpawnLocation = firstSpawnLocation;
    }

    // Update is called once per frame
    void Update() {
        while (Player.Instance.transform.position.x + playerOffset > nextSpawnLocation) {
            SpawnNewObstacle();
        }
    }

    private void SpawnNewObstacle() {
        float randomNum = Random.Range(0f, 1f);
        // if(randomNum)
        // nothing
        // rail
        // ramp
        // cone
        // horizontal scale + random blank space
        // nextSpawnLocation += random blank space
    }
}