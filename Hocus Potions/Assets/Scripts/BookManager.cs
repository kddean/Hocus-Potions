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
        //CurrentPage = GameObject.Find("CurrentPage");

        BookCanvas.SetActive(false);
        Debug.Log("Off");
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
        Vector3 temp;
        GameObject BookBackground = BookCanvas.GetComponentInChildren<Image>().gameObject;
        GameObject newPage = GameObject.Instantiate(CurrentPage);
        newPage.transform.SetParent(BookBackground.transform);
        newPage.transform.position = BookBackground.transform.position;
        temp = newPage.transform.position;
        temp.x -= 300;
        //temp.y -= 250;
        newPage.transform.position = temp;
        Debug.Log("Added newPage");
        List<string> keys = rl.ingredients.Keys.ToList();

        ScrollRect viewpoint = newPage.GetComponent<ScrollRect>();
        GameObject content = GameObject.FindGameObjectWithTag("contentWindow");
        
        foreach (string key in keys)
        {
            GameObject button = Instantiate(ButtonPrefab);
            //button.transform.SetParent(newPage.transform);
            button.transform.SetParent(content.transform);
            button.transform.position = content.transform.position;
            button.GetComponentInChildren<Text>().text = key;
            button.GetComponent<Button>().onClick.AddListener(() => PassName(button));
            
        }
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
        //temp = panel.transform.position;
        //temp.x += 50;
        panel.transform.position = temp;
        Image[] sprites = panel.GetComponentsInChildren<Image>();
            sprites[1].sprite = Resources.Load<Sprite>(rl.ingredients[name].imagePath);

        
    }

    public void PassName(GameObject b)
    {
        SetUpKeyPage(b.GetComponentInChildren<Text>().text);
    }
}
