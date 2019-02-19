using System.Collections;
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
        //CurrentPage = GameObject.Find("CurrentPage");

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
            Vector3 temp = PlantTab.transform.localPosition;
            temp.x *= -1;
            PlantTab.transform.localPosition = temp;
            PlantTab.transform.localScale *= -1;
            CurrentTab = "PlantTab";
            SetUpPage(i);
        }
        else if (i == 1)
        {
            Vector3 temp = PotionTab.transform.localPosition;
            temp.x *= -1;
            PotionTab.transform.localPosition = temp;
            PotionTab.transform.localScale *= -1;
            CurrentTab = "PotionTab";
        }
        else if (i == 2)
        {
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
            button.GetComponent<Button>().onClick.AddListener(PassName);
            
        }
    }

    public void SetUpKeyPage(string name)
    {
        GameObject BookBackground = BookCanvas.GetComponentInChildren<Image>().gameObject;
        GameObject newPage = GameObject.Instantiate(KeyPage);
        newPage.transform.SetParent(BookBackground.transform);
        newPage.transform.position = BookBackground.transform.position;

        GameObject panel = GameObject.Instantiate(PanelPrefab);
        GameObject textBox = GameObject.Instantiate(TextBox);

        panel.transform.SetParent(newPage.transform);
        panel.transform.position = newPage.transform.position;
        panel.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(rl.ingredients[name].imagePath);
    }

    public void PassName()
    {
        SetUpKeyPage(gameObject.name);
    }
}
