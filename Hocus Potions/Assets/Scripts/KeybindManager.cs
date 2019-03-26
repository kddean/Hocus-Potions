using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeybindManager : MonoBehaviour {
    public string key;
    InputManager im;
	// Use this for initialization
	void Start () {
        im = GameObject.FindObjectOfType<InputManager>();
	}
	
    public void SetKeybind() {
        StopAllCoroutines();
        StartCoroutine(WaitForKey());

        GetComponentInChildren<Text>().text = "";
       
    }

    IEnumerator WaitForKey() {
        while (!Input.anyKeyDown) {
            yield return null;
        }

        KeyCode newKey = KeyCode.RightCommand;
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKeyDown(kcode)) {
                newKey = kcode;
                break;
            }
        }

        GetComponentInChildren<Text>().text = newKey.ToString();
        switch (key) {
            case "invToggle":
                im.inventoryKey = newKey;
                break;
            case "inv1":
                im.inventory1 = newKey;
                break;
            case "inv2":
                im.inventory2 = newKey;
                break;
            case "inv3":
                im.inventory3 = newKey;
                break;
            case "inv4":
                im.inventory4 = newKey;
                break;
            case "inv5":
                im.inventory5 = newKey;
                break;
            case "inv6":
                im.inventory6 = newKey;
                break;
            case "inv7":
                im.inventory7 = newKey;
                break;
            case "inv8":
                im.inventory8 = newKey;
                break;
            case "inv9":
                im.inventory9 = newKey;
                break;
            case "inv10":
                im.inventory10 = newKey;
                break;
            case "spellToggle":
                im.spellMenuKey = newKey;
                break;
            case "wildGrowth":
                im.spellKey1 = newKey;
                break;
            case "Ignite":
                im.spellKey2 = newKey;
                break;
            case "Smash":
                im.spellKey3 = newKey;
                break;
            case "Dredge":
                im.spellKey4 = newKey;
                break;
            case "mainMenu":
                im.mainMenuKey = newKey;
                break;
            case "pause":
                im.pauseKey = newKey;
                break;
            case "walkF":
                im.walkForwardKey = newKey;
                break;
            case "walkB":
                im.walkBackwardKey = newKey;
                break;
            case "walkL":
                im.walkLeftKey = newKey;
                break;
            case "walkR":
                im.walkRightKey = newKey;
                break;
            default:
                break;
        }
    }
}
