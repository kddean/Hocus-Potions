using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Seed : Item {
    string seedType;
    int growthTime;
    int growthStages;

    public Seed(string seedType, int growthTime, int growthStages, string name, string icon) {
        this.SeedType = seedType;
        this.GrowthTime = growthTime;
        this.GrowthStages = growthStages;
        this.Name = name;
        imagePath = icon;
    }

    public string SeedType {
        get {
            return seedType;
        }

        set {
            seedType = value;
        }
    }

    public int GrowthTime {
        get {
            return growthTime;
        }

        set {
            growthTime = value;
        }
    }

    public int GrowthStages {
        get {
            return growthStages;
        }

        set {
            growthStages = value;
        }
    }

    public string Name {
        get {
            return name;
        }

        set {
            name = value;
        }
    }

}

