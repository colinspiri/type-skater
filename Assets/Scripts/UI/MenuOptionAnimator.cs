using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    public List<TextMeshProUGUI> texts;
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
        if (texts != null && texts.Count > 0) {
            _normalTextColor = texts[0].color;
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
        if (texts != null) {
            foreach (var text in texts) {
                text.DOColor(_normalTextColor, uiConstants.selectTime).SetUpdate(true);
            }
        }
    }

    public void PlaySelectAnimation() {
        if (selectionBox != null) {
            selectionBox.SetActive(true);
        }
        if (texts != null) {
            foreach (var text in texts) {
                text.DOColor(uiConstants.selectedColor, uiConstants.selectTime).SetUpdate(true);
            }
        }
        PlaySelectSound();
    }

    public void PlaySubmitAnimation()
    {
        // to be implemented
    }

    public void PlaySelectSound() {
        if(AudioManager.Instance) AudioManager.Instance.PlaySelectSound();
    }

    public void PlaySubmitSound()
    {
        if(AudioManager.Instance) AudioManager.Instance.PlaySubmitSound();
    }
    public void PlayBackSound()
    {
        if(AudioManager.Instance) AudioManager.Instance.PlayBackSound();
    }
}