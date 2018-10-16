using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : Object {
    public enum Attributes { sleep, healing, transformation, poison, invisible, magicPP, magicMM, cat, chicken, sheep };
    public enum Modifiers { magicPP, magicMM, cat, chicken, sheep };

    public Attributes[] attributeList;
    public string name;
    public Sprite image;

    public Ingredient(Attributes[] attributeList, string name, Sprite image) {
        this.attributeList = attributeList;
        this.name = name;
        this.image = image;
    }
}
