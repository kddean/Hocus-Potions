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
        RectTransform invPanel = transform.parent.transform as RectTransform;



        if (RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition)) {
            set = false;
            foreach(Button b in invButtons) {
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
            if (!set) {
                transform.SetParent(startingParent);
                transform.localPosition = temp;
                transform.SetSiblingIndex(index);
            }
        } else {
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
        }
        set = false;
        Debug.Log(transform.GetSiblingIndex());
    }

 }
