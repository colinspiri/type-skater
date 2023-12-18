using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Word : ScriptableObject {
    [FormerlySerializedAs("_wordText")] 
    [SerializeField] private string wordText;
    public string Text => wordText;

    public int StartsWith(string substring) {
        int correctLength = 0;
        
        for (int i = 1; i <= substring.Length; i++) {
            string currentWordText = wordText.Substring(0, i);
            string currentSubstring = substring.Substring(0, i);

            if (currentWordText.Equals(currentSubstring)) {
                correctLength++;
            }
            else {
                break;
            }
        }

        return correctLength;
    }

    public void SetText(string value) {
        wordText = value;
    }

    public bool Equals(string text) {
        return wordText.Equals(text);
    }

    public virtual void Complete() { }

    public override string ToString() {
        return wordText;
    }
}