using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float zoomSensitivity = 200;
    public float moveSensitivity = 500;

    private new CinemachineVirtualCamera camera;

    void Start() {
        camera = GetComponent<CinemachineVirtualCamera>();
    }
    void Update() {
        if (Input.GetMouseButton(0)) {
            transform.position -= new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y")) * moveSensitivity *Time.deltaTime;
        }

        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f) {
            camera.m_Lens.OrthographicSize -= zoomSensitivity * Time.deltaTime;
        }
        else if (d < 0f) {
            camera.m_Lens.OrthographicSize += zoomSensitivity * Time.deltaTime;
        }
    }
}
