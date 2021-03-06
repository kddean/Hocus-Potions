﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IPointerDownHandler {
    Player player;
    ResourceLoader rl;
    NPCController controller;
    GameObject effects;
    string characterName;
    Dictionary<string, List<string>> dialogue;
    List<Request> requests;
    NPCController.NPCInfo info;
    public List<Vector3> path;
    public Vector3 nextTarget;
    GameObject dialogueCanvas;
    CanvasGroup buttonCG, nextCG;
    string[] dialoguePieces;
    int currentDialogue;
    bool requested, responding, gavePot, done, buttonsSet, intro, allowedToMove;
    public string region;
    Animator playerAnim, effectsAnim;
    string currentAnim;
    float speed;
    GameObject swapPoint;
    bool destroying;
    public bool sceneSwapped;
    public bool saving;
    public bool closed;
    bool sleeping;
    bool scriptedQuest;
    bool madeChoice;
    bool clicked;


    public enum Status { poisoned, fast, invisible, transformed, asleep }

    public string CharacterName {
        get {
            return characterName;
        }

        set {
            characterName = value;
        }
    }

    public Dictionary<string, List<string>> Dialogue {
        get {
            return dialogue;
        }

        set {
            dialogue = value;
        }
    }

    private void Awake() {
        path = new List<Vector3>();
    }
    private void Start() {
        controller = GameObject.FindObjectOfType<NPCController>();
        player = GameObject.FindObjectOfType<Player>();
        rl = GameObject.FindObjectOfType<ResourceLoader>();
        effects = gameObject.transform.Find("effects").gameObject;
        dialogue = new Dictionary<string, List<string>>();
        dialogueCanvas = Resources.FindObjectsOfTypeAll<DialogueCanvas>()[0].gameObject;
        buttonCG = dialogueCanvas.GetComponentsInChildren<CanvasGroup>()[1];
        nextCG = dialogueCanvas.GetComponentsInChildren<CanvasGroup>()[0];
        playerAnim = GetComponent<Animator>();
        effectsAnim = effects.GetComponent<Animator>();
        sceneSwapped = false;
        requested = false;
        responding = false;
        gavePot = false;
        done = false;
        buttonsSet = false;
        intro = false;
        allowedToMove = true;
        closed = false;
        sleeping = false;
        scriptedQuest = false;
        madeChoice = false;
        clicked = false;
        dialoguePieces = new string[0];
        currentAnim = "Forward";
        if (!controller.npcData.TryGetValue(CharacterName, out info)) {
            Debug.Log("NPC Data not set");
        }
        speed = 4f;
        swapPoint = GameObject.Find("SwapPoint");
        destroying = false;
        if (SceneManager.GetActiveScene().name.Equals("House")) {
            info.map = 0;
        } else {
            info.map = 1;
        }
        info.spawned = true;
        RestartTimers();
    }

    private void Update() {
        if (saving) {
            if (path.Count > 0) {
                info.pathEnd = controller.ConvertToVec(path[path.Count - 1]);
            } else {
                info.pathEnd = new NPCController.Vec3(-9999,-9999,-9999);
            }
            info.nextTarget = controller.ConvertToVec(nextTarget);
            info.x = transform.position.x;
            info.y = transform.position.y;
            info.z = transform.position.z;
            controller.npcData[characterName] = info;
        }
        if (destroying) { return; }
        if (sceneSwapped && nextTarget.x > -9000) {
            info.x = transform.position.x;
            info.y = transform.position.y;
            info.z = transform.position.z;

            controller.npcData[characterName] = info;
            controller.FinishMoveAndSpawn(path, transform.position, characterName, nextTarget);
            destroying = true;
        }

        if (sceneSwapped && nextTarget.x < -9000) {
            info.x = transform.position.x;
            info.y = transform.position.y;
            info.z = transform.position.z;
            controller.npcData[characterName] = info;
            if (path.Count > 0) {
                controller.FinishPathData(path, CharacterName);
            }
            destroying = true;
        }

        if (destroying) { return; }


        if (Monitor.TryEnter(path, 1)) {
            if (playerAnim != null) {
                playerAnim.speed = speed / 7f;
                if (path.Count == 0) {
                    playerAnim.SetBool(currentAnim, false);
                }
            }
            if (!allowedToMove) {
                playerAnim.SetBool(currentAnim, false);
                currentAnim = "Forward";
            }
            if (path.Count > 0 && allowedToMove) {
                NPC[] others = GameObject.FindObjectsOfType<NPC>();
                foreach(NPC n in others) {
                    if(n == this) { continue; }
                    Vector3 otherPos = new Vector3(Mathf.Sign(n.transform.position.x) * (Mathf.Abs((int)n.transform.position.x) + 0.5f), Mathf.Sign(n.transform.position.y) * (Mathf.Abs((int)n.transform.position.y) + 0.5f), 0);
                    if(path[0] == otherPos) {
                        if (n.path.Count > 0) {
                            Vector3 temp = new Vector3(Mathf.Sign(transform.position.x) * (Mathf.Abs((int)transform.position.x) + 0.5f), Mathf.Sign(transform.position.y) * (Mathf.Abs((int)transform.position.y) + 0.5f), 0);
                            if(n.path[0] != temp) {
                                playerAnim.SetBool(currentAnim, false);
                                currentAnim = "Forward";
                                return;
                            } 
                        }
                    }
                }

                //Adjusting colliders
                if (playerAnim.GetBool("Transform")) {
                    GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x / 1.5f, GetComponent<SpriteRenderer>().bounds.size.y / 12);
                    switch (characterName) {
                        case "Amara":
                            GetComponent<BoxCollider2D>().offset = new Vector2(0.02f, 0.1f);
                            break;
                        case "Bernadette":                        
                            GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.08f);
                            break;
                        case "Black_Robed_Traveler":
                            GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.05f);
                            break;
                        case "Franklin":
                            GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.1f);
                            break;
                        case "Dante":
                            GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.15f);
                            break;
                        case "Geoff":
                            GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.1f);
                            break;
                        case "Ralphie":
                            GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.1f);
                            break;
                        case "Red_Robed_Traveler":
                            GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.05f);
                            break;
                        case "White_Robed_Travler":
                            GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.05f);
                            break;
                        default:
                            Debug.Log("Invalid character name: " + characterName);
                            break;
                    } 
                } else {
                    GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x * 1.5f, GetComponent<SpriteRenderer>().bounds.size.y / 2);
                    GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.25f);
                }

                GetComponents<BoxCollider2D>()[1].size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, GetComponent<SpriteRenderer>().bounds.size.y);
                GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 2);

                //Movement
                if (playerAnim != null && playerAnim.enabled) {
                    if (transform.position.x < path[0].x) {
                        playerAnim.SetBool(currentAnim, false);
                        currentAnim = "Right";
                        playerAnim.SetBool(currentAnim, true);
                    } else if (transform.position.x > path[0].x) {
                        playerAnim.SetBool(currentAnim, false);
                        currentAnim = "Left";
                        playerAnim.SetBool(currentAnim, true);

                    } else if (transform.position.y > path[0].y) {
                        playerAnim.SetBool(currentAnim, false);
                        currentAnim = "Forward";
                        playerAnim.SetBool(currentAnim, true);

                    } else if (transform.position.y < path[0].y) {
                        playerAnim.SetBool(currentAnim, false);
                        currentAnim = "Backward";
                        playerAnim.SetBool(currentAnim, true);
                    } 
                }
                transform.position = Vector3.MoveTowards(transform.position, path[0], Time.timeScale * Time.deltaTime * speed);
                if (transform.position == path[0]) {
                    path.RemoveAt(0);
                }
            } else if (speed != 0 && path.Count == 0 && info.map == 1 && transform.position == GameObject.Find("NPCSpawnPoint").transform.position) {
                info.x = transform.position.x;
                info.y = transform.position.y;
                info.z = transform.position.z;
                info.spawned = false;
                controller.npcData[characterName] = info;
                Destroy(this.gameObject);
            }

            if (speed != 0 && path.Count == 0 && nextTarget.x > -9000 && transform.position == swapPoint.transform.position && allowedToMove) {
                if (info.map == 0) {
                    GameObject.FindObjectOfType<Pathfinding>().InitializePath(new Vector3(-7.5f, -1.5f, 0), nextTarget, 1, path);
                    info.map = 1;
                    info.x = -7.5f;
                    info.y = -1.5f;
                    info.z = 0;
                    info.spawned = true;
                    controller.npcData[characterName] = info;
                } else {
                    GameObject.FindObjectOfType<Pathfinding>().InitializePath(new Vector3(0.5f, -4.5f, 0), nextTarget, 0, path);
                    info.map = 0;
                    info.x = 0.5f;
                    info.y = -4.5f;
                    info.z = 0;
                    info.spawned = true;
                    controller.npcData[characterName] = info;
                }
                controller.FinishPathData(path, characterName);
                if (dialogueCanvas.GetComponent<DialogueCanvas>().active && dialogueCanvas.GetComponent<DialogueCanvas>().user.Equals(characterName)) {
                    ExitButton();
                }
                if (!info.returning) {
                    info.requestKey = "";
                    controller.npcData[characterName] = info;
                }
                Destroy(this.gameObject);
            }
        }
        Monitor.Exit(path);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        playerAnim.SetBool(currentAnim, false);
        allowedToMove = false;
    }

    private void OnCollisionExit2D(Collision2D collision) {
        playerAnim.SetBool(currentAnim, true);
        allowedToMove = true;
    }

    public void OnMouseEnter() {
        if (!dialogueCanvas.GetComponent<DialogueCanvas>().active) {
            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Talk Mouse"), Vector2.zero, CursorMode.Auto);
        }
    }

    public void OnMouseExit() {
        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
    }
    public void OnPointerDown(PointerEventData eventData) {
        if (done || player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(transform.position, player.transform.position) > 2) { return; }
        //Left click
        if (eventData.button.Equals(PointerEventData.InputButton.Left)) {
            if (dialogueCanvas.GetComponent<DialogueCanvas>().active || clicked) { return; }
            clicked = true;
            //Dont let them dont wander off
            allowedToMove = false;
            player.allowedToMove = false;
            if (characterName.Contains("Traveler")) {
                dialogueCanvas.GetComponentsInChildren<Text>()[4].text = "???";
            } else {
                dialogueCanvas.GetComponentsInChildren<Text>()[4].text = characterName;
            }

            //Set dialogue list if it isn't already set
            if (dialogue.Count == 0) {
                dialogue = rl.dialogueList[characterName];
            }

            if (dialoguePieces.Length == 0) {
                currentDialogue = 0;
                if (info.timesInteracted < 0) {        //First interaction
                    dialoguePieces = dialogue["intro"][0].Split('*');
                    intro = true;
                    info.timesInteracted++;
                } else if (info.map == 1) {             //Overworld 
                    intro = false;
                    string key = GenerateKey();
                    if (key == null) {
                        allowedToMove = true;
                        player.allowedToMove = true;
                        clicked = false;
                        return;
                    }
                    List<string> initial;
                    try {
                        initial = dialogue[key];
                    } catch (KeyNotFoundException) {
                        allowedToMove = true;
                        player.allowedToMove = true;
                        clicked = false;
                        return;
                    }
                    List<string> options = new List<string>();
                    foreach (string s in initial) {
                        if(s.Length > 5) {
                            options.Add(s);
                        }
                    }
                    if (options.Count == 1) {
                        dialoguePieces = options[0].Split('*');
                    } else {
                        int rand = Random.Range(0, options.Count);
                        dialoguePieces = options[rand].Split('*');
                    }
                } else {        //House
                    if (info.returning) {
                        dialoguePieces = dialogue[info.requestKey][0].Split('*');
                        currentDialogue = dialoguePieces.Length - 1;
                    }

                    if (!rl.requestList.TryGetValue(characterName, out requests)) {
                        allowedToMove = true;
                        player.allowedToMove = true;
                        clicked = false;
                        return;
                    } else if (info.requestKey != null && !info.requestKey.Equals("")) {
                        dialoguePieces = dialogue[info.requestKey][0].Split('*');
                        currentDialogue = dialoguePieces.Length - 1;
                        requested = true;
                        madeChoice = true;
                    } else if (info.shouldGiveHint) {
                        GiveHint();
                    } else {
                        GiveQuest();
                    }
                }
                controller.npcData[characterName] = info;
            } 

            dialogueCanvas.SetActive(true);
            dialogueCanvas.GetComponent<DialogueCanvas>().active = true;
            dialogueCanvas.GetComponent<DialogueCanvas>().user = characterName;
            dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
            if (!buttonsSet) {
                if (info.map == 0) {
                    dialogueCanvas.GetComponentsInChildren<Button>()[0].onClick.AddListener(NextDialogueInside);
                    dialogueCanvas.GetComponentsInChildren<Button>()[2].onClick.AddListener(AcceptButton);
                    dialogueCanvas.GetComponentsInChildren<Button>()[3].onClick.AddListener(WaitButton);
                    dialogueCanvas.GetComponentsInChildren<Button>()[4].onClick.AddListener(DeclineButton);
                } else {
                    dialogueCanvas.GetComponentsInChildren<Button>()[0].onClick.AddListener(NextDialogueOutside);
                }

                dialogueCanvas.GetComponentsInChildren<Button>()[1].onClick.AddListener(ExitButton);
                buttonsSet = true;
            }

            if (!info.returning && !intro && currentDialogue == dialoguePieces.Length - 1 && (!requested || madeChoice)) {
                nextCG.alpha = 0;
                nextCG.interactable = false;
                nextCG.blocksRaycasts = false;
            }
        } else {        //Right click
            if (rl.activeItem != null) {
                if (rl.activeItem.item.item is Potion) {
                    GivePotion(rl.activeItem);
                }
            }
        }
    }

    public void UpdateRegion(string regionName) {
        region = regionName;
        if(dialoguePieces.Length != 0) {
            string key = GenerateKey();
            if (key == null) {
                allowedToMove = true;
                player.allowedToMove = true;
                clicked = false;
                return;
            }
            List<string> initial;
            try {
                initial = dialogue[key];
            } catch (KeyNotFoundException) {
                allowedToMove = true;
                player.allowedToMove = true;
                clicked = false;
                return;
            }
            List<string> options = new List<string>();
            foreach (string s in initial) {
                if (s.Length > 3) {
                    options.Add(s);
                }
            }
            if (options.Count == 1) {
                dialoguePieces = options[0].Split('*');
            } else {
                int rand = Random.Range(0, options.Count);
                dialoguePieces = options[rand].Split('*');
            }
            currentDialogue = 0;
        }

    }

    private string GenerateKey() {
        if (region == null) { return null; }
        MoonCycle mc = GameObject.FindObjectOfType<MoonCycle>();
        string key = "overworld_" + mc.DayPart.ToString().ToLower();
        switch (mc.Days % 6) {
            case 0:
                key += "_waxcres_";
                break;
            case 1:
                key += "_waxhalf_";
                break;
            case 2:
                key += "_full_";
                break;
            case 3:
                key += "_wanhalf_";
                break;
            case 4:
                key += "_wancres_";
                break;
            case 5:
                key += "_new_";
                break;
            default:
                break;
        }

        key += region;


        if (info.affinity <= -3) {
            key += "_bad";
        } else if (info.affinity >= 3) {
            key += "_good";
        } else {
            key += "_neutral";
        }
        return key;
    }

    public void NextDialogueInside() {
        currentDialogue++;
        if (!intro && info.shouldGiveHint) {
            if(currentDialogue == dialoguePieces.Length - 1) {
                nextCG.alpha = 0;
                nextCG.interactable = false;
                nextCG.blocksRaycasts = false;
                info.shouldGiveHint = false;
                GameObject.FindObjectOfType<Pathfinding>().InitializePath(transform.position, new Vector3(0.5f, -4.5f, 0), 0, path);
                nextTarget = new Vector3(69.5f, -12.5f, 0);
            }
            if (currentDialogue < dialoguePieces.Length) {
                dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
            }
            return;
        }

        if (!responding && !requested && !info.returning && currentDialogue == dialoguePieces.Length) {
            intro = false;
            if (info.shouldGiveHint) {
                GiveHint();
            } else {
                GiveQuest();
            }
            return;
        }

        if ((info.returning || requested) && currentDialogue == dialoguePieces.Length) {
            currentDialogue--;
            nextCG.alpha = 0;
            nextCG.interactable = false;
            nextCG.blocksRaycasts = false;
            if (!madeChoice) {
                ShowButtons();
            }
            return;
        }

        if (responding) {
            if (currentDialogue == dialoguePieces.Length - 1) {
                nextCG.alpha = 0;
                nextCG.interactable = false;
                nextCG.blocksRaycasts = false;
                dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
            } else if (currentDialogue == dialoguePieces.Length - 1) {
                done = true;
                nextCG.alpha = 0;
                nextCG.interactable = false;
                nextCG.blocksRaycasts = false;
            }
        }

        dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
        if (currentDialogue == dialoguePieces.Length - 1) {
            if (!rl.requestList.TryGetValue(characterName, out requests)) {
                nextCG.alpha = 0;
                nextCG.interactable = false;
                nextCG.blocksRaycasts = false;
            }
        }

    }

    public void NextDialogueOutside() {
        currentDialogue++;
        dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
        if (currentDialogue == dialoguePieces.Length - 1) {
            nextCG.alpha = 0;
            nextCG.interactable = false;
            nextCG.blocksRaycasts = false;
            if (intro) {
                dialoguePieces = new string[0];
                currentDialogue = 0;
            }
            controller.npcData[CharacterName] = info;
        }
    }

    private void GiveHint() {
        switch (characterName) {
            case "Black_Robed_Traveler":
                dialoguePieces = dialogue["request_OrderHint"][0].Split('*');
                currentDialogue = 0;
                dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
                break;
            case "Red_Robed_Traveler":
                dialoguePieces = dialogue["request_SocialHint"][0].Split('*');
                currentDialogue = 0;
                dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
                break;
            case "White_Robed_Traveler":
                dialoguePieces = dialogue["request_ChaosHint"][0].Split('*');
                currentDialogue = 0;
                dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
                break;
            default:
                info.shouldGiveHint = false;
                GiveQuest();
                break;
        }
    }

    private void GiveQuest() {
        int choice = -1;

        PopulateQuestList();
        scriptedQuest = false;
        
        switch (characterName) {
            case "Amara":
                if (controller.NPCQuestFlags["Bernadette"] && !controller.NPCQuestFlags["Amara"]) {
                    scriptedQuest = true;
                }
                break;
            case "Bernadette":
                if (!controller.NPCQuestFlags["Bernadette"] && info.timesInteracted > 3) {
                    scriptedQuest = true;
                }
                break;
            case "Dante":
                if (controller.NPCQuestFlags["Ralphie"] && !controller.NPCQuestFlags["Dante"]) {
                    scriptedQuest = true;
                }
                break;
            case "Franklin":
                if (controller.NPCQuestFlags["Amara"] && !controller.NPCQuestFlags["Franklin"]) {
                    scriptedQuest = true;
                }
                break;
            case "Ralphie":
                if (controller.NPCQuestFlags["Bernadette"] && !controller.NPCQuestFlags["Ralphie"]) {
                    scriptedQuest = true;
                }
                break;
            case "Geoff":
                if (controller.NPCQuestFlags["Amara"] && !controller.NPCQuestFlags["Geoff"]) {
                    scriptedQuest = true;
                }
                break;
            default:
                break;
        }

        if (scriptedQuest) {
            scriptedQuest = false;
            foreach (Request r in requests) {
                choice++;       
                if (r.Key.Contains(info.scriptedQuestNum.ToString())) {
                    scriptedQuest = true;
                    break;
                }
            }
        }

        string affinity = "";
        if (!scriptedQuest) {

            choice = info.availableQuests[Random.Range(0, info.availableQuests.Count - 1)];
            info.availableQuests.Remove(choice);

            if (info.affinity <= -3) {
                affinity = "_bad";
            } else if (info.affinity >= 3) {
                affinity = "_good";
            } else {
                affinity = "_neutral";
            }
        } else {
            if (info.scriptedQuestNum == 1) {
                if (info.affinity <= -3) {
                    affinity = "_bad";
                } else if (info.affinity >= 3) {
                    affinity = "_good";
                } else {
                    affinity = "_neutral";
                }
            } else {
                if (info.option) {
                    affinity = "_recall_1";
                } else {
                    affinity = "_recall_-1";
                }
            }
        }

       
        string key = requests[choice].Key + affinity;
        info.requestKey = key;
        controller.npcData[CharacterName] = info;
        dialoguePieces = Dialogue[key][0].Split('*');
        currentDialogue = 0;
        requested = true;
    }

    void PopulateQuestList() {
        info.availableQuests = new List<int>();
        for(int i = 0; i < requests.Count; i++) {
            //ignore scripted quests and finished quests
            if(!requests[i].Key.Contains("1") && !requests[i].Key.Contains("2") && !requests[i].Key.Contains("3") && !info.finishedQuests.Contains(i)) {
                //Checks for which quests they can give
                if (characterName.Equals("Bernadette")) {
                    if (controller.npcData["Bernadette"].scriptedQuestNum == 1) {
                        if (requests[i].Key.Contains("birds")) {
                            continue;
                        }
                    } else {
                        if (requests[i].Key.Contains("hide") || requests[i].Key.Contains("skinny") || requests[i].Key.Contains("ring")) {
                            continue;
                        }
                    }
                } else if (characterName.Equals("Amara")) {
                    if (controller.npcData["Bernadette"].scriptedQuestNum > 1) {
                        if (requests[i].Key.Contains("chill")) {
                            if (!info.finishedQuests.Contains(i)) {
                                info.finishedQuests.Add(i);
                            }
                            continue;
                        }
                    } else if(!controller.NPCQuestFlags["Bernadette"]){
                        if (requests[i].Key.Contains("present")) {
                            continue;
                        }
                    }
                } else if (characterName.Equals("Geoff")) {
                    if (controller.npcData["Bernadette"].scriptedQuestNum > 1) {
                        if (requests[i].Key.Contains("pre")) {
                            if (!info.finishedQuests.Contains(i)) {
                                info.finishedQuests.Add(i);
                            }
                            continue;
                        }
                    } else {
                        if (requests[i].Key.Contains("deal") || requests[i].Key.Contains("organize")) {
                            continue;
                        }
                    }
                } else if (characterName.Equals("Franklin")) {
                    if (controller.npcData["Bernadette"].scriptedQuestNum > 1) {
                        if (requests[i].Key.Contains("tired")) {
                            if (!info.finishedQuests.Contains(i)) {
                                info.finishedQuests.Add(i);
                            }
                            continue;
                        }
                    }
                } 

                info.availableQuests.Add(i);
            }
        }
    }

    public void GivePotion(InventorySlot slot) {
        if (!requested || gavePot) { return; }
        allowedToMove = false;
        player.allowedToMove = false;
        Potion temp = slot.item.item as Potion;
        if (slot.item.item.name.Contains("dye") || slot.item.item.name.Contains("of")) {
            currentDialogue = 0;
            dialogueCanvas.SetActive(true);
            dialogueCanvas.GetComponent<DialogueCanvas>().user = characterName;
            dialogueCanvas.GetComponentInChildren<Text>().text = Dialogue["default"][0];
            if (!buttonsSet) {
                if (info.map == 0) {
                    dialogueCanvas.GetComponentsInChildren<Button>()[0].onClick.AddListener(NextDialogueInside);
                    dialogueCanvas.GetComponentsInChildren<Button>()[2].onClick.AddListener(AcceptButton);
                    dialogueCanvas.GetComponentsInChildren<Button>()[3].onClick.AddListener(WaitButton);
                    dialogueCanvas.GetComponentsInChildren<Button>()[4].onClick.AddListener(DeclineButton);
                } else {
                    dialogueCanvas.GetComponentsInChildren<Button>()[0].onClick.AddListener(NextDialogueOutside);
                }

                dialogueCanvas.GetComponentsInChildren<Button>()[1].onClick.AddListener(ExitButton);
                buttonsSet = true;
            }

            nextCG.alpha = 0;
            nextCG.interactable = false;
            nextCG.blocksRaycasts = false;
            return;
        }

        if(temp.Primary == Ingredient.Attributes.none || temp.Primary == null) {
            if (Random.Range(0, 1.0f) < 0.5f) {
                int rand = Random.Range(0, 7);
                switch (rand) {
                    case 0:
                        temp = new Potion("Healing Potion", "Potions/potions_healing", 10, Ingredient.Attributes.healing, null, null, 0);
                        break;
                    case 1:
                        temp = new Potion("Sleep Potion", "Potions/potions_sleep", 40, Ingredient.Attributes.sleep, null, null, 0);
                        break;
                    case 2:
                        temp = new Potion("Invisibility Potion", "Potions/potions_invisibility", 25, Ingredient.Attributes.invisibility, null, null, 0);
                        break;
                    case 3:
                        temp = new Potion("Poison Potion", "Potions/potions_poison", 25, Ingredient.Attributes.poison, null, null, 0);
                        break;
                    case 4:
                        temp = new Potion("Transformation Potion", "Potions/potions_transform", 25, Ingredient.Attributes.transformation, null, null, 0);
                        break;
                    case 5:
                        temp = new Potion("Mana Potion", "Potions/potions_mana", 10, Ingredient.Attributes.mana, null, null, 0);
                        break;
                    case 6:
                        temp = new Potion("Speed Potion", "Potions/potions_speed", 25, Ingredient.Attributes.speed, null, null, 0);
                        break;
                    default:
                        break;
                }
            }
        }

        info.returning = false;
        requested = false;
        done = true;
        gavePot = true;

        if (info.given.Count < 5) {
            info.given.Add(temp);
        } else {
            info.given.RemoveAt(0);
            info.given.Add(temp);
        }
        controller.npcData[characterName] = info;
        Inventory.RemoveItem(slot);

        //Handle VFX and sprite swaps
        closed = false;
        StartCoroutine(PotionEffects(temp));

        string affinity;
        if (!scriptedQuest) {
            if (info.affinity <= -3) {
                affinity = "_bad";
            } else if (info.affinity >= 3) {
                affinity = "_good";
            } else {
                affinity = "_neutral";
            }
        } else {
            affinity = "";
        }

        string response;
        string[] keyBits = info.requestKey.Split('_');
        string rKey = keyBits[0] + "_" + keyBits[1];
        if (temp.Primary == null || temp.Primary == Ingredient.Attributes.none) {
            response = rKey + "_null" + affinity;
        } else {
            response = rKey + "_" + temp.Primary.ToString() + affinity;
        }
        List<string> initial;
        currentDialogue = 0;
        if (Dialogue.TryGetValue(response, out initial)) {
            dialogueCanvas.SetActive(true);
            dialogueCanvas.GetComponent<DialogueCanvas>().user = characterName;
            if (!buttonsSet) {
                if (info.map == 0) {
                    dialogueCanvas.GetComponentsInChildren<Button>()[0].onClick.AddListener(NextDialogueInside);
                    dialogueCanvas.GetComponentsInChildren<Button>()[2].onClick.AddListener(AcceptButton);
                    dialogueCanvas.GetComponentsInChildren<Button>()[3].onClick.AddListener(WaitButton);
                    dialogueCanvas.GetComponentsInChildren<Button>()[4].onClick.AddListener(DeclineButton);
                } else {
                    dialogueCanvas.GetComponentsInChildren<Button>()[0].onClick.AddListener(NextDialogueOutside);
                }

                dialogueCanvas.GetComponentsInChildren<Button>()[1].onClick.AddListener(ExitButton);
                buttonsSet = true;
            }
            responding = true;
            List<string> dia = new List<string>();
            foreach (string s in initial) {
                if (s.Length > 2) {
                    dia.Add(s);
                }
            }
            if (dia.Count > 1) {
                int i = Random.Range(0, dia.Count - 1);
                dialoguePieces = dia[i].Split('*');
                dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[0];
            } else {
                dialoguePieces = dia[0].Split('*');
                dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[0];
            }
            if (dialoguePieces.Length == 1) {
                nextCG.alpha = 0;
                nextCG.interactable = false;
                nextCG.blocksRaycasts = false;
            }
        } else {
            dialogueCanvas.GetComponentInChildren<Text>().text = Dialogue["default"][0];
        }
        string type;
        if (temp.Primary == null) {
            type = "none";
        } else {
            type = temp.Primary.ToString();
        }
        int index = requests.FindIndex(item => item.Key.Equals(rKey));
        if (!scriptedQuest) {
            info.affinity += (requests[index].GetValue(type) * requests[index].Strength);
        }
        info.requestKey = "";
        info.timesInteracted++;
        if (scriptedQuest) {
            info.scriptedQuestNum++;

            bool finishedQuestline = true;
            int i = -1;
            foreach (Request r in requests) {
                i++;
                if (r.Key.Contains(info.scriptedQuestNum.ToString())) {
                    finishedQuestline = false;
                    break;
                }
            }

            if(requests[index].GetValue(type) < 0) {
                info.option = false;
            } else {
                info.option = true;
            }

            if (finishedQuestline) {
                controller.NPCQuestFlags[characterName] = true;
                info.affinity += (requests[index].GetValue(type) * requests[index].Strength);
                if (characterName.Equals("Ralphie")) {
                    controller.LakePotion(temp.Primary);
                }
            }
        }
        info.finishedQuests.Add(index);
        info.percentCompleted = (float)info.finishedQuests.Count / (float)requests.Count;
        controller.npcData[CharacterName] = info;
        GameObject.FindObjectOfType<Pathfinding>().InitializePath(transform.position, new Vector3(0.5f, -4.5f, 0), 0, path);
        nextTarget = new Vector3(69.5f, -12.5f, 0);


        int neg = 0;
        int pos = 0;
        foreach (NPCController.NPCInfo data in controller.npcData.Values) {
            if (data.affinity <= -3) {
                neg++;
            } else if (data.affinity >= 3) {
                pos++;
            }
        }

        if (GameObject.FindObjectOfType<SteamAchievementManager>() != null) {
            if (characterName.Equals("Black_Robed_Traveler") && info.affinity >= 3) {
                SteamAchievementManager sam = GameObject.FindObjectOfType<SteamAchievementManager>();
                sam.UnlockAchievement(sam.m_Achievements[11]);
            } else if (characterName.Equals("White_Robed_Traveler") && info.affinity >= 3) {
                SteamAchievementManager sam = GameObject.FindObjectOfType<SteamAchievementManager>();
                sam.UnlockAchievement(sam.m_Achievements[12]);
            } else if (characterName.Equals("Red_Robed_Traveler") && info.affinity >= 3) {
                SteamAchievementManager sam = GameObject.FindObjectOfType<SteamAchievementManager>();
                sam.UnlockAchievement(sam.m_Achievements[13]);
            }

            if (neg == controller.npcData.Count()) {
                SteamAchievementManager sam = GameObject.FindObjectOfType<SteamAchievementManager>();
                sam.UnlockAchievement(sam.m_Achievements[10]);
            } else if (pos == controller.npcData.Count()) {
                SteamAchievementManager sam = GameObject.FindObjectOfType<SteamAchievementManager>();
                sam.UnlockAchievement(sam.m_Achievements[9]);
            }
        }
        
    }


    void SwapVisibile(CanvasGroup cg) {
        cg.alpha = Mathf.Abs(cg.alpha - 1);
        cg.interactable = !cg.interactable;
        cg.blocksRaycasts = !cg.blocksRaycasts;
    }

    void ShowButtons() {
        dialogueCanvas.GetComponentInChildren<Text>().enabled = false;
        buttonCG.alpha = 1;
        buttonCG.interactable = true;
        buttonCG.blocksRaycasts = true;
        if (info.returning) {
            GameObject.Find("Wait").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Wait").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("Wait").GetComponent<CanvasGroup>().blocksRaycasts = false;
        } else {
            GameObject.Find("Wait").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("Wait").GetComponent<CanvasGroup>().interactable = true;
            GameObject.Find("Wait").GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

    }

    void HideButtons() {
        dialogueCanvas.GetComponentInChildren<Text>().enabled = true;
        buttonCG.alpha = 0;
        buttonCG.interactable = false;
        buttonCG.blocksRaycasts = false;
        nextCG.alpha = 0;
        nextCG.interactable = false;
        nextCG.blocksRaycasts = false;
    }

    public void AcceptButton() {
        ExitButton();
        info.returning = false;
        controller.npcData[CharacterName] = info;
        List<string> queueList = controller.npcQueue.Values.ToList();
        int index = queueList.FindIndex(item => item.Equals(characterName));
        if (index >= 0) {
            controller.npcQueue.RemoveAt(index);
        } else {
            path = new List<Vector3>();
        }
        if(path.Count > 0 && path[path.Count - 1] == new Vector3(0.5f, -4.5f, 0)) {
            path = new List<Vector3>();
        }
        MoonCycle mc = GameObject.FindObjectOfType<MoonCycle>();
        NPCController.Schedule s;
        if ((mc.hour + 4) < 24) {
             s = new NPCController.Schedule(false, mc.Days, mc.Hour + 4, mc.Minutes, "", 1, 69.5f, -12.5f, 0, characterName);
        } else {
             s = new NPCController.Schedule(false, mc.Days + 1, (mc.Hour + 4) % 24, mc.Minutes, "", 1, 69.5f, -12.5f, 0, characterName);
        }
        controller.npcQueue.Add(s, characterName);
        allowedToMove = true;
        player.allowedToMove = true;
        clicked = false;
        madeChoice = true;
    }

    public void WaitButton() {
        info.returning = true;
        MoonCycle mc = GameObject.FindObjectOfType<MoonCycle>();
        List<NPCController.Schedule> temp = info.locations;      
        temp.Add(new NPCController.Schedule(false, mc.Days + 1, mc.Hour, mc.Minutes, "", 0, -6.5f, 0.5f, 0, characterName));
        controller.npcData[CharacterName] = info;
        dialogueCanvas.GetComponentInChildren<Text>().enabled = true;
        dialogueCanvas.GetComponentInChildren<Text>().text = Dialogue["wait"][0];
        HideButtons();
        GameObject.FindObjectOfType<Pathfinding>().InitializePath(transform.position, new Vector3(0.5f, -4.5f, 0), 0, path);
        nextTarget = new Vector3(69.5f, -12.5f, 0);
        done = true;
        madeChoice = true;
    }

    public void DeclineButton() {
        HideButtons();
        dialogueCanvas.GetComponentInChildren<Text>().enabled = true;
        dialogueCanvas.GetComponentInChildren<Text>().text = Dialogue["no"][0];
        info.returning = false;
        info.requestKey = "";
        info.timesInteracted++;
        controller.npcData[CharacterName] = info;
        GameObject.FindObjectOfType<Pathfinding>().InitializePath(transform.position, new Vector3(0.5f, -4.5f, 0), 0, path);
        nextTarget = new Vector3(69.5f, -12.5f, 0);
        done = true;
        madeChoice = true;
    }

    public void ExitButton() {
        dialogueCanvas.GetComponentInChildren<Text>().enabled = true;
        buttonCG.alpha = 0;
        buttonCG.interactable = false;
        buttonCG.blocksRaycasts = false;
        nextCG.interactable = true;
        nextCG.blocksRaycasts = true;
        nextCG.alpha = 1.0f;
        dialogueCanvas.GetComponent<DialogueCanvas>().active = false;
        dialogueCanvas.SetActive(false);
        dialogueCanvas.GetComponent<DialogueCanvas>().user = "";
        player.allowedToMove = true;
        clicked = false;
        dialogueCanvas.GetComponentsInChildren<Button>()[0].onClick.RemoveAllListeners();
        dialogueCanvas.GetComponentsInChildren<Button>()[2].onClick.RemoveAllListeners();
        dialogueCanvas.GetComponentsInChildren<Button>()[3].onClick.RemoveAllListeners();
        dialogueCanvas.GetComponentsInChildren<Button>()[4].onClick.RemoveAllListeners();
        buttonsSet = false;
        closed = true;
        if (sleeping) {
            playerAnim.SetBool("Sleep", true);
        } else {
            allowedToMove = true;
        }
    }

    IEnumerator PotionEffects(Potion pot) {
        effects.SetActive(true);
        Ingredient.Attributes? type = pot.Primary;

        switch (type) {
            case Ingredient.Attributes.healing:
                effectsAnim.SetBool("Healing", true);
                break;
            case Ingredient.Attributes.invisibility:
                info.state.Add(Status.invisible);
                effectsAnim.SetBool("Invisible", true);
                effectsAnim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.83f);
                effectsAnim.SetBool("Invisible", false);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.25f;
                GetComponent<SpriteRenderer>().color = c;
                info.potionTimers.Add(Status.invisible, new NPCController.TimerData(Time.time, pot.Duration));
                break;
            case Ingredient.Attributes.mana:
                effectsAnim.SetBool("Mana", true);
                break;
            case Ingredient.Attributes.poison:
                speed--;
                info.state.Add(Status.poisoned);
                info.potionTimers.Add(Status.poisoned, new NPCController.TimerData(Time.time, pot.Duration));
                effectsAnim.SetBool("Poison", true);
                break;
            case Ingredient.Attributes.sleep:
                info.state.Add(Status.asleep);
                info.potionTimers.Add(Status.asleep, new NPCController.TimerData(Time.time, pot.Duration));
                effectsAnim.SetBool("Sleep", true);
                sleeping = true;
                allowedToMove = false;
                break;
            case Ingredient.Attributes.speed:
                info.state.Add(Status.fast);
                info.potionTimers.Add(Status.fast, new NPCController.TimerData(Time.time, pot.Duration));
                speed = 8;
                effectsAnim.SetBool("Speed", true);
                break;
            case Ingredient.Attributes.transformation:
                effectsAnim.SetBool("Transformation", true);
                effectsAnim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                info.state.Add(Status.transformed);
                info.potionTimers.Add(Status.transformed, new NPCController.TimerData(Time.time, pot.Duration));
                speed++;
                playerAnim.SetBool("Transform", true);
                effectsAnim.SetBool("Transformation", false);
                break;
            case Ingredient.Attributes.none:
                break;
            default:
                break;
        }
        controller.npcData[characterName] = info;
        while (closed == false) {
            yield return null;
        }

        yield return new WaitForSeconds((pot.Duration / 10) * GameObject.FindObjectOfType<MoonCycle>().CLOCK_SPEED);

        switch (type) {
            case Ingredient.Attributes.healing:
                effectsAnim.SetBool("Healing", false);
                break;
            case Ingredient.Attributes.invisibility:
                info.state.Remove(Status.invisible);
                info.potionTimers.Remove(Status.invisible);
                effectsAnim.SetBool("Invisible", true);
                effectsAnim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.83f);
                effectsAnim.SetBool("Invisible", false);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.25f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case Ingredient.Attributes.mana:
                effectsAnim.SetBool("Mana", false);
                break;
            case Ingredient.Attributes.none:
                break;
            case Ingredient.Attributes.poison:
                speed++;
                info.state.Remove(Status.poisoned);
                info.potionTimers.Remove(Status.poisoned);
                effectsAnim.SetBool("Poison", false);
                break;
            case Ingredient.Attributes.sleep:
                info.state.Remove(Status.asleep);
                info.potionTimers.Remove(Status.asleep);
                effectsAnim.SetBool("Sleep", false);
                playerAnim.SetBool("Sleep", false);
                yield return new WaitForSeconds(0.33f);
                sleeping = false;
                allowedToMove = true;
                break;
            case Ingredient.Attributes.speed:
                info.state.Remove(Status.fast);
                info.potionTimers.Remove(Status.fast);
                speed = 4;
                effectsAnim.SetBool("Speed", false);
                break;
            case Ingredient.Attributes.transformation:
                info.state.Remove(Status.transformed);
                info.potionTimers.Remove(Status.transformed);
                speed--;
                effectsAnim.SetBool("Transformation", true);
                effectsAnim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                playerAnim.SetBool("Transform", false);
                effectsAnim.SetBool("Transformation", false);
                break;
            default:
                break;
        }
        controller.npcData[characterName] = info;
        effects.SetActive(false);
    }

    public void RestartTimers() {
        info = 
            controller.npcData[characterName];
        if (info.state.Count > 0) {
            foreach (Status s in info.state) {
                StartCoroutine(RestartPotion(s, info.potionTimers[s]));
            }
        }
    }

    IEnumerator RestartPotion(Status s, NPCController.TimerData t) {
        effects.SetActive(true);
        switch (s) {
            case Status.invisible:
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.25f;
                GetComponent<SpriteRenderer>().color = c;
                break;
                case Status.poisoned:
                speed--;
                effectsAnim.SetBool("Poison", true);
                break;
            case Status.asleep:
                effectsAnim.SetBool("Sleep", true);
                playerAnim.SetBool("Sleep", true);
                allowedToMove = false;
                break;
            case Status.fast:
                speed = 8;
                effectsAnim.SetBool("Speed", true);
                break;
            case Status.transformed:
                speed++;
                playerAnim.SetBool("Transform", true);
                break;
            default:
                break;
        }

        controller.npcData[characterName] = info;
        float duration = ((t.duration / 10) * GameObject.FindObjectOfType<MoonCycle>().CLOCK_SPEED) - (Time.time - t.startTime);
        yield return new WaitForSeconds(duration);

        switch (s) {
            case Status.invisible:
                info.state.Remove(Status.invisible);
                info.potionTimers.Remove(Status.invisible);
                effectsAnim.SetBool("Invisible", true);
                effectsAnim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.83f);
                effectsAnim.SetBool("Invisible", false);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.25f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case Status.poisoned:
                speed++;
                info.state.Remove(Status.poisoned);
                info.potionTimers.Remove(Status.poisoned);
                effectsAnim.SetBool("Poison", false);
                break;
            case Status.asleep:
                info.state.Remove(Status.asleep);
                info.potionTimers.Remove(Status.asleep);
                effectsAnim.SetBool("Sleep", false);
                playerAnim.SetBool("Sleep", false);
                yield return new WaitForSeconds(0.33f);
                allowedToMove = true;
                break;
            case Status.fast:
                info.state.Remove(Status.fast);
                info.potionTimers.Remove(Status.fast);
                speed = 4;
                effectsAnim.SetBool("Speed", false);
                break;
            case Status.transformed:
                info.state.Remove(Status.transformed);
                info.potionTimers.Remove(Status.transformed);
                speed--;
                effectsAnim.SetBool("Transformation", true);
                effectsAnim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                playerAnim.SetBool("Transform", false);
                effectsAnim.SetBool("Transformation", false);
                break;
            default:
                break;
        }
        controller.npcData[characterName] = info;
        effects.SetActive(false);
    }
}
