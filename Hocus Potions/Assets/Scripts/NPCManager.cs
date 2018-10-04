using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour {
    MoonCycle mc;
    int lastHour;
    int spawnHour, spawnMinute;
    bool timeSet;
    public GameObject spawnPoint;
	void Start () {
        mc = (MoonCycle) GameObject.FindObjectOfType(typeof(MoonCycle));
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
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            Traveller trav = go.AddComponent<Traveller>();
            trav.request = new Potion("health potion", Resources.Load<Sprite>("Potions/potions_healing_1"), 1, Ingredient.Attributes.healing, null, null);
            trav.Character = Resources.Load<Sprite>("witch");
            trav.CharacterName = "Random Traveller"+spawnHour.ToString();
            sr.sprite = trav.Character;
            go.name = trav.CharacterName;
            lastHour = spawnHour;
        }

        if (mc.Hour == 21) {
            timeSet = false;
        }
	}
}
