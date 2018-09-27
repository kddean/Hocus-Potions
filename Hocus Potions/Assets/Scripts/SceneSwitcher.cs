using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    public Scene house;
    public Scene garden;
    Scene currentScene;
    public GameObject player;
    public bool enter = true;


	// Use this for initialization
	void Start () {
        currentScene = SceneManager.GetActiveScene();
        house = SceneManager.GetSceneByName("House");
        garden = SceneManager.GetSceneByName("Garden");
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Me?");
        if(currentScene == house)
        {
            SceneManager.LoadScene("Garden");
            currentScene = SceneManager.GetActiveScene();
        }

        if(currentScene == garden)
        {
            SceneManager.LoadScene("House");
            currentScene = SceneManager.GetActiveScene();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Or Me?");
        if (currentScene == house)
        {
            SceneManager.LoadScene("Garden");
            currentScene = SceneManager.GetActiveScene();
        }

        if (currentScene == garden)
        {
            SceneManager.LoadScene("House");
            currentScene = SceneManager.GetActiveScene();
        }
    }

}
