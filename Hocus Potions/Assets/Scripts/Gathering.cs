using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gathering : MonoBehaviour {

    public GameObject spawnedItem;
    ResourceLoader rl;
    public SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        sprite.sprite = Resources.Load<Sprite>("Plants/poppy_inv");

        Instantiate(sprite.sprite, this.transform.position, Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        rl.inv.Add(rl.ingredients["poppy"], rl.ingredients["poppy"].name, rl.ingredients["poppy"].image);
        sprite.sprite = null;
    }
}
