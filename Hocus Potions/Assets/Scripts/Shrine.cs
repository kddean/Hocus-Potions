using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shrine : MonoBehaviour, IPointerDownHandler {
    DialogueCanvas dc;
    ShrineManager manager;
    bool on;
    int index = 0;
    string[] dialogue;
    bool finishedRiddle;

    void Start() {
        manager = GameObject.FindObjectOfType<ShrineManager>();
        on = false;
        finishedRiddle = false;
        dc = Resources.FindObjectsOfTypeAll<DialogueCanvas>()[0];
        if (gameObject.name.Contains("Order")) {
            dialogue = manager.dialogue["order"];
        } else if (gameObject.name.Contains("Social")) {
            dialogue = manager.dialogue["social"];
        } else if (gameObject.name.Contains("Nature")) {
            dialogue = manager.dialogue["nature"];
        }

        StartCoroutine(Check());
    }

    IEnumerator Check() {
        if (gameObject.name.Contains("Order")) {
            if (manager.order) {
                GetComponent<Animator>().SetBool("On", true);
                on = true;
            } else {
                GetComponent<Animator>().SetBool("On", false);
                on = false;
            }
        } else if (gameObject.name.Contains("Social")) {
            if (manager.social) {
                GetComponent<Animator>().SetBool("On", true);
                on = true;
            } else {
                GetComponent<Animator>().SetBool("On", false);
                on = false;
            }
        } else if (gameObject.name.Contains("Nature")) {
            if (manager.nature) {
                GetComponent<Animator>().SetBool("On", true);
                on = true;
            } else {
                GetComponent<Animator>().SetBool("On", false);
                on = false;
            }
        }

        yield return new WaitForSeconds(3);
        StartCoroutine(Check());
    }

    private void OnMouseEnter() {
        if (on) {
            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Talk Mouse"), Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnMouseExit() {
        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (GameObject.FindObjectOfType<Player>().Status.Contains(Player.PlayerStatus.asleep) || GameObject.FindObjectOfType<Player>().Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(transform.position, GameObject.FindObjectOfType<Player>().transform.position) > 3) { return; }

        if (eventData.button.Equals(PointerEventData.InputButton.Left)) {

            if (on) {     
                if (dc.active) {
                    return;
                }

                GameObject.FindObjectOfType<Player>().allowedToMove = false;
                dc.gameObject.SetActive(true);
                dc.active = true;
                dc.GetComponentInChildren<Text>().text = dialogue[0];

                dc.GetComponentsInChildren<Text>()[4].text = "";

                dc.GetComponentsInChildren<Button>()[0].onClick.RemoveAllListeners(); 
                dc.GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners(); 

                dc.GetComponentsInChildren<Button>()[0].onClick.AddListener(Next);
                dc.GetComponentsInChildren<Button>()[1].onClick.AddListener(ExitButton);
            }
        } else if(eventData.button.Equals(PointerEventData.InputButton.Right)) {
            if (!finishedRiddle) { return; }
            if(GameObject.FindObjectOfType<ResourceLoader>().activeItem.item.item is Potion) {
                UsePotion(GameObject.FindObjectOfType<ResourceLoader>().activeItem);
            }
        }

    }

    public void ExitButton() {
        GameObject.FindObjectOfType<Player>().allowedToMove = true;
        dc.active = false;
        dc.gameObject.SetActive(false);
        index = 0;

        dc.GetComponentsInChildren<CanvasGroup>()[0].interactable = true;
        dc.GetComponentsInChildren<CanvasGroup>()[0].blocksRaycasts = true;
        dc.GetComponentsInChildren<CanvasGroup>()[0].alpha = 1.0f;
    }

    public void Next() {
        index++;
        if(index == dialogue.Length - 1) {
            dc.GetComponentsInChildren<CanvasGroup>()[0].interactable = false;
            dc.GetComponentsInChildren<CanvasGroup>()[0].blocksRaycasts = false;
            dc.GetComponentsInChildren<CanvasGroup>()[0].alpha = 0.0f;
            finishedRiddle = true;
        }

        dc.GetComponentInChildren<Text>().text = dialogue[index];

    }


    public void UsePotion(InventorySlot slot) {

        Potion temp = slot.item.item as Potion;

        if (gameObject.name.Contains("Order")) {
            if (temp.name.Contains("Order")) {
                dialogue = manager.acceptDialogue["order"];
                manager.endOrder = true;
                GameObject.FindObjectOfType<Mana>().UpdateMana(0);
                GameObject.FindObjectOfType<Wardrobe>().Unlocked[10] = true;
                GameObject.FindObjectOfType<Wardrobe>().LoadCostume("Costume_Steampunk");
                Inventory.RemoveItem(slot);
                NPCController npcController = GameObject.FindObjectOfType<NPCController>();
                List<string> npcs = npcController.NPCQuestFlags.Keys.ToList();
                foreach(string s in npcs) {
                    npcController.NPCQuestFlags[s] = false;
                }
            } else {
                dialogue = manager.rejectDialogue["order"];
            }
        } else if (gameObject.name.Contains("Social")) {
            if (temp.name.Contains("Kindness")) {
                dialogue = manager.acceptDialogue["social"];
                manager.endSocial = true;
                GameObject.FindObjectOfType<Wardrobe>().Unlocked[9] = true;
                GameObject.FindObjectOfType<Wardrobe>().LoadCostume("Costume_Magic");

                NPCController npcController = GameObject.FindObjectOfType<NPCController>();
                List<string> npcs = npcController.NPCQuestFlags.Keys.ToList();
                foreach (string s in npcs) {
                    npcController.NPCQuestFlags[s] = false;
                }

                List<string> keys = GameObject.FindObjectOfType<NPCController>().npcData.Keys.ToList();

                foreach (string key in keys) {
                    if (key.Contains("Traveler")) { continue; }

                    NPCController.NPCInfo info = GameObject.FindObjectOfType<NPCController>().npcData[key];
                    info.affinity = 5.0f;
                    GameObject.FindObjectOfType<NPCController>().npcData[key] = info;
                }
                Inventory.RemoveItem(slot);
            } else {
                dialogue = manager.rejectDialogue["social"];
            }
        } else if (gameObject.name.Contains("Nature")) {
            if (temp.name.Contains("Chaos")) {
                dialogue = manager.acceptDialogue["nature"];
                manager.endNature = true;
                GameObject.FindObjectOfType<Wardrobe>().Unlocked[8] = true;
                GameObject.FindObjectOfType<Wardrobe>().LoadCostume("Costume_Goddess");
                GameObject.FindObjectOfType<GatheringManager>().defaultResetTime = 1;

                NPCController npcController = GameObject.FindObjectOfType<NPCController>();
                List<string> npcs = npcController.NPCQuestFlags.Keys.ToList();
                foreach (string s in npcs) {
                    npcController.NPCQuestFlags[s] = false;
                }

                Inventory.RemoveItem(slot);
            } else {
                dialogue = manager.rejectDialogue["nature"];
            }
        }

        GameObject.FindObjectOfType<Player>().allowedToMove = false;
        dc.gameObject.SetActive(true);
        dc.active = true;
        dc.GetComponentInChildren<Text>().text = dialogue[0];

        dc.GetComponentsInChildren<Button>()[0].onClick.RemoveAllListeners();
        dc.GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();

        dc.GetComponentsInChildren<Button>()[0].onClick.AddListener(Next);
        dc.GetComponentsInChildren<Button>()[1].onClick.AddListener(ExitButton);
    }
}
