using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    // Level Spawning

    // List of pieces
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longBlocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();
    public List<Piece> allPieces = new List<Piece>();

    public Piece GetPiece(PieceType type, int visualIndex) {
        Piece piece = allPieces.Find(p => p.type == type && p.visualIndex == visualIndex && !p.gameObject.activeSelf);
        if (piece == null) {
            GameObject pieceObject = null;
            if (type == PieceType.RAMP) {
                pieceObject = ramps[visualIndex].gameObject;
            } else if (type == PieceType.LONG_BLOCK) {
                pieceObject = longBlocks[visualIndex].gameObject;
            } else if (type == PieceType.JUMP) {
                pieceObject = jumps[visualIndex].gameObject;
            } else if (type == PieceType.SLIDE) {
                pieceObject = slides[visualIndex].gameObject;
            }

            piece = Instantiate(pieceObject).GetComponent<Piece>();
            allPieces.Add(piece);
        }
        return piece;
    }
}
