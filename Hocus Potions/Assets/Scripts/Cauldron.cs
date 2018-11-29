﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cauldron : MonoBehaviour, IPointerDownHandler {
    public GameObject brewPanel;
    public GameObject ingredientPanel;
    public GameObject canvas;
    public Image first, second, third;
    public Text potionName;
    public Image potionImage;

    public Button brew;
    public Button take;

    public float speedUp;
    Potion pot;
    BrewingManager manager;
    Animator[] anims;
    GameObject sparkles;
    GameObject bubbles;
    ResourceLoader rl;
    Button[] invButtons;
    Player player;

    bool done;
    bool visible;
    bool brewVisible;
    public bool active;

    private void Start() {
        manager = GameObject.Find("BrewingManager").GetComponent<BrewingManager>();
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        sparkles = GameObject.Find("sparkles");
        bubbles = GameObject.Find("bubbles");
        anims = GetComponentsInChildren<Animator>();
        invButtons = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<Button>();
        canvas = brewPanel.transform.parent.gameObject;
        canvas.SetActive(false);
        active = false;
        
        done = false;
        brewVisible = false;

        if (rl.ingredientCount != 0) {
            try {
                first.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>(rl.brewingIngredients[0].imagePath);
                first.GetComponentsInChildren<Image>()[1].enabled = true;
                first.GetComponentInChildren<Text>().text = Regex.Replace(rl.brewingIngredients[0].name, "_", " ");
                first.GetComponentInChildren<CanvasGroup>().alpha = 1;
            } catch (System.NullReferenceException e) { }

            try {
                second.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>(rl.brewingIngredients[1].imagePath);
                second.GetComponentsInChildren<Image>()[1].enabled = true;
                second.GetComponentInChildren<Text>().text = Regex.Replace(rl.brewingIngredients[1].name, "_", " ");
                second.GetComponentInChildren<CanvasGroup>().alpha = 1;
            } catch (System.NullReferenceException e) { }

            try {
                third.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>(rl.brewingIngredients[2].imagePath);
                third.GetComponentsInChildren<Image>()[1].enabled = true;
                third.GetComponentInChildren<Text>().text = Regex.Replace(rl.brewingIngredients[2].name, "_", " ");
                third.GetComponentInChildren<CanvasGroup>().alpha = 1;
            } catch (System.NullReferenceException e) { }
        }

        switch (manager.Brewing) {
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
            foreach (Animator a in anims) {
                a.SetBool("idle", true);
            }
            GetComponent<AudioSource>().Stop();
            bubbles.GetComponent<Animator>().SetBool("full", true);
            sparkles.GetComponent<SpriteRenderer>().enabled = false;
            done = true;
        }

        if (visible && !brewVisible && (rl.ingredientCount == 3 || manager.Brewing == 2)) {
            brewPanel.GetComponent<CanvasGroup>().alpha = 1;
            brewPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
            brewPanel.GetComponent<CanvasGroup>().interactable = true;
            brewVisible = true;
        }
        if (visible && player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed)) {
            Close();
        }

        if (anims[2].GetCurrentAnimatorStateInfo(0).IsName("Ignite") && anims[2].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
            anims[2].SetBool("Ignite", false);
        }
    }


   public void OnPointerDown(PointerEventData eventData) {
        if (player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(player.transform.position, transform.position) > 2.5f) {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left) {
            player.allowedToMove = false;
            canvas.SetActive(true);
            active = true;
            if (!visible && (manager.Brewing == 0 || manager.Brewing == 2)) {
                SetVisible(ingredientPanel.GetComponent<CanvasGroup>());
                visible = true;
                if (manager.Brewing == 2) {
                    SetVisible(brewPanel.GetComponent<CanvasGroup>());
                    brewVisible = true;

                    brew.GetComponent<CanvasGroup>().alpha = 0;
                    brew.GetComponent<CanvasGroup>().interactable = false;
                    brew.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    SetVisible(take.GetComponent<CanvasGroup>());
                    pot = manager.Pot;
                    potionName.text = pot.name;
                    potionImage.sprite = Resources.Load<Sprite>(pot.imagePath);
                    potionImage.GetComponent<CanvasGroup>().alpha = 1;
                }

                foreach (Button b in invButtons) {
                    if (b.GetComponent<InventorySlot>().item != null && !(b.GetComponent<InventorySlot>().item.item is Ingredient)) {
                        Color c = b.image.color;
                        c.a = 0.25f;
                        b.image.color = c;
                    }
                }
            }
        } else {
            if(manager.Brewing == 1 && rl.activeSpell != null && rl.activeSpell.SpellName.Equals("Ignite") && GameObject.FindObjectOfType<Mana>().CurrentMana >= rl.activeSpell.Cost && !GameObject.FindObjectOfType<Mana>().InUse) {
                GetComponentsInChildren<AudioSource>()[1].Play();
                manager.BrewTime = manager.BrewTime * speedUp;
                anims[2].SetBool("Ignite", true);
                if(manager.CurrentTime >= manager.BrewTime) {
                    manager.Brewing = 2;
                }
                GameObject.FindObjectOfType<Mana>().UpdateMana(rl.activeSpell.Cost);
            }
        }
    }

    public void BrewPotion() {
        GetComponent<AudioSource>().Play();
        Brewing b = new Brewing();
        pot = b.Brew(rl.brewingIngredients[0], rl.brewingIngredients[1], rl.brewingIngredients[2]);
        manager.Begin(pot.brewingTime, pot);

        bubbles.GetComponent<SpriteRenderer>().enabled = true;
        sparkles.GetComponent<SpriteRenderer>().enabled = true;
        foreach (Animator a in anims) {
            a.SetBool("idle", false);
        }

        first.GetComponentsInChildren<Image>()[1].sprite = second.GetComponentsInChildren<Image>()[1].sprite = third.GetComponentsInChildren<Image>()[1].sprite = null;
        first.GetComponentsInChildren<Image>()[1].enabled = second.GetComponentsInChildren<Image>()[1].enabled = third.GetComponentsInChildren<Image>()[1].enabled = false;
        first.GetComponentInChildren<CanvasGroup>().alpha = second.GetComponentInChildren<CanvasGroup>().alpha = third.GetComponentInChildren<CanvasGroup>().alpha = 0;
        for (int i = 0; i < 3; i++) {
            rl.brewingIngredients[i] = null;
        }
        rl.ingredientCount = 0;

        Close();
    }

    public void TakePotion() {
        if (Inventory.Add(pot, 1)) {
            potionName.text = "";
            potionImage.GetComponent<CanvasGroup>().alpha = 0;

            anims[0].SetBool("full", false);
            SwapVisible(brew.GetComponent<CanvasGroup>());
            SwapVisible(take.GetComponent<CanvasGroup>());
            SwapVisible(brewPanel.GetComponent<CanvasGroup>());
            brewVisible = false;
            bubbles.GetComponent<SpriteRenderer>().enabled = false;

            manager.Brewing = 0;
            manager.Pot = null;
            done = false;
        }
    }

    public void Close() {
        SwapVisible(ingredientPanel.GetComponent<CanvasGroup>());
        if (manager.Brewing == 0) {
            SetVisible(brew.GetComponent<CanvasGroup>());
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
        foreach (Button b in invButtons) {
            Color c = b.image.color;
            c.a = 1f;
            b.image.color = c;
        }
        visible = false;
        canvas.SetActive(false);
        active = false;
        player.allowedToMove = true;
    }

    void SwapVisible(CanvasGroup cg) {
        cg.alpha = Mathf.Abs(cg.alpha - 1);
        cg.interactable = !cg.interactable;
        cg.blocksRaycasts = !cg.blocksRaycasts;
    }

    void SetVisible(CanvasGroup cg) {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void RemoveIngredient(int i) {
        if (rl.brewingIngredients[i] != null && Inventory.Add(rl.brewingIngredients[i], 1)) {
            rl.ingredientCount--;
            switch (i) {
                case 0:
                    first.GetComponentsInChildren<Image>()[1].sprite = null;
                    first.GetComponentsInChildren<Image>()[1].enabled = false;
                    first.GetComponentInChildren<CanvasGroup>().alpha = 0;
                    rl.brewingIngredients[i] = null;
                    break;
                case 1:
                    second.GetComponentsInChildren<Image>()[1].sprite = null;
                    second.GetComponentsInChildren<Image>()[1].enabled = false;
                    second.GetComponentInChildren<CanvasGroup>().alpha = 0;
                    rl.brewingIngredients[i] = null;
                    break;
                case 2:
                    third.GetComponentsInChildren<Image>()[1].sprite = null;
                    third.GetComponentsInChildren<Image>()[1].enabled = false;
                    third.GetComponentInChildren<CanvasGroup>().alpha = 0;
                    rl.brewingIngredients[i] = null;
                    break;
                default:
                    break;
            }

            if (brewVisible) {
                SwapVisible(brewPanel.GetComponent<CanvasGroup>());
                brewVisible = false;
            }
        }
    }
}
