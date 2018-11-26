using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IPointerDownHandler {

    public enum PlayerStatus { poisoned, fast, invisible, transformed, asleep }
    float speed;
    public float defaultSpeed, poisonedSpeed, fastSpeed;
    int x, y;
    ResourceLoader rl;
    Animator anim;
    Potion lastTaken;
    List<PlayerStatus> status;
    SpriteRenderer sr;
    public GameObject fadeScreen, sleepCanvas;
    public bool swappingScenes;
    public bool allowedToMove;

    public List<PlayerStatus> Status {
        get {
            return status;
        }

        set {
            status = value;
        }
    }

    public float Speed {
        get {
            return speed;
        }

        set {
            speed = value;
        }
    }

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    public void Start() {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        anim = GetComponentInChildren<Animator>();
        Status = new List<PlayerStatus>();
        Speed = defaultSpeed;
        sr = GetComponent<SpriteRenderer>();
        swappingScenes = false;
        allowedToMove = true;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (Status.Contains(PlayerStatus.asleep) || rl.activeItem == null) {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right) {
            if (rl.activeItem.item.item is Potion && (rl.activeItem.item.item != lastTaken)) {
                lastTaken = rl.activeItem.item.item as Potion;
                StartCoroutine(HandlePotions(rl.activeItem.item.item as Potion));
                Inventory.RemoveItem(rl.activeItem);
            }
        }
    }

    private void FixedUpdate() {
        if (!allowedToMove) { return; }
        Vector3 pos = transform.position;
        x = y = 0;
        if (Input.GetKey("w")) {
            y = 1;
        } else if (Input.GetKey("s")) {
            y = -1;
        } else if (Input.GetKey("a")) {
            x = -1;
        } else if (Input.GetKey("d")) {
            x = 1;
        }
        pos.x += x * Speed * Time.deltaTime;
        pos.y += y * Speed * Time.deltaTime;
        transform.position = pos;
    }



    private void OnTriggerEnter2D(Collider2D collision) {
        if (!swappingScenes && GetComponent<BoxCollider2D>().IsTouching(collision)) {
            switch (collision.gameObject.name) {
                case "ToWorld":
                    GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToWorld");
                    swappingScenes = true;
                    break;
                case "ToHouse":
                    GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToHouse");
                    swappingScenes = true;
                    break;
                case "ToGarden":
                    GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToGarden");
                    swappingScenes = true;
                    break;
                default:
                    break;
            }
        }
    }


    IEnumerator HandlePotions(Potion pot) {
        foreach (AnimatorControllerParameter p in anim.parameters) {
            anim.SetBool(p.name, false);
        }

        switch (pot.Primary) {
            case Ingredient.Attributes.healing:
                anim.SetBool("Healing", true);
                if (Status.Contains(PlayerStatus.poisoned)) {
                    Status.Remove(PlayerStatus.poisoned);
                    Speed = defaultSpeed;
                }
                //TODO:Maybe add more use for this potion
                break;
            case Ingredient.Attributes.invisibility:
                anim.SetBool("Invisible", true);
                anim.Play("Invisible", 0, 0);
                Status.Add(PlayerStatus.invisible);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.5f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case Ingredient.Attributes.mana:
                anim.SetBool("Mana", true);            
                GameObject.FindObjectOfType<Mana>().UpdateMana(-50);
                break;
            case Ingredient.Attributes.poison:
                anim.SetBool("Poison", true);
                Status.Add(PlayerStatus.poisoned);
                if (Status.Contains(PlayerStatus.fast)) {
                    Speed = defaultSpeed;
                } else {
                    Speed = poisonedSpeed;
                }
                break;
            case Ingredient.Attributes.sleep:
                anim.SetBool("Sleep", true);
                Status.Add(PlayerStatus.asleep);          
                GameObject.FindObjectOfType<Mana>().UpdateMana(-(pot.Duration / 60) * 10);
                Speed = 0;
                sleepCanvas.SetActive(true);
                StartCoroutine(FadeScreen(1));
                yield return new WaitForSeconds(2);
                Time.timeScale = Time.timeScale / (0.1f / GameObject.Find("Clock").GetComponent<MoonCycle>().CLOCK_SPEED);
                break;
            case Ingredient.Attributes.speed:
                anim.SetBool("Speed", true);
                Status.Add(PlayerStatus.fast);
                if (Status.Contains(PlayerStatus.poisoned)) {
                    Speed = defaultSpeed;
                } else {
                    Speed = fastSpeed;
                }
                break;
            case Ingredient.Attributes.transformation:
                Status.Add(PlayerStatus.transformed);
                anim.SetBool("Transformation", true);
                anim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Characters/cat");
                GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.2f);
                GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.2f);
                GetComponents<BoxCollider2D>()[1].size = new Vector2(0.7f, 0.6f);
                GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, 0);
                Speed = defaultSpeed + 1;
                break;
            case Ingredient.Attributes.none:
                break;
            default:
                break;
        }

        yield return new WaitForSeconds((pot.Duration / 10) * GameObject.Find("Clock").GetComponent<MoonCycle>().CLOCK_SPEED);
        foreach (AnimatorControllerParameter p in anim.parameters) {
            anim.SetBool(p.name, false);
        }

        switch (pot.Primary) {
            case Ingredient.Attributes.healing:
                anim.SetBool("Healing", false);
                break;
            case Ingredient.Attributes.invisibility:
                anim.SetBool("Invisible", true);
                anim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.833f);
                anim.SetBool("Invisible", false);
                Status.Remove(PlayerStatus.invisible);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 1f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case Ingredient.Attributes.mana:
                anim.SetBool("Mana", false);
                break;
            case Ingredient.Attributes.poison:
                anim.SetBool("Poison", false);
                Status.Remove(PlayerStatus.poisoned);
                if (Status.Contains(PlayerStatus.fast)) {
                    Speed = fastSpeed;
                } else {
                    Speed = defaultSpeed;
                }
                break;
            case Ingredient.Attributes.sleep:
                anim.SetBool("Sleep", false);
                Status.Remove(PlayerStatus.asleep);
                Speed = defaultSpeed;
                Time.timeScale = Time.timeScale * (0.1f / GameObject.Find("Clock").GetComponent<MoonCycle>().CLOCK_SPEED);
                StartCoroutine(FadeScreen(-1));
                sleepCanvas.GetComponentsInChildren<CanvasGroup>()[0].blocksRaycasts = false;
                yield return new WaitForSeconds(2);
                sleepCanvas.SetActive(false);
                break;
            case Ingredient.Attributes.speed:
                anim.SetBool("Speed", false);
                Status.Remove(PlayerStatus.fast);
                if (Status.Contains(PlayerStatus.poisoned)) {
                    Speed = poisonedSpeed;
                } else {
                    Speed = defaultSpeed;
                }
                break;
            case Ingredient.Attributes.transformation:
                anim.SetBool("Transformation", true);
                anim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Characters/witch");
                GetComponent<BoxCollider2D>().size = new Vector2(1, 0.5f);
                GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.8f);
                GetComponents<BoxCollider2D>()[1].size = new Vector2(1, 2.1f);
                GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, 0);
                yield return new WaitForSeconds(0.43f);
                anim.SetBool("Transformation", false);
                Status.Remove(PlayerStatus.transformed);
                Speed = defaultSpeed;
                if (Status.Contains(PlayerStatus.fast) && Status.Contains(PlayerStatus.poisoned)) {
                    Speed = defaultSpeed;
                } else if (Status.Contains(PlayerStatus.fast)) {
                    Speed = fastSpeed;
                } else if (Status.Contains(PlayerStatus.poisoned)) {
                    Speed = poisonedSpeed;
                } else {
                    Speed = defaultSpeed;
                }
                break;
            case Ingredient.Attributes.none:
                break;
            default:
                break;
        }

        if (Status.Contains(PlayerStatus.fast)) {
            anim.SetBool("Speed", true);
        }
        if (Status.Contains(PlayerStatus.poisoned)) {
            anim.SetBool("Poison", true);
        }

        lastTaken = null;
    }


    IEnumerator FadeScreen(int i) {
        CanvasGroup cg = fadeScreen.GetComponent<CanvasGroup>();
        cg.alpha += i * (Time.deltaTime / 2);
        while (cg.alpha > 0 && cg.alpha < 1) {
            cg.alpha += i * (Time.deltaTime / 2);
            yield return null;
        }
    }
}
