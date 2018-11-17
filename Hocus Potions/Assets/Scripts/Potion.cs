using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Item {
    public int brewingTime; //in minutes

    int duration;  //in whole hours
    Ingredient.Attributes? primary;
    Ingredient.Attributes? secondary;
    Ingredient.Modifiers? modifier;

    public Potion(string name, Sprite image, int duration, Ingredient.Attributes? primary, Ingredient.Attributes? secondary, Ingredient.Modifiers? modifier, int brewingTime) {
        this.name = name;
        this.image = image;
        this.Duration = duration;
        this.primary = primary;
        this.secondary = secondary;
        this.modifier = modifier;
        this.brewingTime = brewingTime;
    }

    public Ingredient.Attributes? Primary {
        get {
            return primary;
        }
    }

    public Ingredient.Attributes? Secondary {
        get {
            return secondary;
        }
    }

    public Ingredient.Modifiers? Modifier {
        get {
            return modifier;
        }
    }

    public int Duration {
        get {
            return duration;
        }

        set {
            duration = value;
        }
    }
}
