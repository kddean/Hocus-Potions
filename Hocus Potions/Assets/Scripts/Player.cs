using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour {


    public float speed;
    int x, y;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {


        Vector3 pos = transform.position;
        x = y = 0;
        if (Input.GetKey("w")) {
            y = 1;
        } else if (Input.GetKey("s")) {
            y = -1;
        } else if (Input.GetKey("a")) {
            x = -1;
        } else if (Input.GetKey("d")) {
            x = 1;
        }
        pos.x += x * speed;
        pos.y += y * speed;
        transform.position = pos;

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        switch (collision.gameObject.name) {
            case "ToWorld":
                GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToWorld");
                break;
            case "ToHouse":
                GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToHouse");
                break;
            case "ToGarden":
                GameObject.FindGameObjectWithTag("sceneLoader").GetComponent<SceneSwitcher>().SceneSwap("ToGarden");
                break;
            default:
                break;
        }
    }
}
