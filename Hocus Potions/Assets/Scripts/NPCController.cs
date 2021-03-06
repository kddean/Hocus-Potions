﻿using System;
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
    public Dictionary<string, bool> NPCQuestFlags;
    MoonCycle mc;
    int currentMap;
    public bool sceneSwapped;
    Pathfinding pathfinder;
    bool resetting;
    public bool swapping;
    public bool saving;
    public bool gaveHint;

    public int CurrentMap {
        get {
            return currentMap;
        }

        set {
            currentMap = value;
        }
    }

    [Serializable]
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
        public bool option;
        public List<Item> given;
        public List<Schedule> locations;
        public bool spawned;
        public List<NPC.Status> state;
        public Dictionary<NPC.Status, TimerData> potionTimers;
        public Vec3 pathEnd;
        public Vec3 nextTarget;
        public List<int> availableQuests;
        public List<int> finishedQuests;
        public int scriptedQuestNum;
        public float percentCompleted;
        public bool shouldGiveHint;


        public NPCInfo(float x, float y, float z, int map, int timesInteracted, bool returning, string requestKey, float affinity, bool option, int scriptedQuestNum, List<Item> given, List<Schedule> locations, bool spawned, List<NPC.Status> state, Dictionary<NPC.Status, TimerData> potionTimers, Vec3 pathEnd, Vec3 nextTarget, List<int> availableQuests, List<int> finishedQuests, float percentCompleted, bool shouldGiveHint) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.map = map;
            this.timesInteracted = timesInteracted;
            this.returning = returning;
            this.requestKey = requestKey;
            this.affinity = affinity;
            this.option = option;
            this.given = given;
            this.locations = locations;
            this.spawned = spawned;
            this.state = state;
            this.potionTimers = potionTimers;
            this.pathEnd = pathEnd;
            this.nextTarget = nextTarget;
            this.availableQuests = availableQuests;
            this.finishedQuests = finishedQuests;
            this.scriptedQuestNum = scriptedQuestNum;
            this.percentCompleted = percentCompleted;
            this.shouldGiveHint = shouldGiveHint;
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
        NPCQuestFlags = new Dictionary<string, bool>();
        NPCQuestFlags.Add("Bernadette", false);
        NPCQuestFlags.Add("Amara", false);
        NPCQuestFlags.Add("Geoff", false);
        NPCQuestFlags.Add("Dante", false);
        NPCQuestFlags.Add("Ralphie", false);
        NPCQuestFlags.Add("Franklin", false);

        //TODO: Might need to move the initilization to fix execution order errors
        mc = GameObject.FindObjectOfType<MoonCycle>();
        pathfinder = GameObject.FindObjectOfType<Pathfinding>();
        sceneSwapped = false;
        SetQueue(0);
        resetting = false;
        swapping = false;
        gaveHint = false;
    }


    void Update() {
        while (npcQueue.Count > 0 && (mc.days > npcQueue.Keys[0].day ||  (mc.Hour >= npcQueue.Keys[0].hour && mc.Minutes >= npcQueue.Keys[0].minute))) {
            HandleMovement(npcQueue.Keys[0], npcQueue.Values[0]);
            npcQueue.RemoveAt(0);
        }

        if (sceneSwapped && !resetting) {
            resetting = true;
            StartCoroutine(ResetFlag());
        }
    }
    Vector3 ConvertToVector(Vec3 v) {
        return new Vector3(v.x, v.y, v.z);
    }

    public Vec3 ConvertToVec(Vector3 v) {
        return new Vec3(v.x, v.y, v.z);
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
                    npc.nextTarget = ConvertToVector(npcData[s].nextTarget);
                    if(npcData[s].pathEnd.x > -9000) {
                        pathfinder.InitializePath(go.transform.position, ConvertToVector(npcData[s].pathEnd), npcData[s].map, npc.path);
                    }
                } else {
                    if (npcData[s].pathEnd.x > -9000) {
                        if (npcData[s].nextTarget.x > -9000) {
                            List<Vector3> path = new List<Vector3>();
                            pathfinder.InitializePath(new Vector3(npcData[s].x, npcData[s].y, npcData[s].z), ConvertToVector(npcData[s].pathEnd), npcData[s].map, path);
                            StartCoroutine(MoveAndSpawnNPC(path, new Vector3(npcData[s].x, npcData[s].y, npcData[s].z), s, ConvertToVector(npcData[s].nextTarget)));
                        } else {
                            List<Vector3> path = new List<Vector3>();
                            pathfinder.InitializePath(new Vector3(npcData[s].x, npcData[s].y, npcData[s].z), ConvertToVector(npcData[s].pathEnd), npcData[s].map, path);
                            StartCoroutine(MoveNPC(path, new Vector3(npcData[s].x, npcData[s].y, npcData[s].z), s));
                        }
                    }
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
                pathfinder.InitializePath(spawnPoint, new Vector3(s.x, s.y, s.z), 1, npc.path);
                go.transform.position = spawnPoint;
                temp.x = spawnPoint.x;
                temp.y = spawnPoint.y;
                temp.z = spawnPoint.z;
                temp.map = currentMap;
                npcData[n] = temp;
               
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
                pathfinder.InitializePath(spawnPoint, new Vector3(-7.5f, -1.5f, 0), 1, npc.GetComponent<NPC>().path);
                npc.GetComponent<NPC>().nextTarget = new Vector3(s.x, s.y, s.z);
                go.transform.position = spawnPoint;
                temp.x = spawnPoint.x;
                temp.y = spawnPoint.y;
                temp.z = spawnPoint.z;
                temp.map = currentMap;
                npcData[n] = temp;
                
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
        while (swapping) {
            yield return new WaitForEndOfFrame();
        }
        Vector3 lastPos = pos;
        //Wait until the path is calculated
        while (!Monitor.TryEnter(path) || path.Count == 0) {
            Monitor.Exit(path);
            yield return new WaitForEndOfFrame();
        }
        NPCInfo temp = npcData[n];
        while (path.Count > 0) {
            if (saving) {
                temp.x = lastPos.x;
                temp.y = lastPos.y;
                temp.z = lastPos.z;
                temp.spawned = true;
                temp.pathEnd = ConvertToVec(path[path.Count - 1]);
                temp.nextTarget = new Vec3(-9999, -9999, -9999);
                npcData[n] = temp;
            }
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

        while (swapping) {
            yield return new WaitForEndOfFrame();
        }

        Vector3 lastPos = pos;
        bool swapped = false;
        //Wait until the path is calculated
        while (!Monitor.TryEnter(path) || path.Count == 0) {
            Monitor.Exit(path);
            yield return new WaitForEndOfFrame();
        }
        NPCInfo temp = npcData[n];
        while (path.Count > 0) {
            if (saving) {
                temp.x = lastPos.x;
                temp.y = lastPos.y;
                temp.z = lastPos.z;
                temp.spawned = true;
                temp.pathEnd = ConvertToVec(path[path.Count - 1]);
                temp.nextTarget = ConvertToVec(nextTarget);
                npcData[n] = temp;
            }
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
            yield return new WaitForEndOfFrame();
            if (!sceneSwapped) {
                GameObject go = Instantiate(Resources.Load<GameObject>("Characters/" + n));
                go.transform.position = new Vector3(temp.x, temp.y, temp.z);
                NPC npc = go.AddComponent<NPC>();
                npc.path = tempPath;
                npc.nextTarget = new Vector3(-9999, -9999, -9999);
                npc.CharacterName = n;
                go.name = n;
            } else {
                StartCoroutine(MoveNPC(tempPath, new Vector3(npcData[n].x, npcData[n].y, npcData[n].z), n));
            }
        }
    }

    public void SetQueue(int day) {
        List<string> queued = new List<string>();
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
        float averageCompletion = 0.0f;
        int count = 0;
        foreach(string s in npcData.Keys.ToList()) {
            count++;
            averageCompletion += npcData[s].percentCompleted;
        }
        averageCompletion /= count;
        if (averageCompletion < 0.8f || gaveHint) {
            foreach (string key in npcData.Keys.ToList()) {
                if (!queued.Contains(key) && HasAvailableQuests(key)) {
                    available.Add(key);
                }
            }           
        } else {
            NPCInfo BlackData, RedData, WhiteData;
            string highest;
            gaveHint = true;
            BlackData = npcData["Black_Robed_Traveler"];
            RedData = npcData["Red_Robed_Traveler"];
            WhiteData = npcData["White_Robed_Traveler"];
            if (BlackData.affinity >= RedData.affinity) {
                if (BlackData.affinity >= WhiteData.affinity) {
                    highest = "Black_Robed_Traveler";
                    BlackData.shouldGiveHint = true;
                    GameObject.FindObjectOfType<ShrineManager>().order = true;
                } else {
                    highest = "White_Robed_Traveler";
                    WhiteData.shouldGiveHint = true;
                    GameObject.FindObjectOfType<ShrineManager>().nature = true;
                }
            } else {
                if (RedData.affinity >= WhiteData.affinity) {
                    highest = "Red_Robed_Traveler";
                    RedData.shouldGiveHint = true;
                    GameObject.FindObjectOfType<ShrineManager>().social = true;
                } else {
                    highest = "White_Robed_Traveler";
                    WhiteData.shouldGiveHint = true;
                    GameObject.FindObjectOfType<ShrineManager>().nature = true;
                }
            }
            available.Add(highest); 
        }

        string sendChar = "";
        int rand = UnityEngine.Random.Range(0, available.Count);

        int spawnMinute = Mathf.RoundToInt(UnityEngine.Random.Range(0, 50) / 10) * 10;
        int spawnHour = UnityEngine.Random.Range(8, 13);
        if (mc.Days < 4 && available.Contains("Bernadette")) {
            sendChar = "Bernadette";
        } else {
            sendChar = available[rand];
        }
        Schedule schedule = new Schedule(false, day, spawnHour, spawnMinute, "", 0, -7.5f, -0.5f, 0, sendChar);
        npcQueue.Add(schedule, sendChar);
        schedule = new Schedule(false, day, spawnHour + 6, spawnMinute, "", 1, 69.5f, -12.5f, 0, sendChar);
        npcQueue.Add(schedule, sendChar);
        available.Remove(sendChar);
        if (available.Count > 0) {
            spawnMinute = Mathf.RoundToInt(UnityEngine.Random.Range(0, 50) / 10) * 10;
            spawnHour = UnityEngine.Random.Range(13, 17);
            rand = UnityEngine.Random.Range(0, available.Count - 1);
            schedule = new Schedule(false, day, spawnHour, spawnMinute, "", 0, -6.5f, 0.5f, 0, available[rand]);
            npcQueue.Add(schedule, available[rand]);
            schedule = new Schedule(false, day, spawnHour + 6, spawnMinute, "", 1, 69.5f, -12.5f, 0, available[rand]);
            npcQueue.Add(schedule, available[rand]);
            available.RemoveAt(rand);
        }
    }


    private bool HasAvailableQuests(string checkChar) {
        List<Request> requests = new List<Request>();
        GameObject.FindObjectOfType<ResourceLoader>().requestList.TryGetValue(checkChar, out requests);
        if(requests == null) {
            return false;
        }
        int count = 0;
        for (int i = 0; i < requests.Count; i++) {
            //ignore scripted quests and finished quests
            if (!requests[i].Key.Contains("1") && !requests[i].Key.Contains("2") && !requests[i].Key.Contains("3") && !npcData[checkChar].finishedQuests.Contains(i)) {
                //Checks for which quests they can give
                if (checkChar.Equals("Bernadette")) {
                    if (!NPCQuestFlags["Bernadette"]) {
                        if (requests[i].Key.Contains("birds")) {
                            continue;
                        }
                    } else {
                        if (requests[i].Key.Contains("hide") || requests[i].Key.Contains("skinny") || requests[i].Key.Contains("ring")) {
                            continue;
                        }
                    }
                } else if (checkChar.Equals("Amara")) {
                    if (NPCQuestFlags["Bernadette"]) {
                        if (requests[i].Key.Contains("chill")) {
                            continue;
                        }
                    } else {
                        if (requests[i].Key.Contains("present")) {
                            continue;
                        }
                    }
                } else if (checkChar.Equals("Geoff")) {
                    if (NPCQuestFlags["Bernadette"]) {
                        if (requests[i].Key.Contains("pre")) {
                            continue;
                        }
                    } else {
                        if (requests[i].Key.Contains("deal") || requests[i].Key.Contains("organize")) {
                            continue;
                        }
                    }
                } else if (checkChar.Equals("Franklin")) {
                    if (NPCQuestFlags["Bernadette"]) {
                        if (requests[i].Key.Contains("tired")) {
                            continue;
                        }
                    }
                }
                count++;
            }
        }
        if(count > 0) {
            return true;
        } else {
            return false;
        }
    }

    public void LakePotion(Ingredient.Attributes? type) {
        StartCoroutine(LakePotionDelay(type, GameObject.FindObjectOfType<MoonCycle>().Days));
    }

    IEnumerator LakePotionDelay(Ingredient.Attributes? type, int day) {
        while(GameObject.FindObjectOfType<MoonCycle>().Days == day) {
            yield return new WaitForSeconds(GameObject.FindObjectOfType<MoonCycle>().CLOCK_SPEED);
        }
        foreach (string s in npcData.Keys.ToList()) {
            NPCInfo tempInfo = npcData[s];
            if (s.Equals("Ralphie")) { continue; }
            switch (type) {
                case Ingredient.Attributes.invisibility:
                    tempInfo.potionTimers.Add(NPC.Status.invisible, new TimerData(Time.time, 1440));
                    break;
                case Ingredient.Attributes.poison:
                    tempInfo.potionTimers.Add(NPC.Status.poisoned, new TimerData(Time.time, 1440));
                    break;
                case Ingredient.Attributes.speed:
                    tempInfo.potionTimers.Add(NPC.Status.fast, new TimerData(Time.time, 1440));
                    break;
                case Ingredient.Attributes.transformation:
                    tempInfo.potionTimers.Add(NPC.Status.transformed, new TimerData(Time.time, 1440));
                    break;
                case Ingredient.Attributes.sleep:
                    tempInfo.potionTimers.Add(NPC.Status.asleep, new TimerData(Time.time, 1440));
                    break;
                default:
                    break;
            }
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
