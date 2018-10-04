using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MoonCycle : MonoBehaviour {

    // Clock System
    //Needed for the Schedule

    // Moon phase is a day of the week


    public GameObject Clock;
    //static public Time time;
    public Text hoursUI;
    public Text minsUI;

    int[] moonCycle = new int[5];
    public Sprite[] moonCycleSprites = new Sprite[5];
    public Image moonPhase;
    int currentMoonPhase = 0;
 

    int hour;
    int minutes;

    public int Hour {
        get {
            return hour;
        }

        set {
            hour = value;
        }
    }

    public int Minutes {
        get {
            return minutes;
        }

        set {
            minutes = value;
        }
    }

    // int prevTime;


    // Every 2 mins real time is 10 mins game time




    // Use this for initialization
    void Start () {

       
        Hour = 6;
        Minutes = 00;
        hoursUI.text = Hour.ToString();
        minsUI.text = Minutes.ToString();
        moonPhase.sprite = moonCycleSprites[0];
        
       
	}
	
	// Update is called once per frame
	void Update () {

       
        StartCoroutine(PassingTime());
        hoursUI.text = Hour.ToString();
        minsUI.text = Minutes.ToString();
        moonPhase.sprite = moonCycleSprites[currentMoonPhase];


    }

    IEnumerator PassingTime()
    {
        yield return new WaitForSecondsRealtime(5);
        ChangeTime();
       
    }

    void ChangeTime()
    {
        Minutes = Minutes + 10;


        if (Hour == 23 && Minutes == 60)
        {
            currentMoonPhase = (currentMoonPhase + 1) % 5;
        }

        if (Minutes == 60)
        {
            Hour = (Hour + 1) % 24;
            Minutes = 00;

        }

        
    }


}
