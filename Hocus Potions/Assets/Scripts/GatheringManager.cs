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

    public List<string> MeadowList;
    public List<string> ForestList;
    public List<string> MountainList;

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
        SetResetDictionary();

        MeadowList = new List<string>();
        ForestList = new List<string>();
        MountainList = new List<string>();
        CreateLists();

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
        SpawnerResetTime sRT;

        //Debug.Log(spawnerReset.Count);
        if(spawnerReset.Count == 0)
        {
            SetResetDictionary();
            
        }

        if(spawnerReset.TryGetValue(gatherer.gameObject.name, out sRT))
        {           
            if(sRT.numberOfDaysLeft > 0)
            {
                return;
            }
            else
            {
                SpawnerResetTime newTime = new SpawnerResetTime();
                SpawnerData newData = new SpawnerData();
                string newPlant = "";
                /*do {
                    int ran = Random.Range(0, rl.ingredients.Count);
                    newPlant = plants[ran];
                } while (!rl.ingredients[newPlant].imagePath.StartsWith("Plants"));*/

                int ran;
                if (gatherer.name.Contains("Forest"))
                {
                    ran = Random.Range(0, ForestList.Count);
                    newPlant = ForestList[ran];
                }
                else if (gatherer.name.Contains("Meadow"))
                {
                    ran = Random.Range(0, MeadowList.Count);
                    newPlant = MeadowList[ran];
                }
                else if (gatherer.name.Contains("Mountain"))
                {
                    ran = Random.Range(0, MountainList.Count);
                    newPlant = MountainList[ran];
                }

                newData.spawnedItem = rl.ingredients[newPlant];
                newData.hasSpawnedItem = true;

                spawnerData.Add(gatherer.gameObject.name, newData);

                spawnerReset.TryGetValue(gatherer.gameObject.name, out newTime);
                newTime.numberOfDaysLeft = defaultResetTime;
                spawnerReset[gatherer.gameObject.name] =  newTime;

                Sprite[] plantSprites = Resources.LoadAll<Sprite>("Plants/" + newData.spawnedItem.name);
                gatherer.GetComponent<SpriteRenderer>().sprite = plantSprites[plantSprites.Length - 1];
            }
        }
        else
        {
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
       
        if (mc.Days > daystrack)
        {
            List<string> resetKeys = spawnerReset.Keys.ToList();
            foreach (string spawner in resetKeys)
            {
                SpawnerResetTime sRT;
                spawnerReset.TryGetValue(spawner, out sRT);
                sRT.numberOfDaysLeft -= 1;
                spawnerReset[spawner] = sRT;
            }
            daystrack = mc.Days;
        }
        if(spawnerReset.Count == 0 && spawners.Length > 0)
        {
            SetResetDictionary();
            //Debug.Log("Dictionary Set");
        }
        else
        {
            if (SceneManager.GetActiveScene().name.Equals("SampleGameArea"))
            {
                List<string> keys = spawnerData.Keys.ToList();
                foreach (string spawner in keys)
                {
                    SpawnerData sD;
                    if (spawnerData.TryGetValue(spawner, out sD))
                    {
                        if (sD.hasSpawnedItem == true)
                        {
                            Sprite[] plantSprites = Resources.LoadAll<Sprite>("Plants/" + sD.spawnedItem.name);
                            GameObject.Find(spawner).GetComponent<SpriteRenderer>().sprite = plantSprites[plantSprites.Length - 1];
                        }
                        else
                        {
                            yield return new WaitForSeconds(mc.CLOCK_SPEED);
                            StartCoroutine(Spawn());
                        }
                    }
                }
            }

            yield return new WaitForSeconds(mc.CLOCK_SPEED);
            
        }
        StartCoroutine(Spawn());
    }

   public void SetResetDictionary()
    {
        spawners = GameObject.FindGameObjectsWithTag("spawner");

        for (int i = 0; i < spawners.Length; i++)
        {
            SpawnerResetTime newTime = new SpawnerResetTime();
            newTime.numberOfDaysLeft = 0;
            spawnerReset.Add(spawners[i].name, newTime);
            spawners[i].GetComponent<Gathering>().SpawnPlants();
        }   
    }

   public void SeedDrop(Gathering gatherer)
    {
        int numOfSeedsToDrop = 0;
        
        // 0 and 1
        float ran = Random.Range(0, 2);
        //Debug.Log(ran);
        if(ran <= 0.4) { return; }
        else
        {
            numOfSeedsToDrop = 1;
        }

        SpawnerData temp3;
        spawnerData.TryGetValue(gatherer.name, out temp3);
        try {
            Seed droppedSeed = rl.seeds[temp3.spawnedItem.name];
            Inventory.Add(droppedSeed, numOfSeedsToDrop, true);
        } catch (KeyNotFoundException) {
        }
    }

    void CreateLists()
    {
        MeadowList.Add("lavender");
        MeadowList.Add("catnip");
        MeadowList.Add("poppy");
        MeadowList.Add("lambsgrass");
        MeadowList.Add("dandylion");

        ForestList.Add("ghostcap");
        ForestList.Add("morel");
        ForestList.Add("fly_agaric");
        ForestList.Add("nightshade");
        ForestList.Add("lily");
        ForestList.Add("mugwort");

        MountainList.Add("thistle");
        MountainList.Add("indigo");
    }
}
