using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public PieceType type;
    private Piece currentPiece;

    public void Spawn()
    {
        currentPiece = LevelManager.Instance.GetRandomPiece(type);
        currentPiece.gameObject.SetActive(true);
        currentPiece.transform.SetParent(transform);
        currentPiece.transform.localPosition = Vector3.zero;
    }

    public void DeSpawn()
    {
        currentPiece.gameObject.SetActive(false);
    }
}
