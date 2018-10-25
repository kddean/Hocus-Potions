using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour {
    MoonCycle mc;
    int lastHour;
    int spawnHour, spawnMinute;
    bool timeSet;
    public GameObject spawnPoint;
    ResourceLoader rl;
    bool spawned;
    string waiting;


    void Start() {
        mc = (MoonCycle)GameObject.FindObjectOfType(typeof(MoonCycle));
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        timeSet = false;
        Spawned = false;
        Waiting = null;
    }


    void Update() {
        if (!Spawned && Waiting == null) {
            if (!timeSet) {
                spawnHour = Random.Range(8, 21);
                spawnMinute = Random.Range(0, 60);
                float temp = spawnMinute / 10.0f;
                spawnMinute = (int)(Mathf.Round(temp) * 10.0f);
                timeSet = true;
            }

            if (mc.Hour == spawnHour && mc.Minutes == spawnMinute) {
                GameObject go = new GameObject();
                go.transform.position = spawnPoint.transform.position;
                //Swap this to set sorting layer instead once they're set up
                Vector3 tempPos = go.transform.position;
                tempPos.z = -1.0f;
                go.transform.position = tempPos;


                string key = rl.availableNPCs[Random.Range(0, rl.availableNPCs.Count)];
                //This should use prefabs eventually to just spawn them with their data intact
                Traveller trav = go.AddComponent<Traveller>();
                trav.Manager = this;
                trav.CharacterName = key;
                trav.Returning = false;

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = rl.charSpriteList[key];
                BoxCollider2D col = go.AddComponent<BoxCollider2D>();
                Vector2 bounds = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
                col.size = bounds;
                go.name = key;
                lastHour = spawnHour;
                Spawned = true;
            }
        }

        if (Waiting != null & mc.Hour == spawnHour && mc.Minutes == spawnMinute) {
            GameObject go = new GameObject();
            go.transform.position = spawnPoint.transform.position;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            go.AddComponent<BoxCollider2D>();
            string key = rl.availableNPCs[Random.Range(0, rl.availableNPCs.Count)];
            //TODO: This needs to somehow repopulate it with data if the NPC has already existed before
            Traveller trav = go.AddComponent<Traveller>();
            trav.Manager = this;
            trav.CharacterName = key;
            trav.Returning = true;
            sr.sprite = rl.charSpriteList[key];
            go.name = key;
        }

        if (Waiting == null && mc.Hour == 21) {
            timeSet = false;
        }
    }

    public bool Spawned {
        get {
            return spawned;
        }

        set {
            spawned = value;
        }
    }

    public string Waiting {
        get {
            return waiting;
        }

        set {
            waiting = value;
        }
    }
}
