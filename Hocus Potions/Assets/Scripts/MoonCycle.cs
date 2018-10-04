using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MoonCycle : MonoBehaviour {

    // Clock System
    //Needed for the Schedule

    // Moon phase is a day of the week


    static public GameObject Clock;
    //static public Time time;
    public Text hoursUI;
    public Text minsUI;

    int[] moonCycle = new int[5];
    public Sprite[] moonCycleSprites = new Sprite[5];
    public Image moonPhase;
    int currentMoonPhase = 0;
 

    int hour;
    int minutes;

   // int prevTime;


    // Every 2 mins real time is 10 mins game time




	// Use this for initialization
	void Start () {

       
        hour = 6;
        minutes = 00;
        hoursUI.text = hour.ToString();
        minsUI.text = minutes.ToString();
        moonPhase.sprite = moonCycleSprites[0];
        
       
	}
	
	// Update is called once per frame
	void Update () {

       
        StartCoroutine(PassingTime());
        hoursUI.text = hour.ToString();
        minsUI.text = minutes.ToString();
        moonPhase.sprite = moonCycleSprites[currentMoonPhase];


    }

    IEnumerator PassingTime()
    {
        yield return new WaitForSecondsRealtime(5);
        ChangeTime();
       
    }

    void ChangeTime()
    {
        minutes = minutes + 10;


        if (hour == 23 && minutes == 60)
        {
            currentMoonPhase = (currentMoonPhase + 1) % 5;
        }

        if (minutes == 60)
        {
            hour = (hour + 1) % 24;
            minutes = 00;

        }

        
    }


}
