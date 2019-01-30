using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashObject : MonoBehaviour {
    public float totalSeconds;
    public float maxIntensity;
    public Light lightEffect;
    public bool flashing = true;

    public IEnumerator flashNow () {
        if (!this.flashing) {
            yield return null;
        }
        float waitTime = this.totalSeconds / 2;
        while (this.lightEffect.intensity < this.maxIntensity) {
            this.lightEffect.intensity += Time.deltaTime / waitTime;
            yield return null;
        }
        while (this.lightEffect.intensity > 0) {
            this.lightEffect.intensity -= Time.deltaTime / waitTime;
            yield return null;
        }
        yield return null;
    }
}