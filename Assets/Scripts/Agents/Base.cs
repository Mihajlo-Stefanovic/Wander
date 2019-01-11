using System.Collections.Generic;
using UnityEngine;

public class Base : Agent {
    public static Base instance;
    [HideInInspector]
    public Vector3Int virtualCoordinates;
    [HideInInspector]
    public Vector3 idealRealWorldPosition;
    public float realWorldSpeedOfMoving;
    [HideInInspector]
    public bool isMoving;

    public int numOfAgents;
    public GameObject agentPrefab;
    [System.NonSerialized]
    public List<MovingAgent> agents;

    private Dictionary<int, TileMemory> newAgentBroughtMemory;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }

        agents = new List<MovingAgent>();
        agentMemory = new AgentMemory();
        newAgentBroughtMemory = new Dictionary<int, TileMemory>();
    }

    void Update() {
        if (isMoving && this.transform.position.Equals(idealRealWorldPosition)) {
            baseArrived();
        }
        else if (isMoving && Input.GetKey(KeyCode.KeypadEnter)) {
            transform.position = Vector3.MoveTowards(transform.position,
            idealRealWorldPosition, realWorldSpeedOfMoving * Time.deltaTime);
        }
    }
    public void initialize(Tile staringTile) {
        setInitialPosition(staringTile);
        for (int i = 0; i < numOfAgents; i++) {
            GameObject agentObj = Instantiate(agentPrefab, transform.position, Quaternion.identity);
            agentObj.transform.parent = transform.parent;

            MovingAgent agent = agentObj.GetComponent<MovingAgent>();
            agents.Add(agent);
            agent.initialize();
        }
        generateOrders();
    }

    void setInitialPosition(Tile staringTile) {
        transform.position = Planet.instance.planetVisualInfo.getRealWorldCoordinates(staringTile);
        currentTile = staringTile;
        virtualCoordinates = staringTile.virtualCoordinates;
    }

    internal void reposition(Tile startingTile) {
        currentTile = startingTile;

        foreach (var agent in agents) {
            agent.resetAgent();
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

    public void agentArrived(MovingAgent movingAgent) {
        foreach (var pair in movingAgent.agentMemory.tileMemories) {
            if (!newAgentBroughtMemory.ContainsKey(pair.Key)) {
                newAgentBroughtMemory.Add(pair.Key, pair.Value);
            }
        }
        
        foreach (var pair in movingAgent.agentMemory.tileMemories) {
            if (!agentMemory.tileMemories.ContainsKey(pair.Key)) {
                agentMemory.tileMemories.Add(pair.Key, pair.Value);
            }
        }

        movingAgent.isInBase = true;
        movingAgent.transform.parent = this.transform;
        movingAgent.gameObject.SetActive(false);

        bool allInBase = true;
        foreach (MovingAgent agent in agents) {
            if (!agent.isInBase) {
                allInBase = false;
            }
        }

        if (allInBase) {
            baseMove();
        }
    }

    private void baseMove() {
        Tile tileToMoveTo = findBestTileForMove();
        moveTo(tileToMoveTo);
        isMoving = true;
    }

    private void moveTo(Tile tileToMoveTo) {
        currentTile = tileToMoveTo;
        idealRealWorldPosition = Planet.instance.planetVisualInfo.getRealWorldCoordinates(tileToMoveTo);
    }

    private void baseArrived() {
        isMoving = false;
        Planet.instance.generateMapFor(Planet.instance.planetGraphInfo.getTileWithPos(currentTile.virtualCoordinates));
        Planet.instance.detachAgent();

        foreach (MovingAgent agent in agents) {
            agent.gameObject.SetActive(true);
            agent.transform.parent = transform.parent;
            agent.resetAgent();
            agent.initialize();
        }
        newAgentBroughtMemory.Clear();
    }

    private Tile findBestTileForMove() {
        Tile bestTile = currentTile;
        int bestDistance = 0;

        foreach (var pair in newAgentBroughtMemory) {
            int distance = Mathf.Abs(currentTile.virtualCoordinates.x - pair.Value.tile.virtualCoordinates.x)
                + Mathf.Abs(currentTile.virtualCoordinates.y - pair.Value.tile.virtualCoordinates.y)
                + Mathf.Abs(currentTile.virtualCoordinates.z - pair.Value.tile.virtualCoordinates.z);
            if (distance > bestDistance) {
                bestDistance = distance;
                bestTile = pair.Value.tile;
            }
            Debug.Log(bestTile.name + " " + bestDistance);
        }
        return bestTile;
    }
}
