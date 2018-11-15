using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	
}
