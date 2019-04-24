using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartScreen : MonoBehaviour {
    public bool startScreenOpen;
    public VideoPlayer video, video2;
    public RawImage rawImage;
    AudioSource audioSource;
    bool clickedButton;

    private void OnEnable() {
        startScreenOpen = true;
        StartCoroutine(PlayVideo());
        GameObject.FindObjectOfType<HouseAudioController>().startPlaying = false;
        audioSource = GetComponent<AudioSource>();
        clickedButton = false;
    }
    public void NewGame() {
        if (!clickedButton) {
            StartCoroutine(FadeoutAudio(true));
            clickedButton = true;
            GameObject.FindObjectOfType<Tutorial>().doTutorial = true;
        }
    }

    public void LoadGame() {
        if (!clickedButton) {
            StartCoroutine(FadeoutAudio(false));
            clickedButton = true;
        }
    }

    public void ToggleControls() {
        CanvasGroup[] cgs = GetComponentsInChildren<CanvasGroup>();
        foreach(CanvasGroup c in cgs) {
            c.interactable = !c.interactable;
            c.blocksRaycasts = !c.blocksRaycasts;
            c.alpha = Mathf.Abs(c.alpha - 1);
        }
        KeybindManager[] keybindManagers = GameObject.FindObjectsOfType<KeybindManager>();
        foreach (KeybindManager km in keybindManagers) {
            km.LoadKeybindText();
        }
    }

    public void QuitGame() {
        Application.Quit();
    }

    IEnumerator FadeoutAudio(bool newGame) {
        yield return null;
        while (audioSource.volume > 0.05f) {
            audioSource.volume -= 0.02f;
            yield return new WaitForEndOfFrame();
        }
        audioSource.volume = 0;
        audioSource.Stop();

        if (newGame) {
            Time.timeScale = 1;
            startScreenOpen = false;
            gameObject.SetActive(false);
            GameObject.FindObjectOfType<HouseAudioController>().startPlaying = true;
        } else {
            Time.timeScale = 1;
            startScreenOpen = false;
            Resources.FindObjectsOfTypeAll<MainMenu>()[0].gameObject.SetActive(true);
            Resources.FindObjectsOfTypeAll<MainMenu>()[0].LoadGame();
            gameObject.SetActive(false);
        }
    }

    IEnumerator PlayVideo() {
        video.Prepare();
        video2.Prepare();
        while (!video.isPrepared) {
            yield return null;
        }
        rawImage.texture = video.texture;
        video.Play();
        yield return new WaitForSecondsRealtime(2.7f);
        video.Stop();

        while (!video2.isPrepared) {
            yield return null;
        }

        rawImage.texture = video2.texture;
        video2.Play();
    }

}
