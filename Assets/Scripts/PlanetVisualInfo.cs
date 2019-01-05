using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlanetVisualInfo {
    public GameObject tilePrefab;

    public void instantiateVisuals() {
        foreach (Tile tile in Planet.instance.planetGraphInfo.allPlanetTiles) {
            GameObject tileObject = GameObject.Instantiate(tilePrefab, new Vector3
                (tile.coorcinates.x, 0, tile.coorcinates.y), Quaternion.identity);
            generateTileColor(ref tileObject, tile.tileType);
            tile.tileObject = tileObject;
        }
    }


    private void generateTileColor(ref GameObject tile, TileType tileType) {
        var sr = tile.GetComponentInChildren<SpriteRenderer>();
        switch (tileType) {
            case TileType.Dirt:
                sr.color = new Color(0.831f, 0.604f, 0.416f);
                break;
            case TileType.Water:
                sr.color = new Color(0.251f, 0.498f, 0.498f);
                break;
            case TileType.Stone:
                sr.color = new Color(1, 1, 1);
                break;
        }
    }

    public void renderCurrentVisionForAgent(Agent agent) {
        if (agent != null) {
            renderOnlyThisTiles(agent.currentVision);
        }
    }

    public void renderMemoryForAgent(Agent agent) {
        if (agent != null) {
            renderOnlyThisTiles(agent.agentMemory);
        }
    }

    public void renderAllTiles() {
        List<Tile> allTiles = new List<Tile>();
        foreach (Tile tile in Planet.instance.planetGraphInfo.allPlanetTiles) {
            allTiles.Add(tile);
        }
        renderOnlyThisTiles(allTiles);
    }
    private void renderOnlyThisTiles(List<Tile> tilesToRender) {
        foreach (Tile tile in Planet.instance.planetGraphInfo.allPlanetTiles) {
            if (tilesToRender.Contains(tile)) {
                tile.tileObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
            else {
                tile.tileObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
        }
    }

}
