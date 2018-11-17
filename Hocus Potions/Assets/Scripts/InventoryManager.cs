using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

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
        } else if(item != null){
            rl.activeItem = this;
        }
    }


    public void OnMouseEnter() {
         if (item != null && !dragging) {
            text = tooltip.GetComponentsInChildren<Text>();
            text[0].text = item.item.name;
            //text[1].text = item.flavorText;
            //text[2].text = item.attributes;
            tooltip.GetComponent<CanvasGroup>().alpha = 1;
            hovered = true;
        }
    }


    public void OnMouseExit() {
        tooltip.GetComponent<CanvasGroup>().alpha = 0;
        hovered = false;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        temp = transform.localPosition;
        startingParent = transform.parent;
        index = transform.GetSiblingIndex();
        canvas = transform.parent.parent.transform;
        transform.SetParent(canvas);
        hovered = false;
        tooltip.GetComponent<CanvasGroup>().alpha = 0;
        dragging = true;
        triedBrewing = false;
        canvas.GetComponent<Canvas>().sortingOrder = 1;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvas.GetComponent<Canvas>().sortingOrder = 0;
        if(item == null) {
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
            dragging = false;
            return;
        }
        if (SceneManager.GetActiveScene().name.Equals("House") && GameObject.Find("Cauldron").GetComponent<Cauldron>().active) {
            first = GameObject.Find("Ingredient1").GetComponent<Image>();
            second = GameObject.Find("Ingredient2").GetComponent<Image>();
            third = GameObject.Find("Ingredient3").GetComponent<Image>();
            firstIcon = first.GetComponentsInChildren<Image>()[1];
            secondIcon = second.GetComponentsInChildren<Image>()[1];
            thirdIcon = third.GetComponentsInChildren<Image>()[1];
        }

        Button[] invButtons = transform.parent.gameObject.GetComponentsInChildren<Button>();
        RectTransform invPanel = GameObject.FindGameObjectWithTag("inventory").transform as RectTransform;
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        if (RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition)) {
            set = false;
            //check which slot to swap with
            foreach (Button b in invButtons) {
                if (b != this.gameObject.GetComponent<Button>() && RectTransformUtility.RectangleContainsScreenPoint(b.transform as RectTransform, Input.mousePosition)) {
                    transform.SetParent(startingParent);
                    InventoryManager otherMgr = b.GetComponent<InventoryManager>();
                    transform.localPosition = b.transform.localPosition;
                    transform.SetSiblingIndex(b.transform.GetSiblingIndex());
                    b.transform.localPosition = temp;
                    b.transform.SetSiblingIndex(index);

                    //check if combining stacks
                    if (item != null && item.count != item.maxStack && otherMgr.item != null && otherMgr.item.item.name.Equals(item.item.name)) {
                        while (item.count < item.maxStack && otherMgr.item.count != 0) {
                            otherMgr.item.count--;
                            item.count++;
                        }
                        GetComponent<Button>().GetComponentInChildren<Text>().text = item.count.ToString();
                        if (otherMgr.item.count <= 0) {
                            rl.inv.RemoveStack(otherMgr.item, b);
                        } else {
                            b.GetComponentInChildren<Text>().text = otherMgr.item.count.ToString();
                        }
                    }

                    set = true;
                    break;
                }
            }
            //if it's in it's original spot just snap it back
            if (!set) {
                transform.SetParent(startingParent);
                transform.localPosition = temp;
                transform.SetSiblingIndex(index);
            }
        } else if (SceneManager.GetActiveScene().name.Equals("House") && GameObject.Find("Cauldron").GetComponent<Cauldron>().active && RectTransformUtility.RectangleContainsScreenPoint(first.transform as RectTransform, Input.mousePosition)) {
            SetIngredient(first, firstIcon, 0);
        } else if (SceneManager.GetActiveScene().name.Equals("House") && GameObject.Find("Cauldron").GetComponent<Cauldron>().active && RectTransformUtility.RectangleContainsScreenPoint(second.transform as RectTransform, Input.mousePosition)) {
            SetIngredient(second, secondIcon, 1);
        } else if (SceneManager.GetActiveScene().name.Equals("House") && GameObject.Find("Cauldron").GetComponent<Cauldron>().active && RectTransformUtility.RectangleContainsScreenPoint(third.transform as RectTransform, Input.mousePosition)) {
            SetIngredient(third, thirdIcon, 2);
        } else if (SceneManager.GetActiveScene().name.Equals("House") && GameObject.Find("StorageChest").GetComponent<StorageChest>().active && RectTransformUtility.RectangleContainsScreenPoint(GameObject.FindGameObjectWithTag("storage").transform as RectTransform, Input.mousePosition)) {
            GameObject storagePanel = GameObject.FindGameObjectWithTag("storage");
            StorageSlot[] slots = storagePanel.GetComponentsInChildren<StorageSlot>();
            foreach (StorageSlot s in slots) {
                if (s.item != null && s.item == item.item && s.count != s.maxStack) {
                    while (item.count > 0 && s.count < s.maxStack) {
                        item.count--;
                        s.count++;
                    }

                    if (item.count == 0) {
                        rl.inv.RemoveStack(item, gameObject.GetComponent<Button>());
                        
                    } else {
                        gameObject.GetComponent<Image>().sprite = item.item.image;
                        gameObject.GetComponentInChildren<Text>().text = item.count.ToString();
                    }
                    s.gameObject.GetComponentInChildren<Text>().text = s.count.ToString();
                    s.gameObject.GetComponent<Image>().sprite = s.item.image;
                    transform.SetParent(startingParent);
                    transform.localPosition = temp;
                    transform.SetSiblingIndex(index);
                    filledStack = true;
                    s.UpdatedDict();
                }
            }

            if (!filledStack) {
                foreach (StorageSlot s in slots) {
                    if (RectTransformUtility.RectangleContainsScreenPoint(s.gameObject.transform as RectTransform, Input.mousePosition)) {
                        if (s.item != null) {
                            Item tempItem = s.item;
                            int tempCount = s.count;
                            s.item = item.item;
                            s.count = item.count;
                            s.gameObject.GetComponent<Image>().sprite = s.item.image;
                            s.gameObject.GetComponentInChildren<Text>().text = s.count.ToString();
                            item.item = tempItem;
                            item.count = tempCount;
                            gameObject.GetComponent<Image>().sprite = item.item.image;
                            gameObject.GetComponentInChildren<Text>().text = item.count.ToString();
                            transform.SetParent(startingParent);
                            transform.localPosition = temp;
                            transform.SetSiblingIndex(index);
                            s.UpdatedDict();
                        } else {
                            s.item = item.item;
                            s.count = item.count;
                            s.gameObject.GetComponent<Image>().sprite = s.item.image;
                            s.gameObject.GetComponentInChildren<Text>().text = s.count.ToString();
                            s.gameObject.GetComponent<CanvasGroup>().alpha = 1;
                            rl.inv.RemoveStack(item, gameObject.GetComponent<Button>());
                            s.UpdatedDict();
                        }
                    }
                }
                transform.SetParent(startingParent);
                transform.localPosition = temp;
                transform.SetSiblingIndex(index);
            }
            filledStack = false;
        } else {

            //if player drags it out of inventory
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
            rl.inv.DropItem(item, GetComponent<Button>());
        }
        set = false;
        if (item != null && !triedBrewing) {
            hovered = true;
            tooltip.GetComponent<CanvasGroup>().alpha = 1;
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
            rl.inv.RemoveItem(item);
            if (ings[i] != null) {
                if (rl.inv.Add(ings[i], 1, 10)) {
                    ings[i] = temp;
                    icon.sprite = temp.image;
                    Text[] text = slot.GetComponentsInChildren<Text>();
                    text[0].text = temp.name;
                } else {
                    rl.inv.Add(temp, 1, item.maxStack);
                }
            } else {
                ings[i] = temp;
                icon.GetComponent<Image>().enabled = true;
                icon.sprite = temp.image;
                Text[] text = slot.GetComponentsInChildren<Text>();
                text[0].text = temp.name;
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
