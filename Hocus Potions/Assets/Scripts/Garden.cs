﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Garden : MonoBehaviour {

    public Sprite emptyPlot;

    public enum Status { empty, growing, harvestable };
    public Dictionary<string, PlotData> plots;
    ResourceLoader rl;
    MoonCycle mc;
    SteamAchievementManager sam;

    [System.Serializable]
    public struct PlotData {
        public Status stage;
        public int index;
        public string type;
        public int growthTime;
        public int currentTime;
        public string plotScene;
    };

    // Use this for initialization
    void Start() {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        plots = new Dictionary<string, PlotData>();
        mc = GameObject.Find("Clock").GetComponent<MoonCycle>();
        sam = GameObject.FindObjectOfType<SteamAchievementManager>();
        StartCoroutine(Grow());
    }

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }


    //Handles clicking on garden plots
    public void Farm(GardenPlot plot, Seed seed, InventorySlot slot) {
        PlotData data;
        if (plots.TryGetValue(plot.gameObject.name, out data)) {
            if (data.stage == Status.harvestable) {
                Harvest(plot);
            } else {
                return;
            }
        } else {
            if(seed == null) {
                GameObject inv = GameObject.FindGameObjectWithTag("inventory");
                inv.GetComponent<CanvasGroup>().alpha = 1;
                inv.GetComponent<CanvasGroup>().interactable = true;
                inv.GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }
            plot.gameObject.GetComponent<AudioSource>().Play();
            PlotData newData = new PlotData();
            //Set values for plot
            newData.stage = Status.growing;
            newData.type = seed.SeedType;
            newData.growthTime = seed.GrowthTime;
            newData.currentTime = 0;
            newData.index = 0;
            newData.plotScene = SceneManager.GetActiveScene().name;
            //Add plot to dict
            plots.Add(plot.gameObject.name, newData);
            //Remove seed from inv
            Inventory.RemoveItem(slot);
            //Set plot sprite
            SpriteRenderer[] sr = plot.gameObject.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 1; i < 4; i++) {
                sr[i].sprite = Resources.Load<Sprite>("Plants/" + seed.SeedType);
            }
        }
    }



    //Handles harvesting plots and adding ingredients to inventory
    void Harvest(GardenPlot plot) {
        PlotData data = plots[plot.gameObject.name];
        Inventory.Add(rl.ingredients[data.type], 1, true);
        SpriteRenderer[] sr = plot.gameObject.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 1; i < 4; i++) {
            sr[i].sprite = null;
        }
        plots.Remove(plot.gameObject.name);
    }

    public void SpellCast(GardenPlot plot) {
        PlotData data;
        Mana mana = GameObject.FindObjectOfType<Mana>();
        if (!plots.TryGetValue(plot.gameObject.name, out data)) {
            return;
        }
        if (rl.activeSpell.SpellName.Equals("Wild Growth") && data.stage == Status.harvestable) {
            return;
        }

        if (rl.activeSpell.SpellName.Equals("Wild Growth")) { 
            plot.gameObject.GetComponents<AudioSource>()[2].Play();
            data.index++;
            if (data.index == (rl.seeds[data.type].GrowthStages - 1)) {
                data.stage = Status.harvestable;
            }

            SpriteRenderer[] renderers = GameObject.Find(plot.gameObject.name).GetComponentsInChildren<SpriteRenderer>();
            for (int i = 1; i < 4; i++) {
                renderers[i].sprite = Resources.LoadAll<Sprite>("Plants/" + data.type)[data.index];
            }

            plot.gameObject.GetComponentInChildren<Animator>().SetTrigger("Growth");
            plots[plot.gameObject.name] = data;
        } else if (rl.activeSpell.SpellName.Equals("Ignite")) {
            if (GameObject.FindObjectOfType<SteamAchievementManager>() != null) {
                sam.UnlockAchievement(sam.m_Achievements[8]);
            }
            plot.gameObject.GetComponents<AudioSource>()[1].Play();
            plot.gameObject.GetComponentInChildren<Animator>().SetTrigger("Ignite");
            SpriteRenderer[] renderers = GameObject.Find(plot.gameObject.name).GetComponentsInChildren<SpriteRenderer>();
            for (int i = 1; i < 4; i++) {
                renderers[i].sprite = null;
            }
            plots.Remove(plot.gameObject.name);
            Inventory.Add(rl.ingredients["ash"], 1, true);
        }

        mana.UpdateMana(rl.activeSpell.Cost);
    }

    //Grows plants in code; only updates visuals if you're in the garden
    IEnumerator Grow() {
        if (plots.Count == 0) {
            yield return new WaitForSeconds(mc.CLOCK_SPEED);
            StartCoroutine(Grow());
        } else {
            List<string> keys = plots.Keys.ToList();
            foreach (string s in keys) {
                if (plots[s].stage == Status.harvestable) {
                    continue;
                }
                Seed seed = rl.seeds[plots[s].type];
                PlotData temp = plots[s];
                temp.currentTime += 10;
                if (temp.currentTime >= (temp.growthTime / seed.GrowthStages)) {
                    temp.currentTime = 0;
                    temp.index++;
                    //Mark as harvestable if fully grown
                    if (temp.index == (seed.GrowthStages - 1)) {
                        temp.stage = Status.harvestable;
                    }

                    //If you're in the garden update the sprites 
                    if (SceneManager.GetActiveScene().name.Equals(plots[s].plotScene)) {
                        SpriteRenderer[] renderers = GameObject.Find(s).GetComponentsInChildren<SpriteRenderer>();
                        for (int i = 1; i < 4; i++) {
                            renderers[i].sprite = Resources.LoadAll<Sprite>("Plants/" + temp.type)[temp.index];
                        }
                    }
                }

                //Update stored data
                plots[s] = temp;
            }
            yield return new WaitForSeconds(mc.CLOCK_SPEED);
            StartCoroutine(Grow());

        }
    }
}
