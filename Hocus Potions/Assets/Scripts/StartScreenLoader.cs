using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenLoader : MonoBehaviour {

    private void OnEnable() {
        Resources.FindObjectsOfTypeAll<StartScreen>()[0].gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    public void Awake() {
        DontDestroyOnLoad(this);
        if (Resources.FindObjectsOfTypeAll(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }
}
