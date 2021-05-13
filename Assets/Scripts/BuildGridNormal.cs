using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using minegamedis;

public class BuildGridNormal : MonoBehaviour {

    public int mapSize = 15;
    private double bombCount = 0;
    private int[,] map;
    public Tilemap tilemap;
    public Tile cube, bomb, marker, emptyTile, num1, num2, num3, num4, num5, num6, num7, num8;
    public Mines[,] mine;

    // Setup map grid and calculate bomb locations and neighbours
    void Start() {
        bombCount = ((mapSize * mapSize) * 0.20);
        mine = new Mines[mapSize, mapSize];
        map = CreateMap(mapSize, mapSize, mine);
        PlaceBombs(map, (int)bombCount, mine);
        CheckBombs(mapSize, map, mine);
        BuildMap(map, tilemap, cube);
        ScoreScript.bombCount = (int)this.bombCount - 1;
    }

    // Update is called once per frame
    void Update() {

        if (!CountUnopened(map, mine, (int)bombCount)) {
            if (Input.GetMouseButtonDown(0)) {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int mousepos = Vector3Int.FloorToInt(pos);
                if (!mine[mousepos.x, mousepos.y].GetRevealed() && !mine[mousepos.x, mousepos.y].GetMarked()) {
                    if (mine[mousepos.x, mousepos.y].GetBomb()) {
                        Reveal(map, mine, tilemap, bomb, emptyTile, num1, num2, num3, num4, num5, num6, num7, num8, true);
                    } else {
                        mine[mousepos.x, mousepos.y].IsRevealed(mapSize, mine);
                        Reveal(map, mine, tilemap, bomb, emptyTile, num1, num2, num3, num4, num5, num6, num7, num8, false);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1)) {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int mousepos = Vector3Int.FloorToInt(pos);
                if (!mine[mousepos.x, mousepos.y].GetRevealed()) {
                    if (!mine[mousepos.x, mousepos.y].GetMarked()) {
                        MarkerScript.markerCount++;
                        mine[mousepos.x, mousepos.y].SetMarked();
                        tilemap.SetTile(new Vector3Int(mousepos.x, mousepos.y, 0), marker);
                    } else {
                        MarkerScript.markerCount--;
                        mine[mousepos.x, mousepos.y].SetMarked();
                        tilemap.SetTile(new Vector3Int(mousepos.x, mousepos.y, 0), cube);
                    }
                }

            }
        } else {
            VictoryTextScript.victory = true;
        }
    }

    // Setup the map grid
    public static int[,] CreateMap(int w, int h, Mines[,] mine) {
        int[,] map = new int[w, h];
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                mine[x, y] = new Mines(x, y);
            }
        }
        return map;
    }

    // Go through the map grid and generate bombs at random locations
    public static void PlaceBombs(int[,] map, int bombCount, Mines[,] mine) {
        List<int[]> gridList = new List<int[]>();
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                int[] tile = new int[2];
                tile[0] = x;
                tile[1] = y;
                gridList.Add(tile);
            }
        }
        for (int n = 0; n < bombCount; n++) {
            int index = Random.Range(0, gridList.Count);
            int[] chosenTile = gridList[index];
            int x = chosenTile[0];
            int y = chosenTile[1];
            gridList.RemoveAt(index);
            mine[x, y].SetBomb();
        }
    }

    public bool CountUnopened(int[,] map, Mines[,] mine, int bombCount) {
        List<int[]> gridList = new List<int[]>();
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                if (!mine[x, y].GetRevealed()) {
                    int[] tile = new int[2];
                    tile[0] = x;
                    tile[1] = y;
                    gridList.Add(tile);
                }
            }
        }
        if (gridList.Count == bombCount) {
            return true;
        } else {
            return false;
        }
    }

    // Go through the map grid and count the number of bombs near each tile
    public static void CheckBombs(int mapSize, int[,] map, Mines[,] mine) {
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                mine[x, y].CountMines(mapSize, mine);
            }
        }
    }
    
    // Sets every tile to an empty block
    public static void BuildMap(int[,] map, Tilemap tilemap, Tile cube) {
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                tilemap.SetTile(new Vector3Int(x, y, 0), cube);
            }
        }
    }

    // Changes tile based on what has been revealed by the player
    public static void Reveal(int[,] map, Mines[,] mine, Tilemap tilemap, Tile bomb, Tile emptyTile, Tile n1, Tile n2, Tile n3, Tile n4, Tile n5, Tile n6, Tile n7, Tile n8, bool gameOver) {
        int temp = 0;
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                if (gameOver) {
                    mine[x, y].SetRevealed();
                }
                temp = mine[x, y].GetNearbyBomb();
                if (mine[x, y].GetRevealed()) {
                    if (temp == 0) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), emptyTile);
                    } else if (temp == 1) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), n1);
                    } else if (temp == 2) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), n2);
                    } else if (temp == 3) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), n3);
                    } else if (temp == 4) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), n4);
                    } else if (temp == 5) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), n5);
                    } else if (temp == 6) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), n6);
                    } else if (temp == 7) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), n7);
                    } else if (temp == 8) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), n8);
                    } else if (mine[x, y].GetBomb()) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), bomb);
                    }
                }
            }
        }
    }
}
