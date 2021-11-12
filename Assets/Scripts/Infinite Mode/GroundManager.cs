using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour {
    public GameObject groundPrefab;
    public GameObject rail;
    public int randRange;
    
    public GameObject mostRecentGround;
    public GameObject mostRecentRail;
    private List<GameObject> grounds;
    private List<GameObject> rails;
    private int randNum;
    private System.Random rand;
    private int count;
    
    private void Start() {
        grounds = new List<GameObject> {mostRecentGround};
        rails = new List<GameObject> {mostRecentRail};
        rand = new System.Random();
        count=0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.Instance.transform.position.x > mostRecentGround.transform.position.x - mostRecentGround.transform.localScale.x / 2) {
            mostRecentGround = Instantiate(groundPrefab, new Vector3(mostRecentGround.transform.position.x + mostRecentGround.transform.localScale.x / 2, mostRecentGround.transform.position.y, 0), Quaternion.identity);
            grounds.Add(mostRecentGround);

            count++;
            if(count==randRange) {
                mostRecentRail = Instantiate(rail, new Vector3(mostRecentGround.transform.position.x + mostRecentGround.transform.localScale.x / 2, mostRecentGround.transform.position.y+(mostRecentGround.transform.localScale.y / 2)+(mostRecentRail.transform.localScale.y/2)-.2f,0), Quaternion.identity);
                rails.Add(mostRecentRail);
                if (rails.Count >= 5) {
                    //Destroy(rails[0]);
                    rails.RemoveAt(0);
                }
                count=0;
                randRange= (rand.Next()%3)+1;
            }
        }
    }
}
