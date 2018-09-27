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
                    transform.localPosition = b.transform.localPosition;
                    transform.SetSiblingIndex(b.transform.GetSiblingIndex());
                    b.transform.localPosition = temp;
                    b.transform.SetSiblingIndex(index);
                    set = true;
                    break;                  
                }
            }
            //if it's in it's original spot just snap it back
            if (!set) {
                Debug.Log("?");
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
