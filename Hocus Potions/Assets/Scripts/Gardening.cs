using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gardening : MonoBehaviour {

    public Sprite emptyPlot;
    public Sprite sowedPlot;
    public Sprite growPlot;
    public Sprite plantPlot;
    
    public bool harvestReady;

    public GameObject clock;
    public int[] currentTime = new int[2];

    public int growTime = 30;
    int startHour;
    int startMinutes;
    int finishMinutes;

    private Sprite currentSprite;

    private SpriteRenderer spriteRenderer;


    // Use this for initialization
    void Start () {
        clock = GameObject.Find("Clock");
        currentTime[0] = clock.GetComponent<MoonCycle>().getHours();
        currentTime[1] = clock.GetComponents<MoonCycle>().getMinutes();       
	}

    void Update()
    {
        currentTime[0] = clock.GetComponent<MoonCycle>().getHours();
        currentTime[1] = clock.GetComponents<MoonCycle>().getMinutes();
        CheckTime();
    }

    // Update is called once per frame

    private void OnMouseDown()
    {
        Debug.Log("Current: " + Player.heldItem + " " +  harvestReady);
        if(Player.heldItem == "seeds" && harvestReady == false)
        {
            this.GetComponent<SpriteRenderer>().sprite = sowedPlot;
            //Debug.Log("Planted");
            //Debug.Log(this);
            currentSprite = this.GetComponent<SpriteRenderer>().sprite;
            Debug.Log("The current sprite: " + currentSprite);
            GrowTime();
        }
        else if (this.GetComponent<SpriteRenderer>().sprite == plantPlot && harvestReady == true)
        {
            Player.heldItem = "none";
            //Debug.Log(Player.heldItem);
            this.GetComponent<SpriteRenderer>().sprite = emptyPlot;
            this.harvestReady = false;
            //Debug.Log("Empty");
            currentSprite = this.GetComponent<SpriteRenderer>().sprite;
            Debug.Log("The current sprite: " +  currentSprite);
            //StartCoroutine(WaitTime());

            Player.heldItem = "seeds";

        }
        

    }
    private void GrowTime()
    {
        this.startHour = currentTime[0];
        this.startMinutes = currentTime[1];
        //int finishHour;
        this.finishMinutes = startMinutes + growTime;


    }

    private IEnumerator GrowProgressTime()
    {
       yield return new WaitForSeconds(5);
       if (this.GetComponent<SpriteRenderer>().sprite == sowedPlot)
        {
            this.GetComponent<SpriteRenderer>().sprite = growPlot;
        }
        //StartCoroutine(WaitTime());
        
    }

    private IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(10);
        PlantReady();
        //Player.heldItem = "seeds";
        //Debug.Log("Wait time over");
    }

    void PlantReady()
    {
        this.GetComponent<SpriteRenderer>().sprite = plantPlot;
        this.harvestReady = true;
        currentSprite = this.GetComponent<SpriteRenderer>().sprite;
        Debug.Log("The current sprite: " + currentSprite);
    }

    void CheckTime()
    {
        if(finishMinutes == currentTime[1])
        {
            PlantReady();
        }
        else
        {
            GrowProgressTime();
        }
    }
        
}
