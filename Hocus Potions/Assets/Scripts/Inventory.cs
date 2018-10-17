﻿using System.Collections;
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
        Potion p = b.Brew("nightshade", "nightshade", "poppy");
        inventory = new List<InventoryItem>() { new InventoryItem(p, p.name, p.image), new InventoryItem(p, p.name, p.image), new InventoryItem(p, p.name, p.image) };
        Button[] invButtons = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<Button>();
        for (int i = 0; i < 3; i++) {
            invButtons[i].GetComponentInChildren<Text>().text = "";
            invButtons[i].GetComponentInChildren<Image>().sprite = p.image;
            invButtons[i].GetComponent<InventoryManager>().item = inventory[i];
        }
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
            inventory.Remove(item);
            currentSize--;
            foreach (Button b in invButtons) {
                if (b.GetComponent<InventoryManager>().item == item) {
                    b.GetComponentInChildren<Text>().text = "";
                    b.GetComponentInChildren<Image>().sprite = null;
                    b.GetComponent<InventoryManager>().item = null;
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
        foreach(InventoryItem i in inventory) {
            if(i == item) {
                inventory.Remove(i);
                break;
            }
        }

    }

    //TO DO: function to allow items to be used from inventory
    void UseItem() {

    }
}
