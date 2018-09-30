﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

    //This class should be used to handle all keybinds and player inputs
    GameObject invPanel;
    CanvasGroup invGroup;
    bool invToggle = false;
    ResourceLoader rl;

    void Start() {
        DontDestroyOnLoad(this.gameObject);
        invPanel = GameObject.FindGameObjectWithTag("inventory");
        invGroup = invPanel.GetComponent<CanvasGroup>();
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
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
	}
}