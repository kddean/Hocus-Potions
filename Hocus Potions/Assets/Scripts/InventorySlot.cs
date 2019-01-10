using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    Transform startingParent;
    Transform canvas;
    public Inventory.InventoryItem item;
    ResourceLoader rl;
    int index;
    Vector3 temp;
    bool set = false;
    public GameObject tooltip;
    //tooltip text
    Text[] text;
    //flags for displaying tooltips
    bool hovered = false;
    //tooltip offset
    bool dragging = false;
    bool filledStack = false;
    bool triedBrewing = false;
    Vector3 offset = new Vector3(50, 0, 0);
    Image first, second, third, firstIcon, secondIcon, thirdIcon;
    PointerEventData.InputButton clickedButton;

    [Serializable]
    public class SlotData {
        public float x, y, z;
        public int siblingIndex;
        public Inventory.InventoryItem item;

        public SlotData(float x, float y, float z, int siblingIndex, Inventory.InventoryItem item) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.siblingIndex = siblingIndex;
            this.item = item;
        }
    }

    void Start() {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
    }
    void Update() {
        if (hovered) {
            tooltip.transform.position = Input.mousePosition + offset;
        }
    }

    public void SetActive() {
        if (dragging) { return; }
        if(rl.activeItem == this) {
            rl.activeItem = null;
            gameObject.GetComponentsInChildren<Image>()[1].enabled = false;
        } else if(item != null){
            if(rl.activeItem != null) {
                rl.activeItem.gameObject.GetComponentsInChildren<Image>()[1].enabled = false;
            }
            rl.activeItem = this;
            gameObject.GetComponentsInChildren<Image>()[1].enabled = true;
        }
    }


    public void OnMouseEnter() {
         if (item != null && !dragging) {
            DisplayTooltip();
        }
    }

    void DisplayTooltip() {
        text = tooltip.GetComponentsInChildren<Text>();
        text[0].text = Regex.Replace(item.item.name, "_", " ");
        //text[1].text = item.flavorText;
        //text[2].text = item.attributes;
        tooltip.GetComponent<CanvasGroup>().alpha = 1;
        hovered = true;
    }

    public void OnMouseExit() {
        tooltip.GetComponent<CanvasGroup>().alpha = 0;
        hovered = false;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if(dragging) { return; }
        clickedButton = eventData.button;
        temp = transform.localPosition;
        startingParent = transform.parent;
        index = transform.GetSiblingIndex();
        canvas = transform.parent.parent.transform;
        transform.SetParent(canvas);
        tooltip.GetComponent<CanvasGroup>().alpha = 0;
        dragging = true;
        triedBrewing = false;
        canvas.GetComponent<Canvas>().sortingOrder = 1;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if(eventData.button != clickedButton) { return; }
        //TODO: Expand this to cover dragging all objects
        canvas.GetComponent<Canvas>().sortingOrder = 0;
        if (item == null) {
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
            dragging = false;
            return;
        }
        //Initialize images if in the house
        if (SceneManager.GetActiveScene().name.Equals("House") && GameObject.Find("Cauldron").GetComponent<Cauldron>().active) {
            first = GameObject.Find("Ingredient1").GetComponent<Image>();
            second = GameObject.Find("Ingredient2").GetComponent<Image>();
            third = GameObject.Find("Ingredient3").GetComponent<Image>();
            firstIcon = first.GetComponentsInChildren<Image>()[1];
            secondIcon = second.GetComponentsInChildren<Image>()[1];
            thirdIcon = third.GetComponentsInChildren<Image>()[1];
        }

        Button[] invButtons = startingParent.gameObject.GetComponentsInChildren<Button>();

        //Dragging within inventory panel
        if (RectTransformUtility.RectangleContainsScreenPoint(startingParent as RectTransform, Input.mousePosition)) {
            //check which slot to swap with
            foreach (Button b in invButtons) {
                if (b != gameObject.GetComponent<Button>() && RectTransformUtility.RectangleContainsScreenPoint(b.transform as RectTransform, Input.mousePosition)) {
                    InventorySlot slot = b.GetComponent<InventorySlot>();
                    //Combining stacks
                    if (slot.item != null && slot.item.item.name == item.item.name && slot.item.count < slot.item.maxStack) {
                        while (slot.item.count < slot.item.maxStack && item.count > 0) {
                            slot.item.count++;
                            item.count--;
                        }

                        slot.gameObject.GetComponentInChildren<Text>().text = slot.item.count.ToString();

                        if (item.count == 0) {
                            Inventory.RemoveStack(this);
                        } else {
                            if (item.count > 1) {
                                gameObject.GetComponentInChildren<Text>().text = item.count.ToString();
                            } else {
                                gameObject.GetComponentInChildren<Text>().text = "";
                            }
                        }

                        transform.SetParent(startingParent);
                        transform.localPosition = temp;
                        transform.SetSiblingIndex(index);
                        dragging = false;
                        return;
                    } else {
                        transform.SetParent(startingParent);
                        transform.SetSiblingIndex(index);
                        transform.localPosition = b.transform.localPosition;
                        transform.SetSiblingIndex(b.transform.GetSiblingIndex());
                        b.transform.localPosition = temp;
                        b.transform.SetSiblingIndex(index);
                        dragging = false;
                        return;
                    }

                } else {
                    transform.SetParent(startingParent);
                    transform.localPosition = temp;
                    transform.SetSiblingIndex(index);
                    dragging = false;
                }
            }
            DisplayTooltip();
        } else if (SceneManager.GetActiveScene().name.Equals("House") && GameObject.Find("Cauldron").GetComponent<Cauldron>().active && RectTransformUtility.RectangleContainsScreenPoint(first.transform as RectTransform, Input.mousePosition)) {
            SetIngredient(first, firstIcon, 0);
        } else if (SceneManager.GetActiveScene().name.Equals("House") && GameObject.Find("Cauldron").GetComponent<Cauldron>().active && RectTransformUtility.RectangleContainsScreenPoint(second.transform as RectTransform, Input.mousePosition)) {
            SetIngredient(second, secondIcon, 1);
        } else if (SceneManager.GetActiveScene().name.Equals("House") && GameObject.Find("Cauldron").GetComponent<Cauldron>().active && RectTransformUtility.RectangleContainsScreenPoint(third.transform as RectTransform, Input.mousePosition)) {
            SetIngredient(third, thirdIcon, 2);
        } else if (SceneManager.GetActiveScene().name.Equals("House") && GameObject.Find("StorageChest").GetComponent<StorageChest>().active && RectTransformUtility.RectangleContainsScreenPoint(GameObject.FindGameObjectWithTag("storage").transform as RectTransform, Input.mousePosition)) {
            GameObject.FindGameObjectWithTag("storage").GetComponent<GridLayoutGroup>().enabled = false;
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
            dragging = false;
            StorageSlot[] slots = GameObject.FindGameObjectWithTag("storage").GetComponentsInChildren<StorageSlot>();
            foreach (StorageSlot s in slots) {
                //Filling a stack
                if (s.item != null && s.item.name == item.item.name && s.count < s.maxStack) {
                    while (s.count < s.maxStack && item.count > 0) {
                        s.count++;
                        item.count--;
                    }

                    if (item.count == 0) {
                        Inventory.RemoveStack(this);
                    } else {
                        if (item.count > 1) {
                            gameObject.GetComponentInChildren<Text>().text = item.count.ToString();
                        } else {
                            gameObject.GetComponentInChildren<Text>().text = "";
                        }
                    }

                    s.gameObject.GetComponentInChildren<Text>().text = s.count.ToString();
                    s.UpdateDict();
                    return;
                }

            }

            //Adding to storage chest
            foreach (StorageSlot s in slots) {
                if (RectTransformUtility.RectangleContainsScreenPoint(s.transform as RectTransform, Input.mousePosition)) {
                    if (s.item == null) {
                        s.item = item.item;
                        s.count = item.count;
                        s.gameObject.GetComponent<Image>().enabled = true;
                        s.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(item.item.imagePath);
                        s.UpdateDict();
                        if (item.count > 1) {
                            s.gameObject.GetComponentInChildren<Text>().text = item.count.ToString();
                        } else {
                            s.gameObject.GetComponentInChildren<Text>().text = "";
                        }
                        Inventory.RemoveStack(this);
                    } else {                            //Swapping with item in chest
                        Item tempItem = item.item;
                        int tempCount = item.count;
                        item.item = s.item;
                        item.count = s.count;
                        s.item = tempItem;
                        s.count = tempCount;

                        s.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(s.item.imagePath);
                        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(item.item.imagePath);
                        if (s.count > 1) {
                            s.gameObject.GetComponentInChildren<Text>().text = s.count.ToString();
                        } else {
                            s.gameObject.GetComponentInChildren<Text>().text = "";
                        }
                        if (item.count > 1) {
                            gameObject.GetComponentInChildren<Text>().text = item.count.ToString();
                        } else {
                            gameObject.GetComponentInChildren<Text>().text = "";
                        }
                        s.UpdateDict();
                    }

                }
            }

          //if player drags it out of inventory
        } else {
            Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Vector3 mouse = Input.mousePosition;
            mouse.z = cam.transform.position.z;
            mouse = cam.ScreenToWorldPoint(mouse);
            RaycastHit2D[] hit = Physics2D.RaycastAll(new Vector2(mouse.x + 5, mouse.y), new Vector2(-1, 0), Mathf.Infinity, Physics.AllLayers, -Mathf.Infinity, Mathf.Infinity);
            foreach (RaycastHit2D ray in hit) {
                //Use potion on self
                if (item.item is Potion && ray.collider.gameObject.tag.Equals("Player") && ray.collider.bounds.Contains(new Vector2(mouse.x, mouse.y))) {
                    ray.collider.gameObject.GetComponent<Player>().UsePotion(item.item as Potion, this);
                    transform.SetParent(startingParent);
                    transform.localPosition = temp;
                    transform.SetSiblingIndex(index);
                    dragging = false;
                    return;
                }

                //Plant seed
                if (item.item is Seed && ray.collider.bounds.Contains(new Vector2(mouse.x, mouse.y)) && ray.collider.gameObject.GetComponent<GardenPlot>() != null) {
                    ray.collider.gameObject.GetComponent<GardenPlot>().PlantSeed(item.item as Seed, this);
                    transform.SetParent(startingParent);
                    transform.localPosition = temp;
                    transform.SetSiblingIndex(index);
                    dragging = false;
                    return;
                }

                //Use potion on NPC
                if (item.item is Potion && ray.collider.bounds.Contains(new Vector2(mouse.x, mouse.y)) && ray.collider.gameObject.GetComponent<NPC>() != null) {
                    ray.collider.gameObject.GetComponent<NPC>().GivePotion(this);
                    transform.SetParent(startingParent);
                    transform.localPosition = temp;
                    transform.SetSiblingIndex(index);
                    dragging = false;
                    return;
                }
            }
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
            Inventory.DropItem(this);
        }
        dragging = false;
    }

    void SetIngredient(Image slot, Image icon, int i) {
        if (item != null && item.item is Ingredient) {
            Ingredient[] ings = rl.brewingIngredients;
            //Make sure it isn't a duplicate
            foreach (Ingredient ingred in rl.brewingIngredients) {
                if (ingred != null) {
                    if (ingred.name.Equals(item.item.name)) {
                        ResetDrag();
                        triedBrewing = true;
                        return;
                    }
                } else {
                    continue;
                }
            }
            Ingredient temp = item.item as Ingredient;
            Inventory.RemoveItem(this);
            if (ings[i] != null) {
                if (Inventory.Add(ings[i], 1, false)) {
                    ings[i] = temp;
                    icon.sprite = Resources.Load<Sprite>(temp.imagePath);
                    Text[] text = slot.GetComponentsInChildren<Text>();
                    text[0].text = Regex.Replace(temp.name, "_", " ");
                } else {
                   Inventory.Add(temp, 1, false);
                }
            } else {
                ings[i] = temp;
                icon.GetComponent<Image>().enabled = true;
                icon.sprite = Resources.Load<Sprite>(temp.imagePath);
                Text[] text = slot.GetComponentsInChildren<Text>();
                text[0].text = Regex.Replace(temp.name, "_", " "); ;
                slot.GetComponentInChildren<CanvasGroup>().alpha = 1;
                rl.ingredientCount++;
            }
        }
        ResetDrag();
        triedBrewing = true;
    }

    void ResetDrag() {
        transform.SetParent(startingParent);
        transform.localPosition = temp;
        transform.SetSiblingIndex(index);
        tooltip.GetComponent<CanvasGroup>().alpha = 0;
        dragging = false;
        hovered = false;
    }
}
