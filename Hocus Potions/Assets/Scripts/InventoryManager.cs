using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    Vector3 offset = new Vector3(50, 0, 0);

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
         if (item != null) {
            text = tooltip.GetComponentsInChildren<Text>();
            text[0].text = item.name;
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
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        Button[] invButtons = transform.parent.gameObject.GetComponentsInChildren<Button>();
        RectTransform invPanel = GameObject.FindGameObjectWithTag("inventory").transform as RectTransform;
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        if (RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition)) {
            set = false;
            //check which slot to swap with
            foreach (Button b in invButtons) {
                if (RectTransformUtility.RectangleContainsScreenPoint(b.transform as RectTransform, Input.mousePosition)) {
                    transform.SetParent(startingParent);
                    InventoryManager otherMgr = b.GetComponent<InventoryManager>();
                    transform.localPosition = b.transform.localPosition;
                    transform.SetSiblingIndex(b.transform.GetSiblingIndex());
                    b.transform.localPosition = temp;
                    b.transform.SetSiblingIndex(index);

                    //check if combining stacks
                    if (item != null && item.count != item.maxStack && otherMgr.item != null && otherMgr.item.name.Equals(item.name)) {
                        while (item.count < item.maxStack && otherMgr.item.count != 0) {
                            otherMgr.item.count--;
                            item.count++;
                        }
                        GetComponent<Button>().GetComponentInChildren<Text>().text = item.count.ToString();
                        if (otherMgr.item.count <= 0) {
                            rl.inv.DropItem(otherMgr.item, b);
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
        } else {
            //if player drags it out of inventory
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
            rl.inv.DropItem(item, GetComponent<Button>());
        }
        set = false;
        if (item != null) {
            hovered = true;
            tooltip.GetComponent<CanvasGroup>().alpha = 1;
        }
        dragging = false;
    }

 }
