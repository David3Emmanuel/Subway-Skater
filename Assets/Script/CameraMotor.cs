using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform lookAt;
    public Vector3 offset = new Vector3(0, 6.0f, -7.0f);
    public Vector3 rotation = new(35, 0, 0);
    public float catchUpSpeed = 1.0f;
    public float turnSpeed = 1.0f;

    public bool IsMoving { get; set; }

    void LateUpdate()
    {
        if (!IsMoving) return;

        Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, catchUpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), turnSpeed * Time.deltaTime);
    }
}
