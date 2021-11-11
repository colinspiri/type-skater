using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    // trick tutorial
    private bool trickTutorialDone;
    public float trickTutorialDelay;
    public TrickList trickList;
    public GameObject trickCanvas;
    private float previousTimeScale;
    
    // safe tutorial
    private bool safeTutorialDone;
    public float safeTutorialDelay;
    
    // Start is called before the first frame update
    void Start() {
        Player.Instance.onJump += () => {
            if(!trickTutorialDone) StartTrickTutorial();
        };
        TypingManager.Instance.onCompleteWord += word => {
            if (word.Equals("fakie") && !trickTutorialDone) {
                Time.timeScale = previousTimeScale;
                trickTutorialDone = true;
                // start safe tutorial
                if(!safeTutorialDone) StartSafeTutorial();
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartSafeTutorial() {
        safeTutorialDone = true;
    }

    IEnumerator SafeTutorial() {
        float seconds = safeTutorialDelay / (1f / Time.timeScale);

        yield return new WaitForSeconds(seconds);
        
        // start safe tutorial
    }

    private void StartTrickTutorial() {
        StartCoroutine(TrickTutorial());
    }

    IEnumerator TrickTutorial() {
        float seconds = trickTutorialDelay / (1f / Time.timeScale);

        yield return new WaitForSeconds(seconds);

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
        // spawn trick canvas that prompts player to type trick
        Instantiate(trickCanvas, Player.Instance.transform.position, Quaternion.identity);
        // show trick list
        trickList.gameObject.SetActive(true);
    }
}
