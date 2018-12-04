using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour {
    string characterName;
    Dictionary<string,List<string>> dialogue;
    public List<Vector3> path;
    NPCController controller;
    public bool sceneSwapped;
    NPCController.NPCInfo info;
    float speed;
    public Vector3 nextTarget;
    GameObject swapPoint;
    bool destroying;


    public enum Status { poisoned, fast, invisible, transformed, asleep }

    public string CharacterName {
        get {
            return characterName;
        }

        set {
            characterName = value;
        }
    }

    public Dictionary<string, List<string>> Dialogue {
        get {
            return dialogue;
        }

        set {
            dialogue = value;
        }
    }

    private void Awake() {
        path = new List<Vector3>();
    }
    private void Start() {
        controller = GameObject.FindObjectOfType<NPCController>();
        sceneSwapped = false;
        if(!controller.npcData.TryGetValue(CharacterName, out info)) {
            Debug.Log("NPC Data not set");
        }
        speed = 4f;
        swapPoint = GameObject.Find("SwapPoint");
        destroying = false;
        if (SceneManager.GetActiveScene().name.Equals("House")) {
            info.map = 0;
        } else {
            info.map = 1;
        }
        info.spawned = true;
    }

    private void Update() {
        if(destroying) { return; }

        if (sceneSwapped && nextTarget.x > -9000) {
            info.x = transform.position.x;
            info.y = transform.position.y;
            info.z = transform.position.z;
            controller.npcData[characterName] = info;
            controller.FinishMoveAndSpawn(path, transform.position, characterName, nextTarget);
            destroying = true;
        }

        if (sceneSwapped && nextTarget.x < -9000) {
            info.x = transform.position.x;
            info.y = transform.position.y;
            info.z = transform.position.z;
            controller.npcData[characterName] = info;
            if (path.Count > 0) {
                controller.FinishPathData(path, CharacterName);
            }
            destroying = true;
        }

        if(destroying) { return; }


        if (Monitor.TryEnter(path, 1) && path.Count > 0) {
            transform.position = Vector3.MoveTowards(transform.position, path[0], Time.timeScale * Time.deltaTime * speed);
            if (transform.position == path[0]) {
                path.RemoveAt(0);
            }
        } 
        Monitor.Exit(path);

        if (nextTarget != null && transform.position == swapPoint.transform.position) {
            if (info.map == 0) {
                GameObject.FindObjectOfType<Pathfinding>().InitializePath(new Vector3(-7.5f, -1.5f, 0), nextTarget, 1, path);
                info.map = 1;
                info.x = -7.5f;
                info.y = -1.5f;
                info.z = 0;
                info.spawned = true;
                controller.npcData[characterName] = info;
            } else {
                GameObject.FindObjectOfType<Pathfinding>().InitializePath(new Vector3(0.5f, -4.5f, 0), nextTarget, 0, path);
                info.map = 0;
                info.x = 0.5f;
                info.y = -4.5f;
                info.z = 0;
                info.spawned = true;
                controller.npcData[characterName] = info;
            }
            controller.FinishPathData(path, characterName);
            Destroy(this.gameObject);
        }

        if(Monitor.TryEnter(path, 1) && path.Count == 0 && info.map == 1 && transform.position == GameObject.Find("NPCSpawnPoint").transform.position) {
            info.x = transform.position.x;
            info.y = transform.position.y;
            info.z = transform.position.z;
            info.spawned = false;
            controller.npcData[characterName] = info;
            Destroy(this.gameObject);
        }
        Monitor.Exit(path);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.isTrigger) { return; }
        lock (path) {
            if (path.Count > 0) {
                if (SceneManager.GetActiveScene().name.Equals("House")) {
                    Vector3 temp = path[path.Count - 1];
                    path.Clear();
                    GameObject.FindObjectOfType<Pathfinding>().InitializePath(transform.position, temp, 0, path);
                } else {
                    Vector3 temp = path[path.Count - 1];
                    path.Clear();
                    GameObject.FindObjectOfType<Pathfinding>().InitializePath(transform.position, temp, 1, path);
                }
            }
        }
    }
}
