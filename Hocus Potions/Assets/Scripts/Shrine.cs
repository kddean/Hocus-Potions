using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shrine : MonoBehaviour, IPointerDownHandler {
    DialogueCanvas dc;
    ShrineManager manager;
    bool on;
    int index = 0;
    string[] dialogue;

    void Start() {
        manager = GameObject.FindObjectOfType<ShrineManager>();
        on = false;
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
        if (GameObject.FindObjectOfType<Player>().Status.Contains(Player.PlayerStatus.asleep) || GameObject.FindObjectOfType<Player>().Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(transform.position, GameObject.FindObjectOfType<Player>().transform.position) > 2) { return; }

        if (eventData.button.Equals(PointerEventData.InputButton.Left)) {

            if (on) {     
                if (dc.active) {
                    return;
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
        }

        dc.GetComponentInChildren<Text>().text = dialogue[index];

    }
}
