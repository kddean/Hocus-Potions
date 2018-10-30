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

    Animator[] anims;
    GameObject sparkles;
    GameObject bubbles;

    private void Start() {
        sparkles = GameObject.Find("sparkles");
        bubbles = GameObject.Find("bubbles");
        anims = GetComponentsInChildren<Animator>();
        sparkles.SetActive(false);
        bubbles.SetActive(false);

    }
    void OnMouseDown() {
        SwapVisible(panel.GetComponent<CanvasGroup>());
        sparkles.SetActive(true);
        bubbles.SetActive(true);
       
        foreach (Animator a in anims) {
            a.SetBool("idle", false);
        }
    }

    public void BrewPotion() {
        Brewing b = new Brewing();

        pot = b.Brew(first.options[first.value].text, second.options[second.value].text, third.options[third.value].text);
        name.text = pot.name;
        pic.sprite = pot.image;
        pic.GetComponent<CanvasGroup>().alpha = 1;

        //swap which button is visible
        SwapVisible(brew.GetComponent<CanvasGroup>());
        SwapVisible(take.GetComponent<CanvasGroup>());
        sparkles.GetComponent<SpriteRenderer>().enabled = false;
        bubbles.GetComponent<Animator>().SetBool("idle", true);
        bubbles.GetComponent<Animator>().SetBool("full", true);

    }

    public void TakePotion() {
        GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>().inv.Add(pot, pot.name, pot.image); ;
        name.text = "";
        pic.GetComponent<CanvasGroup>().alpha = 0;

        SwapVisible(brew.GetComponent<CanvasGroup>());
        SwapVisible(take.GetComponent<CanvasGroup>());
        sparkles.GetComponent<SpriteRenderer>().enabled = true;
        bubbles.GetComponent<Animator>().SetBool("idle", false);
        bubbles.GetComponent<Animator>().SetBool("full", false);
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

        foreach (Animator a in anims) {
            a.SetBool("idle", true);
        }
        sparkles.SetActive(false);
        bubbles.SetActive(false);
    }

    void SwapVisible(CanvasGroup cg) {
        cg.alpha = Mathf.Abs(cg.alpha - 1);
        cg.interactable = !cg.interactable;
        cg.blocksRaycasts = !cg.blocksRaycasts;
    }
}
