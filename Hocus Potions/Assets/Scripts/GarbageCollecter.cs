using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GarbageCollecter : MonoBehaviour {

    public static int MAX_LIFE_TIME = 360; // In Minutes

    [System.Serializable]
    public struct DroppedItemData {
        public Item item;
        public int count;
        public int lifeTime;
        public float x, y, z;
        public string scene;
  

        public DroppedItemData(Item item, int count, float x, float y, float z, string scene) {
            this.item = item;
            this.count = count;
            this.x = x;
            this.y = y;
            this.z = z;
            this.scene = scene;
            this.lifeTime = MAX_LIFE_TIME;
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
            Vector3 temp = new Vector3(d.x, d.y, d.z);
            if (d.item == i && temp == pos && d.scene.Equals(scene)) {
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
                if (temp.scene.Equals(SceneManager.GetActiveScene().name)) {
                    Pickups[] items = GameObject.FindObjectsOfType<Pickups>();
                    Vector3 pos = new Vector3(temp.x, temp.y, temp.z);

                    foreach (Pickups p in items) {
                        if (p.transform.position == pos && p.Item == temp.item) {
                            Destroy(p.gameObject);
                        }
                    }
                }
            } else {
                junk.Add(temp);
            }
        }
        droppedItems = junk;
        yield return new WaitForSeconds(mc.CLOCK_SPEED);
        StartCoroutine(CleanUp());
    }

    public void SpawnDropped() {
        List<DroppedItemData> junk = new List<DroppedItemData>();
        foreach(DroppedItemData d in droppedItems) {
            if(d.scene.Equals(SceneManager.GetActiveScene().name)) {
                DroppedItemData temp = d;
                GameObject go = new GameObject { name = d.item.name };
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>(d.item.imagePath);
                go.transform.position = new Vector3(d.x, d.y, d.z);
                Vector2 bounds = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
                BoxCollider2D c = go.AddComponent<BoxCollider2D>();
                c.size = bounds;
                c.isTrigger = true;


                if (!(d.item is Potion)) {
                    if (d.item.imagePath.StartsWith("Gem")) {
                        go.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                    } else {
                        go.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                    }
                }

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
