using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public int SegId { get; set; }
    public bool isTransition;

    public int length;
    public int beginY1, beginY2, beginY3;
    public int endY1, endY2, endY3;

    private PieceSpawner[] pieces;

    void Awake()
    {
        pieces = GetComponentsInChildren<PieceSpawner>();
        foreach (PieceSpawner piece in pieces)
        {
            foreach (MeshRenderer mr in piece.GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = LevelManager.Instance.SHOW_COLLIDER;
            }
        }
    }

    public void Spawn()
    {
        gameObject.SetActive(true);
        foreach (PieceSpawner piece in pieces) piece.Spawn();
    }

    public void DeSpawn()
    {
        gameObject.SetActive(false);
        foreach (PieceSpawner piece in pieces) piece.DeSpawn();
    }
}
