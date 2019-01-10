using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAtacher : MonoBehaviour {
    public GameObject cam;
	void Update () {
        if(Input.GetMouseButtonDown(0)) {
            attachAgent(getClickedAgent());
        }

        if (Input.GetKeyDown(KeyCode.M))
            Planet.instance.currentRenderMode = RenderMode.Memory;
        else if (Input.GetKeyDown(KeyCode.V))
            Planet.instance.currentRenderMode = RenderMode.Vision;
        else if (Input.GetKeyDown(KeyCode.X)) { 
            detachAgent();
        }
    }

    private void detachAgent() {
        cam.GetComponent<CinemachineVirtualCamera>().Follow = null;

        Planet.instance.currentAgentToRender = Base.instance;
        Planet.instance.currentRenderMode = RenderMode.Free;

        foreach (Agent agent in Base.instance.agents) {
            agent.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }

        Planet.instance.planetVisualInfo.renderOnlyThisTiles(Planet.getTilesInDepth(
            Base.instance.currentTile, Planet.instance.planetVisualInfo.normalVisionWidth));
    }

    private void attachAgent(MovingAgent agentToAttach) {
        if (agentToAttach!=null) {
            cam.GetComponent<CinemachineVirtualCamera>().Follow = agentToAttach.gameObject.transform;

            foreach (MovingAgent agent in Base.instance.agents) {
                if (agent != agentToAttach)
                    agent.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            Planet.instance.currentAgentToRender = agentToAttach;
        }
    }

    private static MovingAgent getClickedAgent() {
        Ray touchRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits = Physics.RaycastAll(touchRay);

        foreach (RaycastHit hit in hits) {
            if (hit.collider.gameObject.name == "Agent") {
                var agentObj = hit.collider.gameObject;
                return agentObj.GetComponent<MovingAgent>();
            }
        }
        return null;
    }
}
