using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ingredient : Item {
    public enum Attributes { sleep, healing, transformation, mana, speed, poison, invisibility, none};
    public enum Modifiers { magicPP, magicMM, cat, chicken, sheep };

    public Attributes[] attributeList;

    public Ingredient(Attributes[] attributeList, string name, string imagePath) {
        this.attributeList = attributeList;
        this.name = name;
        this.imagePath = imagePath;
    }
}
