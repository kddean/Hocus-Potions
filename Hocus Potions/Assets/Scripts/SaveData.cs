using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveData {
    //Player Info
    public float x, y, z; 
    public string currentScene;
    public List<Player.PlayerStatus> playerStatus;
    public Potion lastTaken;
    public List<float> timers;
    public string currentCostume;
    public bool[] unlockedCostumes;

    //Resource Data
    public float maxMana;
    public float currentMana;

    //Chest Data
    public List<string> chestSlots;
    public List<StorageManager.StoreageData> chestContents;

    //Inventory Data
    public List<string> inventorySlots;
    public List<InventorySlot.SlotData> inventoryContents;

    //Cauldron Data
    public Ingredient[] cauldronContents;
    public int ingredientCount;
    public Potion brewingPotion;
    public int brewingStatus;
    public float brewingTime;
    public float currentTime;

    //Ingredient Data
    public List<Ingredient> ingredNames;
    public List<List<Ingredient.Attributes>> knownAtts;
    public List<string> discoveredKeys;
    public List<bool> discoveredValues;

    //Garden Data
    public List<string> gardenPlots;
    public List<Garden.PlotData> gardenData;

    //Dropped Item Data
    public List<GarbageCollecter.DroppedItemData> droppedItems;

    //Date/Time Data
    public int hour;
    public int minute;
    public int day;
    public MoonCycle.PartOfDay dayPart;

    //NPC Data
    public List<string> npcNames;
    public List<NPCController.NPCInfo> npcInfo;
    public List<NPCController.Schedule> schedules;
    public List<string> scheduleNames;
    public List<string> npcNames2;
    public List<bool> questFlags;
    public int currentMap;

    //Keybinds
    public List<KeyCode> keybinds;

    //Faction Flags
    public bool endOrder;
    public bool endNature;
    public bool endSocial;

   
}
