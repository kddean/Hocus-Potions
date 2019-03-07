using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    Player player;
    StorageManager sm;
    ResourceLoader rl;
    BrewingManager bm;
    Garden garden;
    GarbageCollecter gc;
    MoonCycle mc;
    Mana mana;
    NPCController npcs;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (Resources.FindObjectsOfTypeAll(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        sm = GameObject.FindObjectOfType<StorageManager>();
        rl = GameObject.FindObjectOfType<ResourceLoader>();
        bm = GameObject.FindObjectOfType<BrewingManager>();
        garden = GameObject.FindObjectOfType<Garden>();
        gc = GameObject.FindObjectOfType<GarbageCollecter>();
        mc = GameObject.FindObjectOfType<MoonCycle>();
        mana = GameObject.FindObjectOfType<Mana>();
        npcs = GameObject.FindObjectOfType<NPCController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SaveGame() {
        StartCoroutine(PopulateSaveData());

    }

     IEnumerator PopulateSaveData() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveData.dat");
        SaveData data = new SaveData();
        NPC[] activeNPCs = GameObject.FindObjectsOfType<NPC>();

        foreach (NPC n in activeNPCs) {
            n.saving = true;
        }
        npcs.saving = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Vector3 pos = player.transform.position;
        data.x = pos.x;
        data.y = pos.y;
        data.z = pos.z;

        data.currentScene = SceneManager.GetActiveScene().name;
        data.playerStatus = player.Status;
        data.lastTaken = player.LastTaken;
        data.timers = new List<float>();
        foreach (Player.PlayerStatus status in player.Status) {
            data.timers.Add(player.StartTimers[status].duration - (Time.time - player.StartTimers[status].startTime));
        }

        data.currentCostume = GameObject.FindObjectOfType<Wardrobe>().Current;
        data.unlockedCostumes = GameObject.FindObjectOfType<Wardrobe>().Unlocked;
        data.maxMana = mana.MaxMana;
        data.currentMana = mana.CurrentMana;

        data.chestSlots = sm.storageChest.Keys.ToList();
        data.chestContents = sm.storageChest.Values.ToList();

        data.inventorySlots = new List<string>();
        data.inventoryContents = new List<InventorySlot.SlotData>();
        InventorySlot[] slots = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>();
        foreach (InventorySlot s in slots) {
            data.inventorySlots.Add(s.gameObject.name);
            data.inventoryContents.Add(new InventorySlot.SlotData(s.transform.localPosition.x, s.transform.localPosition.y, s.transform.localPosition.z, s.transform.GetSiblingIndex(), s.item));
        }


        data.cauldronContents = rl.brewingIngredients;
        data.ingredientCount = rl.ingredientCount;
        data.brewingPotion = bm.Pot;
        data.brewingStatus = bm.Brewing;
        data.brewingTime = bm.BrewTime;
        data.currentTime = bm.CurrentTime;

        data.ingredNames = rl.knownAttributes.Keys.ToList();
        data.knownAtts = rl.knownAttributes.Values.ToList();
        data.discoveredKeys = GameObject.FindObjectOfType<BookManager>().potionDiscovery.Keys.ToList();
        data.discoveredValues = GameObject.FindObjectOfType<BookManager>().potionDiscovery.Values.ToList();

        data.gardenPlots = garden.plots.Keys.ToList();
        data.gardenData = garden.plots.Values.ToList();

        data.droppedItems = gc.droppedItems;

        data.hour = mc.Hour;
        data.minute = mc.Minutes;
        data.day = mc.Days;
        data.dayPart = mc.DayPart;

        data.npcNames = npcs.npcData.Keys.ToList();
        data.npcInfo = npcs.npcData.Values.ToList();
        data.schedules = npcs.npcQueue.Keys.ToList();
        data.scheduleNames = npcs.npcQueue.Values.ToList();
        data.currentMap = npcs.CurrentMap;
        
        bf.Serialize(file, data);
        file.Close();
        gameObject.SetActive(false);
    }


    public void LoadGame() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        try {
            file = File.Open(Application.persistentDataPath + "/saveData.dat", FileMode.Open);
        } catch (DirectoryNotFoundException) {
            return;
        } catch (IsolatedStorageException) {
            return;
        }

        SaveData data = (SaveData)bf.Deserialize(file);

        //TODO:This is going to require a ton of setup to make sure all these objects/scripts exist before their data is set
        StartCoroutine(SceneLoader(data));

        player.transform.position = new Vector3(data.x, data.y, data.z);
        player.LastTaken = data.lastTaken;

        bm.Brewing = data.brewingStatus;
        GameObject.FindObjectOfType<Wardrobe>().Current = data.currentCostume;
        mana.MaxMana = data.maxMana;
        mana.CurrentMana = data.currentMana;
        mana.UpdateMana(0);

        sm.storageChest.Clear();
        for (int i = 0; i < data.chestSlots.Count; i++) {
            sm.storageChest.Add(data.chestSlots[i], data.chestContents[i]);
        }

        for (int i = 0; i < data.inventorySlots.Count; i++) {
            InventorySlot slot = GameObject.Find(data.inventorySlots[i]).GetComponent<InventorySlot>();
            slot.transform.SetSiblingIndex(data.inventoryContents[i].siblingIndex);
            slot.transform.localPosition = new Vector3(data.inventoryContents[i].x, data.inventoryContents[i].y, data.inventoryContents[i].z);
            slot.item = data.inventoryContents[i].item;
            if (slot.item == null) {
                slot.GetComponent<Image>().enabled = false;
                slot.GetComponentInChildren<Text>().text = "";
            } else {
                slot.GetComponent<Image>().enabled = true;
                slot.GetComponent<Image>().sprite = Resources.Load<Sprite>(slot.item.item.imagePath);
                if (slot.item.count > 1) {
                    slot.GetComponentInChildren<Text>().text = slot.item.count.ToString();
                } else {
                    slot.GetComponentInChildren<Text>().text = "";
                }
            }
        }

        file.Close();
    }

    public void QuitGame() {
        Application.Quit();
    }

     IEnumerator SceneLoader(SaveData data) {
        AsyncOperation scene = SceneManager.LoadSceneAsync(data.currentScene, LoadSceneMode.Single);
        while (!scene.isDone) {
            yield return null;
        }

        Scene check = SceneManager.GetSceneByName(data.currentScene);
        if (check.IsValid()) {
            player.Status = data.playerStatus;
            if (player.Status.Count > 0) {
                for (int i = 0; i < player.Status.Count(); i++) {
                    player.RestartTimers(player.Status[i], data.timers[i]);
                }
            }
            Wardrobe wardrobe = GameObject.FindObjectOfType<Wardrobe>();
            if (wardrobe != null) {
                wardrobe.LoadCostume(data.currentCostume);
                wardrobe.Unlocked = data.unlockedCostumes;
            }

            rl.brewingIngredients = data.cauldronContents;
            rl.ingredientCount = data.ingredientCount;
            for (int i = 0; i < data.ingredNames.Count; i++) {
                rl.knownAttributes.Add(data.ingredNames[i], data.knownAtts[i]);
            }

            Cauldron cauldron = GameObject.FindObjectOfType<Cauldron>();
            if (cauldron != null) {
                cauldron.UpdateText();
            }

            bm.Pot = data.brewingPotion;
            bm.BrewTime = data.brewingTime;
            bm.CurrentTime = data.currentTime;
            if (bm.Brewing == 1) {
                bm.Begin(bm.BrewTime, bm.Pot);
            }

            garden.plots.Clear();
            for (int i = 0; i < data.gardenPlots.Count; i++) {
                garden.plots.Add(data.gardenPlots[i], data.gardenData[i]);
            }
            List<string> keys = garden.plots.Keys.ToList();
            foreach (string s in keys) {
                if (SceneManager.GetActiveScene().name.Equals(garden.plots[s].plotScene)) {
                    SpriteRenderer[] renderers = GameObject.Find(s).GetComponentsInChildren<SpriteRenderer>();
                    for (int i = 1; i < 4; i++) {
                        renderers[i].sprite = Resources.LoadAll<Sprite>("Plants/" + garden.plots[s].type)[garden.plots[s].index];
                    }
                }
            }

            mc.Hour = data.hour;
            mc.Minutes = data.minute;
            mc.Days = data.day;
            mc.DayPart = data.dayPart;
            mc.CurrentMoonPhase = mc.Days % 6;
            mc.moonPhase.sprite = mc.moonCycleSprites[mc.CurrentMoonPhase];
            if (mc.Hour >= 18 || mc.Hour < 6) {
                mc.timeImage.sprite = mc.timeOfDay[3];
            } else if (mc.Hour >= 6 && mc.Hour < 10) {
                mc.timeImage.sprite = mc.timeOfDay[0];
            } else if (mc.Hour >= 10 && mc.Hour < 14) {
                mc.timeImage.sprite = mc.timeOfDay[1];
            } else if (mc.Hour >= 14 && mc.Hour < 18) {
                mc.timeImage.sprite = mc.timeOfDay[2];
            }

            npcs.npcData.Clear();
            for (int i = 0; i < data.npcNames.Count; i++) {
                npcs.npcData.Add(data.npcNames[i], data.npcInfo[i]);
            }

            npcs.npcQueue.Clear();
            for (int i = 0; i < data.schedules.Count; i++) {
                npcs.npcQueue.Add(data.schedules[i], data.scheduleNames[i]);
            }

            npcs.CurrentMap = data.currentMap;
            npcs.LoadNPCS();

            GameObject.FindObjectOfType<BookManager>().potionDiscovery.Clear();
            for (int i = 0; i < data.discoveredKeys.Count; i++)
            {
                GameObject.FindObjectOfType<BookManager>().potionDiscovery.Add(data.discoveredKeys[i], data.discoveredValues[i]);
            }

            gc.droppedItems = data.droppedItems;
            GameObject.Find("GarbageCollector").GetComponent<GarbageCollecter>().SpawnDropped();
            SceneManager.SetActiveScene(check);
            gameObject.SetActive(false);
        }
    }

}
