using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour {
    Tilemap houseMap, worldMap1, worldMap2;
    bool houseSet, worldSet;
    Vector3[,] houseTiles, worldTiles;

    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
        houseSet = worldSet = false;
    }

    struct PathTileData {
        public Vector2Int index, parentIndex;
        public int pathDistance;
        public float estimate;
        public float combinedDistance;

        public PathTileData(Vector2 index, Vector2 parentIndex, int pathDistance, float estimate, float combinedDistance) {
            this.index = index;
            this.parentIndex = parentIndex;
            this.pathDistance = pathDistance;
            this.estimate = estimate;
            this.combinedDistance = combinedDistance;
        }
    }

    private void Update() {
        if (!houseSet && SceneManager.GetActiveScene().name.Equals("House")) {
            houseMap = GameObject.Find("Tilemap_Floor").GetComponent<Tilemap>();
            houseMap.CompressBounds();
            BoundsInt bounds = houseMap.cellBounds;
            TileBase[] allTiles = houseMap.GetTilesBlock(bounds);
            houseTiles = new Vector3[18, 8];
            int i, j;
            i = j = 0;
            for (int y = 2; y > -6f; y--) {
                for (int x = -9; x < 9; x++) {
                    if (houseMap.HasTile(new Vector3Int(x, y, 0))) {
                        houseTiles[i, j] = houseMap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    } else {
                        houseTiles[i, j] = new Vector3(0, 0, -9999);
                    }
                    i++;
                }
                j++;
                i = 0;
            }
            InitializePath(new Vector3(0.3f, 5.7f, 0), new Vector3(0, 0, 0), 0);
            houseSet = true;
        } else if (!worldSet && !SceneManager.GetActiveScene().name.Equals("Garden") && !SceneManager.GetActiveScene().name.Equals("House")) {
            worldMap1 = GameObject.Find("Ground").GetComponent<Tilemap>();
            worldMap2 = GameObject.Find("Stairs").GetComponent<Tilemap>();
            worldMap1.CompressBounds();
            worldMap2.CompressBounds();
            BoundsInt bounds1 = worldMap1.cellBounds;
            BoundsInt bounds2 = worldMap2.cellBounds;
            worldTiles = new Vector3[149, 104];
            int i, j;
            i = j = 0;
            for (int y = 35; y > -70f; y--) {
                for (int x = -80; x < 70; x++) {
                    if (worldMap1.HasTile(new Vector3Int(x, y, 0))) {
                        worldTiles[i, j] = worldMap1.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    } else if (worldMap2.HasTile(new Vector3Int(x, y, 0))) {
                        worldTiles[i, j] = worldMap2.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    } else {
                        houseTiles[i, j] = new Vector3(0, 0, -9999);
                    }
                    i++;
                }
                j++;
                i = 0;
            }
            worldSet = true;
        }

    }

    void InitializePath(Vector3 pos, Vector3 target, int map) {
        List<PathTileData> openList, closedList;
        openList = new List<PathTileData>();
        closedList = new List<PathTileData>();
        Vector2Int start = new Vector2Int(-999, -999);
        Vector3[,] temp;
        Vector3 tempPos = new Vector3(Mathf.Sign(pos.x) * (Mathf.Round((int)pos.x) + 0.5f), Mathf.Sign(pos.y) * (Mathf.Round((int)pos.y) + 0.5f), 0);

        if (map == 0) {
            temp = houseTiles;
        } else {
            temp = worldTiles;
        }

        for (int i = 0; i < temp.GetLength(0); i++) {
            for (int j = 0; j < temp.GetLength(1); j++) {
                if (temp[i, j] == tempPos) {
                    start = new Vector2Int(i, j);
                }
            }
        }

        closedList.Add(new PathTileData(start, new Vector2(-999, -999), 0, Vector3.Distance(tempPos, target), Vector3.Distance(tempPos, target)));

        if (start.x > 0) {
            Vector2Int ind = new Vector2Int(start.x - 1, start.y);
            if (temp[ind.x, ind.y].z > -1000) {
                openList.Add(new PathTileData(ind, start, 1, Vector3.Distance(tempPos, target), Vector3.Distance(tempPos, target) + 1));
            }
        }
        if (start.x < temp.GetLength(0)) {
            Vector2Int ind = new Vector2Int(start.x + 1, start.y);
            if (temp[ind.x, ind.y].z > -1000) {
                openList.Add(new PathTileData(ind, start, 1, Vector3.Distance(tempPos, target), Vector3.Distance(tempPos, target) + 1));
            }
        }
        if (start.y > 0) {
            Vector2Int ind = new Vector2Int(start.x, start.y - 1);
            if (temp[ind.x, ind.y].z > -1000) {
                openList.Add(new PathTileData(ind, start, 1, Vector3.Distance(tempPos, target), Vector3.Distance(tempPos, target) + 1));
            }
        }
        if (start.y < temp.GetLength(1)) {
            Vector2Int ind = new Vector2Int(start.x, start.y + 1);
            if (temp[ind.x, ind.y].z > -1000) {
                openList.Add(new PathTileData(ind, start, 1, Vector3.Distance(tempPos, target), Vector3.Distance(tempPos, target) + 1));
            }
        }

        FindPath(openList, closedList, target, temp);
    }

    List<Vector3> FindPath(List<PathTileData> openList, List<PathTileData> closedList, Vector3 target, Vector3[,] tileSet) {
        Vector3 centeredTarget = new Vector3(Mathf.Sign(target.x) * (Mathf.Round((int)target.x) + 0.5f), Mathf.Sign(target.y) * (Mathf.Round((int)target.y) + 0.5f), 0);
        PathTileData data;
        while (openList.Count > 0) {
            openList.Sort(new CompareDistance());
            data = openList[0];
            openList.RemoveAt(0);
            closedList.Add(data);

            if (tileSet[data.index.x, data.index.y] == centeredTarget) {
                List<Vector3> path = new List<Vector3>();
                path.Add(tileSet[data.index.x, data.index.y]);
                while (data.parentIndex.x > -900) {
                   // data = 
                }


            }

            if (data.index.x > 0) {
                Vector2Int ind = new Vector2Int(data.index.x - 1, data.index.y);
                if (tileSet[ind.x, ind.y].z > -1000) {
                    openList.Add(new PathTileData(ind, data.index, data.pathDistance + 1, Vector3.Distance(tileSet[ind.x, ind.y], target), Vector3.Distance(tileSet[ind.x, ind.y], target) + data.pathDistance + 1));
                }
            }
            if (data.index.x < tileSet.GetLength(0)) {
                Vector2Int ind = new Vector2Int(data.index.x + 1, data.index.y);
                if (tileSet[ind.x, ind.y].z > -1000) {
                    openList.Add(new PathTileData(ind, data.index, data.pathDistance + 1, Vector3.Distance(tileSet[ind.x, ind.y], target), Vector3.Distance(tileSet[ind.x, ind.y], target) + data.pathDistance + 1));
                }
            }
            if (data.index.y > 0) {
                Vector2Int ind = new Vector2Int(data.index.x, data.index.y - 1);
                if (tileSet[ind.x, ind.y].z > -1000) {
                    openList.Add(new PathTileData(ind, data.index, data.pathDistance + 1, Vector3.Distance(tileSet[ind.x, ind.y], target), Vector3.Distance(tileSet[ind.x, ind.y], target) + data.pathDistance + 1));
                }
            }
            if (data.index.y < tileSet.GetLength(1)) {
                Vector2Int ind = new Vector2Int(data.index.x, data.index.y + 1);
                if (tileSet[ind.x, ind.y].z > -1000) {
                    openList.Add(new PathTileData(ind, data.index, data.pathDistance + 1, Vector3.Distance(tileSet[ind.x, ind.y], target), Vector3.Distance(tileSet[ind.x, ind.y], target) + data.pathDistance + 1));
                }
            }
        }

        return new List<Vector3>();
    }


    [System.Serializable]
    private class CompareDistance : IComparer<PathTileData> {
        int IComparer<PathTileData>.Compare(PathTileData a, PathTileData b) {
            if (a.combinedDistance < b.combinedDistance) {
                return -1;
            } else {
                return 1;
            }
        }
    }
}
