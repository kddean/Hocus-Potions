using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GarbageCollecter : MonoBehaviour {

    public static int MAX_LIFE_TIME = 360; // In Minutes
    public struct DroppedItemData {
        public Item item;
        public int count;
        public int lifeTime;
        public Vector3 position;
        public string scene;
        public GameObject go;

        public DroppedItemData(Item item, int count, Vector3 position, string scene, GameObject go) {
            this.item = item;
            this.count = count;
            this.position = position;
            this.scene = scene;
            this.lifeTime = MAX_LIFE_TIME;
            this.go = go;
        }
    }

    public List<DroppedItemData> droppedItems;

    MoonCycle mc;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        mc = GameObject.Find("Clock").GetComponent<MoonCycle>();
        droppedItems = new List<DroppedItemData>();
        StartCoroutine(CleanUp());
	}

    public void RemoveItem(Item i, Vector3 pos, string scene) {
        foreach(DroppedItemData d in droppedItems) {
            if(d.item == i && d.position == pos && d.scene.Equals(scene)) {
                droppedItems.Remove(d);
                break;
            }
        }
    }
	
    IEnumerator CleanUp() {
        List<DroppedItemData> junk = new List<DroppedItemData>();
        foreach(DroppedItemData d in droppedItems) {
            DroppedItemData temp = d;
            temp.lifeTime -= 10;
            if(temp.lifeTime <= 0) {
                Destroy(temp.go);
            } else {
                junk.Add(temp);
            }
        }
        droppedItems = junk;
        yield return new WaitForSeconds(mc.CLOCK_SPEED);
        StartCoroutine(CleanUp());
    }

    public void CallSpawner() {
        StartCoroutine(SpawnDropped());
    }

    IEnumerator SpawnDropped() {
        yield return new WaitForSeconds(0.01f);
        List<DroppedItemData> junk = new List<DroppedItemData>();
        foreach(DroppedItemData d in droppedItems) {
            if(d.scene.Equals(SceneManager.GetActiveScene().name)) {
                DroppedItemData temp = d;
                GameObject go = new GameObject { name = d.item.name };
                temp.go = go;
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = d.item.image;
                go.transform.position = d.position;
                 if (!(d.item is Ingredient)) {
                    go.transform.localScale = new Vector3(0.3f, 0.3f, 1);
                }

                Vector2 bounds = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
                BoxCollider2D c = go.AddComponent<BoxCollider2D>();
                c.size = bounds;
                c.isTrigger = true;
                Pickups p = go.AddComponent<Pickups>();
                p.Item = d.item;
                p.Count = d.count;
                p.Data = temp;
                junk.Add(temp);
            } else {
                junk.Add(d);
            }
        }
        droppedItems = junk;
    }
}
