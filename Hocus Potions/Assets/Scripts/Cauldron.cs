using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //replace this once we have a player character to attach the inventory to
    Potion pot;

    void OnMouseDown() {
        CanvasGroup cGroup = panel.GetComponent<CanvasGroup>();
        cGroup.alpha = 1f;
        cGroup.blocksRaycasts = true;
        cGroup.interactable = true;
    }

    public void brewPotion() {
        Brewing b = new Brewing();

        pot = b.Brew(first.options[first.value].text, second.options[second.value].text, third.options[third.value].text);
        name.text = pot.name;
        pic.sprite = pot.image;
        pic.GetComponent<CanvasGroup>().alpha = 1;

        //swap which button is visible
        CanvasGroup brewGroup = brew.GetComponent<CanvasGroup>();
        brewGroup.alpha = 0;
        brewGroup.interactable = false;
        brewGroup.blocksRaycasts = false;

        CanvasGroup takeGroup = take.GetComponent<CanvasGroup>();
        takeGroup.alpha = 1;
        takeGroup.interactable = true;
        takeGroup.blocksRaycasts = true;
    }

    public void takePotion() {
        GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>().inv.add(pot, pot.name, pot.image); ;
        name.text = "";
        pic.GetComponent<CanvasGroup>().alpha = 0;
        CanvasGroup brewGroup = brew.GetComponent<CanvasGroup>();
        brewGroup.alpha = 1;
        brewGroup.interactable = true;
        brewGroup.blocksRaycasts = true;


        CanvasGroup takeGroup = take.GetComponent<CanvasGroup>();
        takeGroup.alpha = 0;
        takeGroup.interactable = false;
        takeGroup.blocksRaycasts = false;
    }

    public void close() {
        CanvasGroup cGroup = panel.GetComponent<CanvasGroup>();
        cGroup.alpha = 0f;
        cGroup.blocksRaycasts = false;
        cGroup.interactable = false;
        first.value = -1;
        second.value = -1;
        third.value = -1;
        name.text = "";
        pic.GetComponent<CanvasGroup>().alpha = 0;

        CanvasGroup brewGroup = brew.GetComponent<CanvasGroup>();
        brewGroup.alpha = 1;
        brewGroup.interactable = true;
        brewGroup.blocksRaycasts = true;

        CanvasGroup takeGroup = take.GetComponent<CanvasGroup>();
        takeGroup.alpha = 0;
        takeGroup.interactable = false;
        takeGroup.blocksRaycasts = false;
    }
}
