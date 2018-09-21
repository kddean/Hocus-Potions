using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion {
    //There's probably a better way to do this but it works for now
   // public enum Attributes { sleep, healing, transformation, posion, invisiblity, magicPP, magicMM, cat, sheep, chicken };

    string name;
    Sprite image;

    int duration;  //in whole hours
    Ingredient.Attributes primary;
    Ingredient.Attributes? secondary;
    Ingredient.Modifiers? modifier;

    public Potion(string name, Sprite image, int duration, Ingredient.Attributes primary, Ingredient.Attributes? secondary, Ingredient.Modifiers? modifier) {
        this.name = name;
        this.image = image;
        this.duration = duration;
        this.primary = primary;
        this.secondary = secondary;
        this.modifier = modifier;
    }
}
