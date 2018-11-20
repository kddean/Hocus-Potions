using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour {

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }
    public struct StoreageData {
        public Item item;
        public int count;
        public int index;
        public Vector3 position;

        public StoreageData(Item item, int count, int index, Vector3 position) {
            this.item = item;
            this.count = count;
            this.index = index;
            this.position = position;
        }
    }


    public Dictionary<string, StoreageData> storageChest;
    // Use this for initialization
    void Start () {
        storageChest = new Dictionary<string, StoreageData>();
	}

    public void OpenChest() {
        foreach (string key in storageChest.Keys) {
            GameObject obj = GameObject.Find(key);
            obj.transform.localPosition = storageChest[key].position;
            obj.transform.SetSiblingIndex(storageChest[key].index);
            if(storageChest[key].item != null) {
                obj.GetComponent<Image>().enabled = true;
                obj.GetComponent<Image>().sprite = storageChest[key].item.image;
                obj.GetComponent<StorageSlot>().item = storageChest[key].item;
                obj.GetComponent<StorageSlot>().count = storageChest[key].count;
                if (storageChest[key].count != 1) {
                    obj.GetComponentInChildren<Text>().text = storageChest[key].count.ToString();
                } else {
                    obj.GetComponentInChildren<Text>().text = "";
                }
            } else {
                obj.GetComponent<Image>().enabled = false;
                obj.GetComponentInChildren<Text>().text = "";
            }
        }
    }
}
