using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Potion : Item {
    public int brewingTime; //in minutes

    int duration;  //in minutes
    Ingredient.Attributes? primary;
    Ingredient.Attributes? secondary;
    Ingredient.Modifiers? modifier;

    public string description;

    public Potion(string name, string imagePath, int duration, Ingredient.Attributes? primary, Ingredient.Attributes? secondary, Ingredient.Modifiers? modifier, int brewingTime) {
        this.name = name;
        this.imagePath = imagePath;
        this.Duration = duration;
        this.primary = primary;
        this.secondary = secondary;
        this.modifier = modifier;
        this.brewingTime = brewingTime;
    }

    public Potion(string name, string imagePath, int brewingTime, string description)
    {
        this.name = name;
        this.imagePath = imagePath;
        this.brewingTime = brewingTime;
        this.description = description;

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
