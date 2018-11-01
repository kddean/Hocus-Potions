using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class Traveller : NPC {
    //static references
    ResourceLoader rl;
    MoonCycle mc;
    NPCManager manager;
    GameObject panel;
    CanvasGroup panelCG;
    Object given;
    GameObject effects;
    NPCManager.NPCData data;


    //flags
    bool move;
    bool visible;
    bool wait;
    bool requested;
    bool done;
    bool asleep;

    int waitHour, waitMinute;
    int maxWait = 5;

    string[] dialoguePieces;
    int currentDialogue;
    string[] requests;
    int choice;
    int spawnHour;
    int spawnMinute;


    private void Start() {
        //initializations
        mc = (MoonCycle)GameObject.FindObjectOfType(typeof(MoonCycle));
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        manager = GameObject.Find("NPCManager").GetComponent<NPCManager>();
        waitHour = mc.Hour + maxWait;
        panel = GameObject.FindGameObjectWithTag("dialogue");
        panelCG = panel.GetComponent<CanvasGroup>();
        Dialogue = rl.dialogueList[CharacterName];
        effects = GameObject.Find("effects");
        spawnHour = mc.Hour;
        spawnMinute = mc.Minutes;
       // effects.SetActive(false);
        //set flags
        visible = false;
        move = true;
        wait = true;
        requested = false;
        given = null;
        done = false;
        asleep = false;
        if(!manager.data.TryGetValue(CharacterName, out data)) {
            data = new NPCManager.NPCData();
            data.timesInteracted = 0;
            data.given = new List<Object>();
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
            if (!manager.returnQueue.TryGetValue(data, out junk)) {
                manager.returnQueue.Add(data, CharacterName);
            }
        }
   
    }

    //Set the initial dialogue when NPC is clicked
    private void OnMouseDown() {
        if(done) { return; }

        move = false;
        if (!visible) {
            SwapVisibile(panelCG);
            dialoguePieces = Dialogue["intro"].Split('*');
            panel.GetComponentInChildren<Text>().text = dialoguePieces[0];
            currentDialogue = 0;
            if (dialoguePieces.Length <= 1) {
                SwapVisibile(panel.transform.Find("Next").GetComponent<CanvasGroup>());
            }
                
            visible = true;
        }
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(1)) {
            if(rl.activeItem != null) {
                if(rl.activeItem.item.item is Potion) {
                    if (!requested || rl.activeItem == null || done) { return; }
                    Give();
                }
            }
        }
    }

    void Give() {
        //bail if they haven't asked for anything yet
        requested = false;
        List<object> givenObjects;
        given = rl.activeItem.item.item;
        done = true;
        move = false;
        wait = false;
        CanvasGroup nextButton = panel.transform.Find("Next").GetComponent<CanvasGroup>();
        nextButton.interactable = nextButton.blocksRaycasts = false;
        nextButton.alpha = 0;

        string[] split = requests[choice].Split('=')[1].Split(' ');
        if (split[1].Equals("potion")) {
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
                //Swap sprites
                Potion temp = given as Potion;
                switch (temp.Primary) {
                    case Ingredient.Attributes.transformation:  //swap sprite to cat
                        //transform.parent.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("cat_sprite");
                        break;
                    case Ingredient.Attributes.sleep:
                        StartCoroutine(PotionEffects("Sleep"));
                        break;
                    case Ingredient.Attributes.poison:
                        StartCoroutine(PotionEffects("Poison"));
                        break;
                    case Ingredient.Attributes.invisible:
                        StartCoroutine(PotionEffects("Invisible"));
                        break;
                    case Ingredient.Attributes.healing:
                        StartCoroutine(PotionEffects("Healing"));
                        break;
                    default:
                        break;
                }

                SwapVisibile(panelCG);
                string response = "given_" + temp.Primary.ToString();
                string dia;
                if(Dialogue.TryGetValue(response, out dia)) {
                    panel.GetComponentInChildren<Text>().text = dia;
                } else {
                    panel.GetComponentInChildren<Text>().text = Dialogue["default"];
                }

            } else {  //handle trying to give people the wrong item type
                SwapVisibile(panelCG);
                panel.GetComponentInChildren<Text>().text = Dialogue["wrong"];
            }
        } else {
            //deal with item requests besides potions
        }

        //TODO: handle response - change sprite, alignment, social, dialogue
    }

    public void NextDialogue() {
        if (dialoguePieces.Length > (currentDialogue + 2)) {
            currentDialogue++;
            panel.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
        } else if (dialoguePieces.Length == (currentDialogue + 2)) {
            currentDialogue++;
            panel.GetComponentInChildren<Text>().text = dialoguePieces[currentDialogue];
            if (!rl.requestList.TryGetValue(CharacterName, out requests)) {
                SwapVisibile(panel.transform.Find("Next").GetComponent<CanvasGroup>());
                waitHour = mc.Hour;
            }
        } else {
            if (!requested) {
                choice = Random.Range(0, requests.Length - 1);
                string key = requests[choice].Split('=')[0];
                panel.GetComponentInChildren<Text>().text = Dialogue[key];
                requested = true;
            } else {
                SwapButtonsAndText();
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
    }

    public void DeclineRequest() {
        panel.GetComponentInChildren<Text>().text = Dialogue["no"];
        SwapButtonsAndText();
        move = true;
        wait = false;
        manager.data[CharacterName] = data;
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
        panel.GetComponentInChildren<Text>().text = Dialogue["wait"];
        SwapButtonsAndText();
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

    IEnumerator PotionEffects(string effect) {
        effects.SetActive(true);
        Animator anim = GetComponentInChildren<Animator>();
        anim.SetBool(effect, true);
        if (effect.Equals("Invisible")) {   //transparency 
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = 0.25f;
            GetComponent<SpriteRenderer>().color = c;
        }
        if (effect.Equals("Sleep")) {      //fall asleep
            asleep = true;
        }

        yield return new WaitForSeconds(3);
        anim.SetBool(effect, false);
        if (effect.Equals("Invisible")) {       //fixing transparency
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = 1.0f;
            GetComponent<SpriteRenderer>().color = c;
        }
        if (effect.Equals("Sleep")) {   //waking up
            asleep = false;
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
