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

    void OnMouseDown() {
        CanvasGroup cGroup = panel.GetComponent<CanvasGroup>();
        cGroup.alpha = 1f;
        cGroup.blocksRaycasts = true;
        cGroup.interactable = true;
    }

    public void brewPotion() {
        Brewing b = new Brewing();
        Potion pot = b.Brew(first.options[first.value].text, second.options[second.value].text, third.options[third.value].text);
        name.text = pot.name;
        pic.sprite = pot.image;
        pic.GetComponent<CanvasGroup>().alpha = 1;
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
    }
}
