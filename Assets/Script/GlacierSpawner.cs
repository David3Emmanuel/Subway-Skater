using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacierSpawner : MonoBehaviour
{
    private const float DISTANCE_TO_RESPAWN = 10.0f;
    public float scrollSpeed = 2.0f;
    public float totalLength;
    public bool IsScrolling { set; get; }

    private float scrollLocation;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!IsScrolling) return;

        scrollLocation -= scrollSpeed * Time.deltaTime;
        Vector3 newLocation = (player.position.z + scrollLocation) * Vector3.forward;
        transform.position = newLocation;

        Transform firstChild = transform.GetChild(0);
        if (firstChild.position.z < player.position.z - DISTANCE_TO_RESPAWN)
        {
            MoveGlacier(firstChild);
            Transform secondChild = transform.GetChild(0);
            MoveGlacier(secondChild);
        }
    }

    private void MoveGlacier(Transform glacierTransform)
    {
        glacierTransform.localPosition += Vector3.forward * totalLength;
        glacierTransform.SetSiblingIndex(transform.childCount);
    }
}
