using System.Collections.Generic;
using UnityEngine;
public enum RenderMode {
    Vision, Memory, Free
}
public class Planet : MonoBehaviour {
    public static Planet instance;

    public int randomSeed;

    public PlanetGraphInfo planetGraphInfo;
    public PlanetVisualInfo planetVisualInfo;
    [System.NonSerialized]
    public Agent currentAgentToRender;
    [System.NonSerialized]
    public RenderMode currentRenderMode;

    public Tile graphCenterTile;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }

        currentRenderMode = RenderMode.Free;

        graphCenterTile = planetGraphInfo.initializeGraph();
    }

    void Start() {
        planetVisualInfo.instantiateVisuals(graphCenterTile);
    }

    public void agentMoved(Agent agent) {
        if (agent == currentAgentToRender) {
            rendererAgent();
        }
    }

    private void rendererAgent() {
        if (currentRenderMode == RenderMode.Vision) {
            planetVisualInfo.renderCurrentVisionForAgent(currentAgentToRender);
        }
        else if (currentRenderMode == RenderMode.Memory) {
            planetVisualInfo.renderMemoryForAgent(currentAgentToRender);
        }
    }

    public static List<Tile> getTilesInDepth(Tile tile, int depth) {
        Queue<KeyValuePair<Tile, int>> tilesToVisit = new Queue<KeyValuePair<Tile, int>>();
        List<Tile> visitedTiles = new List<Tile>();

        tilesToVisit.Enqueue(new KeyValuePair<Tile, int>(tile, 0));

        while (tilesToVisit.Count != 0) {

            KeyValuePair<Tile, int> currTileAndDepth = tilesToVisit.Dequeue();

            if (!visitedTiles.Contains(currTileAndDepth.Key) && currTileAndDepth.Value<=depth) {

                visitedTiles.Add(currTileAndDepth.Key);

                foreach (Tile neighour in currTileAndDepth.Key.Neighbours) {
                    tilesToVisit.Enqueue(new KeyValuePair<Tile, int>(neighour, currTileAndDepth.Value + 1));
                }
            }

        }

        return visitedTiles;
    }

    public void generateMapFor(Tile tile) {
        planetGraphInfo.generateGraph(tile, planetVisualInfo.normalVisionWidth);
        planetVisualInfo.instantiateVisuals(tile);
    }
}
