using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {
    // state
    private float previousTimeScale;
    private Color originalColor;
    
    // push text
    [Header("Push")]
    public TextMeshProUGUI pushText;
    public Color greyedOutColor;

    // ollie text
    [Header("Ollie")]
    public GameObject ollieCanvas;
    public TextMeshProUGUI ollieText;

    private enum TutorialStatus {
        Incomplete,
        Triggered,
        WaitingForInput,
        Done,
    }

    // safe
    [Header("Safe")]
    public GameObject safeCanvasPrefab;
    public float safeTutorialDelay;
    private TutorialStatus safeTutorialStatus = TutorialStatus.Incomplete;
    private TextMeshProUGUI safeText;

    // higher ollie
    [Header("Push Then Ollie")]
    public GameObject higherOllieCanvas;
    private TutorialStatus higherOllieTutorialStatus = TutorialStatus.Incomplete;
    private TextMeshProUGUI higherOlliePushText;
    private TextMeshProUGUI higherOllieOllieText;
    
    // fakie
    [Header("Fakie")]
    public TrickList trickList;
    public GameObject fakieCanvasPrefab;
    public float fakieTutorialDelay;
    private TutorialStatus fakieTutorialStatus = TutorialStatus.Incomplete;
    private TextMeshProUGUI fakieText;

    // safe reminder
    [Header("Safe Reminder")]
    public GameObject safeReminderPrefab;

    // grab
    [Header("Grab")]
    public GameObject grabCanvasPrefab;
    public float grabDelay;
    private TutorialStatus grabStatus = TutorialStatus.Incomplete;
    private TextMeshProUGUI grabText;
    
    // drop
    [Header("Drop")]
    public GameObject dropCanvasPrefab; // "drop to land earlier"
    public float dropDelay;
    private TutorialStatus dropStatus = TutorialStatus.Incomplete;
    private TextMeshProUGUI dropText;

    // Start is called before the first frame update
    void Start() {
        ollieCanvas.SetActive(false);
        higherOllieCanvas.SetActive(false);
        trickList.gameObject.SetActive(false);
        
        Player.Instance.onJump += () => {
            if(safeTutorialStatus == TutorialStatus.Incomplete) StartSafeTutorial();
            else if(fakieTutorialStatus == TutorialStatus.Incomplete) StartFakieTutorial();
            else if(grabStatus == TutorialStatus.Incomplete) StartGrabTutorial();
            else if(dropStatus == TutorialStatus.Incomplete) StartDropTutorial();
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
        };
        Player.Instance.onWipeOut += () => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        };
        TrickManager.Instance.OnCompleteTrick += trick => {
            if (trick.Equals("push")) {
                pushText.color = greyedOutColor;
                ollieCanvas.SetActive(true);
                if (higherOllieTutorialStatus == TutorialStatus.Triggered) {
                    higherOlliePushText.color = greyedOutColor;
                }
            }
            else if (trick.Equals("ollie")) {
                ollieText.color = greyedOutColor;
                if (higherOllieTutorialStatus == TutorialStatus.Triggered) {
                    higherOllieOllieText.color = greyedOutColor;
                }
            }
            else if (trick.Equals("fakie") && fakieTutorialStatus == TutorialStatus.WaitingForInput) {
                Time.timeScale = previousTimeScale;
                fakieText.color = greyedOutColor;
                fakieTutorialStatus = TutorialStatus.Done;
                // spawn safe reminder
                Instantiate(safeReminderPrefab, Player.Instance.transform.position, Quaternion.identity);
            }
            else if (trick.Equals("grab") && grabStatus == TutorialStatus.WaitingForInput) {
                Time.timeScale = previousTimeScale;
                grabText.color = greyedOutColor;
                grabStatus = TutorialStatus.Done;
            }
            else if (trick.Equals("drop") && dropStatus == TutorialStatus.WaitingForInput) {
                Time.timeScale = previousTimeScale;
                dropText.color = greyedOutColor;
                dropStatus = TutorialStatus.Done;
            }
        };
        originalColor = pushText.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (safeTutorialStatus == TutorialStatus.WaitingForInput) {
            safeText.color = Input.GetKey(KeyCode.Return) ? greyedOutColor : originalColor;
            if (Input.GetKey(KeyCode.Return)) {
                Time.timeScale = previousTimeScale;
            }
        }
    }

    private void StartSafeTutorial() {
        StartCoroutine(SafeTutorial());
        safeTutorialStatus = TutorialStatus.Triggered;
    }

    private IEnumerator SafeTutorial() {
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

    private IEnumerator FakieTutorial() {
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
        trickList.fakieEnabled = true;
    }
    
    private void StartGrabTutorial() {
        StartCoroutine(GrabTutorial());
        grabStatus = TutorialStatus.Triggered;
    }

    private IEnumerator GrabTutorial() {
        float seconds = grabDelay / (1f / Time.timeScale);

        yield return new WaitForSeconds(seconds);

        grabStatus = TutorialStatus.WaitingForInput;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
        // spawn grab canvas that prompts player to type grab
        var trickCanvas = Instantiate(grabCanvasPrefab, Player.Instance.transform.position, Quaternion.identity);
        grabText = trickCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        trickList.grabEnabled = true;
    }
    
    private void StartDropTutorial() {
        StartCoroutine(DropTutorial());
        dropStatus = TutorialStatus.Triggered;
    }

    private IEnumerator DropTutorial() {
        float seconds = dropDelay / (1f / Time.timeScale);

        yield return new WaitForSeconds(seconds);

        dropStatus = TutorialStatus.WaitingForInput;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
        // spawn drop canvas that prompts player to type drop
        var trickCanvas = Instantiate(dropCanvasPrefab, Player.Instance.transform.position, Quaternion.identity);
        dropText = trickCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        trickList.dropEnabled = true;
    }
}
