using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IPointerDownHandler {

    public enum PlayerStatus { poisoned, fast, invisible, transformed, asleep, none }
    float speed;
    public float defaultSpeed, poisonedSpeed, fastSpeed;
    int x, y;
    ResourceLoader rl;
    Animator anim;
    Potion lastTaken;
    PlayerStatus status;
    SpriteRenderer sr;

    public PlayerStatus Status {
        get {
            return status;
        }

        set {
            status = value;
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
        Status = PlayerStatus.none;
        speed = defaultSpeed;
        sr = GetComponent<SpriteRenderer>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (Status == PlayerStatus.asleep || Status == PlayerStatus.transformed || rl.activeItem == null) {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right) {
            if (rl.activeItem.item.item is Potion && (rl.activeItem.item.item != lastTaken || Status == PlayerStatus.none)) {
                lastTaken = rl.activeItem.item.item as Potion;
                StartCoroutine(HandlePotions(rl.activeItem.item.item as Potion));
                rl.inv.RemoveItem(rl.activeItem.item);
            }
        }
    }

    private void FixedUpdate() {
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
        pos.x += x * speed * Time.deltaTime;
        pos.y += y * speed * Time.deltaTime;
        transform.position = pos;
    }



    private void OnTriggerEnter2D(Collider2D collision) {
        switch (collision.gameObject.name) {
            case "ToWorld":
                GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToWorld");
                break;
            case "ToHouse":
                GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToHouse");
                break;
            case "ToGarden":
                GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToGarden");
                break;
            default:
                break;
        }
    }


    IEnumerator HandlePotions(Potion pot) {
        foreach(AnimatorControllerParameter p in anim.parameters) {
            anim.SetBool(p.name, false);
        }

        switch (pot.Primary) {
            case Ingredient.Attributes.healing:
                anim.SetBool("Healing", true);
                if (Status == PlayerStatus.poisoned) {
                    Status = PlayerStatus.none;
                    speed = defaultSpeed;
                }
                //TODO:Maybe add more use for this potion
                break;
            case Ingredient.Attributes.invisibility:
                anim.SetBool("Invisible", true);
                anim.Play("Invisible", 0, 0);
                Status = PlayerStatus.invisible;
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.5f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case Ingredient.Attributes.mana:
                anim.SetBool("Mana", true);
                //TODO: give you mana back
                break;
            case Ingredient.Attributes.poison:
                anim.SetBool("Poison", true);
                Status = PlayerStatus.poisoned;
                if (Status == PlayerStatus.fast) {
                    speed = defaultSpeed; ;
                } else {
                    speed = poisonedSpeed;
                }
                break;
            case Ingredient.Attributes.sleep:
                anim.SetBool("Sleep", true);
                Status = PlayerStatus.asleep;
                speed = 0;
                break;
            case Ingredient.Attributes.speed:
                anim.SetBool("Speed", true);
                Status = PlayerStatus.fast;
                if (Status == PlayerStatus.poisoned) {
                    speed = defaultSpeed;
                } else {
                    speed = fastSpeed;
                }
                break;
            case Ingredient.Attributes.transformation:
                Status = PlayerStatus.transformed;
                anim.SetBool("Transformation", true);
                anim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Characters/cat");
                Vector2 temp = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
                GetComponent<BoxCollider2D>().size = temp;

                speed = defaultSpeed + 1;
                break;
            case Ingredient.Attributes.none:
                break;
            default:
                break;
        }


        yield return new WaitForSeconds((pot.Duration / 10 ) * GameObject.Find("Clock").GetComponent<MoonCycle>().CLOCK_SPEED);

        switch (pot.Primary) {
            case Ingredient.Attributes.healing:
                anim.SetBool("Healing", false);
                break;
            case Ingredient.Attributes.invisibility:
                anim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.833f);
                anim.SetBool("Invisible", false);
                Status = PlayerStatus.none;
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 1f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case Ingredient.Attributes.mana:
                anim.SetBool("Mana", false);
                break;
            case Ingredient.Attributes.poison:
                anim.SetBool("Poison", false);
                Status = PlayerStatus.none;
                speed = defaultSpeed;
                break;
            case Ingredient.Attributes.sleep:
                anim.SetBool("Sleep", false);
                Status = PlayerStatus.none;
                speed = defaultSpeed;
                break;
            case Ingredient.Attributes.speed:
                anim.SetBool("Speed", false);
                Status = PlayerStatus.none;
                speed = defaultSpeed;
                break;
            case Ingredient.Attributes.transformation:
                anim.Play("Transformation", 0, 0);

                yield return new WaitForSeconds(0.5f);
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Characters/witch");
                Vector2 temp = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
                GetComponent<BoxCollider2D>().size = temp;
                yield return new WaitForSeconds(0.43f);
                anim.SetBool("Transformation", false);
                Status = PlayerStatus.none;
                speed = defaultSpeed;
                break;
            case Ingredient.Attributes.none:
                break;
            default:
                break;
        }
    }
}
