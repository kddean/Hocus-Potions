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
        manager = GameObject.Find("BrewingManager").GetComponent<BrewingManager>();
        mc = GameObject.Find("Clock").GetComponent<MoonCycle>();
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        sparkles = GameObject.Find("sparkles");
        bubbles = GameObject.Find("bubbles");
        anims = GetComponentsInChildren<Animator>();
        done = false;
        brewVisible = false;

        if (rl.ingredientCount != 0) {
            try {
                first.sprite = rl.brewingIngredients[0].image;
                first.GetComponentInChildren<Text>().text = rl.brewingIngredients[0].name;
                first.GetComponentInChildren<CanvasGroup>().alpha = 1;
            } catch (System.NullReferenceException e) { Debug.Log("test1"); }

            try {
                second.sprite = rl.brewingIngredients[1].image;
                second.GetComponentInChildren<Text>().text = rl.brewingIngredients[1].name;
                second.GetComponentInChildren<CanvasGroup>().alpha = 1;
            } catch (System.NullReferenceException e) { Debug.Log("test2"); }

            try {
                third.sprite = rl.brewingIngredients[2].image;
                third.GetComponentInChildren<Text>().text = rl.brewingIngredients[2].name;
                third.GetComponentInChildren<CanvasGroup>().alpha = 1;
            } catch (System.NullReferenceException e) { Debug.Log("test3"); }
        }


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
        if(visible && !brewVisible && (rl.ingredientCount == 3 || manager.Brewing == 2)) {
            brewPanel.GetComponent<CanvasGroup>().alpha = 1;
            brewPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
            brewPanel.GetComponent<CanvasGroup>().interactable = true;
            brewVisible = true;
        }
    }

    void OnMouseUp() {
        if (!visible && manager.Brewing == 0 || manager.Brewing == 2) {
            SwapVisible(ingredientPanel.GetComponent<CanvasGroup>());
            visible = true;
            if(manager.Brewing == 2) {
                SwapVisible(brewPanel.GetComponent<CanvasGroup>());
                brewVisible = true;
            }
        }
    }

    public void BrewPotion() {
        Brewing b = new Brewing();
        pot = b.Brew(rl.brewingIngredients[0], rl.brewingIngredients[1], rl.brewingIngredients[2]);
        manager.Begin((pot.brewingTime / 10) * mc.CLOCK_SPEED, pot);

        bubbles.GetComponent<SpriteRenderer>().enabled = true;
        sparkles.GetComponent<SpriteRenderer>().enabled = true;
        foreach (Animator a in anims) {
            a.SetBool("idle", false);
        }

        first.sprite = second.sprite = third.sprite = null;
        first.GetComponentInChildren<CanvasGroup>().alpha = second.GetComponentInChildren<CanvasGroup>().alpha = third.GetComponentInChildren<CanvasGroup>().alpha = 0;
        for(int i = 0; i < 3; i++) {
            rl.brewingIngredients[i] = null;
        }
        rl.ingredientCount = 0;

        Close();
    }

    public void TakePotion() {
        if (GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>().inv.Add(pot, 1, 10)) {
            potionName.text = "";
            potionImage.GetComponent<CanvasGroup>().alpha = 0;

            anims[0].SetBool("full", false);
            SwapVisible(brew.GetComponent<CanvasGroup>());
            SwapVisible(take.GetComponent<CanvasGroup>());
            SwapVisible(brewPanel.GetComponent<CanvasGroup>());
            brewVisible = false;
            bubbles.GetComponent<SpriteRenderer>().enabled = false;

            manager.Brewing = 0;
            done = false;
        }
    }

    public void Close() {
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
        }
        if (brewVisible) {
            SwapVisible(brewPanel.GetComponent<CanvasGroup>());
            brewVisible = false;
        }
        visible = false;
    }

    void SwapVisible(CanvasGroup cg) {
        cg.alpha = Mathf.Abs(cg.alpha - 1);
        cg.interactable = !cg.interactable;
        cg.blocksRaycasts = !cg.blocksRaycasts;
    }

    public void RemoveIngredient(int i) {
        try {
            if(rl.inv.Add(rl.brewingIngredients[i], 1, 10)) {
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
            if (brewVisible) {
                SwapVisible(brewPanel.GetComponent<CanvasGroup>());
                brewVisible = false;
            }
        } catch (System.NullReferenceException e) {

        }

    }
}
