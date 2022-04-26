using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Instructions : MonoBehaviour {
    public static Instructions Instance;

    // public variables
    public TextMeshProUGUI instructionText;
    public float timeToFadeAway;

    // private state
    private string currentInstruction;
    private List<KeyValuePair<string, string>> queuedInstructions = new List<KeyValuePair<string, string>>();
    private enum InstructionState {
        Active,
        FadingAway,
        None,
    }
    private InstructionState state = InstructionState.None;
    private float fadeTimer;
    private float activeTimer;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("More than one instance of Instructions");
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // set text blank
        instructionText.text = "";
        
        // add starting instructions
        if (SceneManager.GetActiveScene().name == "Level0") {
            queuedInstructions.Add(new KeyValuePair<string, string>("push", "type <color=#FFFFFF>push</color> to gain speed"));
            queuedInstructions.Add(new KeyValuePair<string, string>("ollie", "type <color=#FFFFFF>ollie</color> to get air"));
            queuedInstructions.Add(new KeyValuePair<string, string>("trick", "do a trick in midair"));

            // set finish conditions
            TrickManager.Instance.onCompleteWord += word => {
                if(word.text == "push") FinishInstruction("push");
                else if(word.text == "ollie") FinishInstruction("ollie");
                else if(word.trickScore > 0) FinishInstruction("trick");
            };
        }
        
        // activate queue
        if(queuedInstructions.Count > 0) ShowNextInstruction();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == InstructionState.FadingAway) {
            // fade away
            fadeTimer -= Time.unscaledDeltaTime;
            byte a = (byte) (255 * (fadeTimer / timeToFadeAway));
            instructionText.faceColor = new Color32(instructionText.faceColor.r, instructionText.faceColor.g, instructionText.faceColor.b, a);
            instructionText.fontStyle = FontStyles.Strikethrough;
            // if done fading, remove instruction
            if (fadeTimer <= 0) {
                RemoveCurrentInstruction();
                state = InstructionState.None;
            }
        }
        else if (state == InstructionState.None) {
            if (queuedInstructions.Count > 0) {
                ShowNextInstruction();
            }
        }
    }

    private void RemoveCurrentInstruction() {
        instructionText.enabled = false;
        currentInstruction = "";
    }

    private void ShowNextInstruction() {
        // keep track of current instruction name
        currentInstruction = queuedInstructions[0].Key;

        // set instruction text
        instructionText.enabled = true;
        instructionText.text = queuedInstructions[0].Value;
        instructionText.faceColor = new Color32(instructionText.faceColor.r, instructionText.faceColor.g, instructionText.faceColor.b, 255);
        instructionText.fontStyle = FontStyles.Normal;
        
        // remove from queue
        queuedInstructions.RemoveAt(0);

        // set state
        state = InstructionState.Active;
        fadeTimer = timeToFadeAway;
    }

    public void FinishInstruction(string instructionName) {
        if (currentInstruction == instructionName) {
            state = InstructionState.FadingAway;
        }
    }
}