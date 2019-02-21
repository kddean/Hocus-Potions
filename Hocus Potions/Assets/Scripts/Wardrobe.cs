using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wardrobe : MonoBehaviour {
    string[] costumes;
    bool[] unlocked;
    string current;
    CanvasGroup cg;
    public bool open;

    public string Current {
        get {
            return current;
        }

        set {
            current = value;
        }
    }

    public bool[] Unlocked {
        get {
            return unlocked;
        }

        set {
            unlocked = value;
        }
    }

    void Start () {
        unlocked = new[] { true, true, false, false, true, false, false, true, false, false, true };
        cg = GameObject.FindGameObjectWithTag("wardrobePanel").GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        current = "Player_Default";
        open = false;
    }

    public void Clicked() {
        if (cg.alpha == 0) {
            GameObject.FindObjectOfType<Player>().allowedToMove = false;
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
            open = true;

            Button[] options = cg.gameObject.GetComponentsInChildren<Button>();
            for(int i = 0; i < options.Length - 1; i++){
                if (!unlocked[i]) {
                    options[i].interactable = false;
                    options[i].gameObject.GetComponent<Image>().color = Color.black;
                }
            }
        }
    }

    public void Close() {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        GameObject.FindObjectOfType<Player>().allowedToMove = true;
        open = false;
    }

    public void LoadCostume(string c) {
        current = c;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Characters/" + c);
    }

    private void OnMouseEnter() {
        if (!GameObject.FindObjectOfType<Cauldron>().Visible && !GameObject.FindObjectOfType<StorageChest>().active) {
            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Exclaim Mouse"), Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnMouseExit() {
        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
    }
}
