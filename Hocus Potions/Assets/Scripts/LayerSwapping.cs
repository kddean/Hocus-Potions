using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSwapping : MonoBehaviour {
    List<string> startingLayer;
    GameObject player;
    bool set;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        startingLayer = new List<string>();
        set = false;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!set && !collision.isTrigger && (gameObject.GetComponent<SpriteRenderer>() == null || (player.transform.position.y > (transform.position.y - (gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 6.0f))))) {
            SpriteRenderer[] children = gameObject.GetComponentsInChildren<SpriteRenderer>();
            GameObject.FindObjectOfType<Player>().layerSwapping = true;
            foreach (SpriteRenderer sr in children) {
                if (sr.gameObject.name.Equals("runes")) { continue; }
                startingLayer.Add(sr.sortingLayerName);
                sr.sortingLayerName = "InFrontOfPlayer";
            }
            set = true;
        }

        if (set && !collision.isTrigger && gameObject.GetComponent<SpriteRenderer>() != null && (player.transform.position.y < (transform.position.y - (gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 6.0f)))) {
            SpriteRenderer[] children = gameObject.GetComponentsInChildren<SpriteRenderer>();
            GameObject.FindObjectOfType<Player>().layerSwapping = false;
            for (int i = 0; i < children.Length; i++) {
                if (children[i].gameObject.name.Equals("runes")) { continue; }
                children[i].sortingLayerName = startingLayer[i];
            }
            set = false;
            startingLayer.Clear();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (set && !collision.isTrigger) {
            SpriteRenderer[] children = gameObject.GetComponentsInChildren<SpriteRenderer>();
            GameObject.FindObjectOfType<Player>().layerSwapping = false;
            for (int i = 0; i < children.Length; i++){
                if (children[i].gameObject.name.Equals("runes")) { continue; }
                children[i].sortingLayerName = startingLayer[i];
            }
            set = false;
            startingLayer.Clear();
        }
    }
}
