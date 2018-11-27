using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GatheringManager : MonoBehaviour {

    public GameObject[] spawners;
    MoonCycle mc;
    ResourceLoader rl;
    public List<string> plants;
    GameObject[] plantList;
    public int defaultResetTime = 3;
    public int daystrack;

    public struct SpawnerData
    {
        public bool hasSpawnedItem;
        public Item spawnedItem;
    }

    public struct SpawnerResetTime
    {
        public int numberOfDaysLeft;
        public Gathering spawner;
    }

    public Dictionary<string, SpawnerResetTime> spawnerReset;
    public Dictionary<string, SpawnerData> spawnerData;

    public void Awake()
    {
        DontDestroyOnLoad(this);
        if(FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        mc = (MoonCycle)GameObject.FindObjectOfType(typeof(MoonCycle));
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        spawnerData = new Dictionary<string, SpawnerData>();
        spawnerReset = new Dictionary<string, SpawnerResetTime>();

        plants = rl.ingredients.Keys.ToList();
        daystrack = mc.Days;

        StartCoroutine(Spawn());
	}
	
	// Update is called once per frame
	void Update () {
        if (!SceneManager.GetActiveScene().name.Equals("SampleGameArea"))
        {
            return;
        }
        else
        {
            spawners = GameObject.FindGameObjectsWithTag("spawner");
        }
	}

    public void Populate(Gathering gatherer)
    {
        SpawnerData sD;
        SpawnerResetTime sRT;

        if(spawnerReset.Count == 0)
        {
            SetResetDictionary();
            Debug.Log("Dictionary Set");
        }

        if(spawnerReset.TryGetValue(gatherer.gameObject.name, out sRT))
        {
            if(sRT.numberOfDaysLeft > 0)
            {
                if(spawnerData.TryGetValue(gatherer.gameObject.name, out sD))
                {
                    if(sD.hasSpawnedItem == true)
                    {
                        gatherer.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(rl.ingredients[sD.spawnedItem.name].imagePath);
                    }
                    else { return; }
                }
                else { return; }
            }
            else
            {
                SpawnerResetTime newTime = new SpawnerResetTime();
                SpawnerData newData = new SpawnerData();
                int ran = Random.Range(0, rl.ingredients.Count);
                string newPlant;

                newPlant = plants[ran];

                newData.spawnedItem = rl.ingredients[newPlant];
                newData.hasSpawnedItem = true;

                spawnerData.Add(gatherer.gameObject.name, newData);

                spawnerReset.TryGetValue(gatherer.gameObject.name, out newTime);
                newTime.numberOfDaysLeft = defaultResetTime;

                gatherer.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(newData.spawnedItem.imagePath);
            }
        }
        else
        {
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        List<string> keys = spawnerData.Keys.ToList();
        if (mc.Days > daystrack)
        {
            foreach(string spawner in keys)
            {
                SpawnerResetTime sRT;
                spawnerReset.TryGetValue(spawner, out sRT);
                sRT.numberOfDaysLeft -= 1;
            }
            daystrack = mc.Days;
        }
        if(spawnerReset.Count == 0 && spawners.Length > 0)
        {
            SetResetDictionary();
        }
        else
        {
            
            foreach(string spawner in keys)
            {
                SpawnerData sD;
                if(spawnerData.TryGetValue(spawner, out sD))
                {
                    if(sD.hasSpawnedItem == true)
                    {
                        GameObject.Find(spawner).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(rl.ingredients[sD.spawnedItem.name].imagePath);
                    }
                    else
                    {
                        yield return new WaitForSeconds(mc.CLOCK_SPEED);
                        StartCoroutine(Spawn());
                    }
                }
            }

            yield return new WaitForSeconds(mc.CLOCK_SPEED);
            StartCoroutine(Spawn());
        }
    }

    void SetResetDictionary()
    {
        spawners = GameObject.FindGameObjectsWithTag("spawner");

        for (int i = 0; i < spawners.Length; i++)
        {
            SpawnerResetTime newTime = new SpawnerResetTime();
            newTime.numberOfDaysLeft = 0;
            spawnerReset.Add(spawners[i].name, newTime);
        }
    }
}
