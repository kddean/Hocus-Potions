using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inventory {
    public class InventoryItem {
        public int maxStack = 10;
        public int count = 1;
        public Object item;
        public string name;
        public Sprite image;

         
        public InventoryItem(Object o, string n, Sprite i) {
            item = o;
            name = n;
            image = i;
        }
    }

    public List<InventoryItem> inventory; 
    public int maxSize = 10;
    int currentSize = 3;
    bool display = false;

    //Just for testing stack combining
    public void Testing() {
        Brewing b = new Brewing();
        Potion p = b.Brew("nightshade", "nightshade", "mugwort");
        Seed s = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>().seeds["nightshade"];
        Seed ss = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>().seeds["catnip"];
        inventory = new List<InventoryItem>() { new InventoryItem(p, p.name, p.image), new InventoryItem(p, p.name, p.image), new InventoryItem(p, p.name, p.image),
            new InventoryItem(s, s.Name, s.Icon),  new InventoryItem(ss, ss.Name, s.Icon) };

        Button[] invButtons = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<Button>();
        for (int i = 0; i < 3; i++) {
            invButtons[i].GetComponentInChildren<Text>().text = "";
            invButtons[i].GetComponentInChildren<Image>().sprite = p.image;
            invButtons[i].GetComponent<InventoryManager>().item = inventory[i];
        }

       
        invButtons[3].GetComponentInChildren<Text>().text = "4";
        invButtons[3].GetComponentInChildren<Image>().sprite = s.Icon;
        invButtons[3].GetComponent<InventoryManager>().item = inventory[3];
        invButtons[4].GetComponentInChildren<Text>().text = "2";
        invButtons[4].GetComponentInChildren<Image>().sprite = ss.Icon;
        invButtons[4].GetComponent<InventoryManager>().item = inventory[4];
        currentSize = 5;
        inventory[3].count = 4;
        inventory[4].count = 2;
    }
   

    public bool Add(Object obj, string name, Sprite image) {
        bool add = true;
        Button[] invButtons = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<Button>();

        foreach (InventoryItem i in inventory) {
            if (i.name.Equals(name)) {
                if (i.count < i.maxStack) {
                    i.count++;
                    add = false;
                    foreach (Button b in invButtons) {
                        if (b.GetComponent<InventoryManager>().item == i) {
                            b.GetComponentInChildren<Text>().text = i.count.ToString();
                            break;
                        }
                    }
                }
            }
        }
        if (!add) {
            return true;
        }

        if (currentSize < maxSize) {
            inventory.Add(new InventoryItem(obj, name, image));
            foreach (Button b in invButtons) {
                if (b.GetComponent<InventoryManager>().item == null) {
                    b.GetComponentInChildren<Text>().text = "";
                    b.GetComponentInChildren<Image>().sprite = image;
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
        Button[] invButtons = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<Button>();
        if (item.count == 1) {
            foreach (Button b in invButtons) {
                if (b.GetComponent<InventoryManager>().item == item) {
                    DropItem(item, b);
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


    //TO DO: function to allow players to drop items from their inventory
    public void DropItem(InventoryItem item, Button b) {
        b.GetComponentInChildren<Image>().sprite = null;
        b.GetComponentInChildren<Text>().text = "";
        b.GetComponent<InventoryManager>().item = null;
        currentSize--;
        inventory.Remove(item);
        GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>().activeItem = null;
    }

    //TO DO: function to allow items to be used from inventory
    void UseItem() {

    }
}
