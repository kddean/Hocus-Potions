using System.Collections;
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
    public struct PlotData {
        public Status stage;
        public int index;
        public string type;
        public int growthTime;
        public int currentTime;
    };

    // Use this for initialization
    void Start() {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        plots = new Dictionary<string, PlotData>();
        mc = GameObject.Find("Clock").GetComponent<MoonCycle>();
        StartCoroutine(Grow());
    }

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }


    //Handles clicking on garden plots
    public void Farm(GardenPlot plot) {
        PlotData data;
        if (plots.TryGetValue(plot.gameObject.name, out data)) {
            if (data.stage == Status.harvestable) {
                Harvest(plot);
            } else {
                return;
            }
        } else {
            if (rl.activeItem != null && rl.activeItem.item.item is Seed) {
                PlotData newData = new PlotData();
                Seed seed = rl.activeItem.item.item as Seed;
                //Set values for plot
                newData.stage = Status.growing;
                newData.type = seed.SeedType;
                newData.growthTime = seed.GrowthTime;
                newData.currentTime = 0;
                newData.index = 0;
                //Add plot to dict
                plots.Add(plot.gameObject.name, newData);
                //Remove seed from inv
                rl.inv.RemoveItem(rl.activeItem.item);
                //Set plot sprite
                SpriteRenderer[] sr = plot.gameObject.GetComponentsInChildren<SpriteRenderer>();
                //TODO: FIX THIS TO PULL SPRITE FROM ARRAY OF ALL!!!!! ALSO IGNORE THE FIRST? OR LAST? IDK.. whichever one is the plot renderer
                for (int i = 1; i < 4; i++) {
                    sr[i].sprite = Resources.Load<Sprite>("Plants/" + seed.SeedType);
                }
            }
        }
    }



    //Handles harvesting plots and adding ingredients to inventory
    void Harvest(GardenPlot plot) {
        PlotData data = plots[plot.gameObject.name];
        if (rl.inv.Add(rl.ingredients[data.type], rl.ingredients[data.type].name, rl.ingredients[data.type].image)) {
            SpriteRenderer[] sr = plot.gameObject.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 1; i < 4; i++) {
                sr[i].sprite = null;
            }
            plots.Remove(plot.gameObject.name);
        }
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
                    if (SceneManager.GetActiveScene().name.Equals("Garden") || SceneManager.GetActiveScene().name.Equals("SampleGameArea")) {
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
