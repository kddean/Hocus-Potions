using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farming : MonoBehaviour {

    public Sprite emptyPlot;
    public Sprite sowedPlot;
    public Sprite plantPlot;

    private SpriteRenderer spriteRenderer;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
        if(this.GetComponent<SpriteRenderer>().sprite == sowedPlot)
        {
            StartCoroutine(GrowTime());
           
        }



	}

    private void OnMouseDown()
    {
        if(Player.heldItem == "seeds")
        {
            this.GetComponent<SpriteRenderer>().sprite = sowedPlot;           
        }

        if (this.GetComponent<SpriteRenderer>().sprite == plantPlot)
        {
            Player.heldItem = "none";
            this.GetComponent<SpriteRenderer>().sprite = emptyPlot;
            StartCoroutine(WaitTime());

        }

    }

    private IEnumerator GrowTime()
    {
        print(Time.time);
        yield return new WaitForSeconds(5);
        print(Time.time);
        this.GetComponent<SpriteRenderer>().sprite = plantPlot;
    }

    private IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(2);
        Player.heldItem = "seeds";
    }
}
