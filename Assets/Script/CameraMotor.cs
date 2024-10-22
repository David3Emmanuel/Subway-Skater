using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform lookAt;
    public Vector3 idleOffset = new Vector3(0, 1.0f, -2.0f);
    public Vector3 runningOffset = new Vector3(0, 1.25f, 0);

    void Start() {
        transform.position = lookAt.position + idleOffset;
    }

    void LateUpdate() {
        Vector3 offset = runningOffset;
        if (!GameManager.Instance.IsGameStarted) {
            offset = idleOffset;
        }
        Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime);
    }
}
