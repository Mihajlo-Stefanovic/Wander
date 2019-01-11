using Cinemachine;
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
    [Header("For generation")]
    public Vector3Int virtualCenter; //hack for generating

    [Range(0, 20)]
    public int speed;

    public float wetnessToWaterLimit = 0.9f;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }

#if (UNITY_EDITOR)
        randomSeed = Random.Range(0, int.MaxValue);
#endif
        currentRenderMode = RenderMode.Free;

        graphCenterTile = planetGraphInfo.initializeGraph();
    }

    void Start() {
        planetVisualInfo.instantiateVisuals(graphCenterTile);
        Base.instance.initialize(graphCenterTile);
    }

    void Update() {
#if (UNITY_EDITOR)
        Time.timeScale = speed;
#endif
    }
    public void agentMoved(Agent agent) {
        if (agent == currentAgentToRender) {
            rendererAgent();
        }
    }

    public void rendererAgent() {
        if (currentRenderMode == RenderMode.Vision) {
            planetVisualInfo.renderCurrentVisionForAgent(currentAgentToRender);
        }
        else if (currentRenderMode == RenderMode.Memory) {
            planetVisualInfo.renderMemoryForAgent(currentAgentToRender);
        }
    }

    public static List<Tile> getTilesInDepth(Tile tile, int depth, float wetnessLimit = -1) {
        Queue<KeyValuePair<Tile, int>> tilesToVisit = new Queue<KeyValuePair<Tile, int>>();
        List<Tile> visitedTiles = new List<Tile>();

        tilesToVisit.Enqueue(new KeyValuePair<Tile, int>(tile, 0));

        while (tilesToVisit.Count != 0) {

            KeyValuePair<Tile, int> currTileAndDepth = tilesToVisit.Dequeue();

            if (!visitedTiles.Contains(currTileAndDepth.Key) && currTileAndDepth.Value <= depth
                && currTileAndDepth.Key.Wetness > wetnessLimit) {

                visitedTiles.Add(currTileAndDepth.Key);

                foreach (Tile neighour in currTileAndDepth.Key.Neighbours) {
                    tilesToVisit.Enqueue(new KeyValuePair<Tile, int>(neighour, currTileAndDepth.Value + 1));
                }
            }

        }

        return visitedTiles;
    }

    public void generateMapFor(Tile graphCenterTile) {
        Tile newStartingTile = planetGraphInfo.generateGraph(graphCenterTile, planetVisualInfo.normalVisionWidth);
        this.graphCenterTile = newStartingTile;
        planetVisualInfo.instantiateVisuals(newStartingTile);
        Base.instance.reposition(newStartingTile);
    }

    public void detachAgent() {
        Planet.instance.currentAgentToRender = Base.instance;
        Planet.instance.currentRenderMode = RenderMode.Free;

        foreach (MovingAgent agent in Base.instance.agents) {
            agent.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }

        Planet.instance.planetVisualInfo.renderOnlyThisTiles(Planet.getTilesInDepth(
            Base.instance.currentTile, Planet.instance.planetVisualInfo.normalVisionWidth));
    }
}
