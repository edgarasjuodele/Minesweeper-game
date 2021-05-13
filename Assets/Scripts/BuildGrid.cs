using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using minegamedis;

public class BuildGrid : MonoBehaviour {

    public int mapSize = 8;
    private double bombCount = 0;
    private int markerCount = 0;
    private int pmarkCount = 1;
    private int[,] map;
    private bool empty = true;
    public Tilemap tilemap;
    public Tile cube, bomb, marker, markerb, markerg, purpleBonus, wide, greenBonus, num0, num1, num2, num2b, num3, num3b, num4, num5, num6, num7, num8;
    public Mines[,] mine;

    // Constructor with initializers for the script methods.
    void Start() {
        bombCount = ((mapSize * mapSize) * 0.20);
        mine = new Mines[mapSize, mapSize];
        map = CreateMap(mapSize, mapSize, empty, mine);
        PlaceBombs(map, (int)bombCount, mine);
        CheckBombs(mapSize, map, mine);
        BuildMap(map, tilemap, cube, wide, mine);
        PlaceBonus(map, mine);
        PlaceSpecial(map, mine, mapSize);
        RevealStarting(map, mine);
        Reveal(map, mine, tilemap, bomb, num0, num1, num2, num2b, num3, num3b, num4, num5, num6, num7, num8, purpleBonus, greenBonus, false);
        ScoreScript.bombCount = (int)this.bombCount - 1;
        MarkerScript.markerCount = 0;
    }

    // Detecting mouse input and calculating game progress.
    void Update() {
        if (!CountUnopened(map, mine, (int)bombCount)) {
            if (Input.GetMouseButtonDown(0)) {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int mousepos = Vector3Int.FloorToInt(pos);

                if (!mine[mousepos.x, mousepos.y].GetRevealed() && !mine[mousepos.x, mousepos.y].GetMarked()) {
                    if (mine[mousepos.x, mousepos.y].GetBomb()) {
                        Reveal(map, mine, tilemap, bomb, num0, num1, num2, num2b, num3, num3b, num4, num5, num6, num7, num8, purpleBonus, greenBonus, true);
                    } else {
                        if (mine[mousepos.x, mousepos.y].GetBonus(2)) {
                            mine[mousepos.x, mousepos.y].RevealWide(mapSize, mine);
                            if (mine[mousepos.x - 1, mousepos.y].GetBomb() || mine[mousepos.x + 1, mousepos.y].GetBomb()) {
                                Reveal(map, mine, tilemap, bomb, num0, num1, num2, num2b, num3, num3b, num4, num5, num6, num7, num8, purpleBonus, greenBonus, true);
                            }
                        }
                        if (mine[mousepos.x, mousepos.y].GetBonus(1)) {
                            pmarkCount++;
                        }
                        if (mine[mousepos.x, mousepos.y].GetBonus(0)) {
                            RevealGreens(map, mine);
                        }
                        mine[mousepos.x, mousepos.y].SetRevealed();
                        Reveal(map, mine, tilemap, bomb, num0, num1, num2, num2b, num3, num3b, num4, num5, num6, num7, num8, purpleBonus, greenBonus, false);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1)) {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int mousepos = Vector3Int.FloorToInt(pos);
                if (!mine[mousepos.x, mousepos.y].GetRevealed()) {
                    if (!mine[mousepos.x, mousepos.y].GetMarked()) {
                        mine[mousepos.x, mousepos.y].SetMarked();
                        tilemap.SetTile(new Vector3Int(mousepos.x, mousepos.y, 0), marker);
                        MarkerScript.markerCount++;
                        markerCount++;
                    } else if (mine[mousepos.x, mousepos.y].GetMarked() && !(pmarkCount <= 0)) {
                        if (!mine[mousepos.x, mousepos.y].GetPMark()) {
                            if (mine[mousepos.x, mousepos.y].GetBomb()) {
                                mine[mousepos.x, mousepos.y].SetPMark();
                                tilemap.SetTile(new Vector3Int(mousepos.x, mousepos.y, 0), markerb);
                                pmarkCount--;
                            } else {
                                mine[mousepos.x, mousepos.y].SetPMark();
                                tilemap.SetTile(new Vector3Int(mousepos.x, mousepos.y, 0), markerg);
                                pmarkCount--;
                            }
                        } else {
                            mine[mousepos.x, mousepos.y].SetPMark();
                            MarkerScript.markerCount--;
                            markerCount--;
                            tilemap.SetTile(new Vector3Int(mousepos.x, mousepos.y, 0), cube);
                        }
                    } else {
                        mine[mousepos.x, mousepos.y].SetMarked();
                        MarkerScript.markerCount--;
                        markerCount--;
                        tilemap.SetTile(new Vector3Int(mousepos.x, mousepos.y, 0), cube);
                    }
                }
            }
        } else {
            VictoryTextScript.victory = true;
        }
        
    }
    // Setup the map grid
    public static int[,] CreateMap(int w, int h, bool empty, Mines[,] mine) {
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
        int temp = 0;
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
            if (n == bombCount / 2) {
                if (x >= 1 && x < map.GetUpperBound(0) && y >= 1 && y < map.GetUpperBound(1)) {
                    if (temp != 2) {
                        mine[x, y].SetBonus(2);
                        n--;
                        temp++;
                    }
                }
            } else {
                mine[x, y].SetBomb();
            }

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

    // Very similar method to PlaceBombs however this time we check all 0 nearby bomb 
    // cells and at random select cells to place bonuses
    public static void PlaceBonus(int[,] map, Mines[,] mine) {
        List<int[]> gridList = new List<int[]>();
        int purplecount = Random.Range(0, 2);
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                if (mine[x, y].GetNearbyBomb() == 0) {
                    int[] tile = new int[2];
                    tile[0] = x;
                    tile[1] = y;
                    gridList.Add(tile);
                }
            }
        }
        double temp = Mathf.Sqrt(gridList.Count);
        for (int n = 0; n < (int)temp; n++) {
            int index = Random.Range(0, gridList.Count);
            int[] chosenTile = gridList[index];
            int x = chosenTile[0];
            int y = chosenTile[1];
            gridList.RemoveAt(index);
            if (purplecount != 0) {
                n--;
                purplecount--;
                mine[x, y].SetBonus(1);
            } else {
                mine[x, y].SetBonus(0);
            }
        }
    }


    // Go through the grid and randomly assign bracket tiles
    public static void PlaceSpecial(int[,] map, Mines[,] mine, int w) {
        List<int[]> gridList = new List<int[]>();
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                if (mine[x, y].GetNearbyBomb() == 2 || mine[x, y].GetNearbyBomb() == 3) {
                    mine[x, y].CountTouch(w, mine);
                    if (mine[x, y].GetTouch()) {
                        int[] tile = new int[2];
                        tile[0] = x;
                        tile[1] = y;
                        gridList.Add(tile);
                    }
                }
            }
        }
        double temp = Mathf.Sqrt(gridList.Count);
        for (int n = 0; n < (int)temp; n++) {
            int index = Random.Range(0, gridList.Count);
            int[] chosenTile = gridList[index];
            int x = chosenTile[0];
            int y = chosenTile[1];
            gridList.RemoveAt(index);
            mine[x, y].SetBonus(3);
        }

    }

    // Keep track of unrevealed tile count
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
        if(gridList.Count + 1 == bombCount) {
            return true;
        } else {
            return false;
        }
    }

    // Pick and reveal starting tile
    public static void RevealStarting(int[,] map, Mines[,] mine) {
        List<int[]> gridList = new List<int[]>();
        int widecount = Random.Range(0, 2);
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                if (mine[x, y].GetNearbyBomb() == 0 && !mine[x, y].GetBonus(0) && !mine[x, y].GetBonus(1)) {
                    int[] tile = new int[2];
                    tile[0] = x;
                    tile[1] = y;
                    gridList.Add(tile);
                }
            }
        }
        int index = Random.Range(0, gridList.Count);
        int[] chosenTile = gridList[index];
        int i = chosenTile[0];
        int j = chosenTile[1];
        gridList.RemoveAt(index);
        mine[i, j].SetRevealed();
    }


    // When triggered reveal all green tagged power up tiles
    public static void RevealGreens(int[,] map, Mines[,] mine) {
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                if (mine[x, y].GetBonus(0)) {
                    mine[x, y].SetRevealed();
                }
            }
        }
    }


    // Sets every tile to an empty block and a couple to double ended arrow
    public static void BuildMap(int[,] map, Tilemap tilemap, Tile cube, Tile wide, Mines[,] mine) {
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                tilemap.SetTile(new Vector3Int(x, y, 0), cube);
                if (mine[x, y].GetBonus(2)) {
                    tilemap.SetTile(new Vector3Int(x, y, 0), wide);
                }
            }
        }
    }

    // Changes tile based on what has been revealed by the player
    public static void Reveal(int[,] map, Mines[,] mine, Tilemap tilemap, Tile bomb, Tile n0, Tile n1, Tile n2, Tile n2b, Tile n3, Tile n3b, Tile n4, Tile n5, Tile n6, Tile n7, Tile n8, Tile purpleb, Tile greenb, bool gameOver) {
        int temp = 0;
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                if (gameOver) {
                    mine[x, y].SetRevealed();
                }
                temp = mine[x, y].GetNearbyBomb();
                if (mine[x, y].GetRevealed()) {
                    if (temp == 0 && !mine[x, y].GetBonus(0) && !mine[x, y].GetBonus(1)) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), n0);
                    } else if (temp == 1) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), n1);
                    } else if (temp == 2 && !mine[x, y].GetBonus(3)) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), n2);
                    } else if (temp == 3 && !mine[x, y].GetBonus(3)) {
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
                    } else if (mine[x, y].GetBonus(0)) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), greenb);
                    } else if (mine[x, y].GetBonus(1)) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), purpleb);
                    } else if (mine[x, y].GetBonus(3)) {
                        if (mine[x, y].GetNearbyBomb() == 2) {
                            tilemap.SetTile(new Vector3Int(x, y, 0), n2b);
                        } else if (mine[x, y].GetNearbyBomb() == 3) {
                            tilemap.SetTile(new Vector3Int(x, y, 0), n3b);
                        }
                    } else if (mine[x, y].GetBomb()) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), bomb);
                    }
                }
            }
        }
    }
}
