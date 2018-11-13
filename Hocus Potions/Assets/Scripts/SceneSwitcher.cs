using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    public Scene house;
    public Scene garden;
    public Scene world;
    Scene currentScene;
    public GameObject player;
    public bool enter = true;
    public string portalTo;


	// Use this for initialization
	void Start () {
        currentScene = SceneManager.GetActiveScene();
        house = SceneManager.GetSceneByName("House");
        garden = SceneManager.GetSceneByName("Garden");
        world = SceneManager.GetSceneByName("SampleGameArea");
	}

    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D collision) {
        if (currentScene == house) {
            if (portalTo == "garden") {
                SceneManager.LoadScene("Garden");
                currentScene = SceneManager.GetActiveScene();
            } else if (portalTo == "world") {
                SceneManager.LoadScene("SampleGameArea");
                currentScene = SceneManager.GetActiveScene();
            }
        }

        if (currentScene == garden) {
            if (portalTo == "house") {
                SceneManager.LoadScene("House");
                currentScene = SceneManager.GetActiveScene();
            } else if (portalTo == "world") {
                SceneManager.LoadScene("SampleGameArea");
                currentScene = SceneManager.GetActiveScene();
            }
        }

        if (currentScene == world) {
            if (portalTo == "house") {
                SceneManager.LoadScene("House");
                currentScene = SceneManager.GetActiveScene();
            }

        }

        GameObject.Find("GarbageCollector").GetComponent<GarbageCollecter>().CallSpawner();
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (currentScene == house)
        {
            if (portalTo == "garden")
            {
                SceneManager.LoadScene("Garden");
                currentScene = SceneManager.GetActiveScene();
            }
            else if (portalTo == "world")
            {
                SceneManager.LoadScene("SampleGameArea");
                currentScene = SceneManager.GetActiveScene();
            }
        }

        if (currentScene == garden)
        {
            if (portalTo == "house")
            {
                SceneManager.LoadScene("House");
                currentScene = SceneManager.GetActiveScene();
            }
            else if (portalTo == "world")
            {
                SceneManager.LoadScene("SampleGameArea");
                currentScene = SceneManager.GetActiveScene();
            }
        }

        if (currentScene == world)
        {
            if (portalTo == "house")
            {
                SceneManager.LoadScene("House");
                currentScene = SceneManager.GetActiveScene();
            }
        }
    }*/

}
