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

    //flags
    bool move;
    bool visible;
    bool wait;
    bool returning;
    bool requested;
    bool done;
    bool leaving;

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
        wait = false;
        requested = false;
        done = false;
        leaving = false;

        //set onclick functions for dialogue
        panel.transform.Find("Next").GetComponent<Button>().onClick.AddListener(NextDialogue);
        panel.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(ExitDialogue);
        panel.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(AcceptRequest);
        panel.transform.Find("Wait").GetComponent<Button>().onClick.AddListener(Wait);
        panel.transform.Find("No").GetComponent<Button>().onClick.AddListener(DeclineRequest);
    }

    void Update() {
        if (move && !leaving) {
            transform.position = Vector2.MoveTowards(transform.position, GameObject.Find("WaitPoint").transform.position, 0.025f);
            Vector3 temp = transform.position;
            temp.z = -1.0f;
            transform.position = temp;
        }
        // This will make them leave after they've waited long enough but it makes them instantly disappear when the clock speed is jacked up
        if (move && (waitHour <= mc.Hour || leaving)) {
            leaving = true;
            transform.position = Vector2.MoveTowards(transform.position, GameObject.Find("SpawnPoint").transform.position, 0.025f);
            Vector3 temp = transform.position;
            temp.z = -1.0f;
            transform.position = temp;
            if (transform.position.x == GameObject.Find("SpawnPoint").transform.position.x && transform.position.y == GameObject.Find("SpawnPoint").transform.position.y) {
                Destroy(this.gameObject);
                manager.Spawned = false;
            }
        }
    }

    private void OnMouseDown() {
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
                    Give();
                }
            }
        }
    }

    void Give() {
        rl.givenObjects.Add(CharacterName, rl.activeItem.item.item);
        rl.inv.RemoveItem(rl.activeItem.item);

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
        panel.GetComponentInChildren<Text>().text = Dialogue["yes"];
        SwapButtonsAndText();
        waitHour = mc.Hour;
        //TODO: This needs to check if you actually have the items they're asking for at some point
        //Should it just pull from inventory automatically, should you use the item on them, or are we doing contextual menus because setting this mess up wasn't torture enough?
    }

    public void DeclineRequest() {
        panel.GetComponentInChildren<Text>().text = Dialogue["no"];
        SwapButtonsAndText();
        waitHour = mc.Hour;
    }

    public void Wait() {
        //TODO: can pull this from file if NPCs have different wait times
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
