using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//This class should be used to handle all keybinds and player inputs
public class InputManager : MonoBehaviour {
    public KeyCode inventoryKey = KeyCode.I, inventory1 = KeyCode.Alpha1, inventory2 = KeyCode.Alpha2, inventory3 = KeyCode.Alpha3, inventory4 = KeyCode.Alpha4, inventory5 = KeyCode.Alpha5, inventory6 = KeyCode.Alpha6, inventory7 = KeyCode.Alpha7, inventory8 = KeyCode.Alpha8, inventory9 = KeyCode.Alpha9, inventory10 = KeyCode.Alpha0,
                   spellMenuKey = KeyCode.LeftControl, spellKey1 = KeyCode.F1, spellKey2 = KeyCode.F2, spellKey3 = KeyCode.F3, spellKey4 = KeyCode.F4, pauseKey = KeyCode.P, mainMenuKey = KeyCode.Escape, walkForwardKey = KeyCode.W, walkLeftKey = KeyCode.A, walkRightKey = KeyCode.D, walkBackwardKey = KeyCode.S;
    public List<KeyCode> keybinds;
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
        keybinds = new List<KeyCode>();
        SetupKeybinds();
    }

    void Update() {
        if (GameObject.FindObjectOfType<Player>().Status.Contains(Player.PlayerStatus.asleep) || GameObject.FindObjectOfType<StartScreen>() != null) {
            return;
        }

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

    void SetupKeybinds() {
        keybinds.Clear();
        keybinds.Add(inventoryKey);
        keybinds.Add(inventory1);
        keybinds.Add(inventory2);
        keybinds.Add(inventory3);
        keybinds.Add(inventory4);
        keybinds.Add(inventory5);
        keybinds.Add(inventory6);
        keybinds.Add(inventory7);
        keybinds.Add(inventory8);
        keybinds.Add(inventory9);
        keybinds.Add(inventory10);
        keybinds.Add(spellMenuKey);
        keybinds.Add(spellKey1);
        keybinds.Add(spellKey2);
        keybinds.Add(spellKey3);
        keybinds.Add(spellKey4);
        keybinds.Add(mainMenuKey);
        keybinds.Add(pauseKey);
        keybinds.Add(walkForwardKey);
        keybinds.Add(walkBackwardKey);
        keybinds.Add(walkLeftKey);
        keybinds.Add(walkRightKey);
    }

    public void LoadKeybinds() {
        inventoryKey = keybinds[0];
        inventory1 = keybinds[1];
        inventory2 = keybinds[2];
        inventory3 = keybinds[3];
        inventory4 = keybinds[4];
        inventory5 = keybinds[5];
        inventory6 = keybinds[6];
        inventory7 = keybinds[7];
        inventory8 = keybinds[8];
        inventory9 = keybinds[9];
        inventory10 = keybinds[10];
        spellMenuKey = keybinds[11];
        spellKey1 = keybinds[12];
        spellKey2 = keybinds[13];
        spellKey3 = keybinds[14];
        spellKey4 = keybinds[15];
        mainMenuKey = keybinds[16];
        pauseKey = keybinds[17];
        walkForwardKey = keybinds[18];
        walkBackwardKey = keybinds[19];
        walkLeftKey = keybinds[20];
        walkRightKey = keybinds[21];
    }
}
