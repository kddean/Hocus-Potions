using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Gathering : MonoBehaviour {

    public GameObject spawnedItem;
    ResourceLoader rl;
    SpriteRenderer[] sprites;
    public Sprite sprite;
    public int ran;
    public List<string> plants;

    // Use this for initialization
    void Start () {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        //Display plant if spawener has one else go get one
        GatheringManager.SpawnerData temp;
        if (rl.gatheringManager.spawnerData.TryGetValue(gameObject.name, out temp))
        {
            this.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite = rl.ingredients[temp.spawnedItem].image;
        }
        else
        {
            rl.gatheringManager.Populate(this);
        }

        /*rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        sprite.sprite = Resources.Load<Sprite>("Plants/poppy_inv");
        Instantiate(sprite.sprite, this.transform.position, Quaternion.identity);*/

    }

    // Update is called once per frame
    void Update () {

	}

    private void OnMouseDown()
    {
        GatheringManager.SpawnerData temp;
        rl.gatheringManager.spawnerData.TryGetValue(gameObject.name, out temp);
        rl.inv.Add(rl.ingredients[temp.spawnedItem], rl.ingredients[temp.spawnedItem].name, rl.ingredients[temp.spawnedItem].image);

        this.GetComponent<SpriteRenderer>().sprite = null;
        rl.gatheringManager.spawnerData.Remove(gameObject.name);
    }
}
