using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour {


    public float speed;

    private Rigidbody2D body;

<<<<<<< HEAD
    public static string heldItem = "seeds";

=======
>>>>>>> e113f4a3000f9ed01fa3544f1f348323ffd6b9a6
    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody2D>();
        speed = 10.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
<<<<<<< HEAD
=======
        
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        //body.AddForce(movement * speed);
>>>>>>> e113f4a3000f9ed01fa3544f1f348323ffd6b9a6

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
