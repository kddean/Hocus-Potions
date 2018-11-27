using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Gathering : MonoBehaviour {

	ResourceLoader rl;
	public List<string> plants;

	// Use this for initialization
	void Start () {
		rl = GameObject.FindGameObjectWithTag ("loader").GetComponent<ResourceLoader> ();
		
		GatheringManager.SpawnerData temp;
        GatheringManager.SpawnerResetTime temp2;
        if (rl.gatheringManager.spawnerReset.TryGetValue(gameObject.name, out temp2))
        {
            Debug.Log(gameObject.name);
            Debug.Log(temp2.numberOfDaysLeft);
            // Check how many days are left
            if (temp2.numberOfDaysLeft > 0)
            {

                //Display plant if spawner has one else do nothing
                if (rl.gatheringManager.spawnerData.TryGetValue(gameObject.name, out temp))
                {
                    this.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(temp.spawnedItem.imagePath);
                }
                else { return; }
            }          
            else
            {
                if (rl.gatheringManager.spawnerData.TryGetValue(gameObject.name, out temp)) {

                    if (temp.hasSpawnedItem == true)
                    {
                        rl.gatheringManager.spawnerData.Remove(gameObject.name);
                        Debug.Log("Plant removed");
                    }
  
                }

               // Debug.Log("Generate");
                rl.gatheringManager.Populate(this);
                Debug.Log("Plant added");

            }
        }
	}
			
	// Update is called once per frame
	void Update () {
		
	}

	private void OnMouseDown(){
		GatheringManager.SpawnerData temp;
		rl.gatheringManager.spawnerData.TryGetValue (gameObject.name, out temp);

		if (Inventory.Add (temp.spawnedItem, 1)) {
			this.GetComponent<SpriteRenderer> ().sprite = null;
			rl.gatheringManager.spawnerData.Remove (gameObject.name);
		}
	}
}
