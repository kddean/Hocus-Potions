using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class GardenPlot : MonoBehaviour, IPointerDownHandler {

    // Use this for initialization
    ResourceLoader rl;
    Player player;
    private void Start() {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //Display plants if there's something growing
        Garden.PlotData temp;
        if (rl.garden.plots.TryGetValue(gameObject.name, out temp)) {
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 1; i < 4; i++) {
                sprites[i].sprite = Resources.LoadAll<Sprite>("Plants/" + temp.type)[temp.index];
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData) {
        if(player.Status == Player.PlayerStatus.asleep || player.Status == Player.PlayerStatus.transformed) {
            return;
        }

        rl.garden.Farm(this);
    }

}
