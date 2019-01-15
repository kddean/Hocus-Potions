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
    bool dragging;
    PointerEventData.InputButton clickedButton;
    StorageManager sm;


    private void Awake() {
        sm = GameObject.Find("StorageManager").GetComponent<StorageManager>();
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        invPanel = GameObject.FindGameObjectWithTag("inventory");

        StorageManager.StoreageData temp;
        item = null;
        if (!sm.storageChest.TryGetValue(gameObject.name, out temp)) {
            sm.storageChest.Add(gameObject.name, new StorageManager.StoreageData(null, 0, transform.GetSiblingIndex(), transform.localPosition.x, transform.localPosition.y, transform.localPosition.z));
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (dragging) { return; }
        clickedButton = eventData.button;
        dragging = true;
        GameObject.FindGameObjectWithTag("storage").GetComponent<GridLayoutGroup>().enabled = false;
        temp = transform.localPosition;
        index = transform.GetSiblingIndex();
        startingParent = transform.parent;
        transform.parent.parent.GetComponent<Canvas>().sortingOrder = 1;
        transform.SetParent(transform.parent.parent.transform);
    }

    public void OnDrag(PointerEventData eventData) {
        if (dragging) {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (eventData.button != clickedButton) { return; }
        transform.parent.GetComponent<Canvas>().sortingOrder = 0;
        if (item == null) {
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
            dragging = false;
            return;
        }
        bool filledStack = false;
        if (invPanel.GetComponent<CanvasGroup>().alpha != 0 && RectTransformUtility.RectangleContainsScreenPoint(invPanel.transform as RectTransform, Input.mousePosition)) { //Putting items into inventory
            Button[] invButtons = invPanel.GetComponentsInChildren<Button>();
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
            dragging = false;
            foreach (Button b in invButtons) {
                InventorySlot im = b.GetComponent<InventorySlot>();

                //Filling a stack
                if(im.item != null && im.item.item.name == item.name) {
                    while(im.item.count < im.item.maxStack && count > 0) {
                        im.item.count++;
                        count--;
                    }
                    
                    //Update chest slot 
                    if(count == 0) {
                        gameObject.GetComponent<Image>().enabled = false;
                        gameObject.GetComponentInChildren<Text>().text = "";
                        item = null;                     
                    } else {
                        if (count != 1) {
                            gameObject.GetComponentInChildren<Text>().text = count.ToString();
                        } else {
                            gameObject.GetComponentInChildren<Text>().text = "";
                        }
                    }

                    //Update inventory slot
                    im.gameObject.GetComponentInChildren<Text>().text = im.item.count.ToString();

                    sm.storageChest[gameObject.name] = new StorageManager.StoreageData(item, count, transform.GetSiblingIndex(), transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                    filledStack = true;
                    break;
                }
            }

            //Adding to inventory or swapping with an existing item
            if (!filledStack) {
                foreach(Button b in invButtons) {
                    if(RectTransformUtility.RectangleContainsScreenPoint(b.transform as RectTransform, Input.mousePosition)) {
                        InventorySlot im = b.GetComponent<InventorySlot>();
                        if (im.item == null) {
                            im.item = new Inventory.InventoryItem(item, count);
                            im.gameObject.GetComponent<Image>().enabled = true;
                            im.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(item.imagePath);
                            if (count > 1) {
                                im.gameObject.GetComponentInChildren<Text>().text = count.ToString();
                            } else {
                                im.gameObject.GetComponentInChildren<Text>().text = "";
                            }
                            item = null;
                            count = 0;
                            gameObject.GetComponent<Image>().enabled = false;
                            gameObject.GetComponentInChildren<Text>().text = "";

                            sm.storageChest[gameObject.name] = new StorageManager.StoreageData(item, count, transform.GetSiblingIndex(), transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                        } else {
                            Item tempItem = item;
                            int tempCount = count;
                            item = im.item.item;
                            count = im.item.count;
                            im.item.item = tempItem;
                            im.item.count = tempCount;
                            im.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(tempItem.imagePath);
                            if (tempCount > 1) {
                                im.gameObject.GetComponentInChildren<Text>().text = tempCount.ToString();
                            } else {
                                im.gameObject.GetComponentInChildren<Text>().text = "";
                            }

                            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(item.imagePath);
                            if (count > 1) {
                                gameObject.GetComponentInChildren<Text>().text = count.ToString();
                            } else {
                                gameObject.GetComponentInChildren<Text>().text = "";
                            }
                            sm.storageChest[gameObject.name] = new StorageManager.StoreageData(item, count, transform.GetSiblingIndex(), transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                        }
                        break;
                    }
                }

            }


        } else if(RectTransformUtility.RectangleContainsScreenPoint(startingParent as RectTransform, Input.mousePosition)) {            //Rearranging items in chest
            StorageSlot[] slots = startingParent.GetComponentsInChildren<StorageSlot>();
            foreach(StorageSlot s in slots) {
                if(s != this && RectTransformUtility.RectangleContainsScreenPoint(s.gameObject.transform as RectTransform, Input.mousePosition)) {
                    transform.SetParent(startingParent);
                    transform.SetSiblingIndex(index);
                    transform.localPosition = s.transform.localPosition;
                    transform.SetSiblingIndex(s.transform.GetSiblingIndex());
                    s.transform.localPosition = temp;
                    s.transform.SetSiblingIndex(index);
                    dragging = false;
                    sm.storageChest[gameObject.name] = new StorageManager.StoreageData(item, count, transform.GetSiblingIndex(), transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                    sm.storageChest[s.name] = new StorageManager.StoreageData(s.GetComponent<StorageSlot>().item, s.GetComponent<StorageSlot>().count, s.transform.GetSiblingIndex(), s.transform.localPosition.x, s.transform.localPosition.y, s.transform.localPosition.z);
                    return;
                }
            }
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
            dragging = false;
        } else {                                        //dragging into empty space
            transform.SetParent(startingParent);
            transform.localPosition = temp;
            transform.SetSiblingIndex(index);
            dragging = false;
        }
        dragging = false;
    }

    public void UpdateDict() {
        sm.storageChest[gameObject.name] = new StorageManager.StoreageData(item, count, transform.GetSiblingIndex(), transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }
}
