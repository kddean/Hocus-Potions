using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour {
    public Image manaBar;
    float maxMana, currentMana;
    bool inUse;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    void Start () {
        inUse = false;
        MaxMana = 100;
        CurrentMana = MaxMana;
        manaBar.fillAmount = 1.0f;

    }


    public void UpdateMana(float amount) {
        if(currentMana - amount < 0) {
            amount = currentMana;
        } else if( currentMana - amount > maxMana) {
            amount = maxMana - currentMana;
        }

        StartCoroutine(DrainBar(amount));
    }

    IEnumerator DrainBar(float amount) {
        inUse = true;
        float t = 0;
        while (t < 1) {
            manaBar.fillAmount = Mathf.Lerp(currentMana, currentMana - amount, t) / maxMana;
            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        currentMana -= amount;
        inUse = false;
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

    public bool InUse {
        get {
            return inUse;
        }
    }
}
