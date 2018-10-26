using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class GardenPlot : MonoBehaviour {

    // Use this for initialization
    ResourceLoader rl;
    private void Start() {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        //Display plants if there's something growing
        Garden.PlotData temp;
        if(rl.garden.plots.TryGetValue(gameObject.name, out temp)){
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer s in sprites) {
                s.sprite = Resources.Load<Sprite>("Plants/" + temp.type + "_" + temp.index);
            }
        }
    }
    private void OnMouseDown() {
        rl.garden.Farm(this);
    }

}
