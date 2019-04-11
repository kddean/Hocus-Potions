using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneLocator : MonoBehaviour {

    GameObject bm;
    // Use this for initialization
    void Start() {
        bm = GameObject.Find("BookManager");
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Player")) {
            bm.GetComponent<BookManager>().CurrentZone = this.gameObject;           
            if (!collision.isTrigger) {
                GameObject.FindObjectOfType<OverworldAudioController>().SwapZones(gameObject.name);
            }
            if(this.gameObject.name == "MeadowZone")
            {
                GameObject.FindObjectOfType<BunnyManager>().isPlayerInMeadow = true;
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {           
            if (this.gameObject.name == "MeadowZone")
            {
                GameObject.FindObjectOfType<BunnyManager>().isPlayerInMeadow = false;
            }
        }
    }
}
