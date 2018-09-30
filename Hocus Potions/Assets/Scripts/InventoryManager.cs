using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    Vector3 temp;
    bool set = false;
    Transform startingParent;
    Transform canvas;
    int index;
    public Inventory.inventoryItem item;
    ResourceLoader rl;
    public GameObject tooltip;
    Text[] text;
    bool hovered = false;
    Vector3 offset = new Vector3(100, -45, 0);

    void Update() {
        if (hovered) {
            tooltip.transform.position = Input.mousePosition + offset;
        }
    }

    public void OnMouseEnter() {
        text = tooltip.GetComponentsInChildren<Text>();
        text[0].text = item.name;
        //text[1].text = item.flavorText;
        //text[2].text = item.attributes;
        tooltip.GetComponent<CanvasGroup>().alpha = 1;
        tooltip.transform.position = Input.mousePosition;
        hovered = true;
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
                    if (item.count != item.maxStack && otherMgr.item != null && otherMgr.item.name.Equals(item.name)) {
                        while (item.count < item.maxStack && otherMgr.item.count != 0) {
                            otherMgr.item.count--;
                            item.count++;
                        }
                        GetComponent<Button>().GetComponentInChildren<Text>().text = item.count.ToString();
                        if (otherMgr.item.count <= 0) {
                            rl.inv.dropItem(otherMgr.item, b);
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
            rl.inv.dropItem(item, GetComponent<Button>());
        }
        set = false;
    }

 }
