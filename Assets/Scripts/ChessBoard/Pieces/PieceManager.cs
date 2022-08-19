using System.Collections;
using System.Collections.Generic;
using ChessModel;
using UnityEngine;
using UnityEngine.AI;

public class PieceManager : MonoBehaviour
{
    
    private BoardManager _boardManager;
    public GameObject explosion;

    void Awake()
    {
        _boardManager = GetComponentInParent<BoardManager>();
    }
    
    public void MovePiece(GameObject piece, Vector3 placement, bool rock = false)
    {
        piece.GetComponent<PiecePieces>().Move(placement, rock);
    }

    public void AttackWithPiece(GameObject piece, Vector3 placement, Vector3 enemyPlacement, GameObject enemy)
    {
        piece.GetComponent<PiecePieces>().Attack(placement, enemyPlacement, enemy);
    }

    public void FinishedAnim()
    {
        _boardManager.NextTurn();
    }
}
