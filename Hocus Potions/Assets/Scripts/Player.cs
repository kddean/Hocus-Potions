﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour, IPointerDownHandler {

    public enum PlayerStatus { poisoned, fast, invisible, transformed, asleep, none }
    float speed;
    public float defaultSpeed, poisonedSpeed, fastSpeed;
    int x, y;
    ResourceLoader rl;
    InputManager inputManager;
    Animator effectAnim, playerAnim;
    Potion lastTaken;
    [SerializeField]
    List<PlayerStatus> status;
    Dictionary<PlayerStatus, TimerData> startTimers;
    string currentAnim, lastAnim;
    GameObject fadeScreen, sleepCanvas;
    bool idling;

    public bool swappingScenes;
    public bool allowedToMove;
    public bool layerSwapping;
    public float previousSpeed;

    public struct TimerData {
        public float startTime;
        public float duration;

        public TimerData(float startTime, float duration) {
            this.startTime = startTime;
            this.duration = duration;
        }
    }

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

    public Potion LastTaken {
        get {
            return lastTaken;
        }

        set {
            lastTaken = value;
        }
    }

    public Dictionary<PlayerStatus, TimerData> StartTimers {
        get {
            return startTimers;
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
        inputManager = FindObjectOfType<InputManager>();
        sleepCanvas = Resources.FindObjectsOfTypeAll<SleepCanvas>()[0].gameObject; ;
        fadeScreen = sleepCanvas.GetComponentInChildren<Image>().gameObject;
        playerAnim = GetComponent<Animator>();
        effectAnim = GetComponentsInChildren<Animator>()[1];
        Status = new List<PlayerStatus>();
        startTimers = new Dictionary<PlayerStatus, TimerData>();
        Speed = defaultSpeed;
        swappingScenes = false;
        allowedToMove = true;
        layerSwapping = false;
        idling = false;
        currentAnim = "Idle";
        lastAnim = "None";
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (Status.Contains(PlayerStatus.asleep) || Status.Contains(PlayerStatus.transformed) || rl.activeItem == null) {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right) {
            if (rl.activeItem.item.item is Potion) {
                UsePotion(rl.activeItem.item.item as Potion, rl.activeItem);
            }
        }
    }

    public void UsePotion(Potion pot, InventorySlot slot ) {
        if (status.Contains(ConvertAttribute(pot))) { return; }
        if (pot.name.Contains("dye")) {
            string type = pot.name.Substring(0, pot.name.Length - 4);
            Wardrobe wardrobe = GameObject.FindObjectOfType<Wardrobe>();
            switch (type) {
                case "Black":
                    wardrobe.Unlocked[1] = true;
                    wardrobe.LoadCostume("Player_Black");
                    break;
                case "Blue":
                    wardrobe.Unlocked[2] = true;
                    wardrobe.LoadCostume("Player_Blue");
                    break;
                case "Green":
                    wardrobe.Unlocked[3] = true;
                    wardrobe.LoadCostume("Player_Green");
                    break;
                case "Purple":
                    wardrobe.LoadCostume("Player_Default");
                    break;
                case "Red":
                    wardrobe.Unlocked[4] = true;
                    wardrobe.LoadCostume("Player_Red");
                    break;
                case "White":
                    wardrobe.Unlocked[5] = true;
                    wardrobe.LoadCostume("Player_White");
                    break;
                case "Yellow":
                    wardrobe.Unlocked[6] = true;
                    wardrobe.LoadCostume("Player_Yellow");
                    break;
                default:
                    break;
            }
        } else {
            if (!pot.name.Contains("of")) {
                StartCoroutine(HandlePotions(pot));
            }
        }
        if (!pot.name.Contains("of")) {
            Inventory.RemoveItem(slot);
        }
    }
    PlayerStatus ConvertAttribute(Potion pot) {
        switch (pot.Primary) {
            case Ingredient.Attributes.poison:
                return PlayerStatus.poisoned;
            case Ingredient.Attributes.speed:
                return PlayerStatus.fast;
            case Ingredient.Attributes.invisibility:
                return PlayerStatus.invisible; 
            case Ingredient.Attributes.transformation:
                return PlayerStatus.transformed;
            case Ingredient.Attributes.sleep:
                return PlayerStatus.asleep;
            default:
                return PlayerStatus.none;
        }
    }

    private void FixedUpdate() {
        if (!allowedToMove) {
            StartCoroutine(StartIdle());
            idling = true;
            return; }
        if (!currentAnim.Equals(lastAnim)) {
            if (playerAnim.GetBool("Transform")) {
                GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x / 1.5f, GetComponent<SpriteRenderer>().bounds.size.y / 12);
                GetComponent<BoxCollider2D>().offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 23);
                GetComponents<BoxCollider2D>()[1].size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, GetComponent<SpriteRenderer>().bounds.size.y);
                GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 2);
            } else {
                GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x / 2, GetComponent<SpriteRenderer>().bounds.size.y / 12);
                GetComponent<BoxCollider2D>().offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 23);
                GetComponents<BoxCollider2D>()[1].size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, GetComponent<SpriteRenderer>().bounds.size.y);
                GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 2);
            }
            lastAnim = currentAnim;
        }
        
        Vector3 pos = transform.position;
        playerAnim.speed = speed / 7.5f;
        x = y = 0;
        if (Input.GetKey(inputManager.walkForwardKey)) {
            idling = false;
            if (playerAnim.GetBool("Transform")) {
                playerAnim.Play("T_Backward");
            } else {
                playerAnim.Play("Backward");
            }
            playerAnim.SetBool(currentAnim, false);
            playerAnim.SetBool("Backward", true);
            currentAnim = "Backward";
            y = 1;
        } else if (Input.GetKey(inputManager.walkBackwardKey)) {
            idling = false;
            if (playerAnim.GetBool("Transform")) {
                playerAnim.Play("T_Forward");
            } else {
                playerAnim.Play("Forward");
            }
            playerAnim.SetBool(currentAnim, false);
            playerAnim.SetBool("Forward", true);
            currentAnim = "Forward";
            y = -1;
        } else if (Input.GetKey(inputManager.walkLeftKey)) {
            idling = false;
            if (playerAnim.GetBool("Transform")) {
                playerAnim.Play("T_Left");
            } else {
                playerAnim.Play("Left");
            }
            playerAnim.SetBool(currentAnim, false);
            playerAnim.SetBool("Left", true);
            currentAnim = "Left";
            x = -1;
        } else if (Input.GetKey(inputManager.walkRightKey)) {
            idling = false;
            if (playerAnim.GetBool("Transform")) {
                playerAnim.Play("T_Right");
            } else {
                playerAnim.Play("Right");
            }
            playerAnim.SetBool(currentAnim, false);
            playerAnim.SetBool("Right", true);
            currentAnim = "Right";
            x = 1;
        } else {
            if (!idling) {
                StartCoroutine(StartIdle());
                idling = true;
            }
        }
       
        pos.x += x * Speed * 0.02f;
        pos.y += y * Speed * 0.02f;

        if (currentAnim.Equals("Left")) {
            Vector3 temp = pos;
            temp.x -= GetComponent<SpriteRenderer>().bounds.size.x / 3f;
            RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector3(temp.x, pos.y + 0.5f, 0), new Vector2(0, -1), 0.5f);
            foreach (RaycastHit2D ray in hits) {
                if (ray.collider.gameObject.tag.Equals("tiles")) {
                    if (ray.collider.bounds.Contains(temp)) {
                        return;
                    }
                }
            }
        } else if(currentAnim.Equals("Right")) {
            Vector3 temp = pos;
            temp.x += GetComponent<SpriteRenderer>().bounds.size.x / 3f;
            RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector3(temp.x, pos.y + 0.5f, 0), new Vector2(0, -1), 0.5f);
            foreach (RaycastHit2D ray in hits) {
                if (ray.collider.gameObject.tag.Equals("tiles")) {
                    if (ray.collider.bounds.Contains(temp)) {
                        return;
                    }
                }
            }
        }
        
        transform.position = pos;
    }

    IEnumerator StartIdle() {
        yield return new WaitForSecondsRealtime(0.1f);
        if (idling) {
            playerAnim.SetBool(currentAnim, false);
            currentAnim = "Idle";
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (!swappingScenes && GetComponent<BoxCollider2D>().IsTouching(collision)) {
            switch (collision.gameObject.name) {
                case "ToWorld":
                    GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToWorld");
                    swappingScenes = true;
                    previousSpeed = speed;
                    speed = 0;
                    break;
                case "ToHouse":
                    GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToHouse");
                    swappingScenes = true;
                    previousSpeed = speed;
                    speed = 0;
                    break;
                case "ToGarden":
                    GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToGarden");
                    swappingScenes = true;
                    previousSpeed = speed;
                    speed = 0;
                    break;
                default:
                    break;
            }
        }
    }

   
    private void OnTriggerStay2D(Collider2D collision) {
        if (!layerSwapping && collision.gameObject.GetComponent<Pickups>() != null) {
            collision.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        }
        if (layerSwapping) {
            if (collision.gameObject.GetComponent<Pickups>() != null) {
                collision.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "InFrontOfPlayer";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Pickups>() != null) {
            collision.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "InFrontOfPlayer";
        }
    }

    IEnumerator HandlePotions(Potion pot) {
        foreach (AnimatorControllerParameter p in effectAnim.parameters) {
            effectAnim.SetBool(p.name, false);
        }
        float duration = (pot.Duration / 10) * GameObject.Find("Clock").GetComponent<MoonCycle>().CLOCK_SPEED;
        switch (pot.Primary) {
            case Ingredient.Attributes.healing:
                effectAnim.SetBool("Healing", true);
                if (Status.Contains(PlayerStatus.poisoned)) {
                    Status.Remove(PlayerStatus.poisoned);
                    Speed = defaultSpeed;
                }
                //TODO:Maybe add more use for this potion
                break;
            case Ingredient.Attributes.invisibility:
                effectAnim.SetBool("Invisible", true);
                effectAnim.Play("Invisible", 0, 0);
                Status.Add(PlayerStatus.invisible);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.5f;
                GetComponent<SpriteRenderer>().color = c;
                startTimers.Add(PlayerStatus.invisible, new TimerData(Time.time, duration));
                break;
            case Ingredient.Attributes.mana:
                effectAnim.SetBool("Mana", true);
                GameObject.FindObjectOfType<Mana>().UpdateMana(-50);
                break;
            case Ingredient.Attributes.poison:
                effectAnim.SetBool("Poison", true);
                Status.Add(PlayerStatus.poisoned);
                if (Status.Contains(PlayerStatus.fast)) {
                    Speed = defaultSpeed;
                } else {
                    Speed = poisonedSpeed;
                }
                startTimers.Add(PlayerStatus.poisoned, new TimerData(Time.time, duration));
                break;
            case Ingredient.Attributes.sleep:
                effectAnim.SetBool("Sleep", true);
                Status.Add(PlayerStatus.asleep);
                playerAnim.SetBool(currentAnim, false);
                playerAnim.SetBool("Sleep", true);
                allowedToMove = false;
                sleepCanvas.SetActive(true);
                StartCoroutine(FadeScreen(1));
                yield return new WaitForSeconds(2);
                GameObject.FindObjectOfType<Mana>().UpdateMana(-(pot.Duration / 60) * 10);
                Time.timeScale = Time.timeScale / (0.1f / GameObject.Find("Clock").GetComponent<MoonCycle>().CLOCK_SPEED);
                break;
            case Ingredient.Attributes.speed:
                effectAnim.SetBool("Speed", true);
                Status.Add(PlayerStatus.fast);
                if (Status.Contains(PlayerStatus.poisoned)) {
                    Speed = defaultSpeed;
                } else {
                    Speed = fastSpeed;
                }
                startTimers.Add(PlayerStatus.fast, new TimerData(Time.time, duration));
                break;
            case Ingredient.Attributes.transformation:
                Status.Add(PlayerStatus.transformed);
                effectAnim.SetBool("Transformation", true);
                effectAnim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                playerAnim.SetBool("Transform", true);
                Speed++;
                startTimers.Add(PlayerStatus.transformed, new TimerData(Time.time, duration));
                GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x / 1.5f, GetComponent<SpriteRenderer>().bounds.size.y / 12);
                GetComponent<BoxCollider2D>().offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 23);
                GetComponents<BoxCollider2D>()[1].size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, GetComponent<SpriteRenderer>().bounds.size.y);
                GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 2);
                break;
            case Ingredient.Attributes.none:
                float check = Random.Range(0, 1.0f);
                if ( check < 0.7f) {
                    int rand = Random.Range(0, 7);
                    switch (rand) {
                        case 0:
                            StartCoroutine(HandlePotions(new Potion("Healing Potion", "Potions/potions_healing", 10, Ingredient.Attributes.healing, null, null, 0)));
                            break;
                        case 1:
                            StartCoroutine(HandlePotions(new Potion("Sleep Potion", "Potions/potions_sleep", 40, Ingredient.Attributes.sleep, null, null, 0)));
                            break;
                        case 2:
                            StartCoroutine(HandlePotions(new Potion("Invisibility Potion", "Potions/potions_invisibility", 25, Ingredient.Attributes.invisibility, null, null, 0)));
                            break;
                        case 3:
                            StartCoroutine(HandlePotions(new Potion("Poison Potion", "Potions/potions_poison", 25, Ingredient.Attributes.poison, null, null, 0)));
                            break;
                        case 4:
                            StartCoroutine(HandlePotions(new Potion("Transformation Potion", "Potions/potions_transform", 25, Ingredient.Attributes.transformation, null, null, 0)));
                            break;
                        case 5:
                            StartCoroutine(HandlePotions(new Potion("Mana Potion", "Potions/potions_mana", 10, Ingredient.Attributes.mana, null, null, 0)));
                            break;
                        case 6:
                            StartCoroutine(HandlePotions(new Potion("Speed Potion", "Potions/potions_speed", 25, Ingredient.Attributes.speed, null, null, 0)));
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(duration);
        foreach (AnimatorControllerParameter p in effectAnim.parameters) {
            effectAnim.SetBool(p.name, false);
        }

        switch (pot.Primary) {
            case Ingredient.Attributes.healing:
                effectAnim.SetBool("Healing", false);
                break;
            case Ingredient.Attributes.invisibility:
                effectAnim.SetBool("Invisible", true);
                effectAnim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.833f);
                effectAnim.SetBool("Invisible", false);
                Status.Remove(PlayerStatus.invisible);
                startTimers.Remove(PlayerStatus.invisible);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 1f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case Ingredient.Attributes.mana:
                effectAnim.SetBool("Mana", false);
                break;
            case Ingredient.Attributes.poison:
                effectAnim.SetBool("Poison", false);
                Status.Remove(PlayerStatus.poisoned);
                startTimers.Remove(PlayerStatus.poisoned);
                if (Status.Contains(PlayerStatus.fast)) {
                    Speed = fastSpeed;
                } else {
                    Speed = defaultSpeed;
                }
                break;
            case Ingredient.Attributes.sleep:
                effectAnim.SetBool("Sleep", false);
                Status.Remove(PlayerStatus.asleep);
                allowedToMove = true;
                Time.timeScale = Time.timeScale * (0.1f / GameObject.Find("Clock").GetComponent<MoonCycle>().CLOCK_SPEED);
                StartCoroutine(FadeScreen(-1));
                playerAnim.SetBool("Sleep", false);
                sleepCanvas.GetComponentsInChildren<CanvasGroup>()[0].blocksRaycasts = false;
                yield return new WaitForSeconds(2);
                sleepCanvas.SetActive(false);
                break;
            case Ingredient.Attributes.speed:
                effectAnim.SetBool("Speed", false);
                Status.Remove(PlayerStatus.fast);
                startTimers.Remove(PlayerStatus.fast);
                if (Status.Contains(PlayerStatus.poisoned)) {
                    Speed = poisonedSpeed;
                } else {
                    Speed = defaultSpeed;
                }
                break;
            case Ingredient.Attributes.transformation:
                effectAnim.SetBool("Transformation", true);
                effectAnim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                playerAnim.SetBool("Transform", false);
                Status.Remove(PlayerStatus.transformed);
                startTimers.Remove(PlayerStatus.transformed);
                yield return new WaitForSeconds(0.43f);
                effectAnim.SetBool("Transformation", false);
                GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x / 2, GetComponent<SpriteRenderer>().bounds.size.y / 12);
                GetComponent<BoxCollider2D>().offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 23);
                GetComponents<BoxCollider2D>()[1].size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, GetComponent<SpriteRenderer>().bounds.size.y);
                GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 2);

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
            effectAnim.SetBool("Speed", true);
        }
        if (Status.Contains(PlayerStatus.poisoned)) {
            effectAnim.SetBool("Poison", true);
        }

        if (status.Count == 0) {
            lastTaken = null;
        }
    }

    public void RestartTimers(PlayerStatus s, float d) {
        StartCoroutine(RestartPotions(s, d));
    }

    IEnumerator RestartPotions(PlayerStatus type, float duration) {
        foreach (AnimatorControllerParameter p in effectAnim.parameters) {
            effectAnim.SetBool(p.name, false);
        }

        switch (type) {
            case PlayerStatus.invisible:
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.5f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case PlayerStatus.poisoned:
                effectAnim.SetBool("Poison", true);
                if (Status.Contains(PlayerStatus.fast)) {
                    Speed = defaultSpeed;
                } else {
                    Speed = poisonedSpeed;
                }
                break;
            case PlayerStatus.fast:
                effectAnim.SetBool("Speed", true);
                if (Status.Contains(PlayerStatus.poisoned)) {
                    Speed = defaultSpeed;
                } else {
                    Speed = fastSpeed;
                }
                break;
            case PlayerStatus.transformed:
                playerAnim.SetBool("Transform", true);
                GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x / 1.5f, GetComponent<SpriteRenderer>().bounds.size.y / 12);
                GetComponent<BoxCollider2D>().offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 23);
                GetComponents<BoxCollider2D>()[1].size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, GetComponent<SpriteRenderer>().bounds.size.y);
                GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 2);
                Speed++;
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(duration);
        foreach (AnimatorControllerParameter p in effectAnim.parameters) {
            effectAnim.SetBool(p.name, false);
        }

        switch (type) {
            case PlayerStatus.invisible:
                effectAnim.SetBool("Invisible", true);
                effectAnim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.833f);
                effectAnim.SetBool("Invisible", false);
                Status.Remove(PlayerStatus.invisible);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 1f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case PlayerStatus.poisoned:
                effectAnim.SetBool("Poison", false);
                Status.Remove(PlayerStatus.poisoned);
                if (Status.Contains(PlayerStatus.fast)) {
                    Speed = fastSpeed;
                } else {
                    Speed = defaultSpeed;
                }

                if (Status.Contains(PlayerStatus.transformed)) {
                    speed++;
                }
                break;
            case PlayerStatus.fast:
                effectAnim.SetBool("Speed", false);
                Status.Remove(PlayerStatus.fast);
                if (Status.Contains(PlayerStatus.poisoned)) {
                    Speed = poisonedSpeed;
                } else {
                    Speed = defaultSpeed;
                }

                if (Status.Contains(PlayerStatus.transformed)) {
                    Speed++;
                }
                break;
            case PlayerStatus.transformed:
                effectAnim.SetBool("Transformation", true);
                effectAnim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                playerAnim.enabled = true;
                playerAnim.SetBool("Transform", false);
                Status.Remove(PlayerStatus.transformed);
                GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x / 2, GetComponent<SpriteRenderer>().bounds.size.y / 12);
                GetComponent<BoxCollider2D>().offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 23);
                GetComponents<BoxCollider2D>()[1].size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, GetComponent<SpriteRenderer>().bounds.size.y);
                GetComponents<BoxCollider2D>()[1].offset = new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y / 2);
                yield return new WaitForSeconds(0.43f);
                effectAnim.SetBool("Transformation", false);

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
            default:
                break;
        }

        if (Status.Contains(PlayerStatus.fast)) {
            effectAnim.SetBool("Speed", true);
        }
        if (Status.Contains(PlayerStatus.poisoned)) {
            effectAnim.SetBool("Speed", false);
            effectAnim.SetBool("Poison", true);
        }

        if (status.Count == 0) {
            lastTaken = null;
        }
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

