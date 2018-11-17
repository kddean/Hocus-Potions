using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pickups : MonoBehaviour, IPointerDownHandler {
    Item item;
    int count;
    GarbageCollecter gc;
    ResourceLoader rl;
    GarbageCollecter.DroppedItemData data;
    Player player;

    public Item Item { 
        set {
            item = value;
        }
    }

    public int Count {
        set {
            count = value;
        }
    }

    public GarbageCollecter.DroppedItemData Data {
        get {
            return data;
        }

        set {
            data = value;
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(player.Status == Player.PlayerStatus.asleep) { return; }
        if (rl.inv.Add(item, count, 10)) {
            gc.RemoveItem(item, data.position, data.scene);
            Destroy(this.gameObject);
        } else {
            //TODO: probably play an animation or something to show you can't pick it up
        }
    }

    // Use this for initialization
    void Start() {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        gc = GameObject.Find("GarbageCollector").GetComponent<GarbageCollecter>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
}
