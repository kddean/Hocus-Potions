using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
         foreach (inventoryItem i in inventory) {
                 if (i.name.Equals(name)) {
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
            inventory.Add(new inventoryItem(obj, name, image));
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


    //TO DO: function to allow players to drop items from their inventory
    void dropItem() {

    }

    //TO DO: function to allow items to be used from inventory
    void useItem() {

    }
}
