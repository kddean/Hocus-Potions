using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveller : NPC {
    string[] requests;
    Potion given;
    string[] dialogue;
    int dialogueIndex = 0;
    bool move = true;
    int spawnHour, spawnMinute;
    MoonCycle mc;
    int maxWait = 3;

    private void Start() {
        mc = (MoonCycle)GameObject.FindObjectOfType(typeof(MoonCycle));
        spawnHour = mc.Hour;
        spawnMinute = mc.Minutes;
    }

    void Update() {
        if (move) {
            transform.position = Vector3.MoveTowards(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position, 0.025f);
        }

        if(spawnMinute == mc.Minutes && (spawnHour + maxWait) == mc.Hour) {
            //make them leave if they wait too long
        }
    }

    public string[] Requests {
        get {
            return requests;
        }

        set {
            requests = value;
        }
    }
}
