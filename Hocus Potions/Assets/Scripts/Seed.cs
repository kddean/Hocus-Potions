using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Object {
    string seedType;
    int growthTime;
    int growthStages;

    public Seed(string seedType, int growthTime, int growthStages) {
        this.SeedType = seedType;
        this.GrowthTime = growthTime;
        this.GrowthStages = growthStages;
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
}

