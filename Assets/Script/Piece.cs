using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    NONE,
    RAMP,
    LONG_BLOCK,
    JUMP,
    SLIDE,
}

public class Piece : MonoBehaviour
{
    public PieceType type;
    public int visualIndex;
}
