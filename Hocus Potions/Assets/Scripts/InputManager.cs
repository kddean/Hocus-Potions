using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {
    public KeyCode pauseKey = KeyCode.P, mainMenuKey = KeyCode.Escape, walkForwardKey = KeyCode.W, walkLeftKey = KeyCode.A, walkRightKey = KeyCode.D, walkBackwardKey = KeyCode.S, spellMenuKey = KeyCode.LeftControl, spellKey1 = KeyCode.F1, spellKey2 = KeyCode.F2, spellKey3 = KeyCode.F3, spellKey4 = KeyCode.F4,
        inventoryKey = KeyCode.I, inventory1 = KeyCode.Alpha1, inventory2 = KeyCode.Alpha2, inventory3 = KeyCode.Alpha3, inventory4 = KeyCode.Alpha4, inventory5 = KeyCode.Alpha5, inventory6 = KeyCode.Alpha6, inventory7 = KeyCode.Alpha7, inventory8 = KeyCode.Alpha8, inventory9 = KeyCode.Alpha9, inventory10 = KeyCode.Alpha0;
    //This class should be used to handle all keybinds and player inputs
    GameObject invPanel;
    CanvasGroup invGroup;
    GameObject spellCanvas;
    GameObject mainMenu;
    ResourceLoader rl;
    bool paused; 
    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    void Start() {
        invPanel = GameObject.FindGameObjectWithTag("inventory");
        invGroup = invPanel.GetComponent<CanvasGroup>();
        spellCanvas = GameObject.Find("SpellCanvas");
        mainMenu = GameObject.Find("MainMenuCanvas");
        rl = GameObject.FindObjectOfType<ResourceLoader>();
        mainMenu.SetActive(false);
        spellCanvas.SetActive(false);
        paused = false;
    }

    void Update() {
        if (GameObject.FindObjectOfType<Player>().Status.Contains(Player.PlayerStatus.asleep)) {
            return;
        }
        /*TODO: For Keybinds
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKeyDown(kcode)) {
                Debug.Log("KeyCode down: " + kcode);
                break;
            }
        }*/

        //Pause
        if (Input.GetKeyDown(pauseKey)) {
            if (!paused) {
                Time.timeScale = 0;
                paused = true;
            } else {
                paused = false;
                Time.timeScale = 1;
            }
        }

        //Pause Menu
        if (Input.GetKeyUp(mainMenuKey)) {
            if (mainMenu.activeSelf) {
                paused = false;
                Time.timeScale = 1;
            } else {
                Time.timeScale = 0;
                paused = true;
            }
            mainMenu.SetActive(!mainMenu.activeSelf);
        }
    
        if (paused) { return; }

        //Inventory
        if (Input.GetKeyDown(inventoryKey)) {
            if (invGroup.alpha == 1) {
                invGroup.gameObject.GetComponentsInChildren<AudioSource>()[1].Play();
                invGroup.alpha = 0;
                invGroup.interactable = false;
                invGroup.blocksRaycasts = false;
            } else {
                invGroup.gameObject.GetComponentsInChildren<AudioSource>()[0].Play();
                invGroup.alpha = 1;
                invGroup.interactable = true;
                invGroup.blocksRaycasts = true;
            }
        }

        if (Input.GetKeyDown(inventory1)) {
            GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>()[0].SetActive();
        }
        if (Input.GetKeyDown(inventory2)) {
            GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>()[1].SetActive();
        }
        if (Input.GetKeyDown(inventory3)) {
            GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>()[2].SetActive();
        }
        if (Input.GetKeyDown(inventory4)) {
            GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>()[3].SetActive();
        }
        if (Input.GetKeyDown(inventory5)) {
            GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>()[4].SetActive();
        }
        if (Input.GetKeyDown(inventory6)) {
            GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>()[5].SetActive();
        }
        if (Input.GetKeyDown(inventory7)) {
            GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>()[6].SetActive();
        }
        if (Input.GetKeyDown(inventory8)) {
            GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>()[7].SetActive();
        }
        if (Input.GetKeyDown(inventory9)) {
            GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>()[8].SetActive();
        }
        if (Input.GetKeyDown(inventory10)) {
            GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>()[9].SetActive();
        }

        //Spells
        if (Input.GetKeyDown(spellMenuKey)) {
            spellCanvas.SetActive(true);
        }

        if (Input.GetKeyUp(spellMenuKey)) {
            spellCanvas.SetActive(false);
        }

        if (Input.GetKeyDown(spellKey1)) {
            Resources.FindObjectsOfTypeAll<SpellCanvas>()[0].SetActiveSpell(0);
        }
        if (Input.GetKeyDown(spellKey2)) {
            Resources.FindObjectsOfTypeAll<SpellCanvas>()[0].SetActiveSpell(1);
        }
        if (Input.GetKeyDown(spellKey3)) {
            Resources.FindObjectsOfTypeAll<SpellCanvas>()[0].SetActiveSpell(2);
        }
        if (Input.GetKeyDown(spellKey4)) {
            Resources.FindObjectsOfTypeAll<SpellCanvas>()[0].SetActiveSpell(3);
        }
    }
}
