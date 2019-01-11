using System.Collections.Generic;
using UnityEngine;

public class Base : Agent {
    public static Base instance;
    [HideInInspector]
    public Vector3Int position;

    public int numOfAgents;
    public GameObject agentPrefab;
    [System.NonSerialized]
    public List<MovingAgent> agents;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }

        agents = new List<MovingAgent>();
        agentMemory = new AgentMemory();
    }

    public void initialize(Tile staringTile) {
        setInitialPosition(staringTile);
        for (int i = 0; i < numOfAgents; i++) {
            GameObject agentObj = Instantiate(agentPrefab, transform.position, Quaternion.identity);
            agentObj.transform.parent = transform.parent;

            MovingAgent agent = agentObj.GetComponent<MovingAgent>();
            agents.Add(agent);
            agent.setInitialPosition();
        }
        generateOrders();
    }

    void setInitialPosition(Tile staringTile) {
        transform.position = Planet.instance.planetVisualInfo.getRealWorldCoordinates(staringTile);
        currentTile = staringTile;
        position = staringTile.virtualCoordinates;
    }

    internal void reposition(Tile startingTile) {
        currentTile = startingTile;

        foreach (var agent in agents) {
            agent.resetAgent(startingTile);
        }
    }

    private void generateOrders() {
        Vector2 dirr = Vector2.up;
        foreach (MovingAgent agent in agents) {
            dirr = rotateVec(dirr, 360f / agents.Count);

            dirr *= 1 / (Mathf.Abs(dirr.x) + Mathf.Abs(dirr.y));

            MoveData moveData = new MoveData();
            if (Mathf.Abs(dirr.x) > Constants.FLOATEPSILON) {
                if (dirr.x > 0) {
                    moveData.dirPercent.Add(DirectionEnum.right, dirr.x * 100);
                }
                else {
                    moveData.dirPercent.Add(DirectionEnum.left, -dirr.x * 100);
                }
            }

            if (Mathf.Abs(dirr.y) > Constants.FLOATEPSILON) {
                if (dirr.y > 0) {
                    moveData.dirPercent.Add(DirectionEnum.up, dirr.y * 100);
                }
                else {
                    moveData.dirPercent.Add(DirectionEnum.down, -dirr.y * 100);
                }
            }

            agent.orders = moveData;
        }
    }

    public Vector2 rotateVec(Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}
