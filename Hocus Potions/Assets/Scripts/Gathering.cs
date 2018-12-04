using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Gathering : MonoBehaviour {

	ResourceLoader rl;
    public Sprite[] plants;

	// Use this for initialization
	void Start () {
		rl = GameObject.FindGameObjectWithTag ("loader").GetComponent<ResourceLoader> ();
        SpawnPlants();
	}
			
	// Update is called once per frame
	void Update () {
		
	}

	private void OnMouseDown(){
		GatheringManager.SpawnerData temp;
		rl.gatheringManager.spawnerData.TryGetValue (gameObject.name, out temp);

		if (Inventory.Add (temp.spawnedItem, 1)) {
			this.GetComponent<SpriteRenderer> ().sprite = null;
            rl.gatheringManager.SeedDrop(this);
            rl.gatheringManager.spawnerData.Remove (gameObject.name);
            
		}
	}

   public void SpawnPlants()
    {
        GatheringManager.SpawnerData temp;
        GatheringManager.SpawnerResetTime temp2;
        if (rl.gatheringManager.spawnerReset.TryGetValue(gameObject.name, out temp2))
        {          
            // Check how many days are left
            if (temp2.numberOfDaysLeft > 0)
            {

                //Display plant if spawner has one else do nothing
                if (rl.gatheringManager.spawnerData.TryGetValue(gameObject.name, out temp))
                {
                    plants = Resources.LoadAll<Sprite>("Plants/" + temp.spawnedItem.name);
                    this.GetComponent<SpriteRenderer>().sprite = plants[plants.Length - 1];
                }
                else { return; }
            }
            else
            {
                if (rl.gatheringManager.spawnerData.TryGetValue(gameObject.name, out temp))
                {

                    if (temp.hasSpawnedItem == true)
                    {
                        rl.gatheringManager.spawnerData.Remove(gameObject.name);
                       
                    }

                }

                // Debug.Log("Generate");
                rl.gatheringManager.Populate(this);
               

            }
        }
    }
}
