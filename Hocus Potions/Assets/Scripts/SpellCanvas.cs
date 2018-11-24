using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCanvas : MonoBehaviour {

    public void Awake() {
        DontDestroyOnLoad(this);
        if (Resources.FindObjectsOfTypeAll(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    public void SetActiveSpell(int i) {
        ResourceLoader rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        if (rl.activeSpell == rl.spells[i]) {
            rl.activeSpell = null;
        } else {
            rl.activeSpell = rl.spells[i];
        }
    }
}
