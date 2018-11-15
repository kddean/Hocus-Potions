using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public int maxStack = 10;
    public int count = 0;
    public Item item;

    ResourceLoader rl;
    GameObject invPanel;
    Vector3 temp;
    Transform startingParent;
    int index;
    bool done;
    StorageManager sm;


    private void Start() {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        invPanel = GameObject.FindGameObjectWithTag("inventory");
        sm = GameObject.Find("StorageManager").GetComponent<StorageManager>();
        StorageManager.StoreageData temp;
        if(sm.storageChest.TryGetValue(gameObject.name, out temp)){
            item = temp.item;
            count = temp.count;
            transform.SetSiblingIndex(temp.index);
            transform.localPosition = temp.position;
            if (item != null) {
                GetComponent<Image>().sprite = item.image;
                GetComponentInChildren<Text>().text = count.ToString();
                GetComponent<CanvasGroup>().alpha = 1;
            }
        } else {
            sm.storageChest.Add(gameObject.name, new StorageManager.StoreageData(item, count, transform.GetSiblingIndex(), transform.localPosition));
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        temp = transform.localPosition;
        index = transform.GetSiblingIndex();
        startingParent = transform.parent;
        transform.parent.parent.GetComponent<Canvas>().sortingOrder = 1;
        transform.SetParent(transform.parent.parent.transform);
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if(item == null) {
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
            return;
        }
        transform.parent.GetComponent<Canvas>().sortingOrder = 0;
        if (invPanel.GetComponent<CanvasGroup>().alpha != 0 && RectTransformUtility.RectangleContainsScreenPoint(invPanel.transform as RectTransform, Input.mousePosition) && item != null) {               //Dragging items into inventory
            Button[] invButtons = invPanel.GetComponentsInChildren<Button>();
            foreach (Button b in invButtons) {
                InventoryManager im = b.GetComponent<InventoryManager>();
                if (im.item != null && im.item.item == item) {                                  //filling stacks
                    while (im.item.count < im.item.maxStack && count > 0) {
                        im.item.count++;
                        count--;
                    }
                    if (count == 0) {
                        item = null;
                        GetComponent<Image>().GetComponent<CanvasGroup>().alpha = 0;
                        break;
                    }

                    im.gameObject.GetComponentInChildren<Text>().text = im.item.count.ToString();
                    gameObject.GetComponentInChildren<Text>().text = count.ToString();
                    Debug.Log("testtest");
                    done = true;
                }
            }

            if (!done) {
                Item tempItem = item;
                int tempCount = count;

                foreach (Button b in invButtons) {
                    if (RectTransformUtility.RectangleContainsScreenPoint(b.transform as RectTransform, Input.mousePosition)) {
                        InventoryManager im = b.GetComponent<InventoryManager>();
                        if (im.item == null && item != null) {
                            im.item = new Inventory.InventoryItem(item, count, maxStack);
                            im.gameObject.GetComponent<Image>().sprite = item.image;
                            im.gameObject.GetComponentInChildren<Text>().text = count.ToString();
                            item = null;
                            count = 0;
                            GetComponent<Image>().GetComponent<CanvasGroup>().alpha = 0;
                            GetComponent<Image>().sprite = null;
                        } else if (im.item != null && item != null) {
                            item = im.item.item;
                            count = im.item.count;
                            GetComponent<Image>().sprite = item.image;
                            GetComponentInChildren<Text>().text = count.ToString();
                            im.item.item = tempItem;
                            im.item.count = tempCount;
                            im.gameObject.GetComponent<Image>().sprite = tempItem.image;
                            im.gameObject.GetComponentInChildren<Text>().text = tempCount.ToString();
                        }
                    }
                }
            }

            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
        } else if (RectTransformUtility.RectangleContainsScreenPoint(startingParent as RectTransform, Input.mousePosition)) {            //swapping slots
            Image[] slots = startingParent.GetComponentsInChildren<Image>();
            for (int i = 1; i < slots.Length; i++) {
                if (slots[i] != gameObject.GetComponent<Image>() && RectTransformUtility.RectangleContainsScreenPoint(slots[i].transform as RectTransform, Input.mousePosition)) {
                    transform.SetParent(startingParent);
                    transform.localPosition = slots[i].transform.localPosition;
                    transform.SetSiblingIndex(slots[i].transform.GetSiblingIndex());
                    slots[i].transform.localPosition = temp;
                    slots[i].transform.SetSiblingIndex(index);
                    done = true;
                    sm.storageChest[slots[i].name] = new StorageManager.StoreageData(slots[i].GetComponent<StorageSlot>().item, slots[i].GetComponent<StorageSlot>().count, slots[i].transform.GetSiblingIndex(), slots[i].transform.localPosition);
                    break;
                }
            }
            if (!done) {
                transform.SetParent(startingParent);
                transform.localPosition = temp;
                transform.SetSiblingIndex(index);
            }
        } else {                                                                                                                        //reset it
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
        }
        done = false;
        sm.storageChest[gameObject.name] = new StorageManager.StoreageData(item, count, transform.GetSiblingIndex(), transform.localPosition);
    }


    public void UpdatedDict() {
        sm.storageChest[gameObject.name] = new StorageManager.StoreageData(item, count, transform.GetSiblingIndex(), transform.localPosition);
    }
}
