using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour {
    
    public Dictionary<string, StoreageData> storageChest;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
        storageChest = new Dictionary<string, StoreageData>();
    }

    [System.Serializable]
    public struct StoreageData {
        public Item item;
        public int count;
        public int index;
        public float x, y, z;

        public StoreageData(Item item, int count, int index, float x, float y, float z) {
            this.item = item;
            this.count = count;
            this.index = index;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }


    public void OpenChest() {
        foreach (string key in storageChest.Keys) {
            GameObject obj = GameObject.Find(key);
            obj.transform.localPosition = new Vector3(storageChest[key].x, storageChest[key].y, storageChest[key].z);
            obj.transform.SetSiblingIndex(storageChest[key].index);
            if(storageChest[key].item != null) {
                obj.GetComponent<Image>().enabled = true;
                obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(storageChest[key].item.imagePath);
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
