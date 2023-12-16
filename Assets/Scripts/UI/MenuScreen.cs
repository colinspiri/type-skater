using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour {
    // components
    public RectTransform rectTransform;
    public EventSystem eventSystem;
    public CanvasGroup canvasGroup;

    // constants
    public UIConstants uiConstants;
    public Selectable defaultButton;
    private Selectable _lastButton = null;

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
    }

    private void EnableFunctionality() {
        eventSystem.gameObject.SetActive(true);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        SelectButton(true);
    }

    private void DisableFunctionality() {
        eventSystem.gameObject.SetActive(false);

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        UpdateLastButton();
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
        
        if (_lastButton)
        {
            _lastButton.Select();
        }
        else if (defaultButton)
        {
            defaultButton.Select();
        }
    }

    private IEnumerator SelectButtonCoroutine() {
        yield return null;
        if (_lastButton)
        {
            _lastButton.Select();
        }
        else if (defaultButton)
        {
            defaultButton.Select();
        }
    }

    private void UpdateLastButton() {
        if (eventSystem.currentSelectedGameObject)
        {
            _lastButton = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
        }
    }
}