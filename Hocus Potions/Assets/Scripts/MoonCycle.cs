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


   




    // Use this for initialization
    void Start () {

        Hour = 6;
        Minutes = 00;
        hoursUI.text = Hour.ToString();
        minsUI.text = Minutes.ToString();
        moonPhase.sprite = moonCycleSprites[0];
        StartCoroutine(PassingTime());
    }
	

    IEnumerator PassingTime()
    {
        //Every 2 mins real time is 10 mins game time - 120 for actual time scale
        yield return new WaitForSecondsRealtime(0.5f);
        //Debug.Log("Did I get called?");
        ChangeTime();
        StartCoroutine(PassingTime());
    }

    void ChangeTime()
    {
        Minutes = Minutes + 10;


        if (Hour == 23 && Minutes == 60)
        {
            currentMoonPhase = (currentMoonPhase + 1) % 5;
            moonPhase.sprite = moonCycleSprites[currentMoonPhase];
        }

        if (Minutes == 60)
        {
            Hour = (Hour + 1) % 24;
            Minutes = 00;

        }
        minsUI.text = Minutes.ToString();
        hoursUI.text = Hour.ToString();
    }


}
