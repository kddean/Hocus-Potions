using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class BookManager : MonoBehaviour {

    ResourceLoader rl;
    Dictionary<string, string> plantInfo;
    Dictionary<string, Potion> potionInfo;
    public Dictionary<string, bool> potionDiscovery;
    //Dictionary<string, string> mapInfo;

    public GameObject BookCanvas;
    public GameObject PlantTab;
    public GameObject PotionTab;
    public GameObject MapTab;
    public string CurrentTab;
    public bool PageUp = false;
    public GameObject Tab;

    public GameObject CurrentPage;
    public GameObject KeyPage;
    public GameObject ButtonPrefab;
    public GameObject PanelPrefab;
    public GameObject TextBox;
    public GameObject[] gameObjects;
    public List<string> keys;
    public GameObject Test;
    public GameObject content;
    public GameObject button;

    public GameObject PlantPage;
    public GameObject PotionPage;
    public GameObject MapPage;
    public GameObject[] contents;

    public GameObject ForestZone;
    public GameObject HomeZone;
    public GameObject MeadowZone;
    public GameObject CampsiteZone;
    public GameObject ShrineZone;
    public GameObject MountainsZone;
    public GameObject CurrentZone;
    public GameObject WitchIcon;


    public bool dictExists;

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
    void Start() {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        dictExists = false;
        BookCanvas = GameObject.Find("BookCanvas");
        PlantTab = GameObject.Find("PlantTab");
        PotionTab = GameObject.Find("PotionTab");
        MapTab = GameObject.Find("MapTab");
        plantInfo = new Dictionary<string, string>();
        potionInfo = new Dictionary<string, Potion>();
        potionDiscovery = new Dictionary<string, bool>();
        content = new GameObject();
        contents = GameObject.FindGameObjectsWithTag("contentWindow");
        WitchIcon = GameObject.Find("WitchIcon");
        //CurrentPage = GameObject.Find("CurrentPage");
        CreateDictionary();

        SetUpPlantPage();
        SetUpPotionPage();

        CurrentZone = null;
        MapPage = GameObject.Find("MapPage");
        PlantPage.SetActive(false);
        PotionPage.SetActive(false);
        MapPage.SetActive(false);
        Tab.SetActive(false);
        dictExists = true;
        BookCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
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
        if (BookCanvas.activeSelf == true)
        {
            contents = GameObject.FindGameObjectsWithTag("contentWindow");
            SetUpMapPage();
           
            
        }

        if (!SceneManager.GetActiveScene().name.Equals("SampleGameArea"))
        {
            return;
        }
        else
        {
            ForestZone = GameObject.Find("ForestZone");
            HomeZone = GameObject.Find("HomeZone");
            MeadowZone = GameObject.Find("MeadowZone");
            CampsiteZone = GameObject.Find("CampsiteZone");
            ShrineZone = GameObject.Find("ShrineZone");
            MountainsZone = GameObject.Find("MountainsZone");
}
    }
    public void SetCurrentTab(int i)
    {
        if (i == 0)
        {
            if (PageUp == true && CurrentTab == "PlantTab")
            {
                return;
            }
            else if (PageUp == true && CurrentTab != "PlantTab")
            {
                PageUp = false;
                /*Vector3 t = Tab.transform.localPosition;
                t.x *= -1;
                Tab.transform.localPosition = t;
                Tab.transform.localScale *= -1;*/
                PotionPage.SetActive(false);
                MapPage.SetActive(false);

                GameObject page = GameObject.Find("KeyPage(Clone)");
                Destroy(page);
                Vector3 pt;
                if (CurrentTab == "PotionTab")
                {
                    pt = PotionTab.transform.localPosition;
                    pt.x *= -1;
                    PotionTab.transform.localPosition = pt;
                    PotionTab.transform.localScale *= -1;
                }
                else if (CurrentTab == "MapTab")
                {
                    pt = MapTab.transform.localPosition;
                    pt.x *= -1;
                    MapTab.transform.localPosition = pt;
                    MapTab.transform.localScale *= -1;
                }
            }
            Vector3 temp = PlantTab.transform.localPosition;
            temp.x *= -1;
            PlantTab.transform.localPosition = temp;
            PlantTab.transform.localScale *= -1;
            CurrentTab = "PlantTab";
            PlantPage.SetActive(true);
            PageUp = true;
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
                /*Vector3 t = GameObject.Find(CurrentTab).transform.localPosition;
                t.x *= -1;
                Tab.transform.localPosition = t;
                Tab.transform.localScale *= -1;*/
                PlantPage.SetActive(false);
                MapPage.SetActive(false);

                GameObject page = GameObject.Find("KeyPage(Clone)");
                Destroy(page);

                Vector3 pt;
                if (CurrentTab == "PlantTab")
                {
                    pt = PlantTab.transform.localPosition;
                    pt.x *= -1;
                    PlantTab.transform.localPosition = pt;
                    PlantTab.transform.localScale *= -1;
                }
                else if (CurrentTab == "MapTab")
                {
                    pt = MapTab.transform.localPosition;
                    pt.x *= -1;
                    MapTab.transform.localPosition = pt;
                    MapTab.transform.localScale *= -1;
                }
            }

            Vector3 temp = PotionTab.transform.localPosition;
            temp.x *= -1;
            PotionTab.transform.localPosition = temp;
            PotionTab.transform.localScale *= -1;
            CurrentTab = "PotionTab";
            PotionPage.SetActive(true);
            PageUp = true;
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
                /*Vector3 t = Tab.transform.localPosition;
                t.x *= -1;
                Tab.transform.localPosition = t;
                Tab.transform.localScale *= -1;*/

                PotionPage.SetActive(false);
                PlantPage.SetActive(false);
                GameObject page = GameObject.Find("KeyPage(Clone)");
                Destroy(page);

                Vector3 pt;
                if (CurrentTab == "PlantTab")
                {
                    pt = PlantTab.transform.localPosition;
                    pt.x *= -1;
                    PlantTab.transform.localPosition = pt;
                    PlantTab.transform.localScale *= -1;
                }
                else if (CurrentTab == "PotionTab")
                {
                    pt = PotionTab.transform.localPosition;
                    pt.x *= -1;
                    PotionTab.transform.localPosition = pt;
                    PotionTab.transform.localScale *= -1;                    
                }
            }

            Vector3 temp = MapTab.transform.localPosition;
            temp.x *= -1;
            MapTab.transform.localPosition = temp;
            MapTab.transform.localScale *= -1;
            CurrentTab = "MapTab";
            PageUp = true;
            MapPage.SetActive(true);
            SetUpMapPage();

        }
    }


        /*public void SetCurrentTab(int i)
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
                        Destroy(gameObjects[j]);
                        gameObjects[j] = null;
                    }

                }
                //StartCoroutine(FindContent());
                SetUpPlantPage(i);
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

                        Destroy(gameObjects[j]);
                        gameObjects[j] = null;

                    }

                }
                //StartCoroutine(FindContent());
                SetUpPotionPage(i);
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
                PageUp = true;
                //SetUpPage(i);

            }
        }*/

        public void SetUpPlantPage()
        {
            Vector3 temp;
            GameObject BookBackground = BookCanvas.GetComponentInChildren<Image>().gameObject;
       
            GameObject newPage = GameObject.Find("PlantPage");
            newPage.transform.SetParent(BookBackground.transform);
            newPage.transform.position = BookBackground.transform.position;
            temp = newPage.transform.position;
            temp.x -= 300;
           
            newPage.transform.position = temp;
            newPage.gameObject.name = "PlantPage";
            PlantPage = newPage;
            Debug.Log("Added PlantPage");
            keys = rl.ingredients.Keys.ToList();

            
            content = GameObject.Find("PlantContent");
            PageUp = true;

           

            foreach (string key in keys)
            {
                GameObject button = Instantiate(ButtonPrefab);
              
                button.transform.SetParent(content.transform);
                button.transform.position = content.transform.position;
                button.GetComponentInChildren<Text>().text = key;
                button.GetComponent<Button>().onClick.AddListener(() => PassName(button));
                button.gameObject.name = key;

            }

        }

       /* public void SetUpPlantPage(int i)
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
            newPage.gameObject.name = "PlantPage";
            PlantPage = newPage;
            Debug.Log("Added PlantPage");
            keys = rl.ingredients.Keys.ToList();

            //ScrollRect viewpoint = Test.GetComponent<ScrollRect>();
            content = GameObject.FindGameObjectWithTag("contentWindow");
            PageUp = true;

            /*if (content.transform.childCount != 0)
            {
                for (int k = 0; k < content.transform.childCount-1; k++)
                {
                    Destroy(transform.GetChild(k).gameObject);
                }
            }

            foreach (string key in keys)
            {
                GameObject button = Instantiate(ButtonPrefab);
                //button.transform.SetParent(newPage.transform);
                button.transform.SetParent(content.transform);
                button.transform.position = content.transform.position;
                button.GetComponentInChildren<Text>().text = key;
                button.GetComponent<Button>().onClick.AddListener(() => PassName(button));

            }

        }*/

        /*public void SetUpPotionPage(int i)
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
            Debug.Log("Added PotionPage");

            content = GameObject.FindGameObjectWithTag("contentWindow");
            PageUp = true;

            /* if (content.transform.childCount != 0)
             {
                 for (int k = 0; k < content.transform.childCount-1; k++)
                 {
                     Destroy(transform.GetChild(k).gameObject);
                 }
             }


            foreach (string key in potionInfo.Keys)
            {
                GameObject button = Instantiate(ButtonPrefab);

                button.transform.SetParent(content.transform);
                button.transform.position = content.transform.position;

                button.GetComponent<Button>().onClick.AddListener(() => PassName(button));

                Image[] sprites = button.GetComponentsInChildren<Image>();

                if (potionDiscovery[key] == true)
                {
                    button.GetComponent<Button>().interactable = true;
                    sprites[0].sprite = Resources.Load<Sprite>(potionInfo[key].imagePath);
                    button.GetComponentInChildren<Text>().text = key;

                }
                else
                {
                    button.GetComponentInChildren<Text>().text = "???";
                    button.GetComponent<Button>().interactable = false;
                    sprites[0].sprite = Resources.Load<Sprite>("Potions/potions_mystical");
                    sprites[0].color = Color.black;

                }

            }
        }*/

        public void SetUpPotionPage()
        {
            Vector3 temp;
            GameObject BookBackground = BookCanvas.GetComponentInChildren<Image>().gameObject;
        
            GameObject newPage = GameObject.Find("PotionPage");
            newPage.transform.SetParent(BookBackground.transform);
            newPage.transform.position = BookBackground.transform.position;
            temp = newPage.transform.position;
            temp.x -= 300;
            
            newPage.transform.position = temp;
            newPage.gameObject.name = "PotionPage";
            PotionPage = newPage;
            Debug.Log("Added PotionPage");

            content = GameObject.Find("PotionContent");
            PageUp = true;


            foreach (string key in potionInfo.Keys)
            {
                GameObject button = Instantiate(ButtonPrefab);

                button.transform.SetParent(content.transform);
                button.transform.position = content.transform.position;

                button.GetComponent<Button>().onClick.AddListener(() => PassName(button));
                button.gameObject.name = key;

                Image[] sprites = button.GetComponentsInChildren<Image>();

                if (potionDiscovery[key] == true)
                {
                    button.GetComponent<Button>().interactable = true;
                    sprites[0].sprite = Resources.Load<Sprite>(potionInfo[key].imagePath);
                    button.GetComponentInChildren<Text>().text = key;

                }
                else
                {
                    button.GetComponentInChildren<Text>().text = "???";
                    button.GetComponent<Button>().interactable = false;
                    sprites[0].sprite = Resources.Load<Sprite>("Potions/potions_mystical");
                    sprites[0].color = Color.black;

                }                

            }
        }

        public void SetUpKeyPage(string name)
        {
            GameObject page = GameObject.Find("KeyPage(Clone)");
            Destroy(page);

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

            if (CurrentTab == "PlantTab")
            {


                sprites[1].sprite = Resources.Load<Sprite>(rl.ingredients[name].imagePath);
            Debug.Log(name);
            textBox.
                    GetComponentInChildren<Text>().text =                   
                        plantInfo[name];
            

            GameObject attributes = Instantiate(TextBox);
            attributes.transform.SetParent(newPage.transform);
            attributes.transform.position = newPage.transform.position;
            temp = attributes.transform.position;
            temp.x += 190;
            temp.y += 240;

            attributes.transform.position = temp;

            List<Ingredient.Attributes> list = rl.knownAttributes[rl.ingredients[name]];

             //attributes.GetComponentInChildren<Text>().GetComponent<Text>().text = list.ToString();
            
            foreach (Ingredient.Attributes a in list)
            {
                attributes.GetComponentInChildren<Text>().GetComponent<Text>().text = attributes.GetComponentInChildren<Text>().GetComponent<Text>().text + " " + "\n" + a.ToString();
                attributes.GetComponent<Text>().fontSize = 12;
            }

            }
            else if (CurrentTab == "PotionTab")
            {
                sprites[1].sprite = Resources.Load<Sprite>(potionInfo[name].imagePath);
                textBox.
                    GetComponentInChildren<Text>().text =
                    potionInfo[name].description;
            }
            else if (CurrentTab == "MapTab")
            {

            }

        }

    public void SetUpMapPage()
    {
        /*Forest -222.9, 16
         *Mountains -86.5, 240
         * Home -55, 67
         * Shrine 226.4, 265
         * Campsite 145.3, 79
         * Meadow 145.3, -111
        */
        if(CurrentZone == null)
        {
            return;
        }

        if(CurrentZone.name == ForestZone.name)
        {
            WitchIcon.transform.localPosition = new Vector2(-222.9f, 16f);
        }
        else if (CurrentZone.name == MountainsZone.name)
        {
            WitchIcon.transform.localPosition = new Vector2(-86.5f, 240f);
        }
        else if (CurrentZone.name == HomeZone.name)
        {
            WitchIcon.transform.localPosition = new Vector2(-55f, 67f);
        }
        else if (CurrentZone.name == ShrineZone.name)
        {
            WitchIcon.transform.localPosition = new Vector2(226.4f, 265f);
        }
        else if (CurrentZone.name == CampsiteZone.name)
        {
            WitchIcon.transform.localPosition = new Vector2(-145.3f, 79f);
        }
        else if (CurrentZone.name == MeadowZone.name)
        {
            WitchIcon.transform.localPosition = new Vector2(145.3f, -111f);
        }
    }

    public void UpdateUnlockedPotions()
    {
        BookCanvas.SetActive(true);
        PotionPage.SetActive(true);
        foreach (string key in potionInfo.Keys)
        {

            button = GameObject.Find(key);
           
            Image[] sprites = button.GetComponentsInChildren<Image>();

            if (potionDiscovery[key] == true)
            {
                button.GetComponent<Button>().interactable = true;
                sprites[0].sprite = Resources.Load<Sprite>(potionInfo[key].imagePath);
                button.GetComponentInChildren<Text>().text = key;
                sprites[0].color = Color.white;

            }
            else
            {
                button.GetComponentInChildren<Text>().text = "???";
                button.GetComponent<Button>().interactable = false;
                sprites[0].sprite = Resources.Load<Sprite>("Potions/potions_mystical");
                sprites[0].color = Color.black;

            }

        }
        PotionPage.SetActive(false);
        BookCanvas.SetActive(false);
    }
    public void UpdateAttributes()
    {
        BookCanvas.SetActive(true);
        PlantPage.SetActive(true);
        foreach (string key in keys)
        {
            GameObject button = GameObject.Find(key);

           
            button.GetComponentInChildren<Text>().text = key;
            

        }
        PlantPage.SetActive(true);
        BookCanvas.SetActive(true);
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
        plantInfo.Add("lambsgrass", "A flower in the shape of a bell. It is said that those who sit in a field of them will hear bells.");
        plantInfo.Add("poppy", "A short flower that grow in groups and a variety of colors. It is often used in cakes and other sweets.");
        plantInfo.Add("thistle", "");
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

        potionInfo.Add("Sleeping", new Potion("Sleeping", "Potions/potions_sleep", 0, "A potion that puts the user the sleep"));
        potionInfo.Add("Healing", new Potion("Healing", "Potions/potions_healing", 0, "Restores the user’s health"));
        potionInfo.Add("Invisibility", new Potion("Invisibility", "Potions/potions_invisibility", 0, "Turns the user invisible"));
        potionInfo.Add("Mana", new Potion("Mana", "Potions/potions_mana", 0, "Restores the user’s Mana"));
        potionInfo.Add("Poison", new Potion("Poison", "Potions/potions_poison", 0, "Poisons the user "));
        potionInfo.Add("Transformation", new Potion("Transformation", "Potions/potions_transform", 0, "Turns the user into a cat"));
        potionInfo.Add("Odd", new Potion("Null", "Potions/potions_null", 0, "A potion with a variety of qualities, but no defining ones"));
        potionInfo.Add("Speed", new Potion("Speed", "Potions/potions_speed", 0, "Makes one go fast"));

        potionDiscovery.Add("Sleeping", false);
        potionDiscovery.Add("Healing", false);
        potionDiscovery.Add("Invisibility", false);
        potionDiscovery.Add("Mana", false);
        potionDiscovery.Add("Poison", false);
        potionDiscovery.Add("Transformation", false);
        potionDiscovery.Add("Odd", false);
        potionDiscovery.Add("Speed", false);

    }

        IEnumerator FindContent()
        {
            /*buttons = content.transform.GetComponentsInChildren<Transform>();
            for(int i = 1; i < buttons.Length; i++)
            {
                Destroy(buttons[i]);
            }*/

            for (int i = 0; i < content.transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            yield return new WaitForSecondsRealtime(10f);
        }
    }
