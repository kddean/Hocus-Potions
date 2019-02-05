using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour {

    GameObject player;

    Animator turtleAnim;
    public float speed = 5;

    Vector2 position;
    float orientation;
    Vector2 velocity;
    float rotation;

	// Use this for initialization
	void Start () {
		turtleAnim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
        SeekPlayer();
	}

    void UpdateTurtle(Vector2 vel)
    {
        position += (velocity * Time.deltaTime);
        velocity += (vel * Time.deltaTime);
    }

    void SeekPlayer()
    {
        this.transform.position = Vector2.MoveTowards(this.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position - new Vector3(1f, 0), speed * Time.deltaTime);
    }
}
