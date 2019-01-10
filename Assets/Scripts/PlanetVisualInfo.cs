#define DEBUG

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlanetVisualInfo {
    public GameObject tilePrefab;

    public int normalVisionWidth;

    public Color clrNoWater = new Color(193f / 256, 68f / 256, 14f / 256);
    public Color clrMuchWater = new Color(69f / 256, 24f / 256, 4f / 256);
    public Color clrWater;

    public Vector3 realWorldGraphCenter;

    public void instantiateVisuals(Tile graphCenterTile) {
        List<Tile> tilesToRender = Planet.getTilesInDepth(Planet.instance.planetGraphInfo.currPlanetTiles[0], normalVisionWidth);

        if (graphCenterTile.TileObject != null)
            realWorldGraphCenter = graphCenterTile.TileObject.transform.position;

        foreach (Tile tile in tilesToRender) {
            GameObject tileObject = GameObject.Instantiate(tilePrefab, new Vector3
                (tile.virtualCoordinates.x * tilePrefab.GetComponent<Transform>().localScale.x + realWorldGraphCenter.x, realWorldGraphCenter.z,
                tile.virtualCoordinates.y * tilePrefab.GetComponent<Transform>().localScale.y + realWorldGraphCenter.y), Quaternion.identity);

            tileObject.transform.parent = Planet.instance.gameObject.transform;
            generateTileColor(ref tileObject, tile.Wetness);
            tile.TileObject = tileObject;
        }
    }

    private void generateTileColor(ref GameObject tile, float wetness) {
        var sr = tile.GetComponentInChildren<SpriteRenderer>();
        sr.color = Color.Lerp(clrNoWater, clrMuchWater, wetness);

        if (wetness >= 0.9f) {
            sr.color = clrWater;
        }
    }

    public void renderCurrentVisionForAgent(Agent agent) {
        if (agent != null) {
            renderOnlyThisTiles(agent.currentVision);
        }
    }

    public void renderMemoryForAgent(Agent agent) {
        if (agent != null) {
            renderOnlyThisTiles(getTilesFromMemory(agent.agentMemory));
        }
    }

    private List<Tile> getTilesFromMemory(List<TileMemory> agentMemory) {
        List<Tile> tiles = new List<Tile>();
        foreach (TileMemory tileMemory in agentMemory) {
            tiles.Add(tileMemory.tile);
        }
        return tiles;
    }

    public void renderOnlyThisTiles(List<Tile> tilesToRender) {
        foreach (Tile tile in Planet.instance.planetGraphInfo.currPlanetTiles) {
            if (tile.SpriteRenderer != null) {
                tile.SpriteRenderer.enabled = false;
            }
        }
        foreach (Tile tile in tilesToRender) {
            if (tile.SpriteRenderer != null) {
                tile.SpriteRenderer.enabled = true;
            }
        }
    }

}
