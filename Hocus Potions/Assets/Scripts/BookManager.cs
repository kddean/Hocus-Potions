﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class BookManager : MonoBehaviour {

    ResourceLoader rl;

    public GameObject BookCanvas;
    public GameObject PlantTab;
    public GameObject PotionTab;
    public GameObject MapTab;
    public string CurrentTab;
    public bool PageUp = false;

    public GameObject CurrentPage;
    public GameObject KeyPage;
    public GameObject ButtonPrefab;
    public GameObject PanelPrefab;
    public GameObject TextBox;

    public void Awake()
    {
        
        DontDestroyOnLoad(this);
        if (Resources.FindObjectsOfTypeAll(GetType()).Length > 1)
        {
            Debug.Log(this.name);
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();

        BookCanvas = GameObject.Find("BookCanvas");
        PlantTab = GameObject.Find("PlantTab");
        PotionTab = GameObject.Find("PotionTab");
        MapTab = GameObject.Find("MapTab");

        BookCanvas.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("m"))
        {
            if (BookCanvas.activeSelf == false)
            {
                BookCanvas.SetActive(true);
            }
            else if (BookCanvas.activeSelf == true)
            {
                BookCanvas.SetActive(false);
            }
        }
	}

    public void SetCurrentTab(int i)
    {
        if (i == 0)
        {
            if(PageUp == true && CurrentTab == "PlantTab")
            {
                return;
            }
            else if (CurrentTab != "")
            {
                Destroy(GameObject.Find("CurrentPage"));
                Destroy(GameObject.Find("KeyPage"));
            }
            Vector3 temp = PlantTab.transform.localPosition;
            temp.x *= -1;
            PlantTab.transform.localPosition = temp;
            PlantTab.transform.localScale *= -1;
            CurrentTab = "PlantTab";
            SetUpPage(i);
        }
        else if (i == 1)
        {
            if (PageUp == true && CurrentTab == "PotionTab")
            {
                return;
            }
            else if (CurrentTab != "")
            {
                DestroyImmediate(GameObject.Find("CurrentPage(Clone)"));
                DestroyImmediate(GameObject.Find("KeyPage(Clone)"));
            }
            Vector3 temp = PotionTab.transform.localPosition;
            temp.x *= -1;
            PotionTab.transform.localPosition = temp;
            PotionTab.transform.localScale *= -1;
            CurrentTab = "PotionTab";
        }
        else if (i == 2)
        {
            if (PageUp == true && CurrentTab == "MapTab")
            {
                return;
            }else if(CurrentTab != "")
            {
                Destroy(GameObject.Find("CurrentPage"));
                Destroy(GameObject.Find("KeyPage"));
            }
            Vector3 temp = MapTab.transform.localPosition;
            temp.x *= -1;
            MapTab.transform.localPosition = temp;
            MapTab.transform.localScale *= -1;
            CurrentTab = "MapTab";
        }
    }

    public void SetUpPage(int i)
    {
        GameObject BookBackground = BookCanvas.GetComponentInChildren<Image>().gameObject;
        GameObject newPage = GameObject.Instantiate(CurrentPage);
        newPage.transform.SetParent(BookBackground.transform);
        newPage.transform.position = BookBackground.transform.position;
        Debug.Log("Added newPage");
        List<string> keys = rl.ingredients.Keys.ToList(); 
        
        foreach (string key in keys)
        {
            GameObject button = Instantiate(ButtonPrefab);
            button.transform.SetParent(newPage.transform);
            button.transform.position = newPage.transform.position;
            button.GetComponentInChildren<Text>().text = key;
            button.GetComponent<Button>().onClick.AddListener(() => PassName(button));
            
        }
        PageUp = true;
    }

    public void SetUpKeyPage(string name)
    {
        Vector3 temp;
        GameObject BookBackground = BookCanvas.GetComponentInChildren<Image>().gameObject;
        GameObject newPage = GameObject.Instantiate(KeyPage);
        newPage.transform.SetParent(BookBackground.transform);
        newPage.transform.position = BookBackground.transform.position;
        temp = newPage.transform.position;
        temp.x += 250;
        temp.y -= 250;
        newPage.transform.position = temp;
        GameObject panel = GameObject.Instantiate(PanelPrefab);
        GameObject textBox = GameObject.Instantiate(TextBox);

        panel.transform.SetParent(newPage.transform);
        panel.transform.position = newPage.transform.position;
        temp = panel.transform.position;
        temp.x += 160;
        temp.y += 500;
        panel.transform.position = temp;
        panel.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>(rl.ingredients[name].imagePath);

        textBox.transform.SetParent(newPage.transform);
        textBox.transform.position = newPage.transform.position;
        temp = textBox.transform.localPosition;
        temp.x += 160;
        temp.y += 160;
        textBox.GetComponent<RectTransform>().transform.localPosition = temp;
        textBox.GetComponent<Text>().text = "Test";
        
    }

    public void PassName(GameObject b)
    {
        SetUpKeyPage(b.GetComponentInChildren<Text>().text);
    }
}
