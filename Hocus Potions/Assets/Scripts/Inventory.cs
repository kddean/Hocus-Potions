using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inventory {
    public class inventoryItem {
        public int maxStack = 10;
        public int count = 1;
        public Object item;
        public string name;
        public Sprite image;

         
        public inventoryItem(Object o, string n, Sprite i) {
            item = o;
            name = n;
            image = i;
        }
    }


    public List<inventoryItem> inventory = new List<inventoryItem>();
    public int maxSize = 10;
    int currentSize = 0;
    bool display = false;
   

    public bool add(Object obj, string name, Sprite image) {
        bool add = true;
        Button[] invButtons = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<Button>();
        foreach(Button b in invButtons) {
            Debug.Log(b.GetComponent<InventoryManager>().item);
        }
        foreach (inventoryItem i in inventory) {
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
            inventory.Add(new inventoryItem(obj, name, image));
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

    public void removeItem(inventoryItem item) {
        Button[] invButtons = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<Button>();
        if (item.count == 1) {
            inventory.Remove(item);
            currentSize--;
            foreach (Button b in invButtons) {
                if (b.GetComponent<InventoryManager>().item == item) {
                    b.GetComponentInChildren<Text>().text = "";
                    b.GetComponentInChildren<Image>().sprite = null;
                    b.GetComponent<InventoryManager>().item = null;
                }
            }
        } else {
            item.count--;
            foreach (Button b in invButtons) {
                if (b.GetComponent<InventoryManager>().item == item) {
                    b.GetComponentInChildren<Text>().text = item.count.ToString();
                }
            }
        }
    }


    //TO DO: function to allow players to drop items from their inventory
    public void dropItem(inventoryItem item, Button b) {
        b.GetComponentInChildren<Image>().sprite = null;
        b.GetComponentInChildren<Text>().text = "";
        b.GetComponent<InventoryManager>().item = null;
        currentSize--;
        foreach(inventoryItem i in inventory) {
            if(i == item) {
                inventory.Remove(i);
                break;
            }
        }

    }

    //TO DO: function to allow items to be used from inventory
    void useItem() {

    }
}
