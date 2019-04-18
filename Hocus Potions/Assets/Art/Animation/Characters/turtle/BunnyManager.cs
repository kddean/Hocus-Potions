using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyManager : MonoBehaviour {


    Bunny[] bunnies;

    public bool isPlayerInMeadow;
    public Vector3 bunnyHome;

    public GameObject Player;
    public bool alreadySetBunnies;

    

    /*
     * Be able to use potions on the bunnies 
     * Transformation potion - Turn bunnies into a turtle
     * 
     * Adjust the bunnies movement behaviors 
     * Bunnies should flock together
     * 
     * Add butterflies
     * 
     * Creatures should react to the goddess costume
     * Costume_Goddess
     */

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
        
        if(isPlayerInMeadow && Player.GetComponent<Player>().Status.Contains(global::Player.PlayerStatus.transformed))
        {
            PlayerIsACat();
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
            bunnies[i].fleePlayer = false;
        }
        alreadySetBunnies = false;
    }

    void PlayerIsACat()
    {
        for(int i = 0; i < bunnies.Length; i++)
        {
            bunnies[i].fleePlayer = true;
            bunnies[i].followPlayer = false;
        }
    }

    
}
