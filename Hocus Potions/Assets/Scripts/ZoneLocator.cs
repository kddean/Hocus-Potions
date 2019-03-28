using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneLocator : MonoBehaviour {

    GameObject bm;
	// Use this for initialization
	void Start () {
        bm = GameObject.Find("BookManager");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.gameObject.tag.Equals("Player"))
        {
            return;
        }
        else
        {
            bm.GetComponent<BookManager>().CurrentZone = this.gameObject;
        }
    }
}
