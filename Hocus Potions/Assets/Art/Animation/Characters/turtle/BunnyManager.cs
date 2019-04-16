using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyManager : MonoBehaviour {


    Bunny[] bunnies;

    public bool isPlayerInMeadow;
    public Vector3 bunnyHome;

    public GameObject Player;
    public bool alreadySetBunnies;


	// Use this for initialization
	void Start () {
        bunnies = GameObject.FindObjectsOfType<Bunny>();
        isPlayerInMeadow = false;
        bunnyHome = GameObject.Find("BunnyHome").transform.position;
        Player = GameObject.FindGameObjectWithTag("Player");
        alreadySetBunnies = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (isPlayerInMeadow == true && alreadySetBunnies == false)
        {
            Debug.Log("Now following");
            PlayerFollow();
        }
        else if(isPlayerInMeadow == false)
        {
            PlayerLeft();
        }
	}

    void PlayerFollow()
    {
        int ran = Random.Range(0, bunnies.Length);

        bunnies[ran].followPlayer = true;
        bunnies[ran].destination = Player.transform.position;

        alreadySetBunnies = true;
        Debug.Log("Stalking the Player");
    }

    void PlayerLeft()
    {
        for(int i = 0; i < bunnies.Length; i++)
        {
            bunnies[i].followPlayer = false;
        }
        alreadySetBunnies = false;
    }

    
}
