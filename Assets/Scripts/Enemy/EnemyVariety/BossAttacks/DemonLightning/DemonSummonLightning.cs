using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSummonLightning : Summon {
    public AudioManager audioManager;

    public override void OnStrikeAnim() {
        base.OnStrikeAnim();

        PerformHit();
        audioManager.PlaySfx(audioManager.lightningClip);
    }

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
}
