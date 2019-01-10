using System.Collections.Generic;
using UnityEngine;

public class Base : Agent {
    public static Base instance;
    [HideInInspector]
    public Vector3Int position;

    public List<MovingAgent> agents;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }

        agentMemory = new List<TileMemory>();
    }

    void Start() {
        setInitialPosition();
        foreach(MovingAgent agent in agents) {
            agent.setInitialPosition();
        }
    }

    void setInitialPosition() { //uses real world matrix positions, to improve maybe
        transform.position = Planet.instance.graphCenterTile.TileObject.transform.position;
        currentTile = Planet.instance.planetGraphInfo.currPlanetTiles[0];
        position = Planet.instance.planetGraphInfo.currPlanetTiles[0].virtualCoordinates;
    }

}
