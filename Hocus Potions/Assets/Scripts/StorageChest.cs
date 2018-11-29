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

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        canvas = GameObject.FindGameObjectWithTag("storage").transform.parent.gameObject;
        sm = GameObject.Find("StorageManager").GetComponent<StorageManager>();
        canvas.SetActive(false);
        active = false;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(player.transform.position, transform.position) > 2.0f) { return; }
        canvas.SetActive(true);
        active = true;
        sm.OpenChest();
        player.allowedToMove = false;
        GetComponent<AudioSource>().Play();
    }

    public void Close() {
        GetComponents<AudioSource>()[1].Play();
        canvas.SetActive(false);
        active = false;
        player.allowedToMove = true;

    }
}
