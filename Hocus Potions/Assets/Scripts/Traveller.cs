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

    //flags
    bool move;
    bool visible;
    bool wait;
    bool returning;
    bool requested;
    bool done;

    int waitHour, waitMinute;
    int maxWait = 5;

    string[] dialoguePieces;
    int currentDialogue;
    string[] requests;
    int choice;


    private void Start() {
        //initializations
        mc = (MoonCycle)GameObject.FindObjectOfType(typeof(MoonCycle));
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        waitHour = mc.Hour + maxWait;
        panel = GameObject.FindGameObjectWithTag("dialogue");
        panelCG = panel.GetComponent<CanvasGroup>();
        Dialogue = rl.dialogueList[CharacterName];

        //set flags
        visible = false;
        move = true;
        wait = true;
        requested = false;
        given = null;
        done = false;

        //set onclick functions for dialogue
        panel.transform.Find("Next").GetComponent<Button>().onClick.AddListener(NextDialogue);
        panel.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(ExitDialogue);
        panel.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(AcceptRequest);
        panel.transform.Find("Wait").GetComponent<Button>().onClick.AddListener(Wait);
        panel.transform.Find("No").GetComponent<Button>().onClick.AddListener(DeclineRequest);
    }

    void Update() {
        if (move) {
            if (wait) {
                transform.position = Vector2.MoveTowards(transform.position, GameObject.Find("WaitPoint").transform.position, 0.025f);
                //TODO: Just set sorting layer once they're set up
                Vector3 temp = transform.position;
                temp.z = -1.0f;
                transform.position = temp;
            } else { //GET OUT OF MY HOUSE.... AND OFF MY LAWN
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

        if(waitHour < mc.Hour || mc.Hour == 22) {
            wait = false;
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
        Debug.Log(currentDialogue);
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
                        //transform.parent.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("cat_sprite");
                        break;
                    case Ingredient.Attributes.poison:
                        //Probably do something nicer but this will just make them turn green for now
                        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);
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
    }

    public void DeclineRequest() {
        panel.GetComponentInChildren<Text>().text = Dialogue["no"];
        SwapButtonsAndText();
        move = true;
        wait = false;
    }

    public void Wait() {
        //TODO: can pull this from file if NPCs have different wait times
        wait = true;
        waitHour = mc.Hour + maxWait;
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

    public bool Returning {
        set {
            returning = value;
        }
    }

}
