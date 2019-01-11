using Cinemachine;
using UnityEngine;

public class AgentAtacher : MonoBehaviour {
    public GameObject cam;
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            attachAgent(getClickedAgent());
        }

        if (Input.GetKeyDown(KeyCode.M)) {
            Planet.instance.currentRenderMode = RenderMode.Memory;
            Planet.instance.rendererAgent();
        }
        else if (Input.GetKeyDown(KeyCode.V)) {
            Planet.instance.currentRenderMode = RenderMode.Vision;
            Planet.instance.rendererAgent();
        }
        else if (Input.GetKeyDown(KeyCode.C)) {
            Planet.instance.detachAgent();
        }
        else if (Input.GetKeyDown(KeyCode.X)) {
            cam.GetComponent<CinemachineVirtualCamera>().Follow = null;
            Planet.instance.detachAgent();
        }
    }

    private void attachAgent(Agent agentToAttach) {
        if (agentToAttach != null) {
            cam.GetComponent<CinemachineVirtualCamera>().Follow = agentToAttach.gameObject.transform;

            foreach (var agent in Base.instance.agents) {
                if (agent != agentToAttach) {
                    agent.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
                }
            }
            Planet.instance.currentAgentToRender = agentToAttach;
        }
    }

    private static Agent getClickedAgent() {
        Ray touchRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits = Physics.RaycastAll(touchRay);

        foreach (RaycastHit hit in hits) {
            if (hit.collider.gameObject.tag == "Agent") {
                var agentObj = hit.collider.gameObject;
                return agentObj.GetComponent<Agent>();
            }
        }
        return null;
    }
}
