using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldAudioController : MonoBehaviour {
    Player player;
    AudioSource[] audioSources;
    AudioSource currentAudio;
    public bool fadeOutAudio;
    bool fading;
    // Use this for initialization
    void Start() {
        audioSources = GetComponents<AudioSource>();
        currentAudio = audioSources[0];
        fadeOutAudio = false;
        fading = false;
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update() {
        if (fadeOutAudio) {
            StartCoroutine(FadeOut());
            fadeOutAudio = false;
        }
    }

    public void SwapZones(string zone) {
        StartCoroutine(FadeQueue(zone));
    }

    IEnumerator FadeQueue(string zone) {
        while (fading) {
            yield return null;
        }
        switch (zone) {
            case "ForestZone":
            case "HomeZone":
                if (currentAudio != audioSources[0]) {
                    StartCoroutine(CrossFade(0));
                    Debug.Log("0");
                }
                break;
            case "MeadowZone":
                if (currentAudio != audioSources[1]) {
                    StartCoroutine(CrossFade(1));
                    Debug.Log("1");
                }
                break;
            case "ShrineZone":
                if (currentAudio != audioSources[2]) {
                    StartCoroutine(CrossFade(2));
                    Debug.Log("2");
                }
                break;
            case "MountainsZone":
                if (currentAudio != audioSources[3]) {
                    StartCoroutine(CrossFade(3));
                    Debug.Log("3");
                }
                break;
            default:
                break;
        }
    }
    IEnumerator FadeOut() {
        while (currentAudio.volume > 0.05f) {
            currentAudio.volume -= Time.deltaTime * 1.5f;
            yield return new WaitForEndOfFrame();
        }
        currentAudio.volume = 0;
    }

    IEnumerator FadeIn() {
        while (currentAudio.volume < 0.95f) {
            currentAudio.volume += Time.deltaTime * 1.5f;
            yield return new WaitForEndOfFrame();
        }
        currentAudio.volume = 1;
    }

    IEnumerator CrossFade(int i) {
        fading = true;
        audioSources[i].Play();
        while (currentAudio.volume > 0.05f) {
            currentAudio.volume -= Time.deltaTime / 1.5f;
            audioSources[i].volume += Time.deltaTime / 1.5f;
            yield return new WaitForEndOfFrame();
        }
        currentAudio.volume = 0;
        currentAudio.Stop();
        audioSources[i].volume = 1;
        currentAudio = audioSources[i];
        fading = false;
    }
}
