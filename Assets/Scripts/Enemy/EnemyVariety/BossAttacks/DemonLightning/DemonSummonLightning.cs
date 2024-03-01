using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSummonLightning : Summon {
    public override void OnStrikeAnim() {
        base.OnStrikeAnim();

        PerformHit();
    }
}
