using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour {
    Tilemap houseMap, worldMap1, worldMap2;
    bool houseSet, world1Set, world2Set;
    List<Vector2> houseTiles, worldTiles;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        houseTiles = new List<Vector2>();
        worldTiles = new List<Vector2>();
        if (SceneManager.GetActiveScene().name.Equals("House")) {
            houseMap = GameObject.FindObjectsOfType<Tilemap>()[0];
            foreach(Tile t in houseMap.GetTilesBlock(houseMap.cellBounds)) {
               // houseTiles.Add(t.transform.position);
            }
            houseSet = true;
        } else {
            worldMap1 = GameObject.FindObjectsOfType<Tilemap>()[0];
            worldMap2 = GameObject.FindObjectsOfType<Tilemap>()[1];
            world1Set = true;
            world2Set = true;
        }
    }

    private void Update() {
        if (!houseSet && SceneManager.GetActiveScene().name.Equals("House")) {
            houseMap = GameObject.FindObjectsOfType<Tilemap>()[0];
            houseSet = true;
        } else if(!world1Set && !SceneManager.GetActiveScene().name.Equals("Garden") && !SceneManager.GetActiveScene().name.Equals("House")) {
            worldMap1 = GameObject.FindObjectsOfType<Tilemap>()[0];
            worldMap2 = GameObject.FindObjectsOfType<Tilemap>()[1];
            world1Set = true;
            world2Set = true;
        }

        if (houseSet) {
            Debug.Log(houseMap.GetCellCenterLocal(new Vector3Int(1, 1, 0)));
        }
    }


}
