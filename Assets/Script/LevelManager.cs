using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private const bool SHOW_COLLIDER = true; // NOTE: Set to false when done testing

    // Level Spawning
    private const float DISTANCE_BEFORE_SPAWN = 100.0f;
    private const int INITIAL_SEGMENTS = 10;
    private const int MAX_SEGMENTS_ON_SCREEN = 15;
    private Transform cameraPosition;
    private int amountOfActiveSegments;
    private int continuousSegments;
    private int currentSpawnZ;
    private int currentLevel;
    private int y1, y2, y3;

    // List of pieces
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longBlocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();
    [HideInInspector] public List<Piece> allPieces = new List<Piece>();

    // List of Segments
    public List<Segment> availableSegments = new List<Segment>();
    public List<Segment> availableTransitions = new List<Segment>();
    [HideInInspector] public List<Segment> allSegments = new List<Segment>();

    private bool isMoving = false;

    void Awake()
    {
        Instance = this;
        cameraPosition = Camera.main.transform;
        currentSpawnZ = 0;
        currentLevel = 0;
    }

    void Start()
    {
        for (int i = 0; i < INITIAL_SEGMENTS; i++) GenerateSegment();
    }

    void GenerateSegment() {
        SpawnSegment();

        if (Random.Range(0f, 1f) < (continuousSegments * 0.25f)) {
            continuousSegments = 0;
            SpawnTransition();
        } else {
            continuousSegments++;
        }
    }

    void SpawnSegment() {
        List<Segment> possibleSegments = availableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleSegments.Count);

        Segment segment = GetSegment(id, false);
        y1 = segment.endY1;
        y2 = segment.endY2;
        y3 = segment.endY3;
        segment.transform.parent = transform;
        segment.transform.localPosition = Vector3.forward * currentSpawnZ;
        currentSpawnZ += segment.length;
        amountOfActiveSegments++;
        segment.Spawn();
    }
    
    void SpawnTransition() {
        List<Segment> possibleTransitions = availableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTransitions.Count);

        Segment segment = GetSegment(id, true);
        y1 = segment.endY1;
        y2 = segment.endY2;
        y3 = segment.endY3;
        segment.transform.parent = transform;
        segment.transform.localPosition = Vector3.forward * currentSpawnZ;
        currentSpawnZ += segment.length;
        amountOfActiveSegments++;
        segment.Spawn();
    }

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

public Segment GetSegment(int id, bool isTransition) {
    Segment segment = null;
    segment = allSegments.Find(s => s.SegId == id && s.gameObject.activeSelf == false && s.isTransition == isTransition);
    if (segment == null) {
        GameObject segmentObject = Instantiate((isTransition) ? availableTransitions[id].gameObject : availableSegments[id].gameObject) as GameObject;
        segment = segmentObject.GetComponent<Segment>();
        segment.SegId = id;
        segment.isTransition = isTransition;
        allSegments.Insert(0, segment);
    } else {
        allSegments.Remove(segment);
        allSegments.Insert(0, segment);
    }
    return segment;
}

}