using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell {
    string spellName;
    float cost;
    Sprite icon;

    public Spell(string spellName, float cost, Sprite icon) {
        this.spellName = spellName;
        this.cost = cost;
        this.icon = icon;
    }

    public string SpellName {
        get {
            return spellName;
        }
    }

    public float Cost {
        get {
            return cost;
        }

        set {
            cost = value;
        }
    }

    public Sprite Icon {
        get {
            return icon;
        }
    }
}
