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
    public GameObject ollieCanvas;
    public TextMeshProUGUI ollieText;

    private enum TutorialStatus {
        Incomplete,
        Triggered,
        WaitingForInput,
        Done,
    }

    // safe
    private TutorialStatus safeTutorialStatus = TutorialStatus.Incomplete;
    public float safeTutorialDelay;
    public GameObject safeCanvasPrefab;
    private TextMeshProUGUI safeText;

    // higher ollie
    private TutorialStatus higherOllieTutorialStatus = TutorialStatus.Incomplete;
    public GameObject higherOllieCanvas;
    private TextMeshProUGUI higherOlliePushText;
    private TextMeshProUGUI higherOllieOllieText;
    
    // fakie
    private TutorialStatus fakieTutorialStatus = TutorialStatus.Incomplete;
    public float fakieTutorialDelay;
    public TrickList trickList;
    public GameObject fakieCanvasPrefab;
    private TextMeshProUGUI fakieText;
    
    // safe reminder
    public GameObject safeReminderPrefab;
    
    // speed boost
    public GameObject landingTricksCanvas;

    // Start is called before the first frame update
    void Start() {
        ollieCanvas.SetActive(false);
        higherOllieCanvas.SetActive(false);
        landingTricksCanvas.SetActive(false);
        
        Player.Instance.onJump += () => {
            if(safeTutorialStatus == TutorialStatus.Incomplete) StartSafeTutorial();
            else if(fakieTutorialStatus == TutorialStatus.Incomplete) StartFakieTutorial();
        };
        Player.Instance.onSafeLanding += (float score) => {
            safeTutorialStatus = TutorialStatus.Done;
            // higher ollie
            if (higherOllieTutorialStatus == TutorialStatus.Incomplete) {
                higherOllieCanvas.SetActive(true);
                higherOlliePushText = higherOllieCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                higherOllieOllieText = higherOllieCanvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                higherOllieTutorialStatus = TutorialStatus.Triggered;
            }
            if (score > 0) {
                landingTricksCanvas.SetActive(true);
            }
        };
        Player.Instance.onWipeOut += () => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        };
        TypingManager.Instance.onCompleteWord += word => {
            if (word.Equals("push")) {
                pushText.color = greyedOutColor;
                ollieCanvas.SetActive(true);
                if (higherOllieTutorialStatus == TutorialStatus.Triggered) {
                    higherOlliePushText.color = greyedOutColor;
                }
            }
            else if (word.Equals("ollie")) {
                ollieText.color = greyedOutColor;
                if (higherOllieTutorialStatus == TutorialStatus.Triggered) {
                    higherOllieOllieText.color = greyedOutColor;
                }
            }
            else if (word.Equals("fakie") && fakieTutorialStatus == TutorialStatus.WaitingForInput) {
                Time.timeScale = previousTimeScale;
                fakieText.color = greyedOutColor;
                fakieTutorialStatus = TutorialStatus.Done;
                // spawn safe reminder
                Instantiate(safeReminderPrefab, Player.Instance.transform.position, Quaternion.identity);
            }
        };
        originalColor = pushText.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (safeTutorialStatus == TutorialStatus.WaitingForInput) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                Time.timeScale = previousTimeScale;
            }
            safeText.color = Input.GetKey(KeyCode.Return) ? greyedOutColor : originalColor;
        }
    }

    private void StartSafeTutorial() {
        StartCoroutine(SafeTutorial());
        safeTutorialStatus = TutorialStatus.Triggered;
    }

    IEnumerator SafeTutorial() {
        float seconds = safeTutorialDelay / (1f / Time.timeScale);

        yield return new WaitForSeconds(seconds);

        safeTutorialStatus = TutorialStatus.WaitingForInput;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
        // spawn safe canvas that prompts player to hold ENTER
        var safeCanvas = Instantiate(safeCanvasPrefab, Player.Instance.transform.position, Quaternion.identity);
        safeText = safeCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void StartFakieTutorial() {
        StartCoroutine(FakieTutorial());
        fakieTutorialStatus = TutorialStatus.Triggered;
    }

    IEnumerator FakieTutorial() {
        float seconds = fakieTutorialDelay / (1f / Time.timeScale);

        yield return new WaitForSeconds(seconds);

        fakieTutorialStatus = TutorialStatus.WaitingForInput;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
        // spawn trick canvas that prompts player to type trick
        var trickCanvas = Instantiate(fakieCanvasPrefab, Player.Instance.transform.position, Quaternion.identity);
        fakieText = trickCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        // show trick list
        trickList.gameObject.SetActive(true);
    }
}
