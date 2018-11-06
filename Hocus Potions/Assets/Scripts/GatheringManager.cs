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


    public struct SpawnerData
    {
        public bool hasSpawnedItem;
        public string spawnedItem;
        public int numberOfDaysLeft;
    }

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

        plants = rl.ingredients.Keys.ToList();

       
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

        if (spawnerData.TryGetValue(gatherer.gameObject.name, out sD))
        {
            if(sD.hasSpawnedItem == true)
            {
                gatherer.GetComponent<SpriteRenderer>().sprite = rl.ingredients[sD.spawnedItem].image;
            }
            else
            { return; } 
        }
        else
        {
           
            SpawnerData newData = new SpawnerData();
            int ran = Random.Range(0, rl.ingredients.Count);
            string newPlant;

            newPlant = plants[ran];
            
            newData.spawnedItem = rl.ingredients[newPlant].name;
            newData.numberOfDaysLeft = 2;
            newData.hasSpawnedItem = true;

            spawnerData.Add(gatherer.gameObject.name, newData);

            Debug.Log("Now?");
            //Instantiate(, this.transform.position, Quaternion.identity);

        }
    }
}
