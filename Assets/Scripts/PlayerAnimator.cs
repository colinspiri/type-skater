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
    public SpriteRenderer sprite;
    public ParticleSystem trickParticles;
    public ParticleSystem pushParticles;
    public ParticleSystem grindParticles;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        originalColor = sprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        GrindParticles();
    }

    private void GrindParticles() {
        if(Player.Instance.state == Player.State.OnRail) grindParticles.Play();
        else if (grindParticles.isPlaying) grindParticles.Stop();
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
