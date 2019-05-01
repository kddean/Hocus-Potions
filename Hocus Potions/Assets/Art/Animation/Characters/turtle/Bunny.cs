using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bunny : MonoBehaviour {

    ResourceLoader rl;
    string currentAnim;
    Animator bunnyAnim, effectsAnim;
    float speed;

    public Vector3 currentLocation;
    public Vector3 destination;
    public Vector3 fleeLocation;

    bool idling;
    bool sleeping;
    bool poisoned;
    bool transformed;
    bool onSpeed;
    bool invisible;
    bool healing;
    bool mana;

    public bool followPlayer;
    public bool fleePlayer;

    GameObject effects;

    // Use this for initialization
    void Start () {

        currentAnim = "Forward";
        bunnyAnim = GetComponentInChildren<Animator>();
        idling = false;
        currentLocation = this.transform.position;
        effects = gameObject.transform.Find("effects").gameObject;
        rl = GameObject.FindObjectOfType<ResourceLoader>();
        speed = 1;
        effectsAnim = effects.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        currentLocation = this.transform.position;

        //Movement
        if (bunnyAnim != null && bunnyAnim.enabled)
        {
            if (transform.position.x < destination.x)
            {
                bunnyAnim.SetBool(currentAnim, false);
                currentAnim = "Right";
                bunnyAnim.SetBool(currentAnim, true);
            }
            else if (transform.position.x > destination.x)
            {
                bunnyAnim.SetBool(currentAnim, false);
                currentAnim = "Left";
                bunnyAnim.SetBool(currentAnim, true);

            }
            else if (transform.position.y > destination.y)
            {
                bunnyAnim.SetBool(currentAnim, false);
                currentAnim = "Backward";
                bunnyAnim.SetBool(currentAnim, true);

            }
            else if (transform.position.y < destination.y || idling == true)
            {
                bunnyAnim.SetBool(currentAnim, false);
                currentAnim = "Foreward";
                bunnyAnim.SetBool(currentAnim, true);
            }
        }


        if (followPlayer)
        {
            followingPlayer();
        }
        else if (fleePlayer && fleeLocation == null)
        {
            fleeingPlayer();
        }
        else if (currentLocation != destination)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, destination, speed*Time.deltaTime);

        }
        else { Wandering(); }

    }

    void Wandering ()
    {
        float sideToSide = Random.Range(2, 68);
        float upToDown = Random.Range(-45, -20);

        destination = new Vector2(sideToSide, upToDown);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        destination = GameObject.Find("BunnyHome").transform.position;
        StartCoroutine(Wander());
    }

    IEnumerator Wander()
    {
        yield return new WaitForSeconds(3);
        Wandering();
    }


    void followingPlayer()
    {
       
      if(currentLocation != destination)
        {           
            destination = GameObject.Find("BunnyManager").GetComponent<BunnyManager>().Player.transform.position + new Vector3(1,0);
            this.transform.position =  Vector2.MoveTowards(this.transform.position, destination, speed*Time.deltaTime);
        }
        else
        {
            idling = true;
        }       

    }

    void fleeingPlayer()
    {
        fleeLocation = this.transform.position - GameObject.Find("BunnyManager").GetComponent<BunnyManager>().Player.transform.position;
        if(fleeLocation.x > 2)
        {
            fleeLocation.x = 2;
        }
        if (fleeLocation.x > 68)
        {
            fleeLocation.x = 68;
        }
        if (fleeLocation.y < -20)
        {
            fleeLocation.y = -20;
        }
        if (fleeLocation.y < -45)
        {
            fleeLocation.y = -45;
        }
        Debug.Log(fleeLocation);
        destination = fleeLocation;
    }

    public void OnMouseDown()
    {
        
        {
            if (rl.activeItem != null)
            {
                if (rl.activeItem.item.item is Potion)
                {
                    GivePotion(rl.activeItem);
                }
            }
        }
    }
    public void GivePotion(InventorySlot slot)
    {
        Potion temp = slot.item.item as Potion;

        if (temp.Primary == Ingredient.Attributes.none || temp.Primary == null)
        {
            if (Random.Range(0, 1.0f) < 0.5f)
            {
                int rand = Random.Range(0, 7);
                switch (rand)
                {
                    case 0:
                        temp = new Potion("Healing Potion", "Potions/potions_healing", 10, Ingredient.Attributes.healing, null, null, 0);
                        break;
                    case 1:
                        temp = new Potion("Sleep Potion", "Potions/potions_sleep", 40, Ingredient.Attributes.sleep, null, null, 0);
                        break;
                    case 2:
                        temp = new Potion("Invisibility Potion", "Potions/potions_invisibility", 25, Ingredient.Attributes.invisibility, null, null, 0);
                        break;
                    case 3:
                        temp = new Potion("Poison Potion", "Potions/potions_poison", 25, Ingredient.Attributes.poison, null, null, 0);
                        break;
                    case 4:
                        temp = new Potion("Transformation Potion", "Potions/potions_transform", 25, Ingredient.Attributes.transformation, null, null, 0);
                        break;
                    case 5:
                        temp = new Potion("Mana Potion", "Potions/potions_mana", 10, Ingredient.Attributes.mana, null, null, 0);
                        break;
                    case 6:
                        temp = new Potion("Speed Potion", "Potions/potions_speed", 25, Ingredient.Attributes.speed, null, null, 0);
                        break;
                    default:
                        break;
                }
            }
        }

        StartCoroutine(PotionEffects(temp));
    }

        IEnumerator PotionEffects(Potion pot)
    {
        effects.SetActive(true);
        Ingredient.Attributes? type = pot.Primary;

        switch (type)
        {
            case Ingredient.Attributes.healing:
                effectsAnim.SetBool("Healing", true);
                followPlayer = true;
                fleePlayer = false;
                break;
            case Ingredient.Attributes.invisibility:
                invisible = true;
                effectsAnim.SetBool("Invisible", true);
                effectsAnim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.83f);
                effectsAnim.SetBool("Invisible", false);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.25f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case Ingredient.Attributes.mana:
                effectsAnim.SetBool("Mana", true);
                break;
            case Ingredient.Attributes.poison:
                speed--;
                poisoned = true;             
                effectsAnim.SetBool("Poison", true);
                break;
            case Ingredient.Attributes.sleep:
                sleeping = true;              
                effectsAnim.SetBool("Sleep", true);
                sleeping = true;
                break;
            case Ingredient.Attributes.speed:
                onSpeed = true;
                speed = 8;
                effectsAnim.SetBool("Speed", true);
                break;
            case Ingredient.Attributes.transformation:
                effectsAnim.SetBool("Transformation", true);
                effectsAnim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                transformed = true;
                speed++;
                bunnyAnim.SetBool("Transform", true);
                effectsAnim.SetBool("Transformation", false);
                break;
            case Ingredient.Attributes.none:
                break;
            default:
                break;
        }
        

        yield return new WaitForSeconds((pot.Duration / 10) * GameObject.FindObjectOfType<MoonCycle>().CLOCK_SPEED);

        switch (type)
        {
            case Ingredient.Attributes.healing:
                effectsAnim.SetBool("Healing", false);
                break;
            case Ingredient.Attributes.invisibility:
                invisible = false;
                effectsAnim.SetBool("Invisible", true);
                effectsAnim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.83f);
                effectsAnim.SetBool("Invisible", false);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.25f;
                GetComponent<SpriteRenderer>().color = c;
                break;
            case Ingredient.Attributes.mana:
                effectsAnim.SetBool("Mana", false);
                break;
            case Ingredient.Attributes.none:
                break;
            case Ingredient.Attributes.poison:
                speed++;
                poisoned = false;
                effectsAnim.SetBool("Poison", false);
                break;
            case Ingredient.Attributes.sleep:
                sleeping = false;
                effectsAnim.SetBool("Sleep", false);
                //bunnyAnim.SetBool("Sleep", false);
                yield return new WaitForSeconds(0.33f);
                sleeping = false;
                break;
            case Ingredient.Attributes.speed:
                onSpeed = false;
                speed = 4;
                effectsAnim.SetBool("Speed", false);
                break;
            case Ingredient.Attributes.transformation:
                transformed = false;
                speed--;
                effectsAnim.SetBool("Transformation", true);
                effectsAnim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                bunnyAnim.SetBool("Transform", false);
                effectsAnim.SetBool("Transformation", false);
                break;
            default:
                break;
        }
       
        effects.SetActive(false);
    }

}
