#undef DebugNeighbours
using System.Collections.Generic;
using UnityEngine;

public class PlanetGraphInfo {

    public List<Tile> allPlanetTiles;

    public PlanetGraphInfo(int numOfTiles) {
        generateMatrixLikeGraph(numOfTiles);
    }

    public void generateMatrixLikeGraph(int numOfPlanetTiles) {
        allPlanetTiles = new List<Tile>();
        for (int x = 0; x < Planet.instance.NumOfPlanetTilesX; x++) {
            for (int y = 0; y < Planet.instance.NumOfPlanetTilesY; y++) {
                //maybe determine TileType here
                Tile newTile = new Tile(x, y, (TileType)UnityEngine.Random.
                    Range(0, TileType.GetNames(typeof(TileType)).Length));
                connectWithGraph(newTile);
            }
        }
#if DebugNeighbours
        debugNeighbours();
#endif
    }

    private void connectWithGraph(Tile newTile) {
        foreach (Tile tile in allPlanetTiles) {
            if (newTile.isNeighbourTo(tile)) {
                newTile.addNeighoursConnection(tile);
            }
        }
        allPlanetTiles.Add(newTile);
    }

    private void debugNeighbours() {
        foreach (Tile tile in Planet.instance.planetGraphInfo.allPlanetTiles) {
            Debug.Log(tile.neighbours.Count);
            foreach (Tile neighb in tile.neighbours) {
                Debug.Log(neighb);
            }
        }
    }

    public Tile getTileWithPos(Vector2 coordinates) {
        foreach(Tile tile in allPlanetTiles) {
            if (((int)(tile.coorcinates.x) == (int)(coordinates.x)) && ((int)(tile.coorcinates.y) == (int)(coordinates.y)))
                return tile;
        }
        return null;
    }
}
