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
    public float railY;
    public GameObject rampPrefab;
    public float rampY;
    public GameObject conePrefab;
    public float coneY;
    public int emptySpaceWeight;
    public int railWeight;
    public int rampWeight;
    public int coneWeight;
    public float firstSpawnLocation;
    public float minBlankSpace;
    public float maxBlankSpace;

    private float playerOffset = 30f;
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
        int randomNum = Random.Range(0, emptySpaceWeight + railWeight + rampWeight + coneWeight);
        Debug.Log("spawn new obstacle " + randomNum);

        if (randomNum < emptySpaceWeight) {
            // empty space
            nextSpawnLocation += maxBlankSpace;
        }
        if (randomNum < emptySpaceWeight + railWeight) {
            // rail 
            var rail = Instantiate(railPrefab, new Vector3(nextSpawnLocation, railY, 0), Quaternion.identity);
            nextSpawnLocation += rail.transform.localScale.x;
        }
        else if (randomNum < emptySpaceWeight + railWeight + rampWeight) {
            // ramp
            var ramp = Instantiate(rampPrefab);
            ramp.transform.position = new Vector3(nextSpawnLocation, rampY, 0);
            nextSpawnLocation += ramp.transform.localScale.x;
        }
        else if (randomNum < emptySpaceWeight + railWeight + rampWeight + coneWeight) {
            // cone
            var cone = Instantiate(conePrefab, new Vector3(nextSpawnLocation, coneY, 0), Quaternion.identity);
            nextSpawnLocation += cone.transform.localScale.x;
        }

        nextSpawnLocation += Random.Range(minBlankSpace, maxBlankSpace);
    }
}