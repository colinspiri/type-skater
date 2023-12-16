using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Selectable))]
public class MenuOptionAnimator : MonoBehaviour {
    // constants
    public UIConstants uiConstants;
    [Space]
    
    // components
    [Header("Selectable")]
    private Selectable _selectable;
    public GameObject selectionBox;
    public TextMeshProUGUI text;
    [Header("Toggle")] 
    public Toggle toggle;
    public GameObject whenOn;
    public GameObject whenOff;
    
    // state
    private Color _normalTextColor;

    private void Awake() {
        _selectable = GetComponent<Selectable>();
    }

    private void Start() {
        if (text != null) {
            _normalTextColor = text.color;
        }

        if (toggle != null) {
            toggle.onValueChanged.AddListener(on => {
                UpdateToggle();
            });
        }
    }

    public void UpdateToggle() {
        bool on = toggle.isOn;
        whenOn.SetActive(on);
        whenOff.SetActive(!on);   
    }

    public void Submit() {
        ExecuteEvents.Execute(_selectable.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }
    
    public void Deselect() {
        if(EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    public void PlayDeselectAnimation() {
        if (selectionBox != null) {
            selectionBox.SetActive(false);
        }
        if (text != null) {
            text.DOColor(_normalTextColor, uiConstants.selectTime).SetUpdate(true);
        }
    }

    public void PlaySelectAnimation() {
        if (selectionBox != null) {
            selectionBox.SetActive(true);
        }
        if (text != null) {
            text.DOColor(uiConstants.selectedColor, uiConstants.selectTime).SetUpdate(true);
        }
        // if(AudioManager.Instance) AudioManager.Instance.PlaySelectSound();
    }

    public void PlaySubmitAnimation()
    {
        // to be implemented
    }

    public void PlaySelectSound() {
        if(AudioManager.Instance) AudioManager.Instance.PlayTypingSound();
    }

    public void PlaySubmitSound()
    {
        if(AudioManager.Instance) AudioManager.Instance.PlayTrickSound();
    }
    public void PlayBackSound()
    {
        if(AudioManager.Instance) AudioManager.Instance.PlayTypingWrongSound();
    }
}