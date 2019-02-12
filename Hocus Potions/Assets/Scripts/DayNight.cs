using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNight : MonoBehaviour {
    CanvasGroup shader;
    MoonCycle mc;
    bool fading;
	// Use this for initialization
	void Start () {
        shader = gameObject.GetComponentInChildren<CanvasGroup>();
        mc = GameObject.FindObjectOfType<MoonCycle>();
        fading = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!fading && mc.hour == 17) {
            StartCoroutine(FadeOut());
            fading = true;
        } else if(!fading && shader.alpha > 0 && mc.Hour == 6) {
            StartCoroutine(FadeIn());
            fading = true;
        }
	}

    IEnumerator FadeOut() {
        float a = shader.alpha;
        a += 0.101f;
        shader.alpha = a;
        if (a > 0.70f) {
            fading = false;
            yield return null;
        } else {
            yield return new WaitForSeconds(mc.CLOCK_SPEED);
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeIn() {
        float a = shader.alpha;
        a -= 0.101f;
        shader.alpha = a;
        if (a <= 0f) {
            fading = false;
            yield return null;
        } else {
            yield return new WaitForSeconds(mc.CLOCK_SPEED);
            StartCoroutine(FadeIn());
        }
    }
}
