using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldAudioController : MonoBehaviour {

    AudioSource audio;
    public bool fadeOutAudio;
    // Use this for initialization
    void Start() {
        audio = GetComponent<AudioSource>();
        fadeOutAudio = false;
    }

    // Update is called once per frame
    void Update() {
        if (!fadeOutAudio) {
            if (audio.volume < 0.95f) {
                audio.volume += Time.deltaTime / 8;
            } else {
                audio.volume = 1;
            }
        } else {
            if (audio.volume > 0.05f) {
                audio.volume -= Time.deltaTime * 1.5f;
            } else {
                audio.volume = 0;
            }
        }
    }
}
