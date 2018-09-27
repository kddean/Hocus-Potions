using System;
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
                Button[] invButtons = invPanel.GetComponentsInChildren<Button>();
                Array.Sort(invButtons, delegate(Button x, Button y) { return x.name.CompareTo(y.name); });
                Debug.Log(rl.inv.inventory);
                for(int i = 0; i < rl.inv.inventory.Count; i++) {
                    invButtons[i].GetComponentInChildren<Text>().text = rl.inv.inventory[i].count.ToString();
                    invButtons[i].GetComponentInChildren<Image>().sprite = rl.inv.inventory[i].image;
                }
                for(int j = rl.inv.inventory.Count; j < rl.inv.maxSize; j++) {
                    invButtons[j].GetComponentInChildren<Text>().text = "";
                    invButtons[j].GetComponentInChildren<Image>().sprite = null;
                }
                invToggle = true;
                invGroup.alpha = 1;
                invGroup.interactable = true;
                invGroup.blocksRaycasts = true;
            }
        }
	}
}
