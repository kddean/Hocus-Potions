using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Wardrobe : MonoBehaviour {
    string[] costumes;
    bool[] unlocked;
    string current;
    CanvasGroup cg;
    BookManager bm;
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

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    void Start () {
        unlocked = new[] { true, false, false, false, false, false, false, false, false, false, false };
        cg = GameObject.FindGameObjectWithTag("wardrobePanel").GetComponent<CanvasGroup>();
        bm = GameObject.FindObjectOfType<BookManager>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        current = "Player_Default";
        open = false;
    }

    private void Update() {
        if (SceneManager.GetActiveScene().name.Equals("House")){
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<Button>().interactable = true;
        } else {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<Button>().interactable = false;
        }

        if (!unlocked[7]) {
            bool madeAllPots = true;
            foreach (bool b in bm.potionDiscovery.Values) {
                if (!b) {
                    madeAllPots = false;
                    break;
                }
            }
            if (madeAllPots) {
                unlocked[7] = true;
                LoadCostume("Costume_Cat");
            }
        }
    }

    public void Clicked() {
        if( Vector3.Distance(GameObject.FindObjectOfType<Player>().transform.position, transform.position) > 2) { return; }
        if (cg.alpha == 0) {
            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
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
                } else {
                    options[i].interactable = true;
                    options[i].gameObject.GetComponent<Image>().color = Color.white;
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
        if (!GameObject.FindObjectOfType<Cauldron>().Visible && !GameObject.FindObjectOfType<StorageChest>().active && !Resources.FindObjectsOfTypeAll<StartScreen>()[0].startScreenOpen) {
            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Exclaim Mouse"), Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnMouseExit() {
        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
    }
}
