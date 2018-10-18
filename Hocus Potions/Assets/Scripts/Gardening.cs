﻿using System.Collections;
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
    int halfOfGrowthTime;

    private Sprite currentSprite;

    private SpriteRenderer spriteRenderer;

    Ingredient poppy;


    // Use this for initialization
    void Start () {
        currentTime = new int[2];
        clock = GameObject.Find("Clock");
        currentTime[0] = clock.GetComponent<MoonCycle>().Hour;
        currentTime[1] = clock.GetComponent<MoonCycle>().Minutes;

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


            ResourceLoader rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
            rl.inv.Add(rl.ingredients["poppy"], rl.ingredients["poppy"].name, rl.ingredients["poppy"].image);
            
        }
        

    }
    private void GrowTime()
    {
        this.startHour = currentTime[0];
        this.startMinutes = currentTime[1];
        
        this.finishMinutes = (startMinutes + growTime) % 60;
        this.halfOfGrowthTime = finishMinutes / 2;

        Debug.Log(this.finishMinutes);

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
        if(finishMinutes == currentTime[1] && this.GetComponent<SpriteRenderer>().sprite == growPlot)
        {
            PlantReady();
        }
        else if (this.GetComponent<SpriteRenderer>().sprite == sowedPlot)
        {
            
            StartCoroutine(GrowProgressTime());
        }
    }

    void TestingGardenInv()
    {
        poppy.name = "poppy";
        poppy.image = plantPlot;
    }
        
}
