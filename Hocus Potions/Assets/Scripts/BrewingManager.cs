using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrewingManager : MonoBehaviour {
    int brewing; //0 = off, 1 = brewing, 2 = finished
    Potion pot;
    MoonCycle mc;
    float brewTime;
    float currentTime;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    public void Begin(float t, Potion p) {
        StartCoroutine(StartBrewing(t, p));
    }
    IEnumerator StartBrewing(float time, Potion p) {
        brewTime = time;
        pot = p;
        Brewing = 1;
        while (currentTime < brewTime) {
            yield return new WaitForSeconds(mc.CLOCK_SPEED);
            currentTime += 10;
        }
       
        Brewing = 2;
        currentTime = 0;
    }

    public int Brewing {
        get {
            return brewing;
        }

        set {
            brewing = value;
        }
    }
    private void Start() {
        mc = GameObject.Find("Clock").GetComponent<MoonCycle>();
    }

    public Potion Pot {
        get {
            return pot;
        }

        set {
            pot = value;
        }
    }

    public float BrewTime {
        get {
            return brewTime;
        }

        set {
            brewTime = value;
        }
    }

    public float CurrentTime {
        get {
            return currentTime;
        }

        set {
            currentTime = value;
        }
    }
}
