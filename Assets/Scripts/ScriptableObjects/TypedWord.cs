using UnityEngine;

[CreateAssetMenu]
public class TypedWord : ScriptableObject {
    [SerializeField] private string _wordText;
    
    // public GameEvent onComplete
    
    public TypedWord(string t)
    {
        _wordText = t;
    }
    
    public int StartsWith(string substring) {
        int correctLength = 0;
        
        for (int i = 1; i <= substring.Length; i++) {
            string currentWordText = _wordText.Substring(0, i);
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

    public bool Equals(string text) {
        return _wordText.Equals(text);
    }

    public virtual void Complete() {
        Debug.Log("Completed " + _wordText);
    }

    public override string ToString() {
        return _wordText;
    }
}