using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSwapping : MonoBehaviour {
    string startingLayer;
    GameObject player;
    bool set;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        startingLayer = gameObject.GetComponent<SpriteRenderer>().sortingLayerName;
        set = false;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!set && !collision.isTrigger && player.transform.position.y > transform.position.y) {     
            SpriteRenderer[] children = gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer sr in children) {
                sr.sortingLayerName = "InFrontOfPlayer";
            }
            set = true;
        }
    }



    private void OnTriggerExit2D(Collider2D collision) {
        if (set && !collision.isTrigger && player.transform.position.y > transform.position.y) {
            SpriteRenderer[] children = gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in children) {
                sr.sortingLayerName = startingLayer;
            }
            set = false;
        }
    }
}
