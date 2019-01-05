using System.Collections.Generic;
using UnityEngine;

public class Base : Agent {
    public static Base instance;
    [HideInInspector]
    public Vector2 position;

    public List<MovingAgent> agents;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }

        agentMemory = new List<Tile>();
    }

    void Start() {
        setInitialPosition();
        foreach(MovingAgent agent in agents) {
            agent.setInitialPosition();
        }
    }

    void setInitialPosition() { //uses real world matrix positions, to improve maybe
        transform.position = new Vector3(Planet.instance.NumOfPlanetTilesX / 2,
            transform.position.y, Planet.instance.NumOfPlanetTilesY / 2);

        position = new Vector2(Planet.instance.NumOfPlanetTilesX / 2, Planet.instance.NumOfPlanetTilesY / 2);
        currentTile = Planet.instance.planetGraphInfo.getTileWithPos(position);
    }

}
