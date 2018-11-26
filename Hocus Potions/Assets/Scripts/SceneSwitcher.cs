using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    int house;
    int garden;
    int world;
    int currentScene;
    AsyncOperation scene;


    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }


    void Start () {
        house = 0;
        garden = 1; 
        world = 2;
	}

    // Update is called once per frame

    public void SceneSwap(string s) {
        switch (s) {
            case "ToHouse":
                StartCoroutine(SceneLoader(house));
                break;
            case "ToWorld":
                StartCoroutine(SceneLoader(world));
                break;
            case "ToGarden":
                StartCoroutine(SceneLoader(garden));
                break;
            default:
                break;
        }
    }

    IEnumerator SceneLoader(int index) {
        scene = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        while (!scene.isDone) {
            yield return null;
        }
        OnSceneLoaded(index);
    }

    void OnSceneLoaded(int index) {
        Scene loadingScene = SceneManager.GetSceneByBuildIndex(index);
        if (loadingScene.IsValid()) {
            GameObject spawnPoint = GameObject.Find("SpawnPoint");
            GameObject.FindGameObjectWithTag("Player").transform.position = spawnPoint.transform.position;
            GameObject.Find("GarbageCollector").GetComponent<GarbageCollecter>().SpawnDropped();
            SceneManager.SetActiveScene(loadingScene);         
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().swappingScenes = false;
    }
}
