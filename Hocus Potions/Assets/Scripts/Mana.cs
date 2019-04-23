using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour {
    public Image manaBar;
    public Sprite[] effectSprites;
    public Image effectImage;
    int index = 1;
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
        if (currentMana - amount < 0) {
            amount = currentMana;
        } else if (currentMana - amount > maxMana) {
            amount = -1 * (maxMana - currentMana);
        }

        StartCoroutine(DrainBar(amount));
    }

    public void OOM() {
        StartCoroutine(PlayEffect());
    }

    IEnumerator PlayEffect() {
        effectImage.sprite = effectSprites[index];
        yield return new WaitForSeconds(0.0695f);
        index++;
        if (index < effectSprites.Length) {
            StartCoroutine(PlayEffect());
        }  else {
            effectImage.sprite = effectSprites[0];
            index = 1;
        }
    }
   

    IEnumerator DrainBar(float amount) {
        inUse = true;
        float t = 0;
        while (t < 1) {
            manaBar.fillAmount = (Mathf.Lerp(currentMana, (currentMana - amount), t) / maxMana);
            t += Time.deltaTime * 2f;
            yield return new WaitForEndOfFrame();
        }
        currentMana -= amount;
        if (currentMana == 0) {
            manaBar.fillAmount = 0;
        } else if( currentMana == MaxMana) {
            manaBar.fillAmount = 1;
        }

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
