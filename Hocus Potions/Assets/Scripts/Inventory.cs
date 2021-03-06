﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inventory {
    [System.Serializable]
    public class InventoryItem {
        public int maxStack = 10;
        public int count = 1;
        public Item item;

        public InventoryItem(Item o, int c) {
            item = o;
            count = c;
        }
    }

    //Just for populating inv to start
    public static void Testing() {
        ResourceLoader rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        Brewing b = new Brewing();
        Potion p = b.Brew(rl.ingredients["poppy"], rl.ingredients["dandylion"], rl.ingredients["emerald"]);
        Potion pp = b.Brew(rl.ingredients["mugwort"], rl.ingredients["fly_agaric"], rl.ingredients["selenite"]);
        Potion ppp = b.Brew(rl.ingredients["ghostcap"], rl.ingredients["morel"], rl.ingredients["amber"]);
        Seed s = rl.seeds["thistle"];
        Seed ss = rl.seeds["poppy"];
        Seed sss = rl.seeds["nightshade"];
        Ingredient i = rl.ingredients["thistle"];
        Ingredient ii = rl.ingredients["catnip"];
        Ingredient iii = rl.ingredients["lambsgrass"];

        Add(p, 1, false);
        Add(pp, 10, false);
        Add(ppp, 1, false);
        Add(s, 4, false);
        Add(ss, 4, false);
        Add(sss, 4, false);
        Add(i, 8, false);
        Add(i, 3, false);
        Add(ii, 8, false);
        Add(iii, 8, false);    
    }
   
    public static void Tutorial1() {
        ResourceLoader rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        Ingredient i = rl.ingredients["thistle"];
        Ingredient ii = rl.ingredients["catnip"];
        Ingredient iii = rl.ingredients["lambsgrass"];
        Add(i, 1, false);
        Add(ii, 1, false);
        Add(iii, 1, false);
    }

    public static void Tutorial2() {
        ResourceLoader rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        Seed s = rl.seeds["catnip"];
        Seed ss = rl.seeds["poppy"];
        Seed sss = rl.seeds["nightshade"];
        Seed ssss = rl.seeds["lavender"];
        Add(s, 3, false);
        Add(ss, 3, false);
        Add(sss, 3, false);
        Add(ssss, 3, false);
    }

    public static void TutorialSkip() {
        InventorySlot[] slots = GameObject.FindObjectsOfType<InventorySlot>();
        foreach(InventorySlot s in slots) {
            if (s.item != null) {
                RemoveStack(s);
            }
        }
        ResourceLoader rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        Brewing b = new Brewing();
        Inventory.Add(b.Brew(rl.ingredients["catnip"], rl.ingredients["thistle"], rl.ingredients["lambsgrass"]), 1, false);
        Tutorial2();        

    }

    public static bool Add(Item obj, int count, bool shouldDrop) {
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
                    slot.gameObject.GetComponentInChildren<Text>().text = slot.item.count.ToString();
                    foreach (Button bt in invButtons) {
                        InventorySlot s = bt.GetComponent<InventorySlot>();
                        if (s.item == null) {
                            s.item = new InventoryItem(obj, remainder);
                            s.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(obj.imagePath);
                            s.gameObject.GetComponent<Image>().enabled = true;
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

        //Creating new stack
        foreach (Button b in invButtons) {
            InventorySlot s = b.GetComponent<InventorySlot>();
            if (s.item == null) {
                s.item = new InventoryItem(obj, count);
                s.gameObject.GetComponent<Image>().enabled = true;
                s.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(obj.imagePath);
                if (s.item.count > 1) {
                    s.gameObject.GetComponentInChildren<Text>().text = s.item.count.ToString();
                } else {
                    s.gameObject.GetComponentInChildren<Text>().text = "";
                }
                return true;
            } 
        }

        //No empty slots and no partial stacks to add into
        if (shouldDrop) {
            Discard(obj, count);
        }
        return false;
    }

    public static void RemoveItem(InventorySlot slot) {
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

    static void Discard(Item item, int count) {
        Vector3 tempPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 offset = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        GameObject go = new GameObject();
        go.name = item.name;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>(item.imagePath);
        sr.sortingLayerName = "InFrontOfPlayer";
        sr.sortingOrder = 10;
        go.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + offset;
        Vector2 bounds = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
        BoxCollider2D c = go.AddComponent<BoxCollider2D>();
        c.size = bounds;
        c.isTrigger = true;

        if (!(item is Potion)) {
            if (item.imagePath.StartsWith("Gem")) {
                go.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            } else if (go.name.Equals("algae") || go.name.Equals("snail")) {
                go.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            } else {
                go.transform.localScale = new Vector3(0.4f, 0.4f, 1);
            }
        }

        Pickups p = go.AddComponent<Pickups>();
        p.Item = item;
        p.Count = count;
        p.Data = new GarbageCollecter.DroppedItemData(item, count, go.transform.position.x, go.transform.position.y, go.transform.position.z, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        GameObject.Find("GarbageCollector").GetComponent<GarbageCollecter>().droppedItems.Add(p.Data);
    }

    public static void DropItem(InventorySlot slot) {

        Vector3 tempPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 offset = new Vector3(Random.Range(-1, 1), Random.Range(-1, 0.3f), 0);
        GameObject go = new GameObject();
        go.name = slot.item.item.name;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>(slot.item.item.imagePath);
        sr.sortingLayerName = "InFrontOfPlayer";
        sr.sortingOrder = 10;
        go.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + offset;      
        Vector2 bounds = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
        BoxCollider2D c = go.AddComponent<BoxCollider2D>();
        c.size = bounds;
        c.isTrigger = true;

        if (!(slot.item.item is Potion)) {
            if (slot.item.item.imagePath.StartsWith("Gem")) {
                go.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            } else if (go.name.Equals("algae") || go.name.Equals("snail")) {
                go.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            } else {
                go.transform.localScale = new Vector3(0.4f, 0.4f, 1);
            }
        }
             
        Pickups p = go.AddComponent<Pickups>();
        p.Item = slot.item.item;
        p.Count = slot.item.count;
        p.Data = new GarbageCollecter.DroppedItemData(slot.item.item, slot.item.count, go.transform.position.x, go.transform.position.y, go.transform.position.z, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        GameObject.Find("GarbageCollector").GetComponent<GarbageCollecter>().droppedItems.Add(p.Data);
        RemoveStack(slot);
    }

    public static void RemoveStack(InventorySlot item) {
        ResourceLoader rl = GameObject.FindObjectOfType<ResourceLoader> ();
        if (rl.activeItem == item) {
            rl.activeItem = null;
            item.gameObject.GetComponentsInChildren<Image>()[1].enabled = false;
        }
        item.gameObject.GetComponentInChildren<Image>().enabled = false;
        item.gameObject.GetComponentInChildren<Text>().text = "";
        item.item = null;
    }
}
