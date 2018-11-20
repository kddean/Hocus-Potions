using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inventory {
    public class InventoryItem {
        public int maxStack = 10;
        public int count = 1;
        public Item item;

        public InventoryItem(Item o, int c) {
            item = o;
            count = c;
        }
    }

    //Just for testing stack combining
    public static void Testing() {
        ResourceLoader rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        Brewing b = new Brewing();
        Potion p = b.Brew(rl.ingredients["catnip"], rl.ingredients["mugwort"], rl.ingredients["lily"]);
        Potion pp = b.Brew(rl.ingredients["catnip"], rl.ingredients["mugwort"], rl.ingredients["lily"]);
        Potion ppp = b.Brew(rl.ingredients["catnip"], rl.ingredients["mugwort"], rl.ingredients["lily"]);
        Seed s = rl.seeds["lavender"];
        Seed ss = rl.seeds["poppy"];
        Seed sss = rl.seeds["nightshade"];
        Ingredient i = rl.ingredients["thistle"];
        Ingredient ii = rl.ingredients["catnip"];
        Ingredient iii = rl.ingredients["lambsgrass"];

        Add(p, 1);
        Add(pp, 1);
        Add(ppp, 1);
        Add(s, 4);
        Add(ss, 4);
        Add(sss, 4);
        Add(i, 8);
        Add(ii, 8);
        Add(iii, 8);    
    }
   

    public static bool Add(Item obj, int count) {
        Button[] invButtons = GameObject.FindGameObjectWithTag("inventory").transform.parent.GetComponentsInChildren<Button>();

        foreach (Button b in invButtons) {
            InventorySlot slot = b.GetComponent<InventorySlot>();
            //Adding to existing stack
            if (slot.item != null && slot.item.item.name == obj.name && slot.item.count < slot.item.maxStack) {
                int remainder = count - (slot.item.maxStack - slot.item.count);
                if (remainder <= 0) {
                    slot.item.count = (slot.item.count + count);
                    slot.gameObject.GetComponentInChildren<Text>().text = slot.item.count.ToString();
                    return true;
                } else {
                    slot.item.count = slot.item.maxStack;
                    foreach (Button bt in invButtons) {
                        InventorySlot s = bt.GetComponent<InventorySlot>();
                        if (s.item == null) {
                            s.item = new InventoryItem(obj, remainder);
                            s.gameObject.GetComponent<Image>().sprite = s.item.item.image;
                            if (s.item.count > 1) {
                                s.gameObject.GetComponentInChildren<Text>().text = s.item.count.ToString();
                            } else {
                                s.gameObject.GetComponentInChildren<Text>().text = "";
                            }
                            return true;
                        }
                    }
                }
            }
        }

        foreach (Button b in invButtons) {
            InventorySlot s = b.GetComponent<InventorySlot>();
            if (s.item == null) {
                s.item = new InventoryItem(obj, count);
                s.gameObject.GetComponent<Image>().enabled = true;
                s.gameObject.GetComponent<Image>().sprite = s.item.item.image;
                if (s.item.count > 1) {
                    s.gameObject.GetComponentInChildren<Text>().text = s.item.count.ToString();
                } else {
                    s.gameObject.GetComponentInChildren<Text>().text = "";
                }
                return true;
            }
        }

        //No empty slots and no partial stacks to add into
        return false;
    }

    public static void RemoveItem(InventorySlot slot) {
        Button[] invButtons = GameObject.FindGameObjectWithTag("inventory").transform.parent.GetComponentsInChildren<Button>();
        if (slot.item.count == 1) {
            RemoveStack(slot);
        } else {
            slot.item.count--;
            if (slot.item.count == 1) {
                slot.gameObject.GetComponentInChildren<Text>().text = "";
            } else {
                slot.gameObject.GetComponentInChildren<Text>().text = slot.item.count.ToString();
            }
        }
    }

    public static void DropItem(InventorySlot slot) {
        GameObject go = new GameObject();
        go.name = slot.item.item.name;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = slot.item.item.image;
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        go.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + offset;      
        Vector2 bounds = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
        BoxCollider2D c = go.AddComponent<BoxCollider2D>();
        c.size = bounds;
        c.isTrigger = true;

        if (slot.item.item is Seed) {
            go.transform.localScale = new Vector3(0.4f, 0.4f, 1);
        }
             
        Pickups p = go.AddComponent<Pickups>();
        p.Item = slot.item.item;
        p.Count = slot.item.count;
        p.Data = new GarbageCollecter.DroppedItemData(slot.item.item, slot.item.count, go.transform.position, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, go);
        GameObject.Find("GarbageCollector").GetComponent<GarbageCollecter>().droppedItems.Add(p.Data);
        RemoveStack(slot);
    }

    public static void RemoveStack(InventorySlot item) {
        ResourceLoader rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        if (rl.activeItem == item) {
            rl.activeItem = null;
        }
        item.gameObject.GetComponentInChildren<Image>().enabled = false;
        item.gameObject.GetComponentInChildren<Text>().text = "";
        item.item = null;
    }

    //TODO: function to allow items to be used from inventory
    void UseItem() {

    }
}
