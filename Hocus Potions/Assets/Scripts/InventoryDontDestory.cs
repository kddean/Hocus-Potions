using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDontDestory : MonoBehaviour {

    public void Awake() {
        DontDestroyOnLoad(this);
        if (Resources.FindObjectsOfTypeAll(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }
}
