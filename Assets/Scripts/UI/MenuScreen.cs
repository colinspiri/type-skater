using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour {
    // components
    public UIConstants uiConstants;
    public RectTransform rectTransform;
    public EventSystem eventSystem;
    public CanvasGroup canvasGroup;
    public List<MenuOptionAnimator> typingMenuOptions;

    // constants
    [FormerlySerializedAs("defaultButton")] public Selectable defaultSelectable;
    private Selectable _lastSelected = null;
    
    // state 
    private List<Word> _wordList;
    private Dictionary<string, MenuOptionAnimator> _menuOptionsByWord;

    private void Initialize() {
        _wordList = new List<Word>();
        _menuOptionsByWord = new Dictionary<string, MenuOptionAnimator>();
        foreach (var menuOptionAnimator in typingMenuOptions) {
            _wordList.Add(menuOptionAnimator.typingWord);
            _menuOptionsByWord[menuOptionAnimator.typingText.text] = menuOptionAnimator;
        }
    }

    public void StartOffscreen() {
        rectTransform.anchoredPosition = new Vector2(uiConstants.offscreenDistance, 0);
        canvasGroup.alpha = 0;
        DisableFunctionality();
    }

    public void StartOnscreen() {
        rectTransform.anchoredPosition = new Vector2(0, 0);
        canvasGroup.alpha = 1;
        EnableFunctionality();
    }

    public void Push() {
        rectTransform.DOAnchorPosX(0, uiConstants.menuScreenTransitionTime).SetUpdate(true).onComplete +=
            EnableFunctionality;
        canvasGroup.DOFade(1, uiConstants.menuScreenTransitionTime).SetUpdate(true).SetEase(Ease.OutSine);
    }

    public void PutAway() {
        DisableFunctionality();
        rectTransform.DOAnchorPosX(-uiConstants.offscreenDistance, uiConstants.menuScreenTransitionTime)
            .SetUpdate(true);
        canvasGroup.DOFade(0, uiConstants.menuScreenTransitionTime).SetUpdate(true).SetEase(Ease.OutSine);
    }

    public void Pop() {
        DisableFunctionality();
        rectTransform.DOAnchorPosX(uiConstants.offscreenDistance, uiConstants.menuScreenTransitionTime).SetUpdate(true);
        canvasGroup.DOFade(0, uiConstants.menuScreenTransitionTime).SetUpdate(true).SetEase(Ease.OutSine);
    }

    public void BringBack() {
        rectTransform.DOAnchorPosX(0, uiConstants.menuScreenTransitionTime).SetUpdate(true).onComplete +=
            EnableFunctionality;
        canvasGroup.DOFade(1, uiConstants.menuScreenTransitionTime).SetUpdate(true).SetEase(Ease.OutSine);
    }

    private void OnDisable() {
        rectTransform.DOKill();
        canvasGroup.DOKill();
        DisableFunctionality();
    }

    private void EnableFunctionality() {
        eventSystem.gameObject.SetActive(true);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        SelectButton(true);
        
        // set word list of typing manager & add callbacks
        if(_wordList == null) Initialize();
        Debug.Log(name + " setting word list");
        TypingManager.Instance.SetWordList(_wordList);
        TypingManager.Instance.SetClearOnWrongChar(true);
        TypingManager.OnTypeChar += OnTypeChar;
        TypingManager.OnTypeWord += OnTypeWord;
    }

    private void OnTypeChar(bool charIsCorrect) {
        if (charIsCorrect) {
            if(AudioManager.Instance) AudioManager.Instance.PlayTypingSound();
            
            // check possible words
            foreach (var possibleWord in TypingManager.Instance.PossibleWords) {
                foreach (var pair in _menuOptionsByWord) {
                    if (possibleWord.Equals(pair.Key)) {
                        pair.Value.Selectable.Select();
                        pair.Value.UpdateTypingHighlight(TypingManager.Instance.TypedText);
                        return;
                    }
                }
            }
        }
        if(AudioManager.Instance) AudioManager.Instance.PlayTypingWrongSound();

        // if nothing matches, deselect current selectable
        eventSystem.SetSelectedGameObject(null);
    }
    private void OnTypeWord(Word word) {
        // submit on menu option 
        foreach (var pair in _menuOptionsByWord) {
            if (word.Equals(pair.Key)) {
                pair.Value.Submit();
                return;
            }
        }
    }

    private void DisableFunctionality() {
        eventSystem.gameObject.SetActive(false);

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        UpdateLastButton();
        
        // remove word list of typing manager
        TypingManager.Instance.ClearWordList();
        TypingManager.Instance.SetClearOnWrongChar(false);
        TypingManager.OnTypeChar -= OnTypeChar;
        TypingManager.OnTypeWord -= OnTypeWord;
    }
    
    public void CheckSelectable()
    {
        if (eventSystem.currentSelectedGameObject == null) {
            SelectButton();
        } 
    }
    
    private void SelectButton(bool delay = false) {
        if (delay) {
            StartCoroutine(SelectButtonCoroutine());
            return;
        }
        
        if (_lastSelected)
        {
            _lastSelected.Select();
        }
        else if (defaultSelectable)
        {
            defaultSelectable.Select();
        }
    }

    private IEnumerator SelectButtonCoroutine() {
        yield return null;
        if (_lastSelected)
        {
            _lastSelected.Select();
        }
        else if (defaultSelectable)
        {
            defaultSelectable.Select();
        }
    }

    private void UpdateLastButton() {
        if (eventSystem.currentSelectedGameObject)
        {
            _lastSelected = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
        }
    }
}