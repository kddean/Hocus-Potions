using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public float[] xBounds, yBounds;
    Player player;
    Vector3 pos;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
        pos = player.transform.position;
        pos.x = Mathf.Clamp(pos.x, xBounds[0], xBounds[1]);
        pos.y = Mathf.Clamp(pos.y, yBounds[0], yBounds[1]);
        pos.z = -10;
        transform.position = pos;
	}
}
