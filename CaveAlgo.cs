using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveAlgo : MonoBehaviour {

    // 2 dimensional array that holds the state of each tile, and its position
    int[,] caveMap;

    // 0 is an empty tile
    // 1 is a wall tile


    // ASSIGN THESE IN UNITY!
    // ---------------------
    [Tooltip("Cave X Dimension -- Default: 50")]
    public int caveX;
    [Tooltip("Cave Y Dimension -- Default: 50")]
    public int caveY;
    [Tooltip("%/100 chance for a wall to spawn on initial cave generation -- Default: 45")]
    public int percentWalls;
    [Tooltip("Assign GameObject (1 unit length x and z) to visually show cave generation -- Default: Unity Cube")]
    public GameObject Tile;
    //----------------------

    List<GameObject> tiles = new List<GameObject>();

	// Start algorithm on first run
	void Start () {
        caveMap = new int[caveX, caveY];
        SpawnBaseWall();
	}

    /// <summary>
    /// Visualize (render) tiles in Unity based on the matrix of tiles we've created
    /// </summary>
    void DrawMap() {
        foreach(GameObject daTile in tiles) {
            Destroy(daTile);
        }
        tiles.Clear(); // clear old map if it exists
        for (int y = 0; y < caveY; y++) {
            for (int x = 0; x < caveX; x++) {
                GameObject temp = Instantiate(Tile);
                temp.transform.position = new Vector3(x,0,y);
                if (caveMap[x, y] == 1) { // change to black if wall, default to white
                    temp.GetComponent<Renderer>().material.color = Color.black;
                }
                temp.name = "X: " + x + " Y: " + y; // simple name assignment in unity
                tiles.Add(temp);
            }
        }
    }
    /// <summary>
    /// Called when "Reset Map" is pressed (and also at program start)
    /// </summary>
    public void SpawnBaseWall(){
        for(int y = 0; y < caveY; y++) {
            for(int x = 0; x < caveX; x++) { // randomly decide whether tile is wall or open
                int temp = Random.Range(0, 100);
                if (temp < percentWalls) { // user-defined, ideally would be ~50 if using defaults
                    caveMap[x, y] = 1;
                }
                else {
                    caveMap[x, y] = 0;
                }
            }
        }
        DrawMap();
    }

    /// <summary>
    /// Simple boolean bounds check
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    bool IsOutOfBounds(int x, int y) {
        return (x < 0 || x >= caveX || y < 0 || y >= caveY);
    }

    /// <summary>
    /// Takes a step through algorithm, visualizing results.
    /// </summary>
    public void StepMap() { // button calls step
        int[,] tempMap = new int[caveX, caveY];
        for (int y = 0; y < caveY; y++) { // go through all tiles
            for (int x = 0; x < caveX; x++) {
                tempMap[x, y] = GetNearTiles(x, y); // check if tile should convert or not
            }
        }
        caveMap = tempMap; 
        DrawMap();
    }

    int GetNearTiles(int xPos, int yPos) { // gets the 8 surrounding tiles. returns 0 if should be empty, 1 if wall.
        int totalWalls = 0;
        for (int y = yPos - 1; y <= yPos + 1; y++) {
            for (int x = xPos - 1; x <= xPos + 1; x++) {
                if (!(xPos==x && yPos==y)) { // ignore the tile we're on
                    if (IsWall(x, y)) {
                        totalWalls++;
                    }
                }
            }
        }

        if (caveMap[xPos, yPos] == 1) {
            if (totalWalls >= 3) { // if center tile is wall, and at least 3 surrounding wall tiles,
                return 1; // make this tile a wall.
            }
        }
        else {
            if (totalWalls >= 5) { // 5 walls > 4 open -- convert tile
                return 1;
            }
        }
        return 0; // mark as open tile if "change-to-wall" parameters aren't met
    }

    /// <summary>
    /// Checks bounds and tile to see if tile is a wall
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    bool IsWall(int x, int y) {
        return (IsOutOfBounds(x, y) || caveMap[x, y] == 1 || caveMap[x, y] == 1);
    }

}

/*
 * Process:
 * -Initiliaze base map with SpawnBaseWall(), see state visually
 * -Step through algorithm calling StepMap()
 * -Call SpawnBaseWall() again to reset the map to a different one
 *
 * Can also change percentWalls on the fly to see how it affects the resulting algorithm
 *
 */
