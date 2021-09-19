using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrickList : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject typingManager;
    public List<Word> tricklist;
    public TextMeshProUGUI trickDisplay;
    private string display;
    void Start()
    {
        GameObject typingManager = GameObject.Find("Typing Manager");
        TypingManager manager = typingManager.GetComponent<TypingManager>();
        tricklist = manager.words;
        display = "Tricks\tPoints\n";
        foreach (Word w in tricklist)
        {
            display += w.text + '\t' + w.trickScore + '\n';
        }
    }

    // Update is called once per frame
    void Update()
    {
        trickDisplay.text = display;
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            trickDisplay.text = display;
        }
        else
        {
            trickDisplay.text = "";
        }*/
    }


}
