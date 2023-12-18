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
    public List<Instruction> allInstructions = new List<Instruction>();

    // private state
    private Instruction currentInstruction;
    private List<Instruction> queuedInstructions = new List<Instruction>();
    private enum DisplayState {
        Active,
        FadingAway,
        None,
    }
    private DisplayState state = DisplayState.None;
    private float fadeTimer;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("More than one instance of Instructions");
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        // set text blank
        instructionText.text = "";

        // queue starting instructions
        string sceneName = SceneManager.GetActiveScene().name;
        bool clearPlayerPrefs = sceneName == "Level0";
        for (var i = 0; i < allInstructions.Count; i++)
        {
            var instruction = allInstructions[i];
            string playerPrefsKey = "Instruction" + instruction.instructionName;
            // clear player prefs if on level 0
            if(clearPlayerPrefs) PlayerPrefs.DeleteKey(playerPrefsKey);

            // decide whether or not to queue
            bool alreadyCompleted = PlayerPrefs.GetInt(playerPrefsKey, 0) == 1;
            bool queueOnThisScene = (instruction.queueOnLevel0 && (sceneName == "Level0" || sceneName == "Infinite")) ||
                                    (instruction.queueOnLevel1 && sceneName == "Level1") ||
                                    (instruction.queueOnLevel2 && sceneName == "Level2");
            if (!alreadyCompleted && queueOnThisScene) {
                queuedInstructions.Add(instruction);
                allInstructions.RemoveAt(i);
                i--;
            }
        }

        TrickManager.Instance.OnCompleteTrick += FinishInstruction;
    }

    // Update is called once per frame
    void Update()
    {   
        if (state == DisplayState.FadingAway) {
            // fade away
            fadeTimer -= Time.unscaledDeltaTime;
            byte a = (byte) (255 * (fadeTimer / timeToFadeAway));
            instructionText.faceColor = new Color32(instructionText.faceColor.r, instructionText.faceColor.g, instructionText.faceColor.b, a);
            instructionText.fontStyle = FontStyles.Strikethrough;
            // if done fading, remove instruction
            if (fadeTimer <= 0) {
                RemoveCurrentInstruction();
                state = DisplayState.None;
            }
        }
        else if (state == DisplayState.None) {
            if (queuedInstructions.Count > 0) {
                ShowNextInstruction();
            }
        }
        else if(state == DisplayState.Active) {
            
        }
    }

    private void FinishInstruction(Trick trick) {
        if(trick.Text == "push") FinishInstruction("push");
        if(trick.Text == "ollie") FinishInstruction("ollie");
        if(trick.trickScore > 0) FinishInstruction("trick");
        if(trick.trickScore > 0 && Score.Instance.GetUnsecuredScore() > 0) FinishInstruction("multiple");
        if(trick.Text == "grab") FinishInstruction("grab");
        if(trick.Text == "drop") FinishInstruction("drop");
        if(trick.availableInStates.Contains(Player.State.OnRail)) FinishInstruction("rail_trick");
    }

    private void RemoveCurrentInstruction() {
        instructionText.enabled = false;
        currentInstruction = null;
    }

    private void ShowNextInstruction() {
        // pop from queue
        currentInstruction = queuedInstructions[0];
        queuedInstructions.RemoveAt(0);

        // set instruction text
        instructionText.enabled = true;
        instructionText.text = currentInstruction.instructionText;
        instructionText.faceColor = new Color32(instructionText.faceColor.r, instructionText.faceColor.g, instructionText.faceColor.b, 255);
        instructionText.fontStyle = FontStyles.Normal;

        // set state
        state = DisplayState.Active;
        fadeTimer = timeToFadeAway;
    }

    public void QueueInstruction(string instructionName)
    {
        for (var i = 0; i < allInstructions.Count; i++)
        {
            var instruction = allInstructions[i];
            if (instruction.instructionName == instructionName)
            {
                queuedInstructions.Add(instruction);
                allInstructions.RemoveAt(i);
                return;
            }
        }
    }

    public void FinishInstruction(string instructionName)
    {
        if (currentInstruction == null) return;
        if (currentInstruction.instructionName == instructionName) {
            state = DisplayState.FadingAway;
            PlayerPrefs.SetInt("Instruction" + instructionName, 1);
        }
    }
}

[Serializable]
public class Instruction
{
    public string instructionName;
    public bool queueOnLevel0;
    public bool queueOnLevel1;
    public bool queueOnLevel2;
    [TextArea] public string instructionText;
}