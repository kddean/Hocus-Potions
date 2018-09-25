using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
    public class inventoryItem {
        public int maxStack = 10;
        public int count = 1;
        public Object item;
        public string name;
         
        public inventoryItem(Object o, string n) {
            item = o;
            name = n;
        }
    }


    public List<inventoryItem> inventory = new List<inventoryItem>();
    int maxSize = 1;
    int currentSize = 0;
    bool display = false;

    public bool add(Object o, string n) {
        bool add = true;
         foreach (inventoryItem i in inventory) {
                 if (i.name.Equals(n)) {
                    if (i.count < i.maxStack) {
                        i.count++;
                        add = false;
                    }
                }
            }
        if (!add) {
            return true;
        }

        if (currentSize < maxSize) {
            inventory.Add(new inventoryItem(o, n));
            currentSize++;
            return true;
        } else {
            Debug.Log("inventory full");
            return false;
        }
    }

    public void remove(inventoryItem item) {
        if(item.count == 1) {
            inventory.Remove(item);
            currentSize--;
        } else {
            item.count--;
        }
    }

    //TO DO
    public void displayToggle() {
        

    }
}
