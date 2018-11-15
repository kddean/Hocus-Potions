using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class GardenPlot : MonoBehaviour, IPointerDownHandler {

    // Use this for initialization
    ResourceLoader rl;
    private void Start() {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
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
        rl.garden.Farm(this);
    }

}
