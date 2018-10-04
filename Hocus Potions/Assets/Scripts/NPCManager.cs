using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour {
    MoonCycle mc;
    int lastHour;
    int spawnHour, spawnMinute;
    bool timeSet;
    public GameObject spawnPoint;
    ResourceLoader rl;
	void Start () {
        mc = (MoonCycle) GameObject.FindObjectOfType(typeof(MoonCycle));
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        timeSet = false;
	}
	

	void Update () {
        if (!timeSet) {
            spawnHour = Random.Range(8, 21);
            spawnMinute = Random.Range(0, 60);
            float temp = spawnMinute / 10.0f;
            spawnMinute = (int)(Mathf.Round(temp) * 10.0f);
            timeSet = true;
        }

        if (mc.Hour == spawnHour && mc.Minutes == spawnMinute ) {
            GameObject go = new GameObject();
            go.transform.position = spawnPoint.transform.position;
            //Swap this to set sorting layer instead once theyre set up
            Vector3 tempPos = go.transform.position;
            tempPos.z = -1.0f;
            go.transform.position = tempPos;
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            go.AddComponent<BoxCollider2D>();
            //This should use prefabs eventually to just spawn them with their data intact
            Traveller temp = (Traveller)rl.npcs[Random.Range(0, rl.npcs.Count)];
            Traveller trav =  go.AddComponent<Traveller>();
            trav.Character = temp.Character;
            trav.CharacterName = temp.CharacterName;
            trav.Requests = temp.Requests;
            trav.Dialogue = temp.Dialogue;
            sr.sprite = trav.Character;
            go.name = trav.CharacterName;
            lastHour = spawnHour;
        }

        if (mc.Hour == 21) {
           timeSet = false;
        }
	}


    //Bring up dialogue options
    void OnMouseDown() {

    }
}
