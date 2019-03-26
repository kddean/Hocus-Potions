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


    public void SceneSwap(string s) {
        GameObject.FindObjectOfType<NPCController>().swapping = true;
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
        if (Resources.FindObjectsOfTypeAll<StartScreen>().Length > 0) {
            Resources.FindObjectsOfTypeAll<StartScreen>()[0].gameObject.SetActive(false);
        }
        Resources.FindObjectsOfTypeAll<LoadingScreen>()[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        scene = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        while (!scene.isDone) {
            yield return null;
        }

        OnSceneLoaded(index);
    }

    void OnSceneLoaded(int index) {
        Scene loadingScene = SceneManager.GetSceneByBuildIndex(index);
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (loadingScene.IsValid()) {
            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
            GameObject spawnPoint = GameObject.Find("SpawnPoint");

            player.transform.position = spawnPoint.transform.position;
            GameObject.Find("GarbageCollector").GetComponent<GarbageCollecter>().SpawnDropped();
            if (index == 0) {
                GameObject.FindObjectOfType<NPCController>().CurrentMap = 0;
            } else if (index == 1) {
                GameObject.FindObjectOfType<NPCController>().CurrentMap = 1;
            }
            GameObject.FindObjectOfType<NPCController>().sceneSwapped = true;
            SceneManager.SetActiveScene(loadingScene);
            GameObject.FindObjectOfType<DoorDontDestroy>().gameObject.GetComponent<AudioSource>().Play();
            DialogueCanvas dc = Resources.FindObjectsOfTypeAll<DialogueCanvas>()[0];
            if (dc.active) {
                dc.active = false;
                dc.gameObject.SetActive(false);
            }
        }

        player.GetComponent<Player>().swappingScenes = false;
        player.Speed = player.previousSpeed;
    }
}
