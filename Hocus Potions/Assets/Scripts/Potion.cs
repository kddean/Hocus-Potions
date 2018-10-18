﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Object {

    public string name;
    public Sprite image;

    int duration;  //in whole hours
    Ingredient.Attributes? primary;
    Ingredient.Attributes? secondary;
    Ingredient.Modifiers? modifier;

    public Potion(string name, Sprite image, int duration, Ingredient.Attributes? primary, Ingredient.Attributes? secondary, Ingredient.Modifiers? modifier) {
        this.name = name;
        this.image = image;
        this.duration = duration;
        this.primary = primary;
        this.secondary = secondary;
        this.modifier = modifier;
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
}
