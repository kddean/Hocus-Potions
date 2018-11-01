using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Gathering : MonoBehaviour {

    public GameObject spawnedItem;
    ResourceLoader rl;
    public SpriteRenderer[] sprites;
    public SpriteRenderer sprite;
    public int ran;
    public List<string> plants;

    // Use this for initialization
    void Start () {
        /*rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
 
        plants = rl.ingredients.Keys.ToList();
        int i = 0;
        sprites = new SpriteRenderer[rl.ingredients.Count];
        foreach (string item in plants)
        {
            Debug.Log(item);
            Debug.Log(i);
            Debug.Log(rl.ingredients[item].image);
            this.sprites[i].sprite = Resources.Load<Sprite>("Plants/" + rl.ingredients[item].image);
            
            if (i < rl.ingredients.Count)
            {
                i++;
            }
        }

        ran = Random.Range(0, rl.ingredients.Count);
        SpriteRenderer sprite = new SpriteRenderer();
        sprite.sprite = Resources.Load<Sprite>("Plants/poppy_inv");

        //Instantiate(sprites[0].sprite, this.transform.position, Quaternion.identity);
        Instantiate(sprite.sprite, this.transform.position, Quaternion.identity);*/



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
        for (int i = 0; i < rl.ingredients.Count; i++) {
            sprite.sprite = null;
        }
    }
}
