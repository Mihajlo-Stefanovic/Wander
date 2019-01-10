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
    public int wetImpact {
        get {
            return (int)Mathf.Ceil((1 / wetnesDecreaseFarctor) * wetnesDecreaseFrequency);
        }
    }
    [System.NonSerialized]
    public List<Tile> currPlanetTiles = new List<Tile>();

    public Tile initializeGraph() {
        initialGraphWidth = Planet.instance.planetVisualInfo.normalVisionWidth;
        Tile firstTile = new Tile(0, 0, 0);
        currPlanetTiles.Add(firstTile);

        switch (planetGraphType) {
            case PlanetGraphType.matrix:
                generateMatrixLikeGraph(firstTile, initialGraphWidth + wetImpact);
                break;
            case PlanetGraphType.hexagonal:
                generateHexagonGraph();
                break;
        }
        generateTileWetness(firstTile, initialGraphWidth + wetImpact);
        postProcesTileWetness(firstTile, initialGraphWidth);
        return firstTile;
    }

    public void generateGraph(Tile startingTile, int graphWidth) {
        switch (planetGraphType) {
            case PlanetGraphType.matrix:
                generateMatrixLikeGraph(startingTile, graphWidth + wetImpact);
                break;
            case PlanetGraphType.hexagonal:
                generateHexagonGraph();
                break;
        }
        generateTileWetness(startingTile, graphWidth + wetImpact);
        postProcesTileWetness(startingTile, graphWidth);
    }

    private void generateMatrixLikeGraph(Tile startingTile, int graphWidth) {
        int numOfTiles = (graphWidth * 4) + (4 * (graphWidth * (graphWidth - 1) / 2)) + 1;

        int currNumofTiles = 0;

        Vector2Int[] neighboursPos = {new Vector2Int(-1,0), new Vector2Int(+1,0), new Vector2Int(0,-1)
                , new Vector2Int (0,+1)};

        Queue<Tile> tilesToGenerateAround = new Queue<Tile>();
        tilesToGenerateAround.Enqueue(startingTile);
        currNumofTiles = 1;

        while (currNumofTiles < numOfTiles) {
            Tile currTileToGenerateAround = tilesToGenerateAround.Dequeue();
            foreach (Vector2Int pos in neighboursPos) {
                Vector3Int coor = new Vector3Int(pos.x, pos.y, 0);
                bool add = true;
                foreach (Tile tile in currTileToGenerateAround.Neighbours) {
                    if (tile.virtualCoordinates.Equals(coor + currTileToGenerateAround.virtualCoordinates)) {
                        add = false;
                    }
                }
                if (add) {
                    Tile newTile = new Tile(coor + currTileToGenerateAround.virtualCoordinates);
                    currPlanetTiles.Add(newTile);
                    currTileToGenerateAround.addNeighoursConnection(newTile);
                    //hack for connecting
                    foreach (Tile tile in currTileToGenerateAround.Neighbours) {
                        connectWith(newTile, tile.Neighbours);
                    }
                    tilesToGenerateAround.Enqueue(newTile);
                    currNumofTiles += 1;
                }
            }
        }
#if DebugNeighbours
        debugNeighbours();
#endif
    }

    public void generateHexagonGraph() {
        //TODO implement
    }

    public void generateTileWetness(Tile startingTile, int graphWidth) {
        UnityEngine.Random.InitState(startingTile.GetHashCode());
        List<Tile> tiles = Planet.getTilesInDepth(startingTile, graphWidth);
        switch (wetnessType) {
            case WetnessType.islands:

                List<Tile> wetTiles = new List<Tile>();

                //setting watter tiles
                foreach (Tile tile in tiles) {
                    if (System.Math.Round(Random.Range(0f, 100f), 2) <= wetFrequency) {
                        tile.Wetness = 1;
                        wetTiles.Add(tile);
                    }
                }

                //spreading direct wetness
                spreadDirectWetnessBFS(wetTiles);
                break;
        }
    }

    public void postProcesTileWetness(Tile startingTile, int graphWidth) {
        List<Tile> tiles = Planet.getTilesInDepth(startingTile, graphWidth);

        //setting random oscilations in wetness
        foreach (Tile tile in tiles) {
            tile.Wetness += Random.Range(-oscilRange, oscilRange);
        }

        //spreading undirect wetness, tl;dr smudge
        for (int i = 0; i < wetnessSpread; i++) {
            foreach (Tile tile in tiles) {
                //if ((int)tile.wetness != 1) { //not changing watter tiles
                float wetness = 0;
                foreach (Tile neighbour in tile.Neighbours) {
                    wetness += neighbour.Wetness;
                }
                wetness /= tile.Neighbours.Count;
                tile.Wetness = wetness;
                //}
            }
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
                    currTileAndDepth.Key.Wetness += 1 - wetnesDecreaseFarctor
                        * (currTileAndDepth.Value / wetnesDecreaseFrequency); //1 is max wet

                    foreach (Tile neighour in currTileAndDepth.Key.Neighbours) {
                        tilesToVisit.Enqueue(new KeyValuePair<Tile, int>(neighour, currTileAndDepth.Value + 1));
                    }
                }

            }
        }

    }

    private void connectWith(Tile tileToConnect, List<Tile> listOfTiles) {
        foreach (Tile tile in listOfTiles) {
            if (tileToConnect.shouldBeNeighbourTo(tile, planetGraphType)) {
                tileToConnect.addNeighoursConnection(tile);
            }
        }
    }

    public Tile getTileWithPos(Vector3Int virtualCoordinates) {
        foreach (Tile tile in currPlanetTiles) {
            if (tile.virtualCoordinates.Equals(virtualCoordinates)) {
                return tile;
            }
        }
        return null;
    }

    private void debugNeighbours() {
        foreach (Tile tile in Planet.instance.planetGraphInfo.currPlanetTiles) {
            Debug.Log(tile.name + " num of children: " + tile.Neighbours.Count);
            foreach (Tile neighb in tile.Neighbours) {
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

