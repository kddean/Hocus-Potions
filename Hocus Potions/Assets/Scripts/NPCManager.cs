using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCManager : MonoBehaviour {
    public GameObject spawnPoint;
    MoonCycle mc;
    ResourceLoader rl;
    int lastHour;
    int spawnHour, spawnMinute;
    bool timeSet;
    bool queueLoaded;
    bool spawned;
    string lastSpawned;
    public struct NPCData {
        public int timesInteracted;
        public bool returning;
        public List<Object> given;
        public int returningDay;
        public int returningHour;
        public int returningMinutes;
        public int affinity;
    }

    public Dictionary<string, NPCData> data;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    public SortedList<NPCData, string> returnQueue;

    void Start() {
        mc = (MoonCycle)GameObject.FindObjectOfType(typeof(MoonCycle));
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        data = new Dictionary<string, NPCData>();
        timeSet = false;
        Spawned = false;
        queueLoaded = false;
        returnQueue = new SortedList<NPCData, string>(new CompareTimes());

    }


    void Update() {
        //TODO: Make it load data in the background if you're not there 
        if (!SceneManager.GetActiveScene().name.Equals("House")) {
            return;
        }

        //If nobody is returning today
        if (returnQueue.Count == 0 && !spawned) {
            if (!timeSet) { //Pick a time if it hasn't already
                spawnHour = Random.Range(8, 18);
                spawnMinute = Random.Range(0, 60);
                float temp = spawnMinute / 10.0f;
                spawnMinute = (int)(Mathf.Round(temp) * 10.0f);
                timeSet = true;
            }

            //spawn the NPC if it's the correct time
            if (mc.Hour == spawnHour && mc.Minutes == spawnMinute) {
                GameObject go = new GameObject();
                go.transform.position = spawnPoint.transform.position;
                //Swap this to set sorting layer instead once they're set up
                Vector3 tempPos = go.transform.position;
                tempPos.z = -1.0f;
                go.transform.position = tempPos;

                
                string key = rl.availableNPCs[Random.Range(0, rl.availableNPCs.Count)];
                while (key.Equals(lastSpawned)) {
                    key = rl.availableNPCs[Random.Range(0, rl.availableNPCs.Count)];
                }

                //This should use prefabs eventually to just spawn them with their data intact
                Traveller trav = go.AddComponent<Traveller>();
                trav.Manager = this;
                trav.CharacterName = key;

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = rl.charSpriteList[key];
                BoxCollider2D col = go.AddComponent<BoxCollider2D>();
                Vector2 bounds = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
                col.size = bounds;
                go.name = key;
                GameObject effects = new GameObject();
                effects.AddComponent<SpriteRenderer>();
                Animator anim = effects.AddComponent<Animator>();
                anim.runtimeAnimatorController = Resources.Load<AnimatorController>("NPC_effects");
                effects.transform.SetParent(go.transform);
                effects.name = "effects";
                effects.transform.localPosition = new Vector3(0, 0, -1);

                lastHour = spawnHour;
                Spawned = true;
                lastSpawned = key;
            }
        } else if (returnQueue.Count != 0 && returnQueue.Keys[0].returningDay == mc.Days && returnQueue.Keys[0].returningHour == mc.Hour && returnQueue.Keys[0].returningMinutes == mc.Minutes) {        //if someone is supposed to return

            GameObject go = new GameObject();
            go.transform.position = spawnPoint.transform.position;
            //Swap this to set sorting layer instead once they're set up
            Vector3 tempPos = go.transform.position;
            tempPos.z = -1.0f;
            go.transform.position = tempPos;

            string key = returnQueue.Values[0];
            //This should use prefabs eventually to just spawn them with their data intact
            Traveller trav = go.AddComponent<Traveller>();
            trav.Manager = this;
            trav.CharacterName = key;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = rl.charSpriteList[key];
            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            Vector2 bounds = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
            col.size = bounds;
            go.name = key;
            GameObject effects = new GameObject();
            effects.AddComponent<SpriteRenderer>();
            Animator anim = effects.AddComponent<Animator>();
            anim.runtimeAnimatorController = Resources.Load<AnimatorController>("NPC_effects");
            effects.transform.SetParent(go.transform);
            effects.name = "effects";
            effects.transform.localPosition = new Vector3(0, 0, -1);

            lastHour = spawnHour;
            Spawned = true;
            lastSpawned = key;
            returnQueue.RemoveAt(0);
        }

        if (mc.Hour == 6 && mc.Minutes == 0) {
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


    private class CompareTimes : IComparer<NPCData> {
        int IComparer<NPCData>.Compare(NPCData a, NPCData b) {
            if (a.returningDay < b.returningDay) {
                return -1;
            } else if (a.returningDay > b.returningDay) {
                return 1;
            } else {
                if (a.returningHour < b.returningHour) {
                    return -1;
                } else if (a.returningHour > b.returningHour) {
                    return 1;
                } else {
                    if (a.returningMinutes < b.returningMinutes) {
                        return -1;
                    } else {
                        return 1;
                    }
                }
            }
        }
    }
}

