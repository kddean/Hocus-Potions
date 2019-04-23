using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Signpost : MonoBehaviour {

    GameObject toolTip;
    Text text;
    bool hovered = false;
    string signE;
    string signNE;
    string signW;

	// Use this for initialization
	void Start () {
        toolTip = GameObject.Find("SignTooltip");

        signE = "North - Mountains" + "\n" + "West - Forest" + "\n" + "East - Campsite & Meadow";
        signNE = "West - Mountains" + "\n" + "South - Campsite & Meadow";
        signW = "North - Mountains & Shrine" + "\n" + "West - Forest" + "\n" + "South - Meadow";
    }
	
	// Update is called once per frame
	void Update () {
        if (hovered)
        {
            toolTip.transform.position = Input.mousePosition + new Vector3(200, -50, 0);
        }
    }

    private void OnMouseEnter()
    {
        DisplayTooltip();
    }

    void DisplayTooltip()
    {
        text = toolTip.GetComponentInChildren<Text>();
        //text.text = "404: Sanity Not Found";
            //"Up: Mountains";
        //text[1].text = "Left: Campsite";
        //text[2].text = "Right: Forest";
        toolTip.GetComponent<CanvasGroup>().alpha = 1;
        hovered = true;

        if (gameObject.name.Equals("Sign E"))
        {
            text.text = signE;
        }
        else if (gameObject.name.Equals("Sign NE"))
        {
            text.text = signNE;
        }
        else if (gameObject.name.Equals("Sign W"))
        {
            text.text = signW;
        }
    }

    private void OnMouseExit()
    {
        toolTip.GetComponent<CanvasGroup>().alpha = 0;
        hovered = false;
    }
}
