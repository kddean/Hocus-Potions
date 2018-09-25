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
<<<<<<< HEAD
=======
    public Button brew;
    public Button take;

    //replace this once we have a player character to attach the inventory to
    Inventory inv = new Inventory();
    Potion pot;
>>>>>>> e113f4a3000f9ed01fa3544f1f348323ffd6b9a6

    void OnMouseDown() {
        CanvasGroup cGroup = panel.GetComponent<CanvasGroup>();
        cGroup.alpha = 1f;
        cGroup.blocksRaycasts = true;
        cGroup.interactable = true;
    }

    public void brewPotion() {
        Brewing b = new Brewing();
<<<<<<< HEAD
        Potion pot = b.Brew(first.options[first.value].text, second.options[second.value].text, third.options[third.value].text);
        name.text = pot.name;
        pic.sprite = pot.image;
        pic.GetComponent<CanvasGroup>().alpha = 1;
    }

=======
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
        inv.add(pot, pot.name);

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
>>>>>>> e113f4a3000f9ed01fa3544f1f348323ffd6b9a6
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
<<<<<<< HEAD
=======
        CanvasGroup brewGroup = brew.GetComponent<CanvasGroup>();
        brewGroup.alpha = 1;
        brewGroup.interactable = true;
        brewGroup.blocksRaycasts = true;
>>>>>>> e113f4a3000f9ed01fa3544f1f348323ffd6b9a6
    }
}
