using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour {
    Tilemap houseMap, worldMap1, worldMap2;
    bool houseSet, worldSet;
    List<Vector3> houseTiles, worldTiles;
    AsyncOperation scene;
    AudioListener al;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
        houseSet = worldSet = false;
    }

    struct PathTileData {
        public Vector3 parent;
        public int pathLength;
        public float totalDistance;

        public PathTileData(Vector3 parent, int pathLength, float totalDistance) {
            this.parent = parent;
            this.pathLength = pathLength;
            this.totalDistance = totalDistance;
        }
    }

    private void Start() {
        scene = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        al = GameObject.FindObjectOfType<AudioListener>();
        al.enabled = false;
        StartCoroutine(PopulateWorldTiles());

        houseMap = GameObject.Find("Tilemap_Floor").GetComponent<Tilemap>();
        houseMap.CompressBounds();
        BoundsInt bounds = houseMap.cellBounds;
        TileBase[] allTiles = houseMap.GetTilesBlock(bounds);
        houseTiles = new List<Vector3>();
        for (int y = 2; y > -6f; y--) {
            for (int x = -9; x < 9; x++) {
                if (houseMap.HasTile(new Vector3Int(x, y, 0))) {
                    RaycastHit2D[] check1 = Physics2D.RaycastAll(new Vector2(x + 0.1f, y - 2f), new Vector2(0, 1), 3f, Physics2D.AllLayers, -Mathf.Infinity, Mathf.Infinity);
                    RaycastHit2D[] check2 = Physics2D.RaycastAll(new Vector2(x + 0.5f, y - 2f), new Vector2(0, 1), 3f, Physics2D.AllLayers, -Mathf.Infinity, Mathf.Infinity);
                    RaycastHit2D[] check3 = Physics2D.RaycastAll(new Vector2(x + 0.9f, y - 2f), new Vector2(0, 1), 3f, Physics2D.AllLayers, -Mathf.Infinity, Mathf.Infinity);
                    RaycastHit2D[] check = new RaycastHit2D[check1.Length + check2.Length + check3.Length];
                    check1.CopyTo(check, 0);
                    check2.CopyTo(check, check1.Length);
                    check3.CopyTo(check, check1.Length + check2.Length);

                    if (check.Length == 0) {
                        houseTiles.Add(houseMap.GetCellCenterWorld(new Vector3Int(x, y, 0)));
                    } else {
                        bool triggers = true;
                        foreach (RaycastHit2D r in check) {
                            if (!r.collider.isTrigger && !r.collider.gameObject.tag.Equals("Player") && !r.collider.gameObject.tag.Equals("tiles")) {
                                triggers = false;
                                break;
                            }
                        }
                        if (triggers) {
                            houseTiles.Add(houseMap.GetCellCenterWorld(new Vector3Int(x, y, 0)));
                        }
                    }
                }
            }
        }
    }


    IEnumerator PopulateWorldTiles() {
        while (!scene.isDone) {
            yield return null;
        }

        worldMap1 = GameObject.Find("Ground").GetComponent<Tilemap>();
        worldMap2 = GameObject.Find("Stairs").GetComponent<Tilemap>();
        worldMap1.CompressBounds();
        worldMap2.CompressBounds();
        BoundsInt bounds1 = worldMap1.cellBounds;
        BoundsInt bounds2 = worldMap2.cellBounds;
        worldTiles = new List<Vector3>();
        for (int y = 34; y > -72f; y--) {
            for (int x = -80; x < 70; x++) {
                if (worldMap1.HasTile(new Vector3Int(x, y, 0))) {
                    RaycastHit2D[] check1 = Physics2D.RaycastAll(new Vector2(x + 0.1f, y - 2f), new Vector2(0, 1), 3f, Physics2D.AllLayers, -Mathf.Infinity, Mathf.Infinity);
                    RaycastHit2D[] check2 = Physics2D.RaycastAll(new Vector2(x + 0.5f, y - 2f), new Vector2(0, 1), 3f, Physics2D.AllLayers, -Mathf.Infinity, Mathf.Infinity);
                    RaycastHit2D[] check3 = Physics2D.RaycastAll(new Vector2(x + 0.9f, y - 2f), new Vector2(0, 1), 3f, Physics2D.AllLayers, -Mathf.Infinity, Mathf.Infinity);
                    RaycastHit2D[] check = new RaycastHit2D[check1.Length + check2.Length + check3.Length];
                    check1.CopyTo(check, 0);
                    check2.CopyTo(check, check1.Length);
                    check3.CopyTo(check, check1.Length + check2.Length);

                    if (check.Length == 0) {
                        worldTiles.Add(worldMap1.GetCellCenterWorld(new Vector3Int(x, y, 0)));
                    } else {
                        bool triggers = true;
                        foreach (RaycastHit2D r in check) {
                            if (!r.collider.isTrigger && !r.collider.gameObject.tag.Equals("Player") && !r.collider.gameObject.tag.Equals("tiles")) {
                                triggers = false;
                                break;
                            }
                        }
                        if (triggers) {
                            worldTiles.Add(worldMap1.GetCellCenterWorld(new Vector3Int(x, y, 0)));
                        }
                    }
                } else if (worldMap2.HasTile(new Vector3Int(x, y, 0))) {
                    RaycastHit2D[] check1 = Physics2D.RaycastAll(new Vector2(x + 0.1f, y - 2f), new Vector2(0, 1), 3f, Physics2D.AllLayers, -Mathf.Infinity, Mathf.Infinity);
                    RaycastHit2D[] check2 = Physics2D.RaycastAll(new Vector2(x + 0.5f, y - 2f), new Vector2(0, 1), 3f, Physics2D.AllLayers, -Mathf.Infinity, Mathf.Infinity);
                    RaycastHit2D[] check3 = Physics2D.RaycastAll(new Vector2(x + 0.9f, y - 2f), new Vector2(0, 1), 3f, Physics2D.AllLayers, -Mathf.Infinity, Mathf.Infinity);
                    RaycastHit2D[] check = new RaycastHit2D[check1.Length + check2.Length + check3.Length];
                    check1.CopyTo(check, 0);
                    check2.CopyTo(check, check1.Length);
                    check3.CopyTo(check, check1.Length + check2.Length);

                    if (check.Length == 0) {
                        worldTiles.Add(worldMap2.GetCellCenterWorld(new Vector3Int(x, y, 0)));
                    } else {
                        bool triggers = true;
                        foreach (RaycastHit2D r in check) {
                            if (!r.collider.isTrigger && !r.collider.gameObject.tag.Equals("Player") && !r.collider.gameObject.tag.Equals("tiles")) {
                                triggers = false;
                                break;
                            }
                        }
                        if (triggers) {
                            worldTiles.Add(worldMap2.GetCellCenterWorld(new Vector3Int(x, y, 0)));
                        }
                    }
                }
            }
        }
        SceneManager.UnloadSceneAsync(1);
        al.enabled = true;
    }

    public void InitializePath(Vector3 pos, Vector3 target, int map, List<Vector3> returnPath) {
        bool playerOnMap = false;
        Vector3 centeredPlayerPos = new Vector3();
        if (map == 0 && SceneManager.GetActiveScene().name.Equals("House") || map == 1 && !SceneManager.GetActiveScene().name.Equals("House")) {
            Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            centeredPlayerPos = new Vector3(Mathf.Sign(playerPos.x) * (Mathf.Abs((int)playerPos.x) + 0.5f), Mathf.Sign(playerPos.y) * (Mathf.Abs((int)playerPos.y) + 0.5f), 0);
            playerOnMap = true;
        }
        Dictionary<Vector3, PathTileData> openList, closedList;
        Vector3 centeredTarget = new Vector3(Mathf.Sign(target.x) * (Mathf.Abs((int)target.x) + 0.5f), Mathf.Sign(target.y) * (Mathf.Abs((int)target.y) + 0.5f), 0);
        openList = new Dictionary<Vector3, PathTileData>();
        closedList = new Dictionary<Vector3, PathTileData>();
        Vector2Int start = new Vector2Int(-999, -999);
        List<Vector3> temp;
        Vector3 tempPos = new Vector3(Mathf.Sign(pos.x) * (Mathf.Abs((int)pos.x) + 0.5f), Mathf.Sign(pos.y) * (Mathf.Abs((int)pos.y) + 0.5f), 0);
        if (map == 0) {
            temp = houseTiles;
        } else {
            temp = worldTiles;
        }
        PathTileData data = new PathTileData(new Vector3(-9999, -9999, -9999), 0, Vector3.Distance(tempPos, centeredTarget));
        closedList.Add(tempPos, data);
        Vector3 checkTile = new Vector3(tempPos.x + 1, tempPos.y, 0);
        if (temp.Contains(checkTile)) {
            if ((playerOnMap && checkTile != centeredPlayerPos) || !playerOnMap) {
                PathTileData tile = new PathTileData(tempPos, 1, 1 + Vector3.Distance(new Vector3(tempPos.x + 1, tempPos.y, 0), centeredTarget));
                openList.Add(new Vector3(tempPos.x + 1, tempPos.y, 0), tile);
            }
        }
        checkTile = new Vector3(tempPos.x - 1, tempPos.y, 0);
        if (temp.Contains(checkTile)) {
            if ((playerOnMap && checkTile != centeredPlayerPos) || !playerOnMap) {
                PathTileData tile = new PathTileData(tempPos, 1, 1 + Vector3.Distance(new Vector3(tempPos.x - 1, tempPos.y, 0), centeredTarget));
                openList.Add(new Vector3(tempPos.x - 1, tempPos.y, 0), tile);
            }
        }
        checkTile = new Vector3(tempPos.x, tempPos.y + 1, 0);
        if (temp.Contains(checkTile)) {
            if ((playerOnMap && checkTile != centeredPlayerPos) || !playerOnMap) {
                PathTileData tile = new PathTileData(tempPos, 1, 1 + Vector3.Distance(new Vector3(tempPos.x, tempPos.y + 1, 0), centeredTarget));
                openList.Add(new Vector3(tempPos.x, tempPos.y + 1, 0), tile);
            }
        }
        checkTile = new Vector3(tempPos.x, tempPos.y - 1, 0);
        if (temp.Contains(checkTile)) {
            if ((playerOnMap && checkTile != centeredPlayerPos) || !playerOnMap) {
                PathTileData tile = new PathTileData(tempPos, 1, 1 + Vector3.Distance(new Vector3(tempPos.x, tempPos.y - 1, 0), centeredTarget));
                openList.Add(new Vector3(tempPos.x, tempPos.y - 1, 0), tile);
            }
        }
        Thread thread = new Thread(() => FindPath(openList, closedList, centeredTarget, temp, returnPath));
        thread.Start();
    }

    void FindPath(Dictionary<Vector3, PathTileData> openList, Dictionary<Vector3, PathTileData> closedList, Vector3 centeredTarget, List<Vector3> tileSet, List<Vector3> path) {
        lock (path) {
            PathTileData data = new PathTileData();
            Vector3 key = new Vector3();
            bool foundPath = false;
            while (openList.Count > 0) {
                key = openList.Keys.ToList()[0];
                float max = openList[key].totalDistance;

                foreach (Vector3 k in openList.Keys.ToList()) {
                    if (openList[k].totalDistance < max) {
                        max = openList[k].totalDistance;
                        key = k;
                    }
                }
                data = openList[key];
                openList.Remove(key);
                closedList.Add(key, data);
                if (key == centeredTarget) {
                    foundPath = true;
                    break;
                }
                PathTileData junk = new PathTileData();
                if (tileSet.Contains(new Vector3(key.x + 1, key.y, 0)) && !closedList.TryGetValue(new Vector3(key.x + 1, key.y, 0), out junk)) {
                    if (openList.TryGetValue(new Vector3(key.x + 1, key.y, 0), out junk)) {
                        if (junk.totalDistance > closedList[key].pathLength + 1 + Vector3.Distance(new Vector3(key.x + 1, key.y, 0), centeredTarget)) {
                            openList[new Vector3(key.x + 1, key.y, 0)] = new PathTileData(key, closedList[key].pathLength + 1, 1 + closedList[key].pathLength + Vector3.Distance(new Vector3(key.x + 1, key.y, 0), centeredTarget));
                        }
                    } else {
                        PathTileData tile = new PathTileData(key, closedList[key].pathLength + 1, 1 + closedList[key].pathLength + Vector3.Distance(new Vector3(key.x + 1, key.y, 0), centeredTarget));
                        openList.Add(new Vector3(key.x + 1, key.y, 0), tile);
                    }
                }

                if (tileSet.Contains(new Vector3(key.x - 1, key.y, 0)) && !closedList.TryGetValue(new Vector3(key.x - 1, key.y, 0), out junk)) {
                    if (openList.TryGetValue(new Vector3(key.x - 1, key.y, 0), out junk)) {
                        if (junk.totalDistance > closedList[key].pathLength + 1 + Vector3.Distance(new Vector3(key.x - 1, key.y, 0), centeredTarget)) {
                            openList[new Vector3(key.x - 1, key.y, 0)] = new PathTileData(key, closedList[key].pathLength + 1, 1 + closedList[key].pathLength + Vector3.Distance(new Vector3(key.x - 1, key.y, 0), centeredTarget));
                        }
                    } else {
                        PathTileData tile = new PathTileData(key, closedList[key].pathLength + 1, 1 + closedList[key].pathLength + Vector3.Distance(new Vector3(key.x - 1, key.y, 0), centeredTarget));
                        openList.Add(new Vector3(key.x - 1, key.y, 0), tile);
                    }
                }

                if (tileSet.Contains(new Vector3(key.x, key.y + 1, 0)) && !closedList.TryGetValue(new Vector3(key.x, key.y + 1, 0), out junk)) {
                    if (openList.TryGetValue(new Vector3(key.x, key.y + 1, 0), out junk)) {
                        if (junk.totalDistance > closedList[key].pathLength + 1 + Vector3.Distance(new Vector3(key.x, key.y + 1, 0), centeredTarget)) {
                            openList[new Vector3(key.x, key.y + 1, 0)] = new PathTileData(key, closedList[key].pathLength + 1, 1 + closedList[key].pathLength + Vector3.Distance(new Vector3(key.x, key.y + 1, 0), centeredTarget));
                        }
                    } else {
                        PathTileData tile = new PathTileData(key, closedList[key].pathLength + 1, 1 + closedList[key].pathLength + Vector3.Distance(new Vector3(key.x, key.y + 1, 0), centeredTarget));
                        openList.Add(new Vector3(key.x, key.y + 1, 0), tile);
                    }
                }

                if (tileSet.Contains(new Vector3(key.x, key.y - 1, 0)) && !closedList.TryGetValue(new Vector3(key.x, key.y - 1, 0), out junk)) {
                    if (openList.TryGetValue(new Vector3(key.x, key.y - 1, 0), out junk)) {
                        if (junk.totalDistance > closedList[key].pathLength + 1 + Vector3.Distance(new Vector3(key.x, key.y - 1, 0), centeredTarget)) {
                            openList[new Vector3(key.x, key.y - 1, 0)] = new PathTileData(key, closedList[key].pathLength + 1, 1 + closedList[key].pathLength + Vector3.Distance(new Vector3(key.x, key.y - 1, 0), centeredTarget));
                        }
                    } else {
                        PathTileData tile = new PathTileData(key, closedList[key].pathLength + 1, 1 + closedList[key].pathLength + Vector3.Distance(new Vector3(key.x, key.y - 1, 0), centeredTarget));
                        openList.Add(new Vector3(key.x, key.y - 1, 0), tile);
                    }
                }
            }

            if (foundPath) {
                path.Clear();
                path.Add(key);
                key = data.parent;
                while (closedList[key].parent.x > -9000) {
                    path.Insert(0, key);
                    key = closedList[key].parent;
                }
            }
        }
    }
}
