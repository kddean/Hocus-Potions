using System.Collections;
using System.Collections.Generic;
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
    }

    private void Update() {
        if (sceneSwapped) {
            info.x = transform.position.x;
            info.y = transform.position.y;
            info.z = transform.position.z;
            controller.npcData[characterName] = info;
            if (path.Count > 0) {
                controller.FinishPathData(path, CharacterName);
            }
            sceneSwapped = false;
        }

        if (path.Count > 0) {
            transform.position = Vector3.MoveTowards(transform.position, path[0], Time.deltaTime * speed);
            if (transform.position == path[0]) {
                path.RemoveAt(0);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.isTrigger) { return; }
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
