using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RenderMode {
    Vision,Memory,Free
}
public class Planet : MonoBehaviour {
    public static Planet instance;

    public PlanetGraphInfo planetGraphInfo;
    public PlanetVisualInfo planetVisualInfo;

    public Agent currentAgentToRender;
    public RenderMode currentRenderMode;

    [SerializeField]
    private int numOfPlanetTiles;

    public int NumOfPlanetTilesX {
        get {
            return (int)Mathf.Sqrt(numOfPlanetTiles);
        }
    }
    [HideInInspector]
    public int NumOfPlanetTilesY {
        get {
            return (int)Mathf.Sqrt(numOfPlanetTiles);
        }
    }

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        currentRenderMode = RenderMode.Free;

        planetGraphInfo = new PlanetGraphInfo(numOfPlanetTiles);
    }

    void Start() {
        planetVisualInfo.instantiateVisuals();
    }

    public void agentMoved(Agent agent) {
        if (agent == currentAgentToRender) {
            rendererAgent();
        }
    }

    private void rendererAgent() {
        if (currentRenderMode == RenderMode.Vision)
            planetVisualInfo.renderCurrentVisionForAgent(currentAgentToRender);
        else if (currentRenderMode == RenderMode.Memory)
            planetVisualInfo.renderMemoryForAgent(currentAgentToRender);
    }
}
