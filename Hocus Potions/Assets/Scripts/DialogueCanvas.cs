using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCanvas : MonoBehaviour {
    public bool active;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (Resources.FindObjectsOfTypeAll(GetType()).Length > 1) {
            Destroy(gameObject);
        }
        active = false;
        gameObject.SetActive(false);
    }

}
