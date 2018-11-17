using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inventory {
    public class InventoryItem {
        public int maxStack = 10;
        public int count = 1;
        public Item item;

        public InventoryItem(Item o, int c, int m) {
            item = o;
            maxStack = m;
            count = c;
        }
    }

    public List<InventoryItem> inventory; 
    public int maxSize = 10;
    int currentSize = 3;

    //Just for testing stack combining
    public void Testing() {
        ResourceLoader rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        Brewing b = new Brewing();
        Potion p = b.Brew(rl.ingredients["catnip"], rl.ingredients["mugwort"], rl.ingredients["lily"]);
        Seed s = rl.seeds["lavender"];
        Seed ss = rl.seeds["poppy"];
        Seed sss = rl.seeds["nightshade"];
        Ingredient i = rl.ingredients["thistle"];
        Ingredient ii = rl.ingredients["catnip"];
        Ingredient iii = rl.ingredients["lambsgrass"];
        inventory = new List<InventoryItem>() {
            new InventoryItem(p, 1, 10), new InventoryItem(p, 1, 10), new InventoryItem(p, 1, 10),
            new InventoryItem(s, 4, 10),  new InventoryItem(ss, 4, 10), new InventoryItem(sss, 4, 10),
            new InventoryItem(i, 8, 10), new InventoryItem(ii, 8, 10), new InventoryItem(iii, 8, 10)};

        Button[] invButtons = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<Button>();
        for (int j = 0; j < 3; j++) {
            invButtons[j].GetComponentInChildren<Text>().text = "";
            invButtons[j].GetComponentInChildren<Image>().sprite = p.image;
            invButtons[j].GetComponent<InventoryManager>().item = inventory[j];
        }

       
        invButtons[3].GetComponentInChildren<Text>().text = "4";
        invButtons[3].GetComponentInChildren<Image>().sprite = s.image;
        invButtons[3].GetComponent<InventoryManager>().item = inventory[3];
        invButtons[4].GetComponentInChildren<Text>().text = "4";
        invButtons[4].GetComponentInChildren<Image>().sprite = ss.image;
        invButtons[4].GetComponent<InventoryManager>().item = inventory[4];
        invButtons[5].GetComponentInChildren<Text>().text = "4";
        invButtons[5].GetComponentInChildren<Image>().sprite = sss.image;
        invButtons[5].GetComponent<InventoryManager>().item = inventory[5];
        invButtons[6].GetComponentInChildren<Text>().text = "8";
        invButtons[6].GetComponentInChildren<Image>().sprite = i.image;
        invButtons[6].GetComponent<InventoryManager>().item = inventory[6];
        invButtons[7].GetComponentInChildren<Text>().text = "8";
        invButtons[7].GetComponentInChildren<Image>().sprite = ii.image;
        invButtons[7].GetComponent<InventoryManager>().item = inventory[7];
        invButtons[8].GetComponentInChildren<Text>().text = "8";
        invButtons[8].GetComponentInChildren<Image>().sprite = iii.image;
        invButtons[8].GetComponent<InventoryManager>().item = inventory[8];
        currentSize = 9;
        inventory[3].count = 4;
        inventory[4].count = 4;
        inventory[5].count = 4;
        inventory[6].count = 8;
        inventory[7].count = 8;
        inventory[8].count = 8;
    }
   

    public bool Add(Item obj, int count, int max) {
        bool add = true;
        Button[] invButtons = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<Button>();

        foreach (InventoryItem i in inventory) {
            if (i.item.name.Equals(obj.name)) {
                if ((i.count + count) <= i.maxStack) {
                    i.count += count;
                    add = false;
                    foreach (Button b in invButtons) {
                        if (b.GetComponent<InventoryManager>().item == i) {
                            b.GetComponentInChildren<Text>().text = i.count.ToString();
                            break;
                        }
                    }
                } else if (currentSize < maxSize) {
                    int remaining = count - (maxSize - i.count);
                    i.count = maxSize;
                    add = false;
                    inventory.Add(new InventoryItem(obj, remaining, max));
                    foreach (Button b in invButtons) {
                        if(b.GetComponent<InventoryManager>().item == null) {
                            b.GetComponentInChildren<Text>().text = remaining.ToString();
                            b.GetComponentInChildren<Image>().sprite = obj.image;
                            b.GetComponent<InventoryManager>().item = inventory[currentSize];
                            break;
                        }
                    }
                }
                break;
            }
        }
        if (!add) {
            return true;
        }

        if (currentSize < maxSize) {
            inventory.Add(new InventoryItem(obj, count, max));
            foreach (Button b in invButtons) {
                if (b.GetComponent<InventoryManager>().item == null) {
                    b.GetComponentInChildren<Text>().text = count.ToString();
                    b.GetComponentInChildren<Image>().sprite = obj.image;
                    b.GetComponent<InventoryManager>().item = inventory[currentSize];
                    break;
                }
            }
            currentSize++;
            return true;
        } else {
            Debug.Log("inventory full");
            return false;
        }
    }

    public void RemoveItem(InventoryItem item) {
        Button[] invButtons = GameObject.FindGameObjectWithTag("inventory").transform.parent.GetComponentsInChildren<Button>();
        if (item.count == 1) {
            foreach (Button b in invButtons) {
                if (b.GetComponent<InventoryManager>().item == item) {
                    RemoveStack(item, b);
                    break;
                }
            }
        } else {
            item.count--;
            foreach (Button b in invButtons) {
                if (b.GetComponent<InventoryManager>().item == item) {
                    if (item.count == 1) {
                        b.GetComponentInChildren<Text>().text = "";
                    } else {
                        b.GetComponentInChildren<Text>().text = item.count.ToString();
                    }
                }
            }
        }
    }

    public void DropItem(InventoryItem item, Button b) {
        GameObject go = new GameObject();
        go.name = item.item.name;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        go.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + offset;
        sr.sprite = item.item.image;
        if (item.item is Seed) {
            go.transform.localScale = new Vector3(0.4f, 0.4f, 1);
        }
          

        Vector2 bounds = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
        BoxCollider2D c = go.AddComponent<BoxCollider2D>();
        c.size = bounds;
        c.isTrigger = true;
        Pickups p = go.AddComponent<Pickups>();
        p.Item = item.item;
        p.Count = item.count;
        p.Data = new GarbageCollecter.DroppedItemData(item.item, item.count, go.transform.position, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, go);
        GameObject.Find("GarbageCollector").GetComponent<GarbageCollecter>().droppedItems.Add(p.Data);
        RemoveStack(item, b);
    }

    public void RemoveStack(InventoryItem item, Button b) {
        ResourceLoader rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        if (rl.activeItem == b.GetComponent<InventoryManager>()) {
            rl.activeItem = null;
        }
        inventory.Remove(item);
        b.GetComponentInChildren<Image>().sprite = null;
        b.GetComponentInChildren<Text>().text = "";
        b.GetComponent<InventoryManager>().item = null;
        currentSize--;
    }

    //TODO: function to allow items to be used from inventory
    void UseItem() {

    }
}
