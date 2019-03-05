using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class BookManager : MonoBehaviour {

    ResourceLoader rl;
    Dictionary<string, string> plantInfo;
    Dictionary<string, string> potionInfo;
    //Dictionary<string, string> mapInfo;

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
    public GameObject[] gameObjects;
    public List<string> keys;
    public GameObject Test;

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
        plantInfo = new Dictionary<string, string>();
        potionInfo = new Dictionary<string, string>();
        //CurrentPage = GameObject.Find("CurrentPage");
        CreateDictionary();

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
            else if (PageUp == true && CurrentTab != "PlantTab"){
                PageUp = false;
                Vector3 t = GameObject.Find(CurrentTab).transform.localPosition;
                t.x *= -1;
                GameObject.Find(CurrentTab).transform.localPosition = t;
                GameObject.Find(CurrentTab).transform.localScale *= -1;
            }
            Vector3 temp = PlantTab.transform.localPosition;
            temp.x *= -1;
            PlantTab.transform.localPosition = temp;
            PlantTab.transform.localScale *= -1;
            CurrentTab = "PlantTab";
            gameObjects = GameObject.FindGameObjectsWithTag("page");
            Debug.Log(gameObjects.Length);
            if (gameObjects.Length != 0)
            {
                for(int j = 0; j < gameObjects.Length; j++)
                {
                    //Debug.Log(gameObjects[j].name);
                    //Destroy(gameObjects[j]);               
                }

            }
            SetUpPage(i);
        }
        else if (i == 1)
        {
            if (PageUp == true && CurrentTab == "PotionTab")
            {
                return;
            }
            else if (PageUp == true && CurrentTab != "PotionTab")
            {
                PageUp = false;
                Vector3 t = GameObject.Find(CurrentTab).transform.localPosition;
                t.x *= -1;
                GameObject.Find(CurrentTab).transform.localPosition = t;
                GameObject.Find(CurrentTab).transform.localScale *= -1;
            }

            Vector3 temp = PotionTab.transform.localPosition;
            temp.x *= -1;
            PotionTab.transform.localPosition = temp;
            PotionTab.transform.localScale *= -1;
            CurrentTab = "PotionTab";
            gameObjects = GameObject.FindGameObjectsWithTag("page");
            Debug.Log(gameObjects.Length);
            if (gameObjects.Length != 0)
            {
                for (int j = 0; j < gameObjects.Length; j++)
                {
                    //Debug.Log(gameObjects[j].name);
                    //Destroy(gameObjects[j]);                   
                }

            }
            //SetUpPage(i);
        }
        else if (i == 2)
        {
            if (PageUp == true && CurrentTab == "MapTab")
            {
                return;
            }
            else if (PageUp == true && CurrentTab != "MapTab")
            {
                PageUp = false;
                Vector3 t = GameObject.Find(CurrentTab).transform.localPosition;
                t.x *= -1;
                GameObject.Find(CurrentTab).transform.localPosition = t;
                GameObject.Find(CurrentTab).transform.localScale *= -1;
            }
            Vector3 temp = MapTab.transform.localPosition;
            temp.x *= -1;
            MapTab.transform.localPosition = temp;
            MapTab.transform.localScale *= -1;
            CurrentTab = "MapTab";
            gameObjects = GameObject.FindGameObjectsWithTag("page");
            Debug.Log(gameObjects.Length);
            if (gameObjects.Length != 0)
            {
                for (int j = 0; j < gameObjects.Length; j++)
                {
                    Debug.Log(gameObjects[j].name);
                    Destroy(gameObjects[j]);

                }

            }
            SetUpPage(i);

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
        keys = rl.ingredients.Keys.ToList();

        //ScrollRect viewpoint = Test.GetComponent<ScrollRect>();
        GameObject content = GameObject.FindGameObjectWithTag("contentWindow");
        PageUp = true;
        
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
        temp = panel.transform.position;
        temp.x += 140;
        temp.y += 500;
        panel.transform.position = temp;
        textBox.transform.SetParent(newPage.transform);
        textBox.transform.position = newPage.transform.position;
        temp = textBox.transform.position;
        temp.x += 190;
        
        textBox.transform.position = temp;

        
        Image[] sprites = panel.GetComponentsInChildren<Image>();
            sprites[1].sprite = Resources.Load<Sprite>(rl.ingredients[name].imagePath);

        textBox.
            GetComponentInChildren<Text>().text = 
            plantInfo[name];

        
    }

    public void PassName(GameObject b)
    {
        SetUpKeyPage(b.GetComponentInChildren<Text>().text);
    }

    void CreateDictionary()
    {
        plantInfo.Add("lavender", "A plant known for its purple flowers. It is often picked a bushels and hung over beds in the hopes it will bring sweet dreams.");
        plantInfo.Add("catnip", "A flower with ink blossoms. It was said that cats would trade one of their lives for some of this plant.");
        plantInfo.Add("nightshade", "A dark purple flower famous for its poison. While it’s poison can be used to hurt it can as well be used for healing");
        plantInfo.Add("mugwort", "A bushy white flower. It often grows from the mud.");
        plantInfo.Add("lambgrass", "A flower in the shape of a bell. It is said that those who sit in a field of them will hear bells.");
        plantInfo.Add("poppy", "A short flower that grow in groups and a variety of colors. It is often used in cakes and other sweets.");
        plantInfo.Add("thistle ", "");
        plantInfo.Add("lily", "A very large plant with a beautiful flower, unfortunately it smells of death and decay.");
        plantInfo.Add("indigo", "");
        plantInfo.Add("dandylion", "");
        plantInfo.Add("ghostcap", "");
        plantInfo.Add("morel", "");
        plantInfo.Add("fly_agaric", "");
        plantInfo.Add("ash", "");
        plantInfo.Add("amethyst", "");
        plantInfo.Add("selenite", "");
        plantInfo.Add("lapis_lazuli", "");
        plantInfo.Add("emerald", "");
        plantInfo.Add("amber", "");
        plantInfo.Add("garnet", "");
        plantInfo.Add("jet", "");

        potionInfo.Add("Sleeping", "A potion that puts the user the sleep");
        potionInfo.Add("Healing", "Restores the user’s health");
        potionInfo.Add("Invisibility", "Turns the user invisible");
        potionInfo.Add("Mana", "Restores the user’s Mana");
        potionInfo.Add("Poison", "Poisons the user ");
        potionInfo.Add("Transformation", "Turns the user into a cat");
        potionInfo.Add("Odd", "A potion with a variety of qualities, but no defining ones");
    }
}
