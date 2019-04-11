using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour {


    string currentAnim;
    Animator bunnyAnim;

    public Vector3 currentLocation;
    public Vector3 destination;

    bool idling;
    public bool followPlayer;

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
            else if (transform.position.y < destination.y)
            {
                bunnyAnim.SetBool(currentAnim, false);
                currentAnim = "Backward";
                bunnyAnim.SetBool(currentAnim, true);
            }
        }

        if (currentLocation != destination)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, destination, Time.deltaTime);

        }
        else { Wandering(); }

    }

    void Wandering ()
    {
        float sideToSide = Random.Range(-50, 50);
        float upToDown = Random.Range(-50, 50);

        destination = new Vector2(sideToSide, upToDown);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        destination = GameObject.Find("BunnyHome").transform.position;
        Wander();
    }

    IEnumerator Wander()
    {
        yield return new WaitForSeconds(3);
        Wandering();
    }

}
