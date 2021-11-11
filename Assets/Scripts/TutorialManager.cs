using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {
    private float previousTimeScale;
    public Color greyedOutColor;
    private Color originalColor;
    
    // push text
    public TextMeshProUGUI pushText;
    
    // ollie text
    public TextMeshProUGUI ollieText;

    // trick tutorial
    private bool trickTutorialDone;
    public float trickTutorialDelay;
    public TrickList trickList;
    public GameObject trickCanvasPrefab;
    private TextMeshProUGUI trickText;
    
    // safe tutorial
    private bool safeTutorialStarted;
    private bool safeTutorialDone;
    public float safeTutorialDelay;
    public GameObject safeCanvasPrefab;
    private TextMeshProUGUI safeText;

    // Start is called before the first frame update
    void Start() {
        Player.Instance.onJump += () => {
            if(!trickTutorialDone) StartTrickTutorial();
        };
        Player.Instance.onSafeLanding += () => {
            safeTutorialDone = true;
        };
        Player.Instance.onUnsafeLanding += () => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        };
        TypingManager.Instance.onCompleteWord += word => {
            if (word.Equals("push")) {
                pushText.color = greyedOutColor;
            }
            else if (word.Equals("ollie")) {
                ollieText.color = greyedOutColor;
            }
            else if (word.Equals("fakie") && !trickTutorialDone) {
                Time.timeScale = previousTimeScale;
                trickText.color = greyedOutColor;
                trickTutorialDone = true;
                // start safe tutorial
                if(!safeTutorialDone) StartSafeTutorial();
            }
        };
        originalColor = pushText.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (trickTutorialDone && safeTutorialStarted && !safeTutorialDone) {
            if (Input.GetKey(KeyCode.Return)) {
                Time.timeScale = previousTimeScale;
                safeText.color = greyedOutColor;
            }
            else safeText.color = originalColor;
        }
    }

    private void StartSafeTutorial() {
        StartCoroutine(SafeTutorial());
    }

    IEnumerator SafeTutorial() {
        float seconds = safeTutorialDelay / (1f / Time.timeScale);

        yield return new WaitForSeconds(seconds);
        
        // start safe tutorial
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
        safeTutorialStarted = true;
        // spawn safe canvas that prompts player to hold ENTER
        var safeCanvas = Instantiate(safeCanvasPrefab, Player.Instance.transform.position, Quaternion.identity);
        safeText = safeCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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
        var trickCanvas = Instantiate(trickCanvasPrefab, Player.Instance.transform.position, Quaternion.identity);
        trickText = trickCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        // show trick list
        trickList.gameObject.SetActive(true);
    }
}
