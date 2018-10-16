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

    void OnMouseDown() {
        SwapVisible(panel.GetComponent<CanvasGroup>());
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
    }

    public void TakePotion() {
        GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>().inv.add(pot, pot.name, pot.image); ;
        name.text = "";
        pic.GetComponent<CanvasGroup>().alpha = 0;

        SwapVisible(brew.GetComponent<CanvasGroup>());
        SwapVisible(take.GetComponent<CanvasGroup>());  
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
