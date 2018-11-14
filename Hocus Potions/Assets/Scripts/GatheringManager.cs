using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GatheringManager : MonoBehaviour {

    public GameObject[] spawners;
    MoonCycle mc;
    ResourceLoader rl;
    public List<string> plants;
    GameObject[] plantList;
    public int defaultResetTime = 3;


    public struct SpawnerData
    {
        public bool hasSpawnedItem;
        //public int numberOfDaysLeft;
        public Item spawnedItem;
    }

    public struct SpawnerResetTime
    {
        public int numberOfDaysLeft;
        public Gathering spawner;
    }

    public Dictionary<string, SpawnerResetTime> spawnerReset;
    public Dictionary<string, SpawnerData> spawnerData;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
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

        if (spawnerReset.Count == 0)
        {
            SetResetDictionary();
        }


        if(spawnerReset.TryGetValue(gatherer.gameObject.name, out sRT))
        {

            if (sRT.numberOfDaysLeft > 0)
            {
                if (spawnerData.TryGetValue(gatherer.gameObject.name, out sD))
                {
                    
                    if (sD.hasSpawnedItem == true)
                    {
                        gatherer.GetComponent<SpriteRenderer>().sprite = rl.ingredients[sD.spawnedItem.name].image;
                        
                    }
                    else
                    { return; }
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
                //newData.numberOfDaysLeft = 2;
                newData.hasSpawnedItem = true;

                spawnerData.Add(gatherer.gameObject.name, newData);

                spawnerReset.TryGetValue(gatherer.gameObject.name, out newTime);
                newTime.numberOfDaysLeft = defaultResetTime;

                gatherer.GetComponent<SpriteRenderer>().sprite = newData.spawnedItem.image;

                Debug.Log("Now?");
                //Instantiate(, this.transform.position, Quaternion.identity);

            }


        }
        else
        {
            StartCoroutine(Spawn());
        }
               
       
    }

    IEnumerator Spawn()
    {
 
        if(spawnerReset.Count == 0 && spawners.Length > 0 )
        {
            SetResetDictionary();
        }
       /* else if(spawnerData.Count == 0)
        {
            yield return new WaitForSeconds(mc.CLOCK_SPEED);
            StartCoroutine(Spawn());
        }*/
        else
        {
            List<string> keys = spawnerData.Keys.ToList();
            foreach ( string spawner in keys)
            {
                SpawnerData sD;
                if(spawnerData.TryGetValue(spawner, out sD))
                {
                    if (sD.hasSpawnedItem == true)
                    {
                        GameObject.Find(spawner).GetComponent<SpriteRenderer>().sprite = rl.ingredients[sD.spawnedItem.name].image;
                    }
                    else
                    { yield return new WaitForSeconds(mc.CLOCK_SPEED); }
                }
                

            }
            yield return new WaitForSeconds(mc.CLOCK_SPEED);
        }
    }


    void SetResetDictionary()
    {
        for (int i = 0; i < spawners.Length; i++)
        {
            SpawnerResetTime newTime = new SpawnerResetTime();
            newTime.numberOfDaysLeft = 0;
            spawnerReset.Add(spawners[i].name, newTime);
        }
      
    }
}
