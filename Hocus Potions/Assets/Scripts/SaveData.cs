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

    //TODO:Update this once i rewrite NPC handling to be persistent
    //NPC Data
    public List<string> npcNames;
    public List<NPCManager.NPCData> npcData;
    public SortedList<NPCManager.NPCData, string> returnQueue;
    public bool spawned;

}
