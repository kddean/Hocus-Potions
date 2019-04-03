using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadingScreen : MonoBehaviour {
    public VideoPlayer video;
    public RawImage rawImage;
    public bool loading = false;

    private void Update() {
        if (loading) {
            StartCoroutine(PlayVideo());
            loading = false;
        }
    }

    IEnumerator PlayVideo() {
        video.Prepare();
        while (!video.isPrepared) {
            yield return null;
        }
        rawImage.texture = video.texture;
        video.Play();
    }

}
