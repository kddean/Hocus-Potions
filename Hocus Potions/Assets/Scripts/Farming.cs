using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farming : MonoBehaviour {

    public Sprite emptyPlot;
    public Sprite sowedPlot;
    public Sprite plantPlot;
    public bool harvestReady;

    private Sprite currentSprite;

    private SpriteRenderer spriteRenderer;


    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
        if(this.GetComponent<SpriteRenderer>().sprite == sowedPlot && harvestReady == false)
        {
           
           
        }



	}

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
            StartCoroutine(GrowTime());
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

    private IEnumerator GrowTime()
    {
        //print(Time.time);
        yield return new WaitForSeconds(5);
        //print(Time.time);
        this.GetComponent<SpriteRenderer>().sprite = plantPlot;
        this.harvestReady = true;
        currentSprite = this.GetComponent<SpriteRenderer>().sprite;
        Debug.Log("The current sprite: " + currentSprite);
    }

    private IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(0.5f);
        Player.heldItem = "seeds";
        //Debug.Log("Wait time over");
    }
        
}
