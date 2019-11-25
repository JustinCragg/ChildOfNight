using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************************************************************
// Class for controlling the movement of the camera
************************************************************************************************************/
public class CameraControl : MonoBehaviour {
    [Tooltip("The offset from the player that the camera maintains")]
    public Vector3 cameraOffset = new Vector3(0, 10, -10);
    // Stores the momentum for zooming in and out
    float momentum = 0;

    // Reference to the player
    PlayerControl player;

    void Update() {
        // Momentum falls off over time
        momentum *= 0.5f;
        momentum += Input.GetAxis("CameraZoom");
        cameraOffset.y = Mathf.Clamp(cameraOffset.y - momentum, 5, 20);
        cameraOffset.z = Mathf.Clamp(cameraOffset.z + momentum, -20, -5);
    }
}
