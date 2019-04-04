using System.Collections;
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
                if (dialogueCanvas.GetComponent<DialogueCanvas>().active) {
                    ExitButton();
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
            if (dialogueCanvas.GetComponent<DialogueCanvas>().active) { return; }
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
                        return;
                    }
                    List<string> initial;
                    try {
                        initial = dialogue[key];
                    } catch (KeyNotFoundException) {
                        allowedToMove = true;
                        player.allowedToMove = true;
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
                        return;
                    } else {
                        GiveQuest();
                        return;
                    }
                }
                controller.npcData[characterName] = info;
            }

            dialogueCanvas.SetActive(true);
            dialogueCanvas.GetComponent<DialogueCanvas>().active = true;
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

            if (!info.returning && !requested && currentDialogue == dialoguePieces.Length - 1) {
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

    private string GenerateKey() {
        //TODO: remove this once we add dialogue while they're walking to a place
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


        if (info.affinity < 0) {
            key += "_bad";
        } else if (info.affinity > 0) {
            key += "_good";
        } else {
            key += "_neutral";
        }
        return key;
    }

    public void NextDialogueInside() {
        currentDialogue++;
        if (!responding && !requested && !info.returning && currentDialogue == dialoguePieces.Length) {
            GiveQuest();
            return;
        }

        if ((info.returning || requested) && currentDialogue == dialoguePieces.Length) {
            nextCG.alpha = 0;
            nextCG.interactable = false;
            nextCG.blocksRaycasts = false;
            ShowButtons();
            return;
        }

        if (responding) {
            if (currentDialogue == dialoguePieces.Length - 2) {
                nextCG.alpha = 0;
                nextCG.interactable = false;
                nextCG.blocksRaycasts = false;
                dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
            } else if (currentDialogue == dialoguePieces.Length - 1) {
                done = true;
                nextCG.GetComponent<CanvasGroup>().alpha = 0;
                nextCG.GetComponent<CanvasGroup>().interactable = false;
                nextCG.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }

        dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
        if (currentDialogue == dialoguePieces.Length - 1) {
            if (!rl.requestList.TryGetValue(characterName, out requests)) {
                nextCG.GetComponent<CanvasGroup>().alpha = 0;
                nextCG.GetComponent<CanvasGroup>().interactable = false;
                nextCG.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }

    }

    public void NextDialogueOutside() {
        currentDialogue++;
        dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
        if (currentDialogue == dialoguePieces.Length - 1) {
            nextCG.GetComponent<CanvasGroup>().alpha = 0;
            nextCG.GetComponent<CanvasGroup>().interactable = false;
            nextCG.GetComponent<CanvasGroup>().blocksRaycasts = false;
            if (intro) {
                dialoguePieces = new string[0];
                currentDialogue = 0;
            }
            controller.npcData[CharacterName] = info;
        }
    }

    private void GiveQuest() {
        int choice = -1;
        if(info.givenQuests.Count == requests.Count) {
            info.givenQuests = new List<int>();
        }
        bool scriptedQuest = false;
        foreach (Request r in requests) {
            choice++;
            if (r.Key.Contains(info.timesInteracted.ToString())) {
                scriptedQuest = true;
                break;
            }
        }

        string affinity = "";
        if (!scriptedQuest) {
            do {
                choice = Random.Range(0, requests.Count - 1);
            } while (info.givenQuests.Contains(choice) || requests[choice].Key.Length < 10);
            if (info.affinity < 0) {
                affinity = "_bad";
            } else if (info.affinity > 0) {
                affinity = "_good";
            } else {
                affinity = "_neutral";
            }
        } else {
            if (info.option) {
                affinity = "_A";
            } else {
                affinity = "_B";
            }
        }

        info.givenQuests.Add(choice);

       
        string key = requests[choice].Key + affinity;
        info.requestKey = key;
        controller.npcData[CharacterName] = info;
        dialoguePieces = Dialogue[key][0].Split('*');
        currentDialogue = 0;
        dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[0];
        requested = true;
    }

    public void GivePotion(InventorySlot slot) {
        if (!requested || gavePot) { return; }
        allowedToMove = false;
        player.allowedToMove = false;
        Potion temp = slot.item.item as Potion;
        if (slot.item.item.name.Contains("dye")) {
            currentDialogue = 0;
            dialogueCanvas.SetActive(true);
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


        info.given.Add(slot.item.item);
        info.returning = false;
        requested = false;

        gavePot = true;

        if (info.given.Count < 5) {
            info.given.Add(slot.item.item);
        } else {
            info.given.RemoveAt(0);
            info.given.Add(slot.item.item);
        }
        controller.npcData[characterName] = info;
        Inventory.RemoveItem(slot);

        //Handle VFX and sprite swaps
        closed = false;
        StartCoroutine(PotionEffects(temp));

        string affinity;    //TODO: This might need to be ranges
        if (info.affinity < 0) {
            affinity = "_bad";
        } else if (info.affinity > 0) {
            affinity = "_good";
        } else {
            affinity = "_neutral";
        }

        string response;
        string[] keyBits = info.requestKey.Split('_');
        string rKey = keyBits[0] + "_" + keyBits[1];
        if (temp.Primary == null) {
            response = rKey + "_null" + affinity;
        } else {
            response = rKey + "_" + temp.Primary.ToString() + affinity;
        }
        List<string> initial;
        currentDialogue = 0;
        if (Dialogue.TryGetValue(response, out initial)) {
            dialogueCanvas.SetActive(true);
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
                if (s.Length > 5) {
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
        info.affinity += (requests[index].GetValue(type) * requests[index].Strength);
        info.requestKey = null;
        info.timesInteracted++;
        controller.npcData[CharacterName] = info;
        GameObject.FindObjectOfType<Pathfinding>().InitializePath(transform.position, new Vector3(0.5f, -4.5f, 0), 0, path);
        nextTarget = new Vector3(69.5f, -12.5f, 0);
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
        MoonCycle mc = GameObject.FindObjectOfType<MoonCycle>();
        NPCController.Schedule s = new NPCController.Schedule(false, mc.Days, mc.Hour + 4, mc.Minutes, "", 1, 69.5f, -12.5f, 0, characterName);
        controller.npcQueue.Add(s, characterName);
        allowedToMove = true;
        player.allowedToMove = true;
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
    }

    public void DeclineButton() {
        HideButtons();
        dialogueCanvas.GetComponentInChildren<Text>().enabled = true;
        dialogueCanvas.GetComponentInChildren<Text>().text = Dialogue["no"][0];
        info.returning = false;
        controller.npcData[CharacterName] = info;
        GameObject.FindObjectOfType<Pathfinding>().InitializePath(transform.position, new Vector3(0.5f, -4.5f, 0), 0, path);
        nextTarget = new Vector3(69.5f, -12.5f, 0);
        done = true;
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
        player.allowedToMove = true;
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
            case Ingredient.Attributes.none:
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
