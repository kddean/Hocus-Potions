using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    int house;
    int garden;
    int world;
    AsyncOperation scene;


    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }


    void Start () {
        house = 0;
        world = 1;
        garden = 2; 
	}

    // Update is called once per frame

    public void SceneSwap(string s) {
        NPC[] npcs = GameObject.FindObjectsOfType<NPC>();
        foreach(NPC n in npcs) {
            n.sceneSwapped = true;
        }

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
        //TODO: This won't be needed once it's re-written
        GameObject.FindObjectOfType<NPCManager>().Spawned = false;
        OnSceneLoaded(index);
    }

    void OnSceneLoaded(int index) {
        Scene loadingScene = SceneManager.GetSceneByBuildIndex(index);
        if (loadingScene.IsValid()) {
            GameObject spawnPoint = GameObject.Find("SpawnPoint");
            GameObject.FindGameObjectWithTag("Player").transform.position = spawnPoint.transform.position;
            GameObject.Find("GarbageCollector").GetComponent<GarbageCollecter>().SpawnDropped();
            if (index == 0) {
                GameObject.FindObjectOfType<NPCController>().CurrentMap = 0;
            } else if (index == 1) {
                GameObject.FindObjectOfType<NPCController>().CurrentMap = 1;
            }
            GameObject.FindObjectOfType<NPCController>().sceneSwapped = true;
            SceneManager.SetActiveScene(loadingScene);
            GameObject.FindObjectOfType<DoorDontDestroy>().gameObject.GetComponent<AudioSource>().Play();
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().swappingScenes = false;
    }
}
