using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseAudioController : MonoBehaviour {

    AudioSource audioSource;
    StartScreen start;
    public bool startPlaying;
    public bool fadeOutAudio;
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        start = Resources.FindObjectsOfTypeAll<StartScreen>()[0];
        fadeOutAudio = false;
        startPlaying = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (start.startScreenOpen) {
            return;
        }
        if (startPlaying && !audioSource.isPlaying) {
            audioSource.Play();
        }
        if (!fadeOutAudio) {
            if (audioSource.volume < 0.95f) {
                audioSource.volume += Time.deltaTime / 4;
            } else {
                audioSource.volume = 1;
            }
        } else {
            if (audioSource.volume > 0.05f) {
                audioSource.volume -= Time.deltaTime * 1.5f;
            } else {
                audioSource.volume = 0;
            }
        }
	}
}
