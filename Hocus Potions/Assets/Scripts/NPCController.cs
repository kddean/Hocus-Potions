using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    public Dictionary<string, NPCInfo> npcData;
    SortedList<Schedule, string> npcQueue;
    Dictionary<string, List<Vector3>> unfinishedPaths;
    MoonCycle mc;
    int currentMap;
    public bool sceneSwapped;
    Pathfinding pathfinder;
    GameObject dante;

    public int CurrentMap {
        get {
            return currentMap;
        }

        set {
            currentMap = value;
        }
    }


    //Data for each NPC
    [Serializable]
    public struct NPCInfo {
        public float x, y, z;
        public int map;
        public int timesInteracted;
        public bool returning;
        public string requestKey;
        public float affinity;
        public List<Item> given;
        public List<Schedule> locations;
        public bool spawned;
        public List<NPC.Status> state;

        public NPCInfo(float x, float y, float z, int map, int timesInteracted, bool returning, string requestKey, float affinity, List<Item> given, List<Schedule> locations, bool spawned, List<NPC.Status> state) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.map = map;
            this.timesInteracted = timesInteracted;
            this.returning = returning;
            this.requestKey = requestKey;
            this.affinity = affinity;
            this.given = given;
            this.locations = locations;
            this.spawned = spawned;
            this.state = state;
        }
    }

    //Data for each scheduled event/location for the NPC to be
    [Serializable]
    public struct Schedule {
        public bool repeating;
        public int day;
        public int time;
        public string dialogueKey;
        public int map;                     // 0 for house, 1 for overworld
        public float x, y, z;
        public string characterName;

        public Schedule(bool repeating, int day, int time, string dialogueKey, int map, float x, float y, float z, string characterName) {
            this.repeating = repeating;
            this.day = day;
            this.time = time;
            this.dialogueKey = dialogueKey;
            this.map = map;
            this.x = x;
            this.y = y;
            this.z = z;
            this.characterName = characterName;
        }
    }

    void Start() {
        npcQueue = new SortedList<Schedule, string>(new CompareTimes());
        //TODO: Might need to move the initilization to fix execution order errors
        mc = GameObject.FindObjectOfType<MoonCycle>();
        unfinishedPaths = new Dictionary<string, List<Vector3>>();
        pathfinder = GameObject.FindObjectOfType<Pathfinding>();
        SetQueue(0);
    }


    void Update() {
        while (npcQueue.Count > 0 && mc.Hour == npcQueue.Keys[0].time) {
            HandleMovement(npcQueue.Keys[0], npcQueue.Values[0]);
            npcQueue.RemoveAt(0);
        }
    }

    private void LateUpdate() {
        if (sceneSwapped && unfinishedPaths.Count > 0) {
            FinishPathVisual();
        }
    }

    void HandleMovement(Schedule s, string n) {
        //Player is in the same scene as the NPC, just let it deal with it itself
        if (s.map == CurrentMap) {
            if (npcData[n].spawned) {
                GameObject npc = GameObject.Find(n);
                pathfinder.InitializePath(npc.transform.position, new Vector3(s.x, s.y, s.z), CurrentMap, npc.GetComponent<NPC>().path);
            } else {
                GameObject go = Instantiate(Resources.Load<GameObject>("Characters/" + n));
                go.name = n;
                NPC npc = go.AddComponent<NPC>();
                npc.CharacterName = n;
                NPCInfo temp = npcData[n];
                temp.spawned = true;
                Vector3 spawnPoint = GameObject.Find("NPCSpawnPoint").transform.position;
                go.transform.position = spawnPoint;
                temp.x = spawnPoint.x;
                temp.y = spawnPoint.y;
                temp.z = spawnPoint.z;
                temp.map = currentMap;
                npcData[n] = temp;
                pathfinder.InitializePath(npc.transform.position, new Vector3(s.x, s.y, s.z), CurrentMap, npc.path);
            }
        } else {    //Player isn't in the same scene as the NPC
            List<Vector3> path = new List<Vector3>();
            pathfinder.InitializePath(new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), new Vector3(s.x, s.y, s.z), Mathf.Abs(CurrentMap - 1), path);
            StartCoroutine(MoveNPC(path, new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), n));
        }
    }

    public void FinishPathData (List<Vector3> path, string n){
        StartCoroutine(MoveNPC(path, new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), n));
    }

    public void FinishPathVisual() {
        foreach (string k in unfinishedPaths.Keys.ToList()) {
            GameObject npc = GameObject.Find(k);
            npc.GetComponent<NPC>().path = unfinishedPaths[k];
        }
        unfinishedPaths.Clear();
        sceneSwapped = false;
    }


    IEnumerator MoveNPC(List<Vector3> path, Vector3 pos, string n) {
        Vector3 lastPos = pos;
        while (path.Count > 0) {
            lastPos = Vector3.MoveTowards(lastPos, path[0], Time.deltaTime * 0.01f);
            if (lastPos == path[0]) {
                path.RemoveAt(0);
            }
            if (!sceneSwapped) {
                yield return new WaitForEndOfFrame();
            } else {
                unfinishedPaths.Add(n, path);
                break;
            }
        }
    }

    public void SetQueue(int day) {
        npcQueue.Clear();
        foreach(string key in npcData.Keys.ToList()) {
            List<Schedule> temp = npcData[key].locations;
            foreach(Schedule s in temp) {
                if((s.repeating && s.day == (day % 6)) || (!s.repeating && s.day == day)) {
                    npcQueue.Add(s, s.characterName);
                }
            }
        }
    }


    [System.Serializable]
    private class CompareTimes : IComparer<Schedule> {
        int IComparer<Schedule>.Compare(Schedule a, Schedule b) {
            if (a.time < b.time) {
                return -1;        
            } else {
                return 1;
            }
        }
    }
}
