using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ResourceLoader : MonoBehaviour {

    public Inventory inv;
    public InventoryManager activeItem;
    public Dictionary<string, Ingredient> ingredients;
    public Dictionary<string, Dictionary<string,string>> dialogueList;
    public Dictionary<string, Sprite> portraitList;
    public Dictionary<string, Sprite> charSpriteList;
    public Dictionary<string, string[]> requestList;
    public Dictionary<string, object> givenObjects;
    public List<string> availableNPCs;
    public TextAsset npcTextFile;
    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }
    //To DO: swap this to load from a saved data file rather than creating it from scratch every time
    void Start () {
        dialogueList = new Dictionary<string, Dictionary<string, string>>();
        portraitList = new Dictionary<string, Sprite>();
        charSpriteList = new Dictionary<string, Sprite>();
        requestList = new Dictionary<string, string[]>();
        givenObjects = new Dictionary<string, object>();
        activeItem = null;
        CreateIngredients();
        CreateInventory();
        CreateNPCs();
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("inventory").transform.parent.gameObject);
        //Just for testing stack combining
        inv.Testing();
    }

    //TO DO: Add images for each ingredient once we have sprites for them
    void CreateIngredients() {
        ingredients = new Dictionary<string, Ingredient>();
        ingredients.Add("lavender", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.sleep, Ingredient.Attributes.healing, Ingredient.Attributes.chicken }, "lavender", Resources.Load<Sprite>("Ingredients/lavender")));
        ingredients.Add("catnip", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.sleep, Ingredient.Attributes.transformation, Ingredient.Attributes.cat }, "catnip", Resources.Load<Sprite>("Ingredients/catnip")));
        ingredients.Add("nightshade", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.sleep, Ingredient.Attributes.healing }, "nightshade", Resources.Load<Sprite>("Ingredients/nightshade")));
        ingredients.Add("mugwort", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.magicPP, Ingredient.Attributes.transformation }, "mugwort", Resources.Load<Sprite>("Ingredients/mugwort")));
        ingredients.Add("lambsgrass", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.invisible, Ingredient.Attributes.healing, Ingredient.Attributes.sheep }, "lambsgrass", Resources.Load<Sprite>("Ingredients/lambsgrass")));
        ingredients.Add("poppy", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.invisible, Ingredient.Attributes.poison, Ingredient.Attributes.sleep }, "poppy", Resources.Load<Sprite>("Ingredients/poppy")));
    }

    void CreateInventory() {
        inv = new Inventory();
        /*if(save file exists){
             pull data from that to fill inventory
          } */
    }


    void CreateNPCs() {
        availableNPCs = new List<string>();
        string cleanText = npcTextFile.text;
        cleanText = Regex.Replace(cleanText, @"\r", "");
        cleanText = Regex.Replace(cleanText, @"\n", "");
        string[] splitList = cleanText.Split('/');
        for (int i = 0; i < splitList.Length; i++) {
            string[] data = splitList[i].Split(';');
            availableNPCs.Add(data[0]);
            portraitList.Add(data[0], Resources.Load<Sprite>(data[1]));
            charSpriteList.Add(data[0], Resources.Load<Sprite>(data[2]));
            Dictionary<string, string> temp = new Dictionary<string, string>();
            string[] dialogueSplit = data[3].Split(':');
            for (int j = 0; j < dialogueSplit.Length; j++) {
                temp.Add(dialogueSplit[j].Split('=')[0], dialogueSplit[j].Split('=')[1]);
            }
            dialogueList.Add(data[0], temp);
            if (!data[4].Split(':')[0].Equals(String.Empty)) {
                requestList.Add(data[0], data[4].Split(':'));
            }
        }
    }
}
