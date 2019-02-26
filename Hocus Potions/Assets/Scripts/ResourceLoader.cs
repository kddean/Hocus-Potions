using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ResourceLoader : MonoBehaviour {

    public Garden garden;
    public GatheringManager gatheringManager;
    public InventorySlot activeItem;
    public Spell activeSpell;
    public Dictionary<string, Ingredient> ingredients;
    public Dictionary<string, Seed> seeds;
    public Dictionary<string, Dictionary<string, List<string>>> dialogueList;
    public Dictionary<string, Sprite> portraitList;
    public Dictionary<string, Sprite> charSpriteList;
    public Dictionary<string, List<Request>> requestList;
    public Dictionary<string, List<object>> npcGivenList;
    public Dictionary<Ingredient, List<Ingredient.Attributes>> knownAttributes;
    public Ingredient[] brewingIngredients;
    public List<string> availableNPCs;
    public List<Spell> spells;
    public TextAsset npcData, npcSchedule;
    public int givenListMax = 5;
    public float potionStrength = 1;
    public int ingredientCount;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    void Start() {
        dialogueList = new Dictionary<string, Dictionary<string, List<string>>>();
        portraitList = new Dictionary<string, Sprite>();
        charSpriteList = new Dictionary<string, Sprite>();
        requestList = new Dictionary<string, List<Request>>();
        npcGivenList = new Dictionary<string, List<object>>();
        knownAttributes = new Dictionary<Ingredient, List<Ingredient.Attributes>>();
        brewingIngredients = new Ingredient[3];

        activeItem = null;
        activeSpell = null;
        ingredientCount = 0;
        garden = GameObject.Find("GardenManager").GetComponent<Garden>();
        gatheringManager = GameObject.Find("GatheringManager").GetComponent<GatheringManager>();

        CreateIngredients();
        CreateSeeds();
        CreateSpells();
        CreateInventory();
        CreateNPCs();
        LoadNPCData();

        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("inventory").transform.parent.gameObject);
        DontDestroyOnLoad(GameObject.Find("EventSystem"));
        //Just for force spawning inventory items for testing
        Inventory.Testing();
        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
    }

    //TO DO: Swap sprites to proper inv sprites once we have them
    void CreateIngredients() {
        ingredients = new Dictionary<string, Ingredient> {
            { "lavender", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.sleep, Ingredient.Attributes.healing, Ingredient.Attributes.mana, Ingredient.Attributes.dyePurple }, "lavender", "Plants/lavender_inv") },
            { "catnip", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.sleep, Ingredient.Attributes.transformation, Ingredient.Attributes.speed }, "catnip", "Plants/catnip_inv") },
            { "nightshade", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.sleep, Ingredient.Attributes.healing, Ingredient.Attributes.dyeBlack }, "nightshade", "Plants/nightshade_inv") },
            { "mugwort", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.mana, Ingredient.Attributes.transformation, Ingredient.Attributes.endChaos }, "mugwort", "Plants/mugwort_inv") },
            { "lambsgrass", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.invisibility, Ingredient.Attributes.healing, Ingredient.Attributes.speed, Ingredient.Attributes.dyeYellow }, "lambsgrass", "Plants/lambsgrass_inv") },
            { "poppy", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.invisibility, Ingredient.Attributes.poison, Ingredient.Attributes.sleep, Ingredient.Attributes.endOrder }, "poppy", "Plants/poppy_inv") },
            { "thistle", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.invisibility, Ingredient.Attributes.speed, Ingredient.Attributes.mana, Ingredient.Attributes.dyePurple }, "thistle", "Plants/thistle_inv") },
            { "lily", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.transformation, Ingredient.Attributes.mana, Ingredient.Attributes.none }, "lily", "Plants/lily_inv") },
            { "indigo", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.mana, Ingredient.Attributes.poison, Ingredient.Attributes.sleep, Ingredient.Attributes.dyeBlue }, "indigo", "Plants/indigo_inv") },
            { "dandylion", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.dyeYellow, Ingredient.Attributes.endOrder, Ingredient.Attributes.healing, Ingredient.Attributes.transformation }, "dandylion", "Plants/dandylion_inv") },
            { "ghostcap", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.dyeWhite, Ingredient.Attributes.invisibility, Ingredient.Attributes.endSocial, Ingredient.Attributes.poison }, "ghostcap", "Plants/ghostcap_inv") },
            { "morel", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.none, Ingredient.Attributes.none }, "morel", "Plants/morel_inv") },
            { "fly_agaric", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.poison, Ingredient.Attributes.none, Ingredient.Attributes.none }, "fly_agaric", "Plants/fly_agaric_inv") },
            { "ash", new Ingredient(new Ingredient.Attributes[] {Ingredient.Attributes.dyeBlack, Ingredient.Attributes.dyeBlue, Ingredient.Attributes.dyeGreen, Ingredient.Attributes.dyeRed, Ingredient.Attributes.dyeWhite, Ingredient.Attributes.dyeYellow}, "ash", "Other/ash_inv") },
            { "amethyst", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.dyePurple, Ingredient.Attributes.sleep, Ingredient.Attributes.mana }, "amethyst", "Gems/amethyst_inv") },
            { "selenite", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.dyeWhite, Ingredient.Attributes.healing, Ingredient.Attributes.endChaos }, "selenite", "Gems/selenite_inv") },
            { "lapis_lazuli", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.dyeBlue, Ingredient.Attributes.mana, Ingredient.Attributes.none }, "lapis_lazuli", "Gems/lapis_inv") },
            { "emerald", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.dyeGreen, Ingredient.Attributes.endOrder, Ingredient.Attributes.none }, "emerald", "Gems/emerald_inv") },
            { "amber", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.dyeYellow, Ingredient.Attributes.endSocial, Ingredient.Attributes.transformation }, "amber", "Gems/amber_inv") },
            { "garnet", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.dyeRed, Ingredient.Attributes.none, Ingredient.Attributes.none }, "garnet", "Gems/garnet_inv") },
            { "jet", new Ingredient(new Ingredient.Attributes[] { Ingredient.Attributes.dyeBlack, Ingredient.Attributes.speed, Ingredient.Attributes.transformation }, "jet", "Gems/jet_inv") },
        };

        foreach(Ingredient i in ingredients.Values.ToList()) {
            knownAttributes.Add(i, new List<Ingredient.Attributes>());
        }
    }

    //TODO: Load from file
    void CreateSeeds() {
        seeds = new Dictionary<string, Seed> {
            { "lavender", new Seed("lavender", 180, 4, "Lavender seed", "Seeds/lavender_seed") },
            { "catnip", new Seed("catnip", 180, 5, "Catnip seed", "Seeds/catnip_seed") },
            { "nightshade", new Seed("nightshade", 180, 4, "Nightshade seed", "Seeds/nightshade_seed") },
            { "mugwort", new Seed("mugwort", 180, 5, "Mugwort seed", "Seeds/mugwort_seed") },
            { "lambsgrass", new Seed("lambsgrass", 180, 5, "Lambsgrass seed", "Seeds/lambsgrass_seed") },
            { "poppy", new Seed("poppy", 180, 3, "Poppy Seed", "Seeds/poppy_seed") },
            { "thistle", new Seed("thistle", 180, 4, "Thistle Seed", "Seeds/thistle_seed") },
            { "lily", new Seed("lily", 180, 5, "Lily Seed", "Seeds/lily_seed") }
        };
    }

    void CreateSpells() {
        spells = new List<Spell>();
        spells.Add(new Spell("Wild Growth", 25, null));
        spells.Add(new Spell("Ignite", 15, null));
        spells.Add(new Spell("Smash", 10, null));
        spells.Add(new Spell("Dredge", 10, null));
    }

    void CreateInventory() {
        InventorySlot[] slots = GameObject.FindObjectsOfType<InventorySlot>();
        foreach(InventorySlot s in slots) {
            s.item = null;
            s.gameObject.GetComponentsInChildren<Image>()[1].enabled = false;
        }
    }

    //TODO: Probably load this from a file at some point
    void LoadNPCData() {
        NPCController c = GameObject.FindObjectOfType<NPCController>();
        c.npcData = new Dictionary<string, NPCController.NPCInfo>();
        Regex.Replace(npcSchedule.text, "\r", String.Empty);
        Regex.Replace(npcSchedule.text, " ", String.Empty);
        string[] characterSegments = npcSchedule.text.Split('*');
        for (int i = 0; i < characterSegments.Length; i++) {
            string[] events = characterSegments[i].Split('\n');
            string characterName = events[0].Split(',')[0];
            List<NPCController.Schedule> schedule = new List<NPCController.Schedule>();
            for (int j = 1; j < events.Length; j++) {
                string[] data = events[j].Split(',');
                if (data[0].Equals(String.Empty)) { continue; }
                if (data.Length > 8) {
                    schedule.Add(new NPCController.Schedule(Boolean.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]), data[4], int.Parse(data[5]), float.Parse(data[6]), float.Parse(data[7]), float.Parse(data[8]), characterName));
                }
            }
            c.npcData.Add(characterName, new NPCController.NPCInfo(0, 0, 0, 1, 0, false, null, 0, new List<Item>(), schedule, false, new List<NPC.Status>(), new Dictionary<NPC.Status, NPCController.TimerData>(), new NPCController.Vec3(), new NPCController.Vec3()));
        }
    }

    //TODO: This needs some cleaning up; Probably don't actually need to some of this stuff
    void CreateNPCs() {
        Regex.Replace(npcData.text, "\r", String.Empty);
        string[] characterSegments = npcData.text.Split('\\'); //Split by character
        for (int i = 0; i < characterSegments.Length; i++) {
            string[] dataSegments = characterSegments[i].Split('@');       //split by segment
            string[] first = dataSegments[0].Split('\n');                  //populate name and sprites
            string key = first[0].Split(',')[0];
            availableNPCs.Add(key);
            portraitList.Add(key, Resources.Load<Sprite>("Characters/" + first[1].Split(',')[0]));
            charSpriteList.Add(key, Resources.Load<Sprite>("Characters/" + first[2].Split(',')[0]));

            //Fill dialogue dictionary
            Dictionary<string, List<string>> temp = new Dictionary<string, List<string>>();
            string[] dialogue = dataSegments[1].Split('\n');

            foreach (string s in dialogue) {
                List<string> dialogueSplit = Regex.Split(s, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").ToList();
                if (!dialogueSplit[0].Equals(String.Empty)) {
                    string k = dialogueSplit[0];
                    dialogueSplit.RemoveAt(0);
                    dialogueSplit.RemoveAll(j => j.Equals(String.Empty));
                    for (int f = 0; f < dialogueSplit.Count; f++) {
                        dialogueSplit[f] = dialogueSplit[f].Replace("\"", "");
                    }
                    temp.Add(k, dialogueSplit);
                }
            }
            dialogueList.Add(key, temp);

            //TODO:Maybe change this if we handle requests differently
            string[] requests = dataSegments[2].Split('\n');
            List<Request> questList = new List<Request>();
            foreach (string req in requests) {
                List<string> r = Regex.Split(req, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").ToList();
                if (!r[0].Equals(String.Empty) && r.Count >= 9) {
                    r.RemoveAll(q => q.Equals(String.Empty));
                    Request newRequest = new Request(r[0], potionStrength, float.Parse(r[1]), float.Parse(r[2]), float.Parse(r[3]), float.Parse(r[4]), float.Parse(r[5]), float.Parse(r[6]), float.Parse(r[7]), float.Parse(r[8]));
                    questList.Add(newRequest);
                }
            }
            if (questList.Count > 0) {
                requestList.Add(key, questList);
            }
        }
    }
}
