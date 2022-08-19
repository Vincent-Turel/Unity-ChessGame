using System.Collections;
using System.Collections.Generic;
using ChessModel;
using UnityEngine;

public class MaterialManager : MonoBehaviour {
    
    public Material PawnWhite;
    public Material PawnBlack;
    public Material BishopWhite;
    public Material BishopBlack;
    public Material RookWhite;
    public Material RookBlack;
    public Material KingWhite;
    public Material KingBlack;
    public Material KnightWhite;
    public Material KnightBlack;
    public Material QueenWhite;
    public Material QueenBlack;
    
    public GameObject changeMaterial(GameObject Piece, ChessType chessType, ChessColor color)
    {
        switch (chessType) 
        {
            case ChessType.Bishop: 
                Piece.GetComponentInChildren<SkinnedMeshRenderer>().material =
                    color == ChessColor.Black ? BishopBlack : BishopWhite; 
                break;
            case ChessType.Rook:
                Piece.GetComponentInChildren<SkinnedMeshRenderer>().material =
                    color == ChessColor.Black ? RookBlack : RookWhite;
                break;
            case ChessType.Pawn: 
                Piece.GetComponentInChildren<SkinnedMeshRenderer>().material =
                    color == ChessColor.Black ? PawnBlack : PawnWhite;
                break;
            case ChessType.King: 
                Piece.GetComponentInChildren<SkinnedMeshRenderer>().material =
                    color == ChessColor.Black ? KingBlack : KingWhite;
                break;
            case ChessType.Knight: 
                Piece.GetComponentInChildren<SkinnedMeshRenderer>().material =
                    color == ChessColor.Black ? KnightBlack : KnightWhite;
                break;
            case ChessType.Queen: 
                Piece.GetComponentInChildren<SkinnedMeshRenderer>().material =
                    color == ChessColor.Black ? QueenBlack : QueenWhite;
                break;
        }
        return Piece;
    }
}
