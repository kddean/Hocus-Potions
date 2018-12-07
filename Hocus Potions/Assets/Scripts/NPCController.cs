using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    public Dictionary<string, NPCInfo> npcData;
    public SortedList<Schedule, string> npcQueue;
    MoonCycle mc;
    int currentMap;
    public bool sceneSwapped;
    Pathfinding pathfinder;
    bool resetting;
    public bool swapping;
    public bool saving;

    public int CurrentMap {
        get {
            return currentMap;
        }

        set {
            currentMap = value;
        }
    }

    public struct Vec3 {
        public float x, y, z;

        public Vec3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
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
        public Dictionary<NPC.Status, TimerData> potionTimers;
        public List<Vec3> path;

        public NPCInfo(float x, float y, float z, int map, int timesInteracted, bool returning, string requestKey, float affinity, List<Item> given, List<Schedule> locations, bool spawned, List<NPC.Status> state, Dictionary<NPC.Status, TimerData> potionTimers, List<Vec3> path) {
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
            this.potionTimers = potionTimers;
            this.path = path;
        }
    }

    public struct TimerData {
        public float startTime;
        public float duration;

        public TimerData(float startTime, float duration) {
            this.startTime = startTime;
            this.duration = duration;
        }
    }

    //Data for each scheduled event/location for the NPC to be
    [Serializable]
    public struct Schedule {
        public bool repeating;
        public int day;
        public int hour;
        public int minute;
        public string dialogueKey;
        public int map;                     // 0 for house, 1 for overworld
        public float x, y, z;
        public string characterName;

        public Schedule(bool repeating, int day, int hour, int minute, string dialogueKey, int map, float x, float y, float z, string characterName) {
            this.repeating = repeating;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
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
        pathfinder = GameObject.FindObjectOfType<Pathfinding>();
        sceneSwapped = false;
        SetQueue(0);
        resetting = false;
        swapping = false;
    }


    void Update() {
        while (npcQueue.Count > 0 && mc.Hour == npcQueue.Keys[0].hour && mc.Minutes == npcQueue.Keys[0].minute) {
            HandleMovement(npcQueue.Keys[0], npcQueue.Values[0]);
            npcQueue.RemoveAt(0);
        }

        if (sceneSwapped && !resetting) {
            resetting = true;
            StartCoroutine(ResetFlag());
        }
    }
    List<Vector3> ConvertPathToVector(List<Vec3> path) {
        List<Vector3> temp = new List<Vector3>();
        foreach(Vec3 v in path) {
            temp.Add(new Vector3(v.x, v.y, v.z));
        }
        return temp;
    }

    public List<Vec3> ConvertPathToVec(List<Vector3> path) {
        List<Vec3> temp = new List<Vec3>();
        foreach (Vector3 v in path) {
            temp.Add(new Vec3(v.x, v.y, v.z));
        }
        return temp;
    }

    public void LoadNPCS() {
        foreach (string s in npcData.Keys.ToList()) {
            if (npcData[s].spawned) {
                if(npcData[s].map == currentMap) {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Characters/" + s));
                    go.name = s;
                    go.transform.position = new Vector3(npcData[s].x, npcData[s].y, npcData[s].z);
                    NPC npc = go.AddComponent<NPC>();
                    npc.CharacterName = s;
                }
            }
        }
    }
    IEnumerator ResetFlag() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        sceneSwapped = false;
        swapping = false;
        resetting = false;
        foreach (string s in npcData.Keys.ToList()) {
            if (npcData[s].spawned && npcData[s].map == currentMap) {
                if (GameObject.Find(s)) { continue; }
                GameObject go = Instantiate(Resources.Load<GameObject>("Characters/" + s));
                go.name = s;
                go.transform.position = new Vector3(npcData[s].x, npcData[s].y, npcData[s].z);
                NPC npc = go.AddComponent<NPC>();
                npc.CharacterName = s;
            }
        }
    }
    //TODO: these conditions can probably be cleaned up/reordered to be shorter
    void HandleMovement(Schedule s, string n) {
        //Player is outside and the NPC is going somewhere outside
        if (s.map == 1 && currentMap == 1) {
            if (npcData[n].spawned && npcData[n].map == 1) {                //NPC is already outside
                GameObject npc = GameObject.Find(n);
                pathfinder.InitializePath(npc.transform.position, new Vector3(s.x, s.y, s.z), CurrentMap, npc.GetComponent<NPC>().path);
            } else if (npcData[n].spawned && npcData[n].map == 0) {         //NPC is inside
                List<Vector3> path = new List<Vector3>();
                pathfinder.InitializePath(new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), new Vector3(0.5f, -4.5f, 0), 0, path);
                StartCoroutine(MoveAndSpawnNPC(path, new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), n, new Vector3(s.x, s.y, s.z)));
            } else if (!npcData[n].spawned) {                                                        //NPC isn't spawned but is going somewhere outside
                GameObject go = Instantiate(Resources.Load<GameObject>("Characters/" + n));
                go.name = n;
                NPC npc = go.AddComponent<NPC>();
                npc.CharacterName = n;
                npc.nextTarget = new Vector3(-9999, -9999, -9999);
                NPCInfo temp = npcData[n];
                temp.spawned = true;
                Vector3 spawnPoint = GameObject.Find("NPCSpawnPoint").transform.position;
                go.transform.position = spawnPoint;
                temp.x = spawnPoint.x;
                temp.y = spawnPoint.y;
                temp.z = spawnPoint.z;
                temp.map = currentMap;
                npcData[n] = temp;
                pathfinder.InitializePath(npc.transform.position, new Vector3(s.x, s.y, s.z), 1, npc.path);
            }
            //Player is inside and NPC is going somewhere inside
        } else if (s.map == 0 && currentMap == 0) {
            if (npcData[n].spawned && npcData[n].map == 0) {                //NPC is already inside
                GameObject npc = GameObject.Find(n);
                pathfinder.InitializePath(npc.transform.position, new Vector3(s.x, s.y, s.z), CurrentMap, npc.GetComponent<NPC>().path);
            } else if (npcData[n].spawned && npcData[n].map == 1) {         //NPC is outside
                List<Vector3> path = new List<Vector3>();
                pathfinder.InitializePath(new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), new Vector3(-7.5f, -1.5f, 0), 1, path);
                StartCoroutine(MoveAndSpawnNPC(path, new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), n, new Vector3(s.x, s.y, s.z)));
            } else if (!npcData[n].spawned) {                                      //NPC isn't spwaned but is going somewhere inside
                NPCInfo temp = npcData[n];
                temp.spawned = true;
                temp.x = 69.5f;
                temp.y = -12.5f;
                temp.z = 0;
                npcData[n] = temp;
                List<Vector3> path = new List<Vector3>();
                pathfinder.InitializePath(new Vector3(69.5f, -12.5f, 0), new Vector3(-7.5f, -1.5f, 0), 1, path);
                StartCoroutine(MoveAndSpawnNPC(path, new Vector3(69.5f, -12.5f, 0), n, new Vector3(s.x, s.y, s.z)));
            }
        } else if (s.map == 1 && currentMap == 0) {
            if (npcData[n].spawned && npcData[n].map == 1) {
                List<Vector3> path = new List<Vector3>();
                pathfinder.InitializePath(new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), new Vector3(s.x, s.y, s.z), 1, path);
                StartCoroutine(MoveNPC(path, new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), n));
            } else if (npcData[n].spawned && npcData[n].map == 0) {
                GameObject npc = GameObject.Find(n);
                pathfinder.InitializePath(npc.transform.position, new Vector3(0.5f, -4.5f, 0), 0, npc.GetComponent<NPC>().path);
                npc.GetComponent<NPC>().nextTarget = new Vector3(s.x, s.y, s.z);
            } else if (!npcData[n].spawned) {
                NPCInfo temp = npcData[n];
                temp.spawned = true;
                temp.x = 69.5f;
                temp.y = -12.5f;
                temp.z = 0;
                npcData[n] = temp;
                List<Vector3> path = new List<Vector3>();
                pathfinder.InitializePath(new Vector3(69.5f, -12.5f, 0), new Vector3(s.x,s.y,s.z), 1, path);
                StartCoroutine(MoveNPC(path, new Vector3(69.5f, -12.5f, 0), n));
            }
        } else if (s.map == 0 && currentMap == 1) {
            if (npcData[n].spawned && npcData[n].map == 1) {
                GameObject npc = GameObject.Find(n);
                pathfinder.InitializePath(npc.transform.position, new Vector3(-7.5f, -1.5f, 0), 1, npc.GetComponent<NPC>().path);
                npc.GetComponent<NPC>().nextTarget = new Vector3(s.x, s.y, s.z);
            } else if (npcData[n].spawned && npcData[n].map == 0) {
                List<Vector3> path = new List<Vector3>();
                pathfinder.InitializePath(new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), new Vector3(s.x, s.y, s.z), 0, path);
                StartCoroutine(MoveNPC(path, new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), n));
            } else if (!npcData[n].spawned) {
                GameObject go = Instantiate(Resources.Load<GameObject>("Characters/" + n));
                go.name = n;
                NPC npc = go.AddComponent<NPC>();
                npc.CharacterName = n;
                npc.nextTarget = new Vector3(s.x, s.y, s.z);
                NPCInfo temp = npcData[n];
                temp.spawned = true;
                Vector3 spawnPoint = GameObject.Find("NPCSpawnPoint").transform.position;
                go.transform.position = spawnPoint;
                temp.x = spawnPoint.x;
                temp.y = spawnPoint.y;
                temp.z = spawnPoint.z;
                temp.map = currentMap;
                npcData[n] = temp;
                pathfinder.InitializePath(npc.transform.position, new Vector3(-7.5f, -1.5f, 0), 1, npc.GetComponent<NPC>().path);
                npc.GetComponent<NPC>().nextTarget = new Vector3(s.x, s.y, s.z);
            }
        }
    }

    public void FinishPathData(List<Vector3> path, string n) {
        StartCoroutine(MoveNPC(path, new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), n));
    }

    public void FinishMoveAndSpawn(List<Vector3> path, Vector3 pos, string n, Vector3 nextTarget) {
        StartCoroutine(MoveAndSpawnNPC(path, pos, n, nextTarget));
    }


    IEnumerator MoveNPC(List<Vector3> path, Vector3 pos, string n) {
        yield return new WaitForSeconds(0.3f);
        while (swapping) {
            yield return new WaitForEndOfFrame();
        }
        Vector3 lastPos = pos;
        //Wait until the path is calculated
        while (!Monitor.TryEnter(path)) {
            Monitor.Exit(path);
            yield return new WaitForEndOfFrame();
        }
        NPCInfo temp = npcData[n];
        while (path.Count > 0) {
            lastPos = Vector3.MoveTowards(lastPos, path[0], Time.timeScale * Time.deltaTime * 4f);
            if (lastPos == path[0]) {
                path.RemoveAt(0);
            }
            if (!sceneSwapped) {
                yield return new WaitForEndOfFrame();
            } else {
                GameObject go = Instantiate(Resources.Load<GameObject>("Characters/" + n));
                go.name = n;
                NPC npc = go.AddComponent<NPC>();
                npc.CharacterName = n;
                npc.nextTarget = new Vector3(-9999, -9999, -9999);
                go.transform.position = lastPos;
                npcData[n] = temp;
                npc.path = path;
                break;
            }
        }
        temp.x = lastPos.x;
        temp.y = lastPos.y;
        temp.z = lastPos.z;
        if (temp.map == 1 && temp.x == 69.5f && temp.y == -12.5f) {
            temp.spawned = false;
        } else {
            temp.spawned = true;
        }

        npcData[n] = temp;

        Monitor.Exit(path);
    }

    IEnumerator MoveAndSpawnNPC(List<Vector3> path, Vector3 pos, string n, Vector3 nextTarget) {
        yield return new WaitForSeconds(0.3f);
        while (swapping) {
            yield return new WaitForEndOfFrame();
        }

        Vector3 lastPos = pos;
        bool swapped = false;
        //Wait until the path is calculated
        while (!Monitor.TryEnter(path)) {
            Monitor.Exit(path);
            yield return new WaitForEndOfFrame();
        }
      
        while (path.Count > 0) {
            lastPos = Vector3.MoveTowards(lastPos, path[0], Time.timeScale * Time.deltaTime * 4f);
            if (lastPos == path[0]) {
                path.RemoveAt(0);
            }
            if (!sceneSwapped) {
                yield return new WaitForEndOfFrame();
            } else {
                GameObject go = Instantiate(Resources.Load<GameObject>("Characters/" + n));
                go.name = n;
                NPC npc = go.AddComponent<NPC>();
                npc.CharacterName = n;
                NPCInfo temp = npcData[n];
                temp.spawned = true;
                go.transform.position = lastPos;
                temp.x = lastPos.x;
                temp.y = lastPos.y;
                temp.z = lastPos.z;
                npcData[n] = temp;
                npc.path = path;
                npc.nextTarget = nextTarget;
                swapped = true;
                break;
            }
        }

        Monitor.Exit(path);
        if (!swapped) {
            NPCInfo temp = npcData[n];
            temp.map = Mathf.Abs(temp.map - 1);
            if (temp.map == 1) {
                temp.x = -7.5f;
                temp.y = -1.5f;
                temp.z = 0;
            } else {
                temp.x = 0.5f;
                temp.y = -4.5f;
                temp.z = 0;
            }
            temp.spawned = true;
            npcData[n] = temp;
            List<Vector3> tempPath = new List<Vector3>();
            pathfinder.InitializePath(new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), nextTarget, temp.map, tempPath);
            yield return new WaitForSeconds(0.3f);
            GameObject go = Instantiate(Resources.Load<GameObject>("Characters/" + n));
            go.transform.position = new Vector3(temp.x, temp.y, temp.z);
            NPC npc = go.AddComponent<NPC>();
            npc.path = tempPath;
            npc.nextTarget = new Vector3(-9999, -9999, -9999);
            npc.CharacterName = n;
            go.name = n;
           
        }
    }

    public void SetQueue(int day) {
        List<string> queued = new List<string>();
        npcQueue.Clear();
        foreach (string key in npcData.Keys.ToList()) {
            List<Schedule> temp = new List<Schedule>();
            foreach (Schedule s in npcData[key].locations) {
                if ((s.repeating && s.day == (day % 6)) || (!s.repeating && s.day == day)) {
                    if (s.repeating) {
                        temp.Add(s);
                    }
                    npcQueue.Add(s, s.characterName);
                    queued.Add(s.characterName);
                } else {
                    temp.Add(s);
                }
            }
            NPCInfo tempInfo = npcData[key];
            tempInfo.locations = temp;
            npcData[key] = tempInfo;
        }
        
        List<string> available = new List<string>();
        foreach (string key in npcData.Keys.ToList()) {
            if (!queued.Contains(key)) {
                available.Add(key);
            }
        }

        int rand = UnityEngine.Random.Range(0, available.Count - 1);
        int spawnMinute = Mathf.RoundToInt(UnityEngine.Random.Range(0, 50) / 10) * 10;
        int spawnHour = UnityEngine.Random.Range(8, 13);
        Schedule schedule = new Schedule(false, day, spawnHour, spawnMinute, "", 0, -6.5f, 0.5f, 0, available[rand]);
        npcQueue.Add(schedule, available[rand]);
        schedule = new Schedule(false, day, spawnHour + 7, spawnMinute, "", 1, 69.5f, -12.5f, 0, available[rand]);
        npcQueue.Add(schedule, available[rand]);
        available.RemoveAt(rand);
        if (available.Count > 0) {
            spawnMinute = Mathf.RoundToInt(UnityEngine.Random.Range(0, 50) / 10) * 10;
            //TODO: Change the times for this later
            spawnHour = UnityEngine.Random.Range(13, 17);
            rand = UnityEngine.Random.Range(0, available.Count - 1);
            schedule = new Schedule(false, day, spawnHour, spawnMinute, "", 0, -7.5f, 0.5f, 0, available[rand]);
            npcQueue.Add(schedule, available[rand]);
            schedule = new Schedule(false, day, spawnHour + 7, spawnMinute, "", 1, 69.5f, -12.5f, 0, available[rand]);
            npcQueue.Add(schedule, available[rand]);
            available.RemoveAt(rand);
        }
    }



    [System.Serializable]
    private class CompareTimes : IComparer<Schedule> {
        int IComparer<Schedule>.Compare(Schedule a, Schedule b) {
            if (a.hour < b.hour) {
                return -1;
            } else if (a.hour > b.hour) {
                return 1;
            } else {
                if (a.minute < b.minute) {
                    return -1;
                } else {
                    return 1;
                }
            }
        }
    }
}
