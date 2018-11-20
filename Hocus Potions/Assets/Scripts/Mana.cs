using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour {
    public Image manaBar;
    float maxMana, currentMana;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    void Start () {
        MaxMana = 100;
        //CurrentMana = MaxMana;
        currentMana = 0;
        UpdateMana();
	}


    public void UpdateMana() {
        manaBar.fillAmount = currentMana / MaxMana;
    }

    public float CurrentMana {
        get {
            return currentMana;
        }

        set {
            currentMana = value;
        }
    }

    public float MaxMana {
        get {
            return maxMana;
        }

        set {
            maxMana = value;
        }
    }
}
