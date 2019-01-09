#undef DebugNeighbours
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PlanetGraphType {
    hexagonal, matrix
}
public enum WetnessType {
    islands, gradient
}
[System.Serializable]
public class PlanetGraphInfo {
    public PlanetGraphType planetGraphType;
    private int initialGraphWidth;
    public WetnessType wetnessType;
    public float wetFrequency;
    public float wetnesDecreaseFarctor;
    public int wetnesDecreaseFrequency;
    public float oscilRange;
    public int wetnessSpread;

    [System.NonSerialized]
    public List<Tile> allPlanetTiles = new List<Tile>();

    public void generateGraph() {
        initialGraphWidth = Planet.instance.planetVisualInfo.normalVisionWidth;
        Tile firstTile = new Tile(0, 0, 0);
        allPlanetTiles.Add(firstTile);

        switch (planetGraphType) {
            case PlanetGraphType.matrix:
                generateMatrixLikeGraph(firstTile, initialGraphWidth);
                break;
            case PlanetGraphType.hexagonal:
                generateHexagonGraph();
                break;
        }
    }
    public void generateMatrixLikeGraph(Tile startingTile, int graphWidth) {
        int numOfTiles = (int) Mathf.Pow((graphWidth * 2) +1,2);
        int currNumofTiles = 0;

        Queue<Tile> tilesToGenerateAround = new Queue<Tile>();
        tilesToGenerateAround.Enqueue(startingTile);
        currNumofTiles = 1;

        while (currNumofTiles < numOfTiles) {
            Tile currTile = tilesToGenerateAround.Dequeue();
            for(int x=-1;x<=1;x++)
                for(int y = -1; y <= 1; y++) {
                    if (x != 0 || y != 0) { //pos(0,0)
                        Vector3Int coor = new Vector3Int(x, y,0);
                        bool add = true;
                        foreach(Tile tile in currTile.neighbours) {
                            if (tile.virtualCoordinates.Equals(coor+currTile.virtualCoordinates)) {
                                add = false;
                            }
                        }
                        if (add) {
                            Tile newTile = new Tile(coor + currTile.virtualCoordinates);
                            allPlanetTiles.Add(newTile);
                            currTile.addNeighoursConnection(newTile);
                            tilesToGenerateAround.Enqueue(newTile);
                            currNumofTiles += 1;
                        }
                    }
                }
            foreach(Tile tile in currTile.neighbours) {
                connectWithGraph(tile, currTile.neighbours);
            }
        }
#if DebugNeighbours
        debugNeighbours();
#endif
    }

    public void generateHexagonGraph() {
        //TODO implement
    }

    public void generateTileWetness() {
        switch (wetnessType) {
            case WetnessType.islands:

                List<Tile> wetTiles = new List<Tile>();

                //setting watter tiles
                foreach (Tile tile in allPlanetTiles) {
                    if (System.Math.Round(Random.Range(0f, 100f), 2) <= wetFrequency) {
                        tile.wetness = 1;
                        wetTiles.Add(tile);
                    }
                }

                //spreading direct wetness
                spreadDirectWetnessBFS(wetTiles);

                //setting random oscilations in wetness
                foreach (Tile tile in allPlanetTiles) {
                    tile.wetness += Random.Range(-oscilRange, oscilRange);
                }

                //spreading undirect wetness
                for (int i = 0; i < wetnessSpread; i++) {
                    foreach (Tile tile in allPlanetTiles) {
                        if (tile.wetness != 1) { //not changing watter tiles
                            float wetness = 0;
                            foreach (Tile neighbour in tile.neighbours) {
                                wetness += neighbour.wetness;
                            }
                            wetness /= tile.neighbours.Count;
                            tile.wetness = wetness;
                        }
                    }
                }
                break;
        }
    }

    private void spreadDirectWetnessBFS(List<Tile> wetTiles) {
        foreach (Tile tile in wetTiles) {
            Queue<KeyValuePair<Tile, int>> tilesToVisit = new Queue<KeyValuePair<Tile, int>>();
            List<Tile> visitedTiles = new List<Tile>();

            tilesToVisit.Enqueue(new KeyValuePair<Tile, int>(tile, 0));

            while (tilesToVisit.Count != 0) {

                KeyValuePair<Tile, int> currTileAndDepth = tilesToVisit.Dequeue();

                if (!visitedTiles.Contains(currTileAndDepth.Key) &&
                        wetnesDecreaseFarctor * (currTileAndDepth.Value / wetnesDecreaseFrequency) < 1) {

                    visitedTiles.Add(currTileAndDepth.Key);
                    currTileAndDepth.Key.wetness += 1 - wetnesDecreaseFarctor
                        * (currTileAndDepth.Value / wetnesDecreaseFrequency); //1 is max wet

                    if (currTileAndDepth.Key.wetness > 1) {
                        currTileAndDepth.Key.wetness = 1;
                    }

                    foreach (Tile neighour in currTileAndDepth.Key.neighbours) {
                        tilesToVisit.Enqueue(new KeyValuePair<Tile, int>(neighour, currTileAndDepth.Value + 1));
                    }
                }

            }
        }

    }

    private void connectWithGraph(Tile tileToConnect, List<Tile> graph) {
        foreach (Tile tile in graph) {
            if (tileToConnect.shouldBeNeighbourTo(tile, planetGraphType)) {
                tileToConnect.addNeighoursConnection(tile);
            }
        }
    }

    public Tile getTileWithPos(Vector2Int virtualCoordinates) {
        foreach (Tile tile in allPlanetTiles) {
            if ((tile.virtualCoordinates.x == virtualCoordinates.x) && (tile.virtualCoordinates.y == virtualCoordinates.y)) {
                return tile;
            }
        }
        return null;
    }

    private void debugNeighbours() {
        foreach (Tile tile in Planet.instance.planetGraphInfo.allPlanetTiles) {
            Debug.Log(tile.neighbours.Count);
            foreach (Tile neighb in tile.neighbours) {
                Debug.Log(neighb);
            }
        }
    }
    /*OLD
     * public void generateMatrixLikeGraph(Tile startingTile, int matrixWidth) {
        Vector2Int currPos;

        for (int currMatrixWidth = 1; currMatrixWidth < matrixWidth; currMatrixWidth++) {
            currPos = new Vector2Int(-currMatrixWidth, currMatrixWidth); //starts from lower upper corner
            bool circleCompleted = false;

            List<Vector2Int> dirrs = new List<Vector2Int>() {
                new Vector2Int(1,0),new Vector2Int(0,-1),new Vector2Int(-1,0),new Vector2Int(0,1)
            };
            int dirInd = 0;
            while (!circleCompleted) {
                Vector2Int dirr = dirrs[dirInd++];
                for (int numOfEdgeTiles = 0; numOfEdgeTiles < currMatrixWidth * 2; numOfEdgeTiles++) {
                    allPlanetTiles.Add(new Tile(currPos));
                    currPos += dirr;
                }
                if (dirInd >= dirrs.Count) {
                    circleCompleted = true;
                }
            }
        }

        foreach(Tile tile in allPlanetTiles) {
            connectWithGraph(tile, allPlanetTiles);
        }
#if DebugNeighbours
        debugNeighbours();
#endif
    }*/
}
    
