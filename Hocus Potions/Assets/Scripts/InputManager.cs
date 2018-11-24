using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    //This class should be used to handle all keybinds and player inputs
    GameObject invPanel;
    CanvasGroup invGroup;
    GameObject spellCanvas;
    bool invToggle = false;
    ResourceLoader rl;
    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    void Start() {
        invPanel = GameObject.FindGameObjectWithTag("inventory");
        invGroup = invPanel.GetComponent<CanvasGroup>();
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        spellCanvas = GameObject.Find("SpellCanvas");
        spellCanvas.SetActive(false);
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.I)) {
            if (invToggle) {
                invToggle = false;
                invGroup.alpha = 0;
                invGroup.interactable = false;
                invGroup.blocksRaycasts = false;
            } else {
                invToggle = true;
                invGroup.alpha = 1;
                invGroup.interactable = true;
                invGroup.blocksRaycasts = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            spellCanvas.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            spellCanvas.SetActive(false);
        }
	}
}
