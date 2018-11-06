using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cauldron : MonoBehaviour {
    public GameObject panel;
    public Dropdown first;
    public Dropdown second;
    public Dropdown third;
    public Text name;
    public Image pic;

    public Button brew;
    public Button take;
    Potion pot;
    BrewingManager manager;
    MoonCycle mc;
    Animator[] anims;
    GameObject sparkles;
    GameObject bubbles;

    bool done;

    private void Start() {
        manager = GameObject.Find("BrewingManager").GetComponent<BrewingManager>();
        mc = GameObject.Find("Clock").GetComponent<MoonCycle>();
        sparkles = GameObject.Find("sparkles");
        bubbles = GameObject.Find("bubbles");
        anims = GetComponentsInChildren<Animator>();
        done = false;
        switch (GameObject.Find("BrewingManager").GetComponent<BrewingManager>().Brewing) {
            case 0:
                foreach (Animator a in anims) {
                    a.SetBool("idle", true);
                }
                bubbles.GetComponent<SpriteRenderer>().enabled = false;
                sparkles.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case 1:
                foreach (Animator a in anims) {
                    a.SetBool("idle", false);
                }
                bubbles.GetComponent<SpriteRenderer>().enabled = true;
                sparkles.GetComponent<SpriteRenderer>().enabled = true;
                break;
            case 2:
                foreach (Animator a in anims) {
                    a.SetBool("idle", true);
                }
                anims[0].SetBool("full", true);
                bubbles.GetComponent<SpriteRenderer>().enabled = true;
                sparkles.GetComponent<SpriteRenderer>().enabled = false;
                break;
            default:
                Debug.Log("How did it get to this");
                break;
        }
    }

    private void Update() {
        if (manager.Brewing == 2 && !done) {
            SwapVisible(brew.GetComponent<CanvasGroup>());
            SwapVisible(take.GetComponent<CanvasGroup>());

            foreach (Animator a in anims) {
                a.SetBool("idle", true);
            }
            bubbles.GetComponent<Animator>().SetBool("full", true);
            sparkles.GetComponent<SpriteRenderer>().enabled = false;

            pot = manager.Pot;
            name.text = pot.name;
            pic.sprite = pot.image;
            pic.GetComponent<CanvasGroup>().alpha = 1;
            done = true;
        }
    }

    void OnMouseDown() {
        if (manager.Brewing == 0 || manager.Brewing == 2) {
            SwapVisible(panel.GetComponent<CanvasGroup>());
        }
    }

    public void BrewPotion() {
        Brewing b = new Brewing();
        bubbles.GetComponent<SpriteRenderer>().enabled = true;
        sparkles.GetComponent<SpriteRenderer>().enabled = true;
        foreach(Animator a in anims) {
            a.SetBool("idle", false);
        }
        pot = b.Brew(first.options[first.value].text, second.options[second.value].text, third.options[third.value].text);
        manager.Begin((pot.brewingTime / 10) * mc.CLOCK_SPEED, pot);
        Close();
    }

    public void TakePotion() {
        GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>().inv.Add(pot, pot.name, pot.image); ;
        name.text = "";
        pic.GetComponent<CanvasGroup>().alpha = 0;

        anims[0].SetBool("full", false);
        SwapVisible(brew.GetComponent<CanvasGroup>());
        SwapVisible(take.GetComponent<CanvasGroup>());
        bubbles.GetComponent<SpriteRenderer>().enabled = false;

        manager.Brewing = 0;
        done = false;
    }

    public void Close() {
        first.value = -1;
        second.value = -1;
        third.value = -1;
        name.text = "";
        pic.GetComponent<CanvasGroup>().alpha = 0;

        SwapVisible(panel.GetComponent<CanvasGroup>());
        brew.GetComponent<CanvasGroup>().alpha = 1;
        brew.GetComponent<CanvasGroup>().interactable = true;
        brew.GetComponent<CanvasGroup>().blocksRaycasts = true;
        take.GetComponent<CanvasGroup>().alpha = 0;
        take.GetComponent<CanvasGroup>().interactable = false;
        take.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void SwapVisible(CanvasGroup cg) {
        cg.alpha = Mathf.Abs(cg.alpha - 1);
        cg.interactable = !cg.interactable;
        cg.blocksRaycasts = !cg.blocksRaycasts;
    }
}
