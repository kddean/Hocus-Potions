using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageChest : MonoBehaviour, IPointerDownHandler {
    GameObject canvas;
    Player player;
    public bool active;
    StorageManager sm;
    float temp;
    GameObject inv;
    bool alreadyOpen;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        canvas = GameObject.FindGameObjectWithTag("storage").transform.parent.gameObject;
        sm = GameObject.Find("StorageManager").GetComponent<StorageManager>();
        inv = GameObject.FindGameObjectWithTag("inventory");
        canvas.SetActive(false);
        active = false;
        alreadyOpen = true;
    }
    private void OnMouseEnter() {
        if (!active && !GameObject.FindObjectOfType<Cauldron>().Visible && !GameObject.FindObjectOfType<Wardrobe>().open) {
            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Exclaim Mouse"), Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnMouseExit() {
        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
    }
    public void OnPointerDown(PointerEventData eventData) {
        if (player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(player.transform.position, transform.position) > 2.0f) { return; }
        canvas.SetActive(true);
        active = true;
        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
        GameObject.FindGameObjectWithTag("storage").GetComponent<GridLayoutGroup>().enabled = true;
        sm.OpenChest();
        if (inv.GetComponent<CanvasGroup>().alpha == 0) {
            inv.GetComponent<CanvasGroup>().alpha = 1;
            inv.GetComponent<CanvasGroup>().interactable = true;
            inv.GetComponent<CanvasGroup>().blocksRaycasts = true;
            alreadyOpen = false;
        } else {
            alreadyOpen = true;
        }
        player.allowedToMove = false;
        GetComponent<AudioSource>().Play();
    }

    public void Close() {
        GetComponents<AudioSource>()[1].Play();
        canvas.SetActive(false);
        active = false;
        if (!alreadyOpen) {
            inv.GetComponent<CanvasGroup>().alpha = 0;
            inv.GetComponent<CanvasGroup>().interactable = false;
            inv.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        player.allowedToMove = true;

    }
}
