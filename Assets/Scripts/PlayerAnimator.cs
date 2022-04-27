using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    public static PlayerAnimator Instance;
    
    // constants
    public Color flashColor;
    private Color originalColor;
    
    // components
    private SpriteRenderer sprite;
    public ParticleSystem trickParticles;
    public ParticleSystem pushParticles;

    private void Awake() {
        Instance = this;

        sprite = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start() {
        originalColor = sprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PushParticles() {
        pushParticles.Play();
    }

    public void Flash() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(sprite.DOColor(flashColor, 0.1f));
        sequence.Append(sprite.DOColor(originalColor, 0.2f));

        trickParticles.Play();
    }
}
