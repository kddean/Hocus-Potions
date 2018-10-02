using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceLoader : MonoBehaviour {

    public Inventory inv;
    public List<Ingredient> ingredients;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }
    //To DO: swap this to load from a saved data file rather than creating it from scratch every time
    void Start () {
        createIngredients();
        createInventory();
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("inventory").transform.parent.gameObject);
        //Just for testing stack combining
        inv.testing();
    }

    //TO DO: Add images for each ingredient once we have sprites for them
    void createIngredients() {
        ingredients = new List<Ingredient>();
        ingredients.Add(new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.sleep, Ingredient.Attributes.healing, Ingredient.Attributes.chicken }, "lavender", Resources.Load<Sprite>("Ingredients/lavender")));
        ingredients.Add(new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.sleep, Ingredient.Attributes.transformation, Ingredient.Attributes.cat }, "catnip", Resources.Load<Sprite>("Ingredients/catnip")));
        ingredients.Add(new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.sleep, Ingredient.Attributes.healing }, "nightshade", Resources.Load<Sprite>("Ingredients/nightshade")));
        ingredients.Add(new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.magicPP, Ingredient.Attributes.transformation }, "mugwort", Resources.Load<Sprite>("Ingredients/mugwort")));
        ingredients.Add(new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.invisible, Ingredient.Attributes.healing, Ingredient.Attributes.sheep }, "lambsgrass", Resources.Load<Sprite>("Ingredients/lambsgrass")));
        ingredients.Add(new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.invisible, Ingredient.Attributes.poison, Ingredient.Attributes.sleep }, "poppy", Resources.Load<Sprite>("Ingredients/poppy")));
    }

    void createInventory() {
        inv = new Inventory();
        /*if(save file exists){
             pull data from that to fill inventory
          } */
    }
}
