﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageChest : MonoBehaviour, IPointerDownHandler {
    GameObject canvas;

    private void Start() {
        canvas = GameObject.FindGameObjectWithTag("storage").transform.parent.gameObject;
        canvas.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData) {
        canvas.SetActive(true);
        GameObject panel = GameObject.FindGameObjectWithTag("storage");
        CanvasGroup[] cg = panel.GetComponentsInChildren<CanvasGroup>();
        foreach(CanvasGroup c in cg) {
            c.GetComponent<CanvasGroup>().interactable = true;
            c.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        cg[0].alpha = 1.0f;

        StorageSlot[] slots = panel.GetComponentsInChildren<StorageSlot>();
        foreach(StorageSlot s in slots) { 
            if(s.item != null) {
               s.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
            }
        }

    }

    public void Close() {
        GameObject panel = GameObject.FindGameObjectWithTag("storage");
        panel.GetComponent<CanvasGroup>().alpha = 0;
        panel.GetComponent<CanvasGroup>().interactable = false;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        canvas.SetActive(false);
    }
}