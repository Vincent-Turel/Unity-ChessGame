using System;
using System.Collections;
using System.Collections.Generic;
using ChessModel;
using UnityEngine;

public class PromotionUIScript : MonoBehaviour {

    private Piece piece;
    public BoardManager _boardManager;
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void promotionSelect(ChessType chessType)
    {
        _boardManager.GivePromotion(piece, chessType);
        gameObject.SetActive(false);
    }

    public void promotionSelectRook()
    {
        promotionSelect(ChessType.Rook);
    }
    
    public void promotionSelectBishop()
    {
        promotionSelect(ChessType.Bishop);
    }
    
    public void promotionSelectQueen()
    {
        promotionSelect(ChessType.Queen);
    }
    
    public void promotionSelectKnight()
    {
        promotionSelect(ChessType.Knight);
    }

    public void show(Piece piece)
    {
        gameObject.SetActive(true);
        this.piece = piece;
    }

    private IEnumerator showCorRountine()
    {
        yield return new WaitForSeconds(0.8f);
        gameObject.SetActive(true);
    }
}
