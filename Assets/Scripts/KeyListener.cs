#define DEBUG
using UnityEngine;

public class KeyListener : MonoBehaviour {
    public float speedChangingSpeed;
    void Start() {

    }

    void Update() {
#if !(UNITY_EDITOR)
        if (Input.GetKey(KeyCode.UpArrow)) {
            if (Time.timeScale + speedChangingSpeed * Time.deltaTime < 100) {
                Time.timeScale = Time.timeScale + speedChangingSpeed * Time.deltaTime;
            }
            else {
                Time.timeScale = 100;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow)) {
            if (Time.timeScale - speedChangingSpeed * Time.deltaTime > 0) {
                Time.timeScale -= speedChangingSpeed * Time.deltaTime;
            }
            else {
                Time.timeScale = 0;
            }
        }
#endif
    }
}
