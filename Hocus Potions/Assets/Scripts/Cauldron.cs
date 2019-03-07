using System.Collections;
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
    GameObject inv;
    ResourceLoader rl;
    Button[] invButtons;
    Player player;

    bool done;
    bool alreadyOpen;
    bool visible;
    bool brewVisible;
    public bool active;

    public bool Visible {
        get {
            return visible;
        }
    }

    private void Start() {
        manager = GameObject.Find("BrewingManager").GetComponent<BrewingManager>();
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        sparkles = GameObject.Find("sparkles");
        bubbles = GameObject.Find("bubbles");
        inv = GameObject.FindGameObjectWithTag("inventory");
        anims = GetComponentsInChildren<Animator>();
        invButtons = GameObject.FindGameObjectWithTag("inventory").GetComponentsInChildren<Button>();
        canvas = brewPanel.transform.parent.gameObject;
        canvas.SetActive(false);
        active = false;
        alreadyOpen = true;
        done = false;
        brewVisible = false;
 
        if (rl.ingredientCount != 0) {
            try {
                first.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>(rl.brewingIngredients[0].imagePath);
                first.GetComponentsInChildren<Image>()[1].enabled = true;
                first.GetComponentInChildren<Text>().text = Regex.Replace(rl.brewingIngredients[0].name.Substring(0, 1).ToUpper() + rl.brewingIngredients[0].name.Substring(1), "_", " ");
                first.GetComponentInChildren<CanvasGroup>().alpha = 1;
            } catch (System.NullReferenceException) { }

            try {
                second.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>(rl.brewingIngredients[1].imagePath);
                second.GetComponentsInChildren<Image>()[1].enabled = true;
                second.GetComponentInChildren<Text>().text = Regex.Replace(rl.brewingIngredients[1].name.Substring(0, 1).ToUpper() + rl.brewingIngredients[1].name.Substring(1), "_", " ");
                second.GetComponentInChildren<CanvasGroup>().alpha = 1;
            } catch (System.NullReferenceException) { }

            try {
                third.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>(rl.brewingIngredients[2].imagePath);
                third.GetComponentsInChildren<Image>()[1].enabled = true;
                third.GetComponentInChildren<Text>().text = Regex.Replace(rl.brewingIngredients[2].name.Substring(0, 1).ToUpper() + rl.brewingIngredients[2].name.Substring(1), "_", " ");
                third.GetComponentInChildren<CanvasGroup>().alpha = 1;
            } catch (System.NullReferenceException) { }
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
        if (visible && (player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed))) {
            Close();
        }

        if (anims[2].GetCurrentAnimatorStateInfo(0).IsName("Ignite") && anims[2].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
            anims[2].SetBool("Ignite", false);
        }
    }

    public void OnMouseEnter() {
        if (!visible && !GameObject.FindObjectOfType<StorageChest>().active && !GameObject.FindObjectOfType<Wardrobe>().open) {
            switch (manager.Brewing) {
                case 0:
                    Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Exclaim Mouse"), Vector2.zero, CursorMode.Auto);
                    break;
                case 1:
                    if (rl.activeSpell != null && rl.activeSpell.SpellName.Equals("Ignite")) {
                        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Fire Mouse"), Vector2.zero, CursorMode.Auto);
                    } else {
                        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Wait Mouse"), Vector2.zero, CursorMode.Auto);
                    }
                    break;
                case 2:
                    Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Collect Mouse"), Vector2.zero, CursorMode.Auto);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnMouseExit() {
        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
    }
    public void OnPointerDown(PointerEventData eventData) {
        if (player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(player.transform.position, transform.position) > 2.5f) {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left) {
            player.allowedToMove = false;
            canvas.SetActive(true);
            active = true;
            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
            if (!visible && (manager.Brewing == 0 || manager.Brewing == 2)) {
                SetVisible(ingredientPanel.GetComponent<CanvasGroup>());
                if (inv.GetComponent<CanvasGroup>().alpha == 0) {
                    inv.GetComponent<CanvasGroup>().alpha = 1;
                    inv.GetComponent<CanvasGroup>().interactable = true;
                    inv.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    alreadyOpen = false;
                } else {
                    alreadyOpen = true;
                }
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
        if (Inventory.Add(pot, 1, false)) {
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

        if (!alreadyOpen) {
            inv.GetComponent<CanvasGroup>().alpha = 0;
            inv.GetComponent<CanvasGroup>().interactable = false;
            inv.GetComponent<CanvasGroup>().blocksRaycasts = false;
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
        if (rl.brewingIngredients[i] != null && Inventory.Add(rl.brewingIngredients[i], 1, false)) {
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


    public void UpdateText() {
        if (rl.brewingIngredients[0] != null) {
            if (rl.knownAttributes[rl.brewingIngredients[0]].Count > 0) {
                first.GetComponentsInChildren<Text>()[1].text = "";
                foreach (Ingredient.Attributes att in rl.knownAttributes[rl.brewingIngredients[0]]) {
                    first.GetComponentsInChildren<Text>()[1].text += "- " + att.ToString() + "\n";
                }
            }
        }
        if (rl.brewingIngredients[1] != null) {
            if (rl.knownAttributes[rl.brewingIngredients[1]].Count > 0) {
                second.GetComponentsInChildren<Text>()[1].text = "";
                foreach (Ingredient.Attributes att in rl.knownAttributes[rl.brewingIngredients[1]]) {
                    second.GetComponentsInChildren<Text>()[1].text += "- " + att.ToString() + "\n";
                }
            }
        }
        if (rl.brewingIngredients[2] != null) {
            if (rl.knownAttributes[rl.brewingIngredients[2]].Count > 0) {
                third.GetComponentsInChildren<Text>()[1].text = "";
                foreach (Ingredient.Attributes att in rl.knownAttributes[rl.brewingIngredients[2]]) {
                    third.GetComponentsInChildren<Text>()[1].text += "- " + att.ToString() + "\n";
                }
            }
        }
    }
}
