using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlanetVisualInfo {
    public GameObject tilePrefab;
    public Color clrNoWater = new Color(193f / 256, 68f / 256, 14f / 256);
    public Color clrWater = new Color(69f / 256, 24f / 256, 4f / 256);

    public void instantiateVisuals() {
        foreach (Tile tile in Planet.instance.planetGraphInfo.allPlanetTiles) {
            GameObject tileObject = GameObject.Instantiate(tilePrefab, new Vector3
                (tile.coorcinates.x, 0, tile.coorcinates.y), Quaternion.identity);
            generateTileColor(ref tileObject, tile.wetness);
            tile.tileObject = tileObject;
        }
    }


    private void generateTileColor(ref GameObject tile, float wetness) {
        var sr = tile.GetComponentInChildren<SpriteRenderer>();
        sr.color = Color.Lerp(clrNoWater, clrWater, wetness);
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
