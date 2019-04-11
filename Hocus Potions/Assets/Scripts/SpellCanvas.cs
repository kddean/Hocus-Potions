using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCanvas : MonoBehaviour {

    GameObject activeSpell;
    public Sprite[] smash;
    public Sprite[] ignite;
    public Sprite[] dredge;
    public Sprite[] growth;
    public void Awake() {
        DontDestroyOnLoad(this);
        if (Resources.FindObjectsOfTypeAll(GetType()).Length > 1) {
            Destroy(gameObject);
        }
        activeSpell = GameObject.Find("SpellActive");
    }
    float i;
    Sprite[] current;
    private void Start() {
        i = 0;
        current = growth;
    }

    private void Update() {
        if (i < current.Length) {
            activeSpell.GetComponent<Image>().sprite = current[(int)i];
        }
        i = (i + 0.02f * current.Length) % current.Length;
    }

    public void SetActiveSpell(int i) {
        ResourceLoader rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        if (rl.activeSpell == rl.spells[i]) {
            rl.activeSpell = null;
            activeSpell.GetComponent<Image>().enabled = false;
        } else {
            rl.activeSpell = rl.spells[i];
            activeSpell.GetComponent<Image>().enabled = true;
            switch (rl.activeSpell.SpellName) {
                case "Wild Growth":
                    current = growth;
                    i = 0;
                    break;
                case "Ignite":
                    current = ignite;
                    i = 0;
                    break;
                case "Smash":
                    current = smash;
                    i = 0;
                    break;
                case "Dredge":
                    current = dredge;
                    i = 0;
                    break;
                default:
                    break;
            }
        }
    }
}
