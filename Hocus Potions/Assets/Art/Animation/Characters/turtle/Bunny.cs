using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour {


    string currentAnim;
    Animator bunnyAnim, effectsAnim;
    float speed;

    public Vector3 currentLocation;
    public Vector3 destination;
    public Vector3 fleeLocation;

    bool idling;
    bool sleeping;
    public bool followPlayer;
    public bool fleePlayer;

	// Use this for initialization
	void Start () {

        currentAnim = "Forward";
        bunnyAnim = GetComponentInChildren<Animator>();
        idling = false;
        currentLocation = this.transform.position;
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
            this.transform.position = Vector2.MoveTowards(this.transform.position, destination, Time.deltaTime);

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
            this.transform.position =  Vector2.MoveTowards(this.transform.position, destination, Time.deltaTime);
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

    /*IEnumerator PotionEffects(Potion pot)
    {
        effects.SetActive(true);
        Ingredient.Attributes? type = pot.Primary;

        switch (type)
        {
            case Ingredient.Attributes.healing:
                effectsAnim.SetBool("Healing", true);
                break;
            case Ingredient.Attributes.invisibility:
                info.state.Add(Status.invisible);
                effectsAnim.SetBool("Invisible", true);
                effectsAnim.Play("Invisible", 0, 0);
                yield return new WaitForSeconds(0.83f);
                effectsAnim.SetBool("Invisible", false);
                Color c = GetComponent<SpriteRenderer>().color;
                c.a = 0.25f;
                GetComponent<SpriteRenderer>().color = c;
                info.potionTimers.Add(Status.invisible, new NPCController.TimerData(Time.time, pot.Duration));
                break;
            case Ingredient.Attributes.mana:
                effectsAnim.SetBool("Mana", true);
                break;
            case Ingredient.Attributes.poison:
                speed--;
                info.state.Add(Status.poisoned);
                info.potionTimers.Add(Status.poisoned, new NPCController.TimerData(Time.time, pot.Duration));
                effectsAnim.SetBool("Poison", true);
                break;
            case Ingredient.Attributes.sleep:
                info.state.Add(Status.asleep);
                info.potionTimers.Add(Status.asleep, new NPCController.TimerData(Time.time, pot.Duration));
                effectsAnim.SetBool("Sleep", true);
                sleeping = true;
                allowedToMove = false;
                break;
            case Ingredient.Attributes.speed:
                info.state.Add(Status.fast);
                info.potionTimers.Add(Status.fast, new NPCController.TimerData(Time.time, pot.Duration));
                speed = 8;
                effectsAnim.SetBool("Speed", true);
                break;
            case Ingredient.Attributes.transformation:
                effectsAnim.SetBool("Transformation", true);
                effectsAnim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                info.state.Add(Status.transformed);
                info.potionTimers.Add(Status.transformed, new NPCController.TimerData(Time.time, pot.Duration));
                speed++;
                playerAnim.SetBool("Transform", true);
                effectsAnim.SetBool("Transformation", false);
                break;
            case Ingredient.Attributes.none:
                break;
            default:
                break;
        }
        controller.npcData[characterName] = info;
        while (closed == false)
        {
            yield return null;
        }

        yield return new WaitForSeconds((pot.Duration / 10) * GameObject.FindObjectOfType<MoonCycle>().CLOCK_SPEED);

        switch (type)
        {
            case Ingredient.Attributes.healing:
                effectsAnim.SetBool("Healing", false);
                break;
            case Ingredient.Attributes.invisibility:
                info.state.Remove(Status.invisible);
                info.potionTimers.Remove(Status.invisible);
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
                info.state.Remove(Status.poisoned);
                info.potionTimers.Remove(Status.poisoned);
                effectsAnim.SetBool("Poison", false);
                break;
            case Ingredient.Attributes.sleep:
                info.state.Remove(Status.asleep);
                info.potionTimers.Remove(Status.asleep);
                effectsAnim.SetBool("Sleep", false);
                playerAnim.SetBool("Sleep", false);
                yield return new WaitForSeconds(0.33f);
                sleeping = false;
                allowedToMove = true;
                break;
            case Ingredient.Attributes.speed:
                info.state.Remove(Status.fast);
                info.potionTimers.Remove(Status.fast);
                speed = 4;
                effectsAnim.SetBool("Speed", false);
                break;
            case Ingredient.Attributes.transformation:
                info.state.Remove(Status.transformed);
                info.potionTimers.Remove(Status.transformed);
                speed--;
                effectsAnim.SetBool("Transformation", true);
                effectsAnim.Play("Transformation", 0, 0);
                yield return new WaitForSeconds(0.5f);
                playerAnim.SetBool("Transform", false);
                effectsAnim.SetBool("Transformation", false);
                break;
            default:
                break;
        }
        controller.npcData[characterName] = info;
        effects.SetActive(false);
    }*/

}
