using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {
    public bool doTutorial = false;
    string[] dialogue;
    int index = 0;
    Animator anim;
    string currentAnim;
    float speed;
    Player player;
    List<Vector3> path = new List<Vector3>();
    GameObject doorway;
    GameObject cat;
    GameObject skipButton;
    GameObject exitButton;
    DialogueCanvas dc;
    public GameObject notification;
    bool startText = false;
    bool finishedTutorial = false;
    // Use this for initialization
    void Start() {
        skipButton = GameObject.Find("TutorialCanvas");
        skipButton.SetActive(false);
        doorway = GameObject.Find("ToWorld");
    }

    // Update is called once per frame
    void Update() {
        if (doTutorial) {
            StartCoroutine(RunTutorial());
            skipButton.SetActive(true);
            doTutorial = false;
        }

        if (finishedTutorial && cat != null && cat.transform.position == new Vector3(0.5f, -4.5f, 0)) {
            ExitButton();
            Destroy(cat);
            Destroy(this);
        }

        if (Monitor.TryEnter(path, 1)) {
            if (anim != null) {
                anim.speed = speed / 7f;
            }

            if (path.Count > 0) {
                //Adjusting colliders
                cat.GetComponent<BoxCollider2D>().size = new Vector2(cat.GetComponent<SpriteRenderer>().bounds.size.x * 1.5f, cat.GetComponent<SpriteRenderer>().bounds.size.y / 2);
                cat.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.25f);
                cat.GetComponents<BoxCollider2D>()[1].size = new Vector2(cat.GetComponent<SpriteRenderer>().bounds.size.x, cat.GetComponent<SpriteRenderer>().bounds.size.y);
                cat.GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, cat.GetComponent<SpriteRenderer>().bounds.size.y / 2);
                //Movement
                if (anim != null && anim.enabled) {
                    if (cat.transform.position.x < path[0].x) {
                        anim.SetBool(currentAnim, false);
                        currentAnim = "Right";
                        anim.SetBool(currentAnim, true);
                    } else if (cat.transform.position.x > path[0].x) {
                        anim.SetBool(currentAnim, false);
                        currentAnim = "Left";
                        anim.SetBool(currentAnim, true);

                    } else if (cat.transform.position.y > path[0].y) {
                        anim.SetBool(currentAnim, false);
                        currentAnim = "Forward";
                        anim.SetBool(currentAnim, true);

                    } else if (cat.transform.position.y < path[0].y) {
                        anim.SetBool(currentAnim, false);
                        currentAnim = "Backward";
                        anim.SetBool(currentAnim, true);
                    }
                }
                cat.transform.position = Vector3.MoveTowards(cat.transform.position, path[0], 0.02f * speed);
                if (cat.transform.position == path[0]) {
                    path.RemoveAt(0);
                    if (path.Count == 0) {
                        anim.SetBool(currentAnim, false);
                        currentAnim = "Forward";
                        startText = true;
                    }
                }
            }
        }
        Monitor.Exit(path);
    }

    IEnumerator RunTutorial() {
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 0;
        doorway.SetActive(false);
        player = GameObject.FindObjectOfType<Player>();
        player.allowedToMove = false;
        speed = 4;
        cat = Instantiate(Resources.Load<GameObject>("Characters/TutorialCat"));
        cat.transform.position = new Vector3(0.5f, -4.5f, 0);
        cat.name = "Princess";
        GameObject.FindObjectOfType<Pathfinding>().InitializePath(cat.transform.position, new Vector3(2.5f, 1.5f, 0), 0, path);
        anim = cat.GetComponent<Animator>();
        currentAnim = "Forward";

        dc = Resources.FindObjectsOfTypeAll<DialogueCanvas>()[0];
        exitButton = dc.GetComponentsInChildren<Button>()[1].gameObject;
        dialogue = new string[] { "Hail, young witch", "Oh, don’t look so surprised!", "How do you expect to become a witch if you can’t handle a talking cat?", "I’m Princess, I’ve lived in these parts for a while, I even knew your Great Aunt back in the day", "She would be proud to know you’ve decided to take over the family business", "I’m glad to see you’ve already started dressing the part, too.", "So... you don’t really know where to start, do you?", "Well, luckily for you your Great Aunt left her old spellbook. I think you can open it up with M",
            "Great, I’m glad that old pile of papers still works, although it looks like some pages have gone missing over the years.", "You’re new to brewing potions, right? Well, it’s not too hard to get started.", "Here, take these ingredients, plop ‘em in the cauldron over there, and see what you can make.",
            "Nice, that looks like a great potion of speed. You can drink it now if you want, or you can save it for later.", "You can get more ingredients by finding them in the wild surrounding your house", "Or you can grow them yourself! Here are some seeds you can plant in the old garden plots outside.",
            "There, now you have some to get you started.", "Don’t worry about saying thanks, I’m a cat, I don’t have thumbs so I can’t use them anyways.", "You can put them in that storage chest for now, if you’d like.", "Why don’t you go ahead and make sure the darn thing isn’t rusted shut?",
            "One last thing -- your Aunt passed her spellcraft knowledge along, didn’t she?", "You can equip spells by pressing and holding Left Ctrl.", "Spells aren’t my area of expertise though, so you’ll have to figure out how to use them yourself",
            "I think you’re ready to get started, I better get back home before my roommates get worried.", "Oh, one last thing...", "There’s been talk of strange entities in these woods...", "You best watch yourself around anyone who looks... suspicious.", "That aside though, I expect the locals will be coming by and asking you for help once word gets out that there’s a new witch in town.", "They may not all like magic, but they do like shoving their problems on others", "Good luck, little witch." };



        while (!startText) {
            yield return null;
        }

        dc.gameObject.SetActive(true);
        dc.active = true;

        dc.GetComponentsInChildren<Button>()[0].onClick.RemoveAllListeners();

        dc.GetComponentsInChildren<Button>()[0].onClick.AddListener(NextButton);
        exitButton.SetActive(false);
        dc.GetComponentInChildren<Text>().text = dialogue[index];
        dc.GetComponentsInChildren<Text>()[4].text = "Princess";

    }

    public void ExitButton() {
        dc.GetComponentInChildren<Text>().enabled = true;
        dc.GetComponentsInChildren<CanvasGroup>()[0].interactable = true;
        dc.GetComponentsInChildren<CanvasGroup>()[0].blocksRaycasts = true;
        dc.GetComponentsInChildren<CanvasGroup>()[0].alpha = 1.0f;
        dc.GetComponent<DialogueCanvas>().active = false;

        dc.GetComponentsInChildren<Button>()[0].onClick.RemoveAllListeners();
        exitButton.SetActive(true);
        dc.gameObject.SetActive(false);
        if (finishedTutorial) {
            Time.timeScale = 1;
            GameObject.FindObjectOfType<Pathfinding>().InitializePath(cat.transform.position, new Vector3(0.5f, -4.5f, 0), 0, path);
            player.allowedToMove = true;
            doorway.SetActive(true);

        }
    }

    public void NextButton() {
        index++;
        dc.GetComponentInChildren<Text>().text = dialogue[index];

        if (index == 8 || index == 11 || index == 14 || index == 18 || index == 21) {
            StartCoroutine(Delay());
        } else if (index == dialogue.Length - 1) {
            dc.GetComponentsInChildren<CanvasGroup>()[0].interactable = false;
            dc.GetComponentsInChildren<CanvasGroup>()[0].blocksRaycasts = false;
            dc.GetComponentsInChildren<CanvasGroup>()[0].alpha = 0.0f;
            exitButton.SetActive(true);

            dc.GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();
            dc.GetComponentsInChildren<Button>()[1].onClick.AddListener(ExitButton);

            finishedTutorial = true;
            skipButton.SetActive(false);
        }
    }

    IEnumerator Delay() {
        bool madePotion = false;
        if(index == 8) {
            dc.gameObject.SetActive(false);
            notification.GetComponent<CanvasGroup>().alpha = 1;
            notification.GetComponentInChildren<Text>().text = "Press M to open the journal";
            while (GameObject.Find("BookCanvas") == null) {
                yield return null;
            }
            notification.GetComponent<CanvasGroup>().alpha = 0;
            while (GameObject.Find("BookCanvas") != null) {
                yield return null;
            }
            dc.gameObject.SetActive(true);
        } else if (index == 11) {
            dc.gameObject.SetActive(false);
            notification.GetComponent<CanvasGroup>().alpha = 1;
            notification.GetComponentInChildren<Text>().text = "Click the cauldron and place the ingredients inside";
            Inventory.Tutorial1();
            InventorySlot[] slots = GameObject.FindObjectsOfType<InventorySlot>();
            while (!madePotion) {
                foreach (InventorySlot s in slots) {
                    if (s.item != null && s.item.item != null && s.item.item.name.Contains("Potion")) {
                        madePotion = true;
                        break;
                    }
                }
                yield return null;
            }
            notification.GetComponent<CanvasGroup>().alpha = 0;
            dc.gameObject.SetActive(true);
        } else if (index == 14) {
            dc.gameObject.SetActive(false);
            Inventory.Tutorial2();
            yield return new WaitForSecondsRealtime(1);

            dc.gameObject.SetActive(true);
        } else if (index == 18) {
            dc.gameObject.SetActive(false);
            notification.GetComponent<CanvasGroup>().alpha = 1;
            notification.GetComponentInChildren<Text>().text = "Click the chest to open it";
            while (GameObject.FindGameObjectWithTag("storage") == null) {
                yield return null;
            }
            notification.GetComponent<CanvasGroup>().alpha = 0;
            while (GameObject.FindGameObjectWithTag("storage") != null) {
                yield return null;
            }

            dc.gameObject.SetActive(true);
        } else if (index == 21) {
            dc.gameObject.SetActive(false);
            notification.GetComponent<CanvasGroup>().alpha = 1;
            notification.GetComponentInChildren<Text>().text = "Press left control and select a spell";
            while (GameObject.FindObjectOfType<ResourceLoader>().activeSpell == null) {
                yield return null;
            }
            notification.GetComponent<CanvasGroup>().alpha = 0;
            dc.gameObject.SetActive(true);
        }
    }

    public void SkipTutorial() {
        Inventory.TutorialSkip();
        dc = Resources.FindObjectsOfTypeAll<DialogueCanvas>()[0];
        if (dc.gameObject.activeSelf) {
            dc.GetComponentsInChildren<CanvasGroup>()[0].interactable = true;
            dc.GetComponentsInChildren<CanvasGroup>()[0].blocksRaycasts = true;
            dc.GetComponentsInChildren<CanvasGroup>()[0].alpha = 1.0f;
            dc.GetComponent<DialogueCanvas>().active = false;
            dc.gameObject.SetActive(false);
            dc.GetComponentsInChildren<Button>()[0].onClick.RemoveAllListeners();
            exitButton.SetActive(true);
        }
        Time.timeScale = 1;
        if (cat != null) {
            GameObject.FindObjectOfType<Pathfinding>().InitializePath(cat.transform.position, new Vector3(0.5f, -4.5f, 0), 0, path);
        }
        skipButton.SetActive(false);
        doorway.SetActive(true);
        GameObject.FindObjectOfType<Player>().allowedToMove = true;
        finishedTutorial = true;
    }
}
