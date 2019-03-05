using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour {
    BrewingManager manager;
	// Use this for initialization
	void Start () {
        manager = GameObject.FindObjectOfType<BrewingManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(manager.Brewing == 1) {
            GetComponent<SpriteRenderer>().enabled = true;
        } else {
            GetComponent<SpriteRenderer>().enabled = false;
        }
	}
}
