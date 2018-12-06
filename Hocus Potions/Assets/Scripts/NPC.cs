using System.Collections;
using System.Collections.Generic;
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
    CanvasGroup buttonCG;
    string[] dialoguePieces;
    int currentDialogue;
    bool requested, responding, gavePot, done, buttonsSet, intro;
    public string region;

    float speed;
    GameObject swapPoint;
    bool destroying;
    public bool sceneSwapped;


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
        effects = GameObject.Find("effects");
        dialogue = new Dictionary<string, List<string>>();
        dialogueCanvas = Resources.FindObjectsOfTypeAll<DialogueCanvas>()[0].gameObject;
        buttonCG = dialogueCanvas.GetComponentsInChildren<CanvasGroup>()[1];
        sceneSwapped = false;
        requested = false;
        responding = false;
        gavePot = false;
        done = false;
        buttonsSet = false;
        intro = false;
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
    }

    private void Update() {
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
            if (path.Count > 0) {
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

            if (speed != 0 && path.Count == 0 && nextTarget.x > -9000 && transform.position == swapPoint.transform.position) {
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
                Destroy(this.gameObject);
            }
        }
        Monitor.Exit(path);


    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.isTrigger) { return; }
        lock (path) {
            if (path.Count > 0) {
                if (SceneManager.GetActiveScene().name.Equals("House")) {
                    Vector3 temp = path[path.Count - 1];
                    path.Clear();
                    GameObject.FindObjectOfType<Pathfinding>().InitializePath(transform.position, temp, 0, path);
                } else {
                    Vector3 temp = path[path.Count - 1];
                    path.Clear();
                    GameObject.FindObjectOfType<Pathfinding>().InitializePath(transform.position, temp, 1, path);
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (done || player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(transform.position, player.transform.position) > 2) { return; }
        //Left click
        if (eventData.button.Equals(PointerEventData.InputButton.Left)) {
            if (dialogueCanvas.GetComponent<DialogueCanvas>().active) { return; }
            //Set speed to 0 so they dont wander off
            speed = 0;

            //Set dialogue list if it isn't already set
            if (dialogue.Count == 0) {
                dialogue = rl.dialogueList[characterName];
            }

            if (dialoguePieces == null) {
                currentDialogue = 0;
                if (info.timesInteracted == 0) {        //First interaction
                    dialoguePieces = dialogue["intro"][0].Split('*');
                    intro = true;
                } else if (info.map == 1) {             //Overworld 
                    intro = false;
                    string key = GenerateKey();
                    if (key == null) {
                        speed = 4;
                        return;
                    }
                    List<string> options = dialogue[key];
                    if (options.Count == 1) {
                        Debug.Log("one option");
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
                        speed = 4;
                        return;
                    } else {
                        GiveQuest();
                        return;
                    }
                }
                info.timesInteracted++;
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
                GameObject.Find("Next").GetComponent<CanvasGroup>().alpha = 0;
                GameObject.Find("Next").GetComponent<CanvasGroup>().interactable = false;
                GameObject.Find("Next").GetComponent<CanvasGroup>().blocksRaycasts = false;
            }

        } else {        //Right click
            if (rl.activeItem != null) {
                if (rl.activeItem.item.item is Potion) {
                    if (!requested || gavePot) { return; }
                    GivePotion();
                }
            }
        }
    }

    string GenerateKey() {
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
        if (!requested && !info.returning && currentDialogue == dialoguePieces.Length) {
            GiveQuest();
            return;
        }

        if ((info.returning || requested) && currentDialogue == dialoguePieces.Length) {
            GameObject.Find("Next").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Next").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("Next").GetComponent<CanvasGroup>().blocksRaycasts = false;
            ShowButtons();
            return;
        }

        if (responding) {
            if (currentDialogue == dialoguePieces.Length - 2) {
                GameObject.Find("Next").GetComponent<CanvasGroup>().alpha = 0;
                GameObject.Find("Next").GetComponent<CanvasGroup>().interactable = false;
                GameObject.Find("Next").GetComponent<CanvasGroup>().blocksRaycasts = false;
                dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
            } else if (currentDialogue == dialoguePieces.Length - 1) {
                done = true;
            }
        }

        dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
        if (currentDialogue == dialoguePieces.Length - 1) {
            if (!rl.requestList.TryGetValue(characterName, out requests)) {
                GameObject.Find("Next").GetComponent<CanvasGroup>().alpha = 0;
                GameObject.Find("Next").GetComponent<CanvasGroup>().interactable = false;
                GameObject.Find("Next").GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }

    }

    public void NextDialogueOutside() {
        currentDialogue++;
        dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
        if (currentDialogue == dialoguePieces.Length - 1) {
            GameObject.Find("Next").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.Find("Next").GetComponent<CanvasGroup>().interactable = false;
            GameObject.Find("Next").GetComponent<CanvasGroup>().blocksRaycasts = false;
            if (intro) {
                dialoguePieces = null;
            }
        }
    }

    void GiveQuest() {
        int choice = Random.Range(0, requests.Count - 1);
        string affinity;
        if (info.affinity < 0) {
            affinity = "_bad";
        } else if (info.affinity > 0) {
            affinity = "_good";
        } else {
            affinity = "_neutral";
        }
        string key = requests[choice].Key + affinity;
        info.requestKey = key;
        controller.npcData[CharacterName] = info;
        dialoguePieces = Dialogue[key][0].Split('*');
        currentDialogue = 0;
        dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[0];
        requested = true;
    }

    void GivePotion() {
        info.given.Add(rl.activeItem.item.item);
        info.returning = false;
        requested = false;

        gavePot = true;

        if (info.given.Count < 5) {
            info.given.Add(rl.activeItem.item.item);
        } else {
            info.given.RemoveAt(0);
            info.given.Add(rl.activeItem.item.item);
        }
        controller.npcData[characterName] = info;
        Potion temp = rl.activeItem.item.item as Potion;
        Inventory.RemoveItem(rl.activeItem);

        //Handle VFX and sprite swaps
        StartCoroutine(PotionEffects(temp));

        string affinity;    //TODO: This might need to be ranges
        if (info.affinity < 0) {
            affinity = "_bad";
        } else if (info.affinity > 0) {
            affinity = "_good";
        } else {
            affinity = "_neutral";
        }

        //Choose from possible responses or use default if there is no response(this shouldn't ever happen)

        string response;
        string[] keyBits = info.requestKey.Split('_');
        string rKey = keyBits[0] + "_" + keyBits[1];
        if (temp.Primary == null) {
            response = rKey + "_null" + affinity;
        } else {
            response = rKey + "_" + temp.Primary.ToString() + affinity;
        }
        List<string> dia;
        currentDialogue = 0;
        if (Dialogue.TryGetValue(response, out dia)) {
            dialogueCanvas.SetActive(true);
            responding = true;
            if (dia.Count > 1) {
                int i = Random.Range(0, dia.Count - 1);
                dialoguePieces = dia[i].Split('*');
                dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[0];
            } else {
                dialoguePieces = dia[0].Split('*');
                dialogueCanvas.GetComponentInChildren<Text>().text = dialoguePieces[0];
            }
            if (dialoguePieces.Length == 1) {
                GameObject.Find("Next").GetComponent<CanvasGroup>().alpha = 0;
                GameObject.Find("Next").GetComponent<CanvasGroup>().interactable = false;
                GameObject.Find("Next").GetComponent<CanvasGroup>().blocksRaycasts = false;
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
        controller.npcData[CharacterName] = info;
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
        GameObject.Find("Next").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("Next").GetComponent<CanvasGroup>().interactable = false;
        GameObject.Find("Next").GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void AcceptButton() {
        ExitButton();
        info.returning = false;
        controller.npcData[CharacterName] = info;
    }

    public void WaitButton() {
        info.returning = true;
        List<NPCController.Schedule> temp = info.locations;
        MoonCycle mc = GameObject.FindObjectOfType<MoonCycle>();
        temp.Add(new NPCController.Schedule(false, mc.Days + 1, mc.Hour, mc.Minutes, "", 0, transform.position.x, transform.position.y, transform.position.z, characterName));
        controller.npcData[CharacterName] = info;
        dialogueCanvas.GetComponentInChildren<Text>().enabled = true;
        dialogueCanvas.GetComponentInChildren<Text>().text = Dialogue["wait"][0];
        HideButtons();
    }

    public void DeclineButton() {
        HideButtons();
        dialogueCanvas.GetComponentInChildren<Text>().enabled = true;
        dialogueCanvas.GetComponentInChildren<Text>().text = Dialogue["no"][0];
        info.returning = false;
        controller.npcData[CharacterName] = info;
    }

    public void ExitButton() {
        dialogueCanvas.GetComponentInChildren<Text>().enabled = true;
        buttonCG.alpha = 0;
        buttonCG.interactable = false;
        buttonCG.blocksRaycasts = false;
        GameObject.Find("Next").GetComponent<CanvasGroup>().interactable = true;
        GameObject.Find("Next").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("Next").GetComponent<CanvasGroup>().alpha = 1.0f;
        dialogueCanvas.GetComponent<DialogueCanvas>().active = false;
        dialogueCanvas.SetActive(false);
        speed = 4f;
    }
    IEnumerator PotionEffects(Potion pot) {
        effects.SetActive(true);
        Ingredient.Attributes? type = pot.Primary;
        Animator anim = GetComponentInChildren<Animator>();

        switch (type) {
            case Ingredient.Attributes.healing:
                anim.SetBool("Healing", true);
                break;
            case Ingredient.Attributes.invisibility:
                info.state.Add(Status.invisible);
                anim.SetBool("Invisible", true);
                anim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.83f);
                anim.SetBool("Invisible", false);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.25f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case Ingredient.Attributes.mana:
                anim.SetBool("Mana", true);
                break;
            case Ingredient.Attributes.none:
                break;
            case Ingredient.Attributes.poison:
                speed--;
                info.state.Add(Status.poisoned);
                anim.SetBool("Poison", true);
                break;
            case Ingredient.Attributes.sleep:
                info.state.Add(Status.asleep);
                anim.SetBool("Sleep", true);
                break;
            case Ingredient.Attributes.speed:
                info.state.Add(Status.fast);
                speed = 8;
                anim.SetBool("Speed", true);
                break;
            case Ingredient.Attributes.transformation:
                anim.SetBool("Transformation", true);
                anim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                info.state.Add(Status.transformed);
                speed++;
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Characters/cat");
                GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.15f);
                GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.075f);
                GetComponents<BoxCollider2D>()[1].size = new Vector2(0.7f, 0.6f);
                GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, 0.3f);
                anim.SetBool("Transformation", false);
                break;
            default:
                break;
        }
        controller.npcData[characterName] = info;
        yield return new WaitForSeconds((pot.Duration / 10) * GameObject.FindObjectOfType<MoonCycle>().CLOCK_SPEED);

        switch (type) {
            case Ingredient.Attributes.healing:
                anim.SetBool("Healing", false);
                break;
            case Ingredient.Attributes.invisibility:
                info.state.Remove(Status.invisible);
                anim.SetBool("Invisible", true);
                anim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.83f);
                anim.SetBool("Invisible", false);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.25f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case Ingredient.Attributes.mana:
                anim.SetBool("Mana", false);
                break;
            case Ingredient.Attributes.none:
                break;
            case Ingredient.Attributes.poison:
                speed++;
                info.state.Remove(Status.poisoned);
                anim.SetBool("Poison", false);
                break;
            case Ingredient.Attributes.sleep:
                info.state.Remove(Status.asleep);
                anim.SetBool("Sleep", false);
                break;
            case Ingredient.Attributes.speed:
                info.state.Remove(Status.fast);
                speed = 4;
                anim.SetBool("Speed", false);
                break;
            case Ingredient.Attributes.transformation:
                info.state.Remove(Status.transformed);
                speed--;
                anim.SetBool("Transformation", true);
                anim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                GetComponent<SpriteRenderer>().sprite = rl.charSpriteList[CharacterName];
                GetComponent<BoxCollider2D>().size = new Vector2(1, 0.3f);
                GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.15f);
                GetComponents<BoxCollider2D>()[1].size = new Vector2(1, 2f);
                GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, 1);
                anim.SetBool("Transformation", false);
                break;
            default:
                break;
        }
        controller.npcData[characterName] = info;
        effects.SetActive(false);
    }
}
