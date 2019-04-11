using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyManager : MonoBehaviour {


    Bunny[] bunnies;

    public bool isPlayerInMeadow;
    public Vector3 bunnyHome;


	// Use this for initialization
	void Start () {
        bunnies = GameObject.FindObjectsOfType<Bunny>();
        isPlayerInMeadow = false;
        bunnyHome = GameObject.Find("BunnyHome").transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		if(isPlayerInMeadow == true)
        {
            PlayerFollow();
        }
        else
        {
            PlayerLeft();
        }
	}

    void PlayerFollow()
    {
        int ran = Random.Range(0, bunnies.Length);

        bunnies[ran].followPlayer = true;
    }

    void PlayerLeft()
    {
        for(int i = 0; i < bunnies.Length; i++)
        {
            bunnies[i].followPlayer = false;
        }
    }

    
}
