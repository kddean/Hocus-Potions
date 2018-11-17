using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class Traveller : NPC, IPointerDownHandler {
    //static references
    ResourceLoader rl;
    MoonCycle mc;
    NPCManager manager;
    GameObject panel;
    CanvasGroup panelCG;
    Item given;
    GameObject effects;
    NPCManager.NPCData data;
    Player player;


    //flags
    bool move;
    bool visible;
    bool wait;
    bool requested;
    bool done;
    bool asleep;
    bool responding;
    bool showButtons;
    bool chosen;

    int waitHour, waitMinute;
    int maxWait = 5;

    string[] dialoguePieces;
    int currentDialogue;
    List<Request> requests;
    int choice;
    int spawnHour;
    int spawnMinute;


    private void Start() {
        //initializations
        mc = (MoonCycle)GameObject.FindObjectOfType(typeof(MoonCycle));
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        manager = GameObject.Find("NPCManager").GetComponent<NPCManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        waitHour = mc.Hour + maxWait;
        panel = GameObject.FindGameObjectWithTag("dialogue");
        panelCG = panel.GetComponent<CanvasGroup>();
        Dialogue = rl.dialogueList[CharacterName];
        effects = GameObject.Find("effects");
        spawnHour = mc.Hour;
        spawnMinute = mc.Minutes;
        chosen = false;
       // effects.SetActive(false);
        //set flags
        visible = false;
        move = true;
        wait = true;
        requested = false;
        given = null;
        done = false;
        asleep = false;
        responding = false;
        showButtons = false;
        
        currentDialogue = 0;
        if(!manager.data.TryGetValue(CharacterName, out data)) {
            data = new NPCManager.NPCData();
            data.timesInteracted = 0;
            data.given = new List<Item>();
            data.affinity = 0;
            manager.data.Add(CharacterName, data);
        }
        data.returning = false;
        //set onclick functions for dialogue
        panel.transform.Find("Next").GetComponent<Button>().onClick.AddListener(NextDialogue);
        panel.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(ExitDialogue);
        panel.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(AcceptRequest);
        panel.transform.Find("Wait").GetComponent<Button>().onClick.AddListener(Wait);
        panel.transform.Find("No").GetComponent<Button>().onClick.AddListener(DeclineRequest);
    }

    void Update() {
        if (move && !asleep) {
            if (wait) {
                transform.position = Vector2.MoveTowards(transform.position, GameObject.Find("WaitPoint").transform.position, 0.025f);
                //TODO: Just set sorting layer once they're set up
                Vector3 temp = transform.position;
                temp.z = -1.0f;
                transform.position = temp;
            } else { 
                transform.position = Vector2.MoveTowards(transform.position, GameObject.Find("SpawnPoint").transform.position, 0.025f);
                //TODO: Removable once sorting layers are set
                Vector3 temp = transform.position;
                temp.z = -1.0f;
                transform.position = temp;

                if (transform.position.x == GameObject.Find("SpawnPoint").transform.position.x && transform.position.y == GameObject.Find("SpawnPoint").transform.position.y) {
                    Destroy(this.gameObject);
                    manager.Spawned = false;
                }
            }
        }

        if(waitHour < mc.Hour) {
            wait = false;
        }

        if(wait && mc.Hour == 22 && mc.Minutes == 0) {
            wait = false;
            data.returning = true;
            data.returningDay = (mc.Days + 1);
            data.returningHour = spawnHour;
            data.returningMinutes = spawnMinute;
            manager.data[CharacterName] = data;
            string junk;
           /* if (!manager.returnQueue.TryGetValue(data, out junk)) {       TODO: This works but it causes glitches with the time set too fast
                manager.returnQueue.Add(data, CharacterName);
            }*/
        }
   
    }

    //Set the initial dialogue when NPC is clicked
   public void OnPointerDown(PointerEventData eventData) {      
        if (done || player.Status == Player.PlayerStatus.asleep || player.Status == Player.PlayerStatus.transformed) { return; }

        if (eventData.button == PointerEventData.InputButton.Left) {
            move = false;
            if (!visible) {
                SwapVisibile(panelCG);
                if (dialoguePieces == null) {
                    dialoguePieces = Dialogue["intro"][0].Split('*');
                }
                panel.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
                if ((dialoguePieces.Length <= 1 || (currentDialogue == dialoguePieces.Length - 1)) && (requests == null || chosen)) {
                    panel.transform.Find("Next").GetComponent<CanvasGroup>().alpha = 0;
                    panel.transform.Find("Next").GetComponent<CanvasGroup>().interactable = false;
                    panel.transform.Find("Next").GetComponent<CanvasGroup>().blocksRaycasts = false;
                }

                visible = true;
            }
        } else if(eventData.button == PointerEventData.InputButton.Right) {
            if (rl.activeItem != null) {
                if (rl.activeItem.item.item is Potion) {
                    if (!requested || rl.activeItem == null || done) { return; }
                    Give();
                }
            }
        }
    }

   void Give() {
        List<object> givenObjects;
        given = rl.activeItem.item.item;
        done = true;
        move = false;
        wait = false;
        CanvasGroup nextButton = panel.transform.Find("Next").GetComponent<CanvasGroup>();
        nextButton.interactable = nextButton.blocksRaycasts = false;
        nextButton.alpha = 0;

        // if (requests[choice].Item.ToLower().Contains("potion")) {
        if (given is Potion) { //Give them the type of object they wanted
            data.timesInteracted++;
            if (data.given.Count < 5) {
                data.given.Add(given);
            } else {
                data.given.RemoveAt(0);
                data.given.Add(given);
            }
            rl.inv.RemoveItem(rl.activeItem.item);

            if (rl.npcGivenList.TryGetValue(CharacterName, out givenObjects)) { //add the item to the list of stuff you've given them before
                if (givenObjects.Count == rl.givenListMax) {
                    givenObjects.RemoveAt(0);
                    givenObjects.Add(given);
                } else {
                    givenObjects.Add(given);
                }
            } else {
                givenObjects = new List<object> { given };
                rl.npcGivenList.Add(CharacterName, givenObjects);
            }

            //Handle VFX and sprite swaps
            Potion temp = given as Potion;
            StartCoroutine(PotionEffects(temp));

            //Handle dialogue response
            SwapVisibile(panelCG);
            string affinity;    //TODO: This might need to be ranges
            if (manager.data[CharacterName].affinity < 0) {
                affinity = "_bad";
            } else if (manager.data[CharacterName].affinity > 0) {
                affinity = "_good";
            } else {
                affinity = "_neutral";
            }

            //Choose from possible responses or use default if there is no response(this shouldn't ever happen)

            string response;
            string rKey = requests[choice].Key;
            if (temp.Primary == null) {
                response = rKey + "_null" + affinity;
            } else {
                response = rKey + "_" + temp.Primary.ToString() + affinity;
            }
            List<string> dia;
            currentDialogue = 0;
            if (Dialogue.TryGetValue(response, out dia)) {
                responding = true;
                if (dia.Count > 1) {
                    int i = Random.Range(0, dia.Count - 1);
                    dialoguePieces = dia[i].Split('*');
                    panel.GetComponentInChildren<Text>().text = dialoguePieces[0];
                } else {
                    dialoguePieces = dia[0].Split('*');
                    panel.GetComponentInChildren<Text>().text = dialoguePieces[0];
                }
                if (dialoguePieces.Length > 1) {
                    nextButton = panel.transform.Find("Next").GetComponent<CanvasGroup>();
                    nextButton.interactable = nextButton.blocksRaycasts = true;
                    nextButton.alpha = 1;
                }
            } else {
                panel.GetComponentInChildren<Text>().text = Dialogue["default"][0];
            }
            string type;
            if (temp.Primary == null) {
                type = "none";
            } else {
                type = temp.Primary.ToString();
            }
            data.affinity += (requests[choice].GetValue(type) * requests[choice].Strength);
            manager.data[CharacterName] = data;
        } else {  //handle trying to give people the wrong item type
            SwapVisibile(panelCG);
            panel.GetComponentInChildren<Text>().text = Dialogue["wrong"][0];
        }
        /*} else {
            //deal with item requests besides potions - probably make the mess above into a function that can just be called with item types
        }*/
    }

    public void NextDialogue() {
        //Handles responses to being given items
        if (responding) {
            if (dialoguePieces.Length > (currentDialogue + 2)) {
                currentDialogue++;
                panel.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
            } else if (dialoguePieces.Length == (currentDialogue + 2)) {
                currentDialogue++;
                panel.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
                SwapVisibile(panel.transform.Find("Next").GetComponent<CanvasGroup>());
            } 
        } else {

            //Handling intro dialogue and requesting items
            if (dialoguePieces.Length > (currentDialogue + 2)) {
                currentDialogue++;
                panel.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
            } else if (dialoguePieces.Length == (currentDialogue + 2)) {
                currentDialogue++;
                panel.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
                if (!showButtons && requested || !rl.requestList.TryGetValue(CharacterName, out requests)) {
                    SwapVisibile(panel.transform.Find("Next").GetComponent<CanvasGroup>());
                }
            } else {
                if (!requested && requests != null) {
                    choice = Random.Range(0, requests.Count - 1);
                    string affinity;
                    if (manager.data[CharacterName].affinity < 0) {
                        affinity = "_bad";
                    } else if (manager.data[CharacterName].affinity > 0) {
                        affinity = "_good";
                    } else {
                        affinity = "_neutral";
                    }
                    string key = requests[choice].Key + affinity;
                    dialoguePieces = Dialogue[key][0].Split('*');
                    currentDialogue = 0;
                    panel.GetComponentInChildren<Text>().text = dialoguePieces[0];
                    requested = true;
                    showButtons = true;
                } else {
                    SwapButtonsAndText();
                }
            }
        }
    }

    public void ExitDialogue() {
        SwapVisibile(panelCG);
        panel.transform.Find("Next").GetComponent<CanvasGroup>().interactable = true;
        panel.transform.Find("Next").GetComponent<CanvasGroup>().blocksRaycasts = true;
        panel.transform.Find("Next").GetComponent<CanvasGroup>().alpha = 1.0f;
        visible = false;
        move = true;
    }

    public void AcceptRequest() {
        wait = true;
        waitHour = mc.Hour + maxWait;
        requested = true;
        SwapButtonsAndText();
        ExitDialogue();
        manager.data[CharacterName] = data;
        showButtons = false;
        chosen = true;
    }

    public void DeclineRequest() {
        panel.GetComponentInChildren<Text>().text = Dialogue["no"][0];
        SwapButtonsAndText();
        move = true;
        wait = false;
        manager.data[CharacterName] = data;
        showButtons = false;
        chosen = true;
    }

    public void Wait() {
        //TODO: can pull this from file if NPCs have different wait times
        wait = false;
        move = true;
        data.returning = true;
        data.returningDay = (mc.Days + 1);
        data.returningHour = spawnHour;
        data.returningMinutes = spawnMinute;
        manager.data[CharacterName] = data;
        manager.returnQueue.Add(data, CharacterName);
        panel.GetComponentInChildren<Text>().text = Dialogue["wait"][0];
        SwapButtonsAndText();
        showButtons = false;
        chosen = true;
    }

    void SwapVisibile(CanvasGroup cg) {
        cg.alpha = Mathf.Abs(cg.alpha - 1);
        cg.interactable = !cg.interactable;
        cg.blocksRaycasts = !cg.blocksRaycasts;
    }

    void SwapButtonsAndText() {
        SwapVisibile(panel.GetComponentInChildren<Text>().GetComponent<CanvasGroup>());
        SwapVisibile(panel.transform.Find("Yes").GetComponent<CanvasGroup>());
        SwapVisibile(panel.transform.Find("Wait").GetComponent<CanvasGroup>());
        SwapVisibile(panel.transform.Find("No").GetComponent<CanvasGroup>());
        panel.transform.Find("Next").GetComponent<CanvasGroup>().interactable = false;
        panel.transform.Find("Next").GetComponent<CanvasGroup>().blocksRaycasts = false;
        panel.transform.Find("Next").GetComponent<CanvasGroup>().alpha = 0;
    }

    IEnumerator PotionEffects(Potion pot) {
        effects.SetActive(true);
        string effect = pot.Primary.ToString();
        Animator anim = GetComponentInChildren<Animator>();
        anim.SetBool(effect, true);

        switch (effect) {
            case "Invisible":
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.25f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case "Sleep":
                asleep = true;
                break;
            case "Transformation":
                anim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.92f);
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Characters/cat");
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(3);
        anim.SetBool(effect, false);
        switch (effect) {
            case "Invisible":
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 1.0f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case "Sleep":
                asleep = false;
                break;
            case "Transformation":
                anim.SetBool(effect, true);
                anim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.9f);
                GetComponent<SpriteRenderer>().sprite = rl.charSpriteList[CharacterName];
                anim.SetBool(effect, false);
                break;
            default:
                break;
        }

        effects.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        move = false;
    }

    private void OnCollisionExit2D(Collision2D collision) {
        move = true;
    }

    public NPCManager Manager {
        set {
            manager = value;
        }
    }

}
