﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wardrobe : MonoBehaviour {
    string[] costumes;
    int current;

    public int Current {
        get {
            return current;
        }

        set {
            current = value;
        }
    }

    void Start () {
        costumes = new[] {"Player_Default", "Costume_Cat", "Costume_Magic", "Costume_Steampunk", "Costume_Goddess" };
        current = 0;
    }

    public void Cycle() {
        if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < 3) {
            Current = (Current + 1) % costumes.Length;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Characters/" + costumes[Current]);
        }
    }

    public void LoadCostume(int c) {
        current = c;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Characters/" + costumes[Current]);
    }
}