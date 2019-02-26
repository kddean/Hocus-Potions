using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNight : MonoBehaviour {
    CanvasGroup shader;
    MoonCycle mc;
    bool fading;
    bool wasCat;
    Player player;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        shader = gameObject.GetComponentInChildren<CanvasGroup>();
        mc = GameObject.FindObjectOfType<MoonCycle>();
        fading = false;
        wasCat = false;
        if(mc.hour > 17 || mc.hour < 6) {
            if (player.Status.Contains(Player.PlayerStatus.transformed)) {
                wasCat = true;
                shader.alpha = 0.503f;
            } else {
                shader.alpha = 0.707f;
            }
        } else if(mc.hour == 17) {
            shader.alpha += 0.101f * (mc.minutes / 10);
            StartCoroutine(FadeOut());
            fading = true;
        } else if(mc.hour == 6) {
            shader.alpha = 0.707f;
            shader.alpha -= 0.101f * (mc.minutes / 10);
            StartCoroutine(FadeIn());
            fading = true;
        }


	}
	
	// Update is called once per frame
	void Update () {
        if(shader.alpha > 0.6f && player.Status.Contains(Player.PlayerStatus.transformed)) {
            wasCat = true;
            shader.alpha = 0.503f;
        }

        if (wasCat && !player.Status.Contains(Player.PlayerStatus.transformed)) {
            wasCat = false;
            if(shader.alpha == 0.503f) {
                shader.alpha = 0.707f;
            }
        }
		if(!fading && mc.hour == 17) {
            StartCoroutine(FadeOut());
            fading = true;
        } else if(!fading && shader.alpha > 0 && mc.Hour == 6) {
            StartCoroutine(FadeIn());
            fading = true;
        }
	}

    IEnumerator FadeOut() { 
        shader.alpha += 0.101f;
        if ((!player.Status.Contains(Player.PlayerStatus.transformed) && shader.alpha > 0.70f) || (player.Status.Contains(Player.PlayerStatus.transformed) && shader.alpha > 0.5f)) {
            fading = false;
            yield return null;
        } else {
            yield return new WaitForSeconds(mc.CLOCK_SPEED);
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeIn() {
        shader.alpha -= 0.101f;
        if (shader.alpha <= 0f) {
            fading = false;
            yield return null;
        } else {
            yield return new WaitForSeconds(mc.CLOCK_SPEED);
            StartCoroutine(FadeIn());
        }
    }
}
