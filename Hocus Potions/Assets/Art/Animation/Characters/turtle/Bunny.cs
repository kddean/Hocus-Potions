using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour {


    string currentAnim;
    Animator bunnyAnim;

    public Vector3 currentLocation;
    public Vector3 destination;
    public Vector3 fleeLocation;

    bool idling;
    public bool followPlayer;
    public bool fleePlayer;

	// Use this for initialization
	void Start () {

        currentAnim = "Forward";
        bunnyAnim = GetComponentInChildren<Animator>();
        idling = false;
        currentLocation = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        currentLocation = this.transform.position;

        //Movement
        if (bunnyAnim != null && bunnyAnim.enabled)
        {
            if (transform.position.x < destination.x)
            {
                bunnyAnim.SetBool(currentAnim, false);
                currentAnim = "Right";
                bunnyAnim.SetBool(currentAnim, true);
            }
            else if (transform.position.x > destination.x)
            {
                bunnyAnim.SetBool(currentAnim, false);
                currentAnim = "Left";
                bunnyAnim.SetBool(currentAnim, true);

            }
            else if (transform.position.y > destination.y)
            {
                bunnyAnim.SetBool(currentAnim, false);
                currentAnim = "Forward";
                bunnyAnim.SetBool(currentAnim, true);

            }
            else if (transform.position.y < destination.y || idling == true)
            {
                bunnyAnim.SetBool(currentAnim, false);
                currentAnim = "Backward";
                bunnyAnim.SetBool(currentAnim, true);
            }
        }


        if (followPlayer)
        {
            followingPlayer();
        }
        else if (fleePlayer && fleeLocation == null)
        {
            fleeingPlayer();
        }
        else if (currentLocation != destination)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, destination, Time.deltaTime);

        }
        else { Wandering(); }

    }

    void Wandering ()
    {
        float sideToSide = Random.Range(2, 68);
        float upToDown = Random.Range(-45, -20);

        destination = new Vector2(sideToSide, upToDown);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        destination = GameObject.Find("BunnyHome").transform.position;
        StartCoroutine(Wander());
    }

    IEnumerator Wander()
    {
        yield return new WaitForSeconds(3);
        Wandering();
    }


    void followingPlayer()
    {
       
      if(currentLocation != destination)
        {           
            destination = GameObject.Find("BunnyManager").GetComponent<BunnyManager>().Player.transform.position + new Vector3(1,0);
            this.transform.position =  Vector2.MoveTowards(this.transform.position, destination, Time.deltaTime);
        }
        else
        {
            idling = true;
        }       

    }

    void fleeingPlayer()
    {
        fleeLocation = this.transform.position - GameObject.Find("BunnyManager").GetComponent<BunnyManager>().Player.transform.position;
        if(fleeLocation.x > 2)
        {
            fleeLocation.x = 2;
        }
        if (fleeLocation.x > 68)
        {
            fleeLocation.x = 68;
        }
        if (fleeLocation.y < -20)
        {
            fleeLocation.y = -20;
        }
        if (fleeLocation.y < -45)
        {
            fleeLocation.y = -45;
        }
        Debug.Log(fleeLocation);
        destination = fleeLocation;
    }

}
