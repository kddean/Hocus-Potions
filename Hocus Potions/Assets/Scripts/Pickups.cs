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
        get {
            return item;
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
        if(player.Status.Contains(Player.PlayerStatus.asleep) || Vector3.Distance(player.transform.position, transform.position) > 1.5f) { return; }

        if (Inventory.Add(item, count)) {
            Vector3 temp = new Vector3(data.x, data.y, data.z);
            gc.RemoveItem(item, temp, data.scene);
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
