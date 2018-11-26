using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    NPCManager npcs;

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
        npcs = GameObject.FindObjectOfType<NPCManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SaveGame() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveData.dat");
        SaveData data = new SaveData();
        Vector3 pos = player.transform.position;
        data.x = pos.x;
        data.y = pos.y;
        data.z = pos.z;

        data.currentScene = SceneManager.GetActiveScene().name;
        data.playerStatus = player.Status;
        data.lastTaken = player.LastTaken;
        data.timers = new List<float>();
        foreach(Player.PlayerStatus status in player.Status) {
            data.timers.Add(player.StartTimers[status].duration - (Time.time - player.StartTimers[status].startTime));
        }

        data.maxMana = mana.MaxMana;
        data.currentMana = mana.CurrentMana;

        data.chestSlots = sm.storageChest.Keys.ToList();
        data.chestContents = sm.storageChest.Values.ToList();

        data.inventorySlots = new List<string>();
        data.inventoryContents = new List<InventorySlot.SlotData>();
        InventorySlot[] slots = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<InventorySlot>();
        foreach(InventorySlot s in slots) {
            data.inventorySlots.Add(s.gameObject.name);
            data.inventoryContents.Add(new InventorySlot.SlotData(s.transform.localPosition.x, s.transform.localPosition.y, s.transform.localPosition.z, s.transform.GetSiblingIndex(), s.item));
        }


        data.cauldronContents = rl.brewingIngredients;
        data.ingredientCount = rl.ingredientCount;
        data.brewingPotion = bm.Pot;
        data.brewingStatus = bm.Brewing;
        data.brewingTime = bm.BrewTime;
        data.currentTime = bm.CurrentTime;

        data.gardenPlots = garden.plots.Keys.ToList();
        data.gardenData = garden.plots.Values.ToList();

        data.droppedItems = gc.droppedItems;

        data.hour = mc.Hour;
        data.minute = mc.Minutes;
        data.day = mc.Days;

        data.npcNames = npcs.data.Keys.ToList();
        data.npcData = npcs.data.Values.ToList();
        data.returnQueue = npcs.returnQueue;
        data.spawned = npcs.Spawned;

        bf.Serialize(file, data);
        file.Close();
        gameObject.SetActive(false);

    }

    public void LoadGame() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        try {
            file = File.Open(Application.persistentDataPath + "/saveData.dat", FileMode.Open);
        } catch (DirectoryNotFoundException e) {
            return;
        }
        SaveData data = (SaveData)bf.Deserialize(file);

        //TODO:This is going to require a ton of setup to make sure all these objects/scripts exist before their data is set
        SceneManager.LoadScene(data.currentScene, LoadSceneMode.Single);
        player.transform.position = new Vector3(data.x, data.y, data.z);

        player.Status = data.playerStatus;
        player.LastTaken = data.lastTaken;

        if(player.Status.Count > 0) {
            for(int i = 0; i < player.Status.Count(); i++) {
                player.RestartTimers(player.Status[i], data.timers[i]);
            }
        }

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


        rl.brewingIngredients = data.cauldronContents;
        rl.ingredientCount = data.ingredientCount;
        bm.Pot = data.brewingPotion;

        bm.Brewing = data.brewingStatus;
        bm.BrewTime = data.brewingTime;
        bm.CurrentTime = data.currentTime;
        if(bm.Brewing == 1) {
            bm.Begin(bm.BrewTime, bm.Pot);
        }

        garden.plots.Clear();
        for(int i = 0; i < data.gardenPlots.Count; i++) {
            garden.plots.Add(data.gardenPlots[i], data.gardenData[i]);
        }

        gc.droppedItems = data.droppedItems;

        mc.Hour = data.hour;
        mc.Minutes = data.minute;
        mc.Days = data.day;
        mc.CurrentMoonPhase = mc.Days % 6;
        mc.moonPhase.sprite = mc.moonCycleSprites[mc.CurrentMoonPhase];
        if(mc.Hour >= 18 || mc.Hour < 6) {
            mc.timeImage.sprite = mc.timeOfDay[3];
        } else if( mc.Hour >= 6 && mc.Hour < 10) {
            mc.timeImage.sprite = mc.timeOfDay[0];
        } else if(mc.Hour >= 10 && mc.Hour < 14) {
            mc.timeImage.sprite = mc.timeOfDay[1];
        } else if(mc.Hour >= 14 && mc.Hour < 18) {
            mc.timeImage.sprite = mc.timeOfDay[2];
        }

        npcs.data.Clear();
        for(int i = 0; i < data.npcNames.Count; i++) {
            npcs.data.Add(data.npcNames[i], data.npcData[i]);
        }
        npcs.returnQueue = data.returnQueue;
        npcs.Spawned = data.spawned;

        file.Close();
        gameObject.SetActive(false);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
