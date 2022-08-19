using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChessModel;
using Exploder.Utils;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    private MaterialManager _materialManager;
    
    private const int BoardsToInitialise = 1;

    public GameObject PawnPiece;
    private List<GameObject> pooledPawn;
    public GameObject RookPiece;
    private List<GameObject> pooledRook;
    public GameObject BishopPiece;
    private List<GameObject> pooledBishop;
    public GameObject KnightPiece;
    private List<GameObject> pooledKnignt;
    public GameObject KingPiece;
    private List<GameObject> pooledKing;
    public GameObject QueenPiece;
    private List<GameObject> pooledQueen;
    
    private void instantiateLootObjetcts(GameObject objectToPool, ref List<GameObject> pooledObjects)
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        int piecesToInitialise = 0;
        if (objectToPool == PawnPiece)
            piecesToInitialise = 16 * BoardsToInitialise;
        else if(objectToPool == RookPiece || objectToPool == BishopPiece || objectToPool == KnightPiece || objectToPool == QueenPiece)
            piecesToInitialise = 6 * BoardsToInitialise;
        else if(objectToPool == KingPiece)
            piecesToInitialise = 2 * BoardsToInitialise;
        for (int i = 0; i < piecesToInitialise; i++)
        {
            tmp = Instantiate(objectToPool, new Vector3(), Quaternion.identity, transform);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    public GameObject getPooledPiece(ChessType chessType, ChessColor color, Vector3 placement)
    {
        GameObject ChessPiece; 
        switch (chessType)
        {
            case ChessType.Bishop:
                ChessPiece = GetPooledObject(pooledBishop);
                break;
            case ChessType.King:
                ChessPiece = GetPooledObject(pooledKing);
                break;
            case ChessType.Knight:
                ChessPiece = GetPooledObject(pooledKnignt);
                break;
            case ChessType.Pawn:
                ChessPiece = GetPooledObject(pooledPawn);
                break;
            case ChessType.Queen:
                ChessPiece = GetPooledObject(pooledQueen);
                break;
            case ChessType.Rook:
                ChessPiece = GetPooledObject(pooledRook);
                break;
            default:
                return null;
        }
        ChessPiece = _materialManager.changeMaterial(ChessPiece , chessType, color);
        
        ChessPiece.GetComponent<PiecePieces>().IsWhite = color==ChessColor.White;
        ChessPiece.transform.position = placement;
        ChessPiece.SetActive(true);
        ExploderSingleton.Instance.CrackObject(ChessPiece);
        return ChessPiece;
    }
    
    public GameObject GetPooledObject(List<GameObject> pooledObjects)
    {
        return pooledObjects.Find(pooledObject => !pooledObject.activeInHierarchy);
    }
    
    void Start()
    {
        _materialManager = GetComponent<MaterialManager>();
        
        instantiateLootObjetcts(PawnPiece, ref pooledPawn);
        instantiateLootObjetcts(BishopPiece, ref pooledBishop);
        instantiateLootObjetcts(RookPiece, ref pooledRook);
        instantiateLootObjetcts(KingPiece, ref pooledKing);
        instantiateLootObjetcts(KnightPiece, ref pooledKnignt);
        instantiateLootObjetcts(QueenPiece, ref pooledQueen);
    }
}
