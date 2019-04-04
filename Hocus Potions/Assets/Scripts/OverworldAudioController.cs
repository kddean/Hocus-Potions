using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldAudioController : MonoBehaviour {

    AudioSource[] audioSources;
    AudioSource currentAudio;
    public bool fadeOutAudio;
    // Use this for initialization
    void Start() {
        audioSources = GetComponents<AudioSource>();
        currentAudio = audioSources[0];
        fadeOutAudio = false;
    }

    // Update is called once per frame
    void Update() {
        if (!fadeOutAudio) {
            if (currentAudio.volume < 0.95f) {
                currentAudio.volume += Time.deltaTime / 8;
            } else {
                currentAudio.volume = 1;
            }
        } else {
            if (currentAudio.volume > 0.05f) {
                currentAudio.volume -= Time.deltaTime * 1.5f;
            } else {
                currentAudio.volume = 0;
            }
        }
    }
}
