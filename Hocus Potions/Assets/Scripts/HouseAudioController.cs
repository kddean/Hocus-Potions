using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseAudioController : MonoBehaviour {

    AudioSource audio;
    StartScreen start;
    public bool startPlaying;
    public bool fadeOutAudio;
	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
        start = Resources.FindObjectsOfTypeAll<StartScreen>()[0];
        fadeOutAudio = false;
        startPlaying = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (start.startScreenOpen) {
            return;
        }
        if (startPlaying && !audio.isPlaying) {
            audio.Play();
        }
        if (!fadeOutAudio) {
            if (audio.volume < 0.95f) {
                audio.volume += Time.deltaTime / 4;
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
