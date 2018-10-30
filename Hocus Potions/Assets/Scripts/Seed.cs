using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Object {
    string seedType;
    int growthTime;
    int growthStages;
    new string name;

    public Seed(string seedType, int growthTime, int growthStages, string name) {
        this.SeedType = seedType;
        this.GrowthTime = growthTime;
        this.GrowthStages = growthStages;
        this.Name = name;
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

