using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour {
    public bool startScreenOpen;

    private void OnEnable() {
        startScreenOpen = true;
    }
    public void NewGame() {
        Time.timeScale = 1;
        startScreenOpen = false;
        gameObject.SetActive(false);
    }

    public void LoadGame() {
        Time.timeScale = 1;
        startScreenOpen = false;
        Resources.FindObjectsOfTypeAll<MainMenu>()[0].gameObject.SetActive(true);
        Resources.FindObjectsOfTypeAll<MainMenu>()[0].LoadGame();
        gameObject.SetActive(false);
    }

    public void ToggleControls() {
        CanvasGroup[] cgs = GetComponentsInChildren<CanvasGroup>();
        foreach(CanvasGroup c in cgs) {
            c.interactable = !c.interactable;
            c.blocksRaycasts = !c.blocksRaycasts;
            c.alpha = Mathf.Abs(c.alpha - 1);
        }
    }

    public void QuitGame() {
        Application.Quit();
    }

}
