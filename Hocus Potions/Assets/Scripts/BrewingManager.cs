using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrewingManager : MonoBehaviour {
    int brewing; //0 = off, 1 = brewing, 2 = finished
    Potion pot;
    MoonCycle mc;

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
        
        pot = p;
        Brewing = 1;
        yield return new WaitForSeconds(time/10 * mc.CLOCK_SPEED);
        Brewing = 2;
    }
}
