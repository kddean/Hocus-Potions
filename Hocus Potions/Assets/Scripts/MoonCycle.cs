﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MoonCycle : MonoBehaviour {

    // Clock System
    //Needed for the Schedule

    // Moon phase is a day of the week


    public enum PartOfDay { Morning, Afternoon, Evening, Night }
    static public GameObject Clock;
    public float CLOCK_SPEED = 0.5f;

    public Sprite[] moonCycleSprites = new Sprite[5];
    public Sprite[] timeOfDay = new Sprite[4];
    public Image moonPhase;
    public Image timeImage;
    int currentMoonPhase = 0;

    int days;
    int hour;
    int minutes;
    PartOfDay dayPart;

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

    public int Days {
        get {
            return days;
        }

        set {
            days = value;
        }
    }

    public int CurrentMoonPhase {
        get {
            return currentMoonPhase;
        }

        set {
            currentMoonPhase = value;
        }
    }

    public PartOfDay DayPart {
        get {
            return dayPart;
        }

        set {
            dayPart = value;
        }
    }


    // Use this for initialization
    void Start () {
        Hour = 6;
        Minutes = 00;
        moonPhase.sprite = moonCycleSprites[0];
        StartCoroutine(PassingTime());
        Days = 0;
        DayPart = PartOfDay.Morning;
    }

    public void Awake() {
        DontDestroyOnLoad(this.transform.parent);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    IEnumerator PassingTime()
    {
        //Every 2 mins real time is 10 mins game time - 120 for actual time scale
        yield return new WaitForSeconds(CLOCK_SPEED);
        ChangeTime();
        StartCoroutine(PassingTime());
    }

    void ChangeTime()
    {
        Minutes = Minutes + 10;

        switch (Hour) {
            case 6:
                timeImage.sprite = timeOfDay[0];
                DayPart = PartOfDay.Morning;
                break;
            case 12:
                timeImage.sprite = timeOfDay[1];
                DayPart = PartOfDay.Afternoon;
                break;
            case 17:
                timeImage.sprite = timeOfDay[2];
                DayPart = PartOfDay.Evening;
                break;
            case 20:
                timeImage.sprite = timeOfDay[3];
                DayPart = PartOfDay.Night;
                break;
            default:
                break;
        }

        if (Hour == 23 && Minutes == 60)
        {
            currentMoonPhase = (currentMoonPhase + 1) % 6;
            moonPhase.sprite = moonCycleSprites[currentMoonPhase];
            days++;
            GameObject.FindObjectOfType<NPCController>().SetQueue(days);
        }

        if (Minutes == 60)
        {
            Hour = (Hour + 1) % 24;
            Minutes = 00;

        }
    }


}
