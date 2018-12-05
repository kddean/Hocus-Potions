using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionCheck : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.isTrigger) { return; }
        NPC npc = collision.gameObject.GetComponent<NPC>();
        if( npc != null) {
            npc.region = gameObject.name;
        }
    }
}
