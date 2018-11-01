using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ResourceLoader : MonoBehaviour {

    public Inventory inv;
    public Garden garden;
    public InventoryManager activeItem;
    public Dictionary<string, Ingredient> ingredients;
    public Dictionary<string, Seed> seeds;
    public Dictionary<string, Dictionary<string,string>> dialogueList;
    public Dictionary<string, Sprite> portraitList;
    public Dictionary<string, Sprite> charSpriteList;
    public Dictionary<string, string[]> requestList;
    public Dictionary<string, List<object>> npcGivenList;
    public List<string> availableNPCs;
    public TextAsset npcTextFile;
    public int givenListMax = 5;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    void Start () {
        dialogueList = new Dictionary<string, Dictionary<string, string>>();
        portraitList = new Dictionary<string, Sprite>();
        charSpriteList = new Dictionary<string, Sprite>();
        requestList = new Dictionary<string, string[]>();
        npcGivenList = new Dictionary<string, List<object>>();
        activeItem = null;
        garden = GameObject.Find("GardenManager").GetComponent<Garden>();
        CreateIngredients();
        CreateSeeds();
        CreateInventory();
        CreateNPCs();
      
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("inventory").transform.parent.gameObject);
        DontDestroyOnLoad(GameObject.Find("EventSystem"));
        //Just for force spawning inventory items for testing
        inv.Testing();
    }

    //TO DO: Swap sprites to proper inv sprites once we have them
    void CreateIngredients() {
        ingredients = new Dictionary<string, Ingredient> {
            { "lavender", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.sleep, Ingredient.Attributes.healing, Ingredient.Attributes.chicken }, "lavender", Resources.Load<Sprite>("Plants/lavender_inv")) },
            { "catnip", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.sleep, Ingredient.Attributes.transformation, Ingredient.Attributes.cat }, "catnip", Resources.Load<Sprite>("Plants/catnip_inv")) },
            { "nightshade", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.sleep, Ingredient.Attributes.healing }, "nightshade", Resources.Load<Sprite>("Plants/nightshade_inv")) },
            { "mugwort", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.magicPP, Ingredient.Attributes.transformation }, "mugwort", Resources.Load<Sprite>("Plants/mugwort_inv")) },
            { "lambsgrass", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.invisible, Ingredient.Attributes.healing, Ingredient.Attributes.sheep }, "lambsgrass", Resources.Load<Sprite>("Plants/lambsgrass_inv")) },
            { "poppy", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.invisible, Ingredient.Attributes.poison, Ingredient.Attributes.sleep }, "poppy", Resources.Load<Sprite>("Plants/poppy_inv")) }
        };
    }

    //TODO: Load from file
    void CreateSeeds() {
        seeds = new Dictionary<string, Seed> {
            { "lavender", new Seed("lavender", 180, 4, "Lavender seed", Resources.Load<Sprite>("Seeds/lavender_seed")) },
            { "catnip", new Seed("catnip", 180, 5, "Catnip seed", Resources.Load<Sprite>("Seeds/catnip_seed")) },
            { "nightshade", new Seed("nightshade", 180, 4, "Nightshade seed", Resources.Load<Sprite>("Seeds/nightshade_seed")) },
            { "mugwort", new Seed("mugwort", 180, 5, "Mugwort seed", Resources.Load<Sprite>("Seeds/mugwort_seed")) },
            { "lambsgrass", new Seed("lambsgrass", 180, 5, "Lambsgrass seed", Resources.Load<Sprite>("Seeds/lambsgrass_seed")) },
            { "poppy", new Seed("poppy", 180, 3, "Poppy Seed", Resources.Load<Sprite>("Seeds/poppy_seed")) }
        };
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
