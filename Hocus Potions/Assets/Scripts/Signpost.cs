using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Signpost : MonoBehaviour {

    GameObject toolTip;
    Text text;
    bool hovered = false;

	// Use this for initialization
	void Start () {
        toolTip = GameObject.Find("SignTooltip");
	}
	
	// Update is called once per frame
	void Update () {
        if (hovered)
        {
            toolTip.transform.position = Input.mousePosition + new Vector3(30, -50, 0);
        }
    }

    private void OnMouseEnter()
    {
        DisplayTooltip();
    }

    void DisplayTooltip()
    {
        text = toolTip.GetComponentInChildren<Text>();
        text.text = "404: Sanity Not Found";
            //"Up: Mountains";
        //text[1].text = "Left: Campsite";
        //text[2].text = "Right: Forest";
        toolTip.GetComponent<CanvasGroup>().alpha = 1;
        hovered = true;
    }

    private void OnMouseExit()
    {
        toolTip.GetComponent<CanvasGroup>().alpha = 0;
        hovered = false;
    }
}
