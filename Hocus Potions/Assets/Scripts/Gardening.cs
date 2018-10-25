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

    public int growTime = 60;
    int startHour;
    int startMinutes;
    int finishHour;
    int finishMinutes;
    int halfOfGrowthTimeHour;
    int halfOfGrowthTimeMinutes;

    private Sprite currentSprite;

    private SpriteRenderer spriteRenderer;

    Ingredient poppy;

    ResourceLoader rl;

    // Use this for initialization
    void Start () {
        currentTime = new int[2];
        clock = GameObject.Find("Clock");
        currentTime[0] = clock.GetComponent<MoonCycle>().Hour;
        currentTime[1] = clock.GetComponent<MoonCycle>().Minutes;
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();

        //TestingGardenInv();

    }

    void Update()
    {
        currentTime[0] = clock.GetComponent<MoonCycle>().Hour;
        currentTime[1] = clock.GetComponent<MoonCycle>().Minutes;
        CheckTime();
        //Debug.Log(currentTime[0]);
        //Debug.Log(currentTime[1]);
    }

    // Update is called once per frame

    private void OnMouseDown()
    {
        Debug.Log("Current: " + Player.heldItem + " " +  harvestReady);
        if(Player.heldItem == "seeds" && harvestReady == false)
        {
            this.GetComponent<SpriteRenderer>().sprite = sowedPlot;
            Debug.Log("Planted");
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





         
            rl.inv.Add(rl.ingredients["poppy"], rl.ingredients["poppy"].name, rl.ingredients["poppy"].image);
            
        }
        

    }
    private void GrowTime()
    {
        this.startHour = currentTime[0];
        this.startMinutes = currentTime[1];

        // Calculate the time when growing should finish
        if((this.startMinutes + growTime > 60))
        {
            int increaseHourBy = (this.startMinutes + growTime) / 60;
            Debug.Log("Increase Hour By: " + increaseHourBy);
            
            this.finishHour = this.startHour + increaseHourBy;
            Debug.Log("Finish Hour is " + this.finishHour);
        }
        this.finishMinutes = (startMinutes + growTime) % 60;

        //Calculate the half of the finish time for updating sprites
        int halfOfGrowthTime = growTime / 2;
        if((this.startMinutes + halfOfGrowthTime) > 60)
        {
            int increaseBy = (this.startMinutes + halfOfGrowthTime) / 60;
            this.halfOfGrowthTimeHour = this.startHour + increaseBy;
        }
        else
        {
            this.halfOfGrowthTimeHour = currentTime[0];
        }
        this.halfOfGrowthTimeMinutes = (this.startMinutes + halfOfGrowthTime) % 60;


        Debug.Log("Finish Minutes is: " + this.finishMinutes);
        Debug.Log("Half time is " + this.halfOfGrowthTimeHour + ":" + this.halfOfGrowthTimeMinutes);
        Debug.Log("Finish time is " + this.finishHour + ":" + this.finishMinutes);

        //StartCoroutine(GrowProgressTime());

    }

    private IEnumerator GrowProgressTime()
    {
       yield return new WaitForSeconds(10);
       if (this.GetComponent<SpriteRenderer>().sprite == sowedPlot)
        {
            this.GetComponent<SpriteRenderer>().sprite = growPlot;
        }
        
        //StartCoroutine(WaitTime());
        
    }

    private IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(50);
        //PlantReady();
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
        if (this.finishHour == currentTime[0] && this.finishMinutes == currentTime[1] && this.GetComponent<SpriteRenderer>().sprite == growPlot)
        {
            PlantReady();
        }
        else if (this.halfOfGrowthTimeHour == currentTime[0] && this.halfOfGrowthTimeMinutes == currentTime[1] && this.GetComponent<SpriteRenderer>().sprite == sowedPlot)
        {

            this.GetComponent<SpriteRenderer>().sprite = growPlot;

        }
    }

    void TestingGardenInv()
    {
        poppy.name = "poppy";
        poppy.image = plantPlot;
    }
        
}
