using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour {


    public float speed;

    private Rigidbody2D body;

    public static string heldItem = "seeds";

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody2D>();
        speed = 7.5f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {

        Vector2 pos = transform.position;

        if (Input.GetKey("w"))
        {
            pos.y += speed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos.y -= speed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos.x += speed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos.x -= speed * Time.deltaTime;
        }


        transform.position = pos;

    }
}
