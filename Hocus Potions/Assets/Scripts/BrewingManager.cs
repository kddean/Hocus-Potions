using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrewingManager : MonoBehaviour {
    int brewing; //0 = off, 1 = brewing, 2 = finished
    Potion pot;

    public int Brewing {
        get {
            return brewing;
        }

        set {
            brewing = value;
        }
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
        yield return new WaitForSeconds(time);
        Brewing = 2;
    }
}
