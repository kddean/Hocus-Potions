using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour {
    public enum Attributes { sleep, healing, transformation, poison, invisible, magicPP, magicMM, cat, chicken, sheep };
    public enum Modifiers { magicPP, magicMM, cat, chicken, sheep };

    public Attributes[] attributeList;
    string name;
    public Sprite image;	
}
