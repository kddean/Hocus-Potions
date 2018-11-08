using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cauldron : MonoBehaviour {
    public GameObject brewPanel;
    public GameObject ingredientPanel;
    public Image first, second, third;
    public Text potionName;
    public Image potionImage;

    public Button brew;
    public Button take;
    Potion pot;
    BrewingManager manager;
    MoonCycle mc;
    Animator[] anims;
    GameObject sparkles;
    GameObject bubbles;
    ResourceLoader rl;

    bool done;
    bool visible;
    bool brewVisible;

    private void Start() {
        ingredientPanel.SetActive(false);
        brewPanel.SetActive(false);
        manager = GameObject.Find("BrewingManager").GetComponent<BrewingManager>();
        mc = GameObject.Find("Clock").GetComponent<MoonCycle>();
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        sparkles = GameObject.Find("sparkles");
        bubbles = GameObject.Find("bubbles");
        anims = GetComponentsInChildren<Animator>();
        done = false;
        brewVisible = false;
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
            potionName.text = pot.name;
            potionImage.sprite = pot.image;
            potionImage.GetComponent<CanvasGroup>().alpha = 1;
            done = true;
        }
        if(visible && !brewVisible && rl.ingredientCount == 3) {
            brewPanel.SetActive(true);
            brewPanel.GetComponent<CanvasGroup>().alpha = 1;
            brewPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
            brewPanel.GetComponent<CanvasGroup>().interactable = true;
            brewVisible = true;
        }
    }

    void OnMouseUp() {
        if (!visible && manager.Brewing == 0 || manager.Brewing == 2) {
            ingredientPanel.SetActive(true);
            SwapVisible(ingredientPanel.GetComponent<CanvasGroup>());
            visible = true;
            if(manager.Brewing == 2) {
                brewPanel.SetActive(true);
                brewVisible = true;
            }
        }
    }

    public void BrewPotion() {
        Brewing b = new Brewing();
        bubbles.GetComponent<SpriteRenderer>().enabled = true;
        sparkles.GetComponent<SpriteRenderer>().enabled = true;
        foreach(Animator a in anims) {
            a.SetBool("idle", false);
        }
        pot = b.Brew(rl.brewingIngredients[0], rl.brewingIngredients[1], rl.brewingIngredients[2]);
        manager.Begin((pot.brewingTime / 10) * mc.CLOCK_SPEED, pot);
        Close();
        first.sprite = second.sprite = third.sprite = null;
        first.GetComponentInChildren<CanvasGroup>().alpha = second.GetComponentInChildren<CanvasGroup>().alpha = third.GetComponentInChildren<CanvasGroup>().alpha = 0;
        for(int i = 0; i < 3; i++) {
            rl.brewingIngredients[i] = null;
        }
        rl.ingredientCount = 0;
    }

    public void TakePotion() {
        if (GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>().inv.Add(pot, pot.name, pot.image)) {
            potionName.text = "";
            potionImage.GetComponent<CanvasGroup>().alpha = 0;

            anims[0].SetBool("full", false);
            SwapVisible(brew.GetComponent<CanvasGroup>());
            SwapVisible(take.GetComponent<CanvasGroup>());
            bubbles.GetComponent<SpriteRenderer>().enabled = false;

            manager.Brewing = 0;
            done = false;
            brewPanel.SetActive(false);
            brewVisible = false;
        }
    }

    public void Close() {
       // first.value = -1;
      //  second.value = -1;
      //  third.value = -1;
      
        SwapVisible(ingredientPanel.GetComponent<CanvasGroup>());
        if (manager.Brewing == 0) {
            brew.GetComponent<CanvasGroup>().alpha = 1;
            brew.GetComponent<CanvasGroup>().interactable = true;
            brew.GetComponent<CanvasGroup>().blocksRaycasts = true;
            take.GetComponent<CanvasGroup>().alpha = 0;
            take.GetComponent<CanvasGroup>().interactable = false;
            take.GetComponent<CanvasGroup>().blocksRaycasts = false;
            potionName.text = "";
            potionImage.GetComponent<CanvasGroup>().alpha = 0;
            SwapVisible(brewPanel.GetComponent<CanvasGroup>());
            brewVisible = false;
        }
        ingredientPanel.SetActive(false);
        brewPanel.SetActive(false);
        visible = false;
    }

    void SwapVisible(CanvasGroup cg) {
        cg.alpha = Mathf.Abs(cg.alpha - 1);
        cg.interactable = !cg.interactable;
        cg.blocksRaycasts = !cg.blocksRaycasts;
    }

    public void RemoveIngredient(int i) {
        try {
            if(rl.inv.Add(rl.brewingIngredients[i], rl.brewingIngredients[i].name, rl.brewingIngredients[i].image)) {
                rl.ingredientCount--;
                switch (i) {
                    case 0:
                        first.sprite = null;
                        first.GetComponentInChildren<CanvasGroup>().alpha = 0;
                        rl.brewingIngredients[i] = null;
                        break;
                    case 1:
                        second.sprite = null;
                        second.GetComponentInChildren<CanvasGroup>().alpha = 0;
                        rl.brewingIngredients[i] = null;
                        break;
                    case 2:
                        third.sprite = null;
                        third.GetComponentInChildren<CanvasGroup>().alpha = 0;
                        rl.brewingIngredients[i] = null;
                        break;
                    default:
                        break;
                }
            }
            brewPanel.SetActive(false);
            brewVisible = false;
        } catch (System.NullReferenceException e) {

        }

    }
}
