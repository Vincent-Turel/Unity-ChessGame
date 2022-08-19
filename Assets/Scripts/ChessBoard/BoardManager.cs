using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChessModel;
using Exploder;
using Exploder.Utils;
using UnityEngine;

public class BoardManager : MonoBehaviour{
    private TileManager _tileManager;
    private PieceManager _pieceManager;
    private ObjectPool _objectPool;
    public PromotionUIScript _promotionScript;
    public EndGameUI EndGameUI;

    private ChessBoard _chessBoard;

    private Dictionary<ChessColor, Player.Player> _players;

    private List<Move> _legalMoves;

    public static bool _humainPlayer;
    private bool _firstClick;
    
    public bool playing { get; set; }
    private bool paused;

    public GameObject whiteCam;
    public GameObject menuCam;

    private Dictionary<Piece, GameObject> _map;

    private void Start()
    {
        _tileManager = GetComponentInChildren<TileManager>();
        _pieceManager = GetComponentInChildren<PieceManager>();
        _objectPool = GetComponentInChildren<ObjectPool>();

        _chessBoard = new ChessBoard(this);
        _chessBoard.Rock += RockDone;
        
        _map = new Dictionary<Piece, GameObject>(32);

        _humainPlayer = false;
        _firstClick = true;

        playing = false;
        paused = true;

        _legalMoves = new List<Move>();
        
        _chessBoard.InitializeBoard();
        
        foreach (var piece in _chessBoard.Board)
        {
            if (piece.Type != ChessType.None)
                _map.Add(piece, createPieceOnPlacement(piece.Type, piece.Color, piece.Position));   
            if(piece.Color == ChessColor.Black) _map[piece].transform.Rotate(0,180,0);
        }
        
    }

    private void SwapCam()
    {
        whiteCam.SetActive(!whiteCam.activeInHierarchy);
    }

    public void RestartGame()
    {
        _chessBoard.InitializeBoard();
        
        FragmentPool.Instance.DeactivateFragments();
        FragmentPool.Instance.DestroyFragments();
        FragmentPool.Instance.Reset(ExploderSingleton.Instance.Params);
       
        foreach (var piece in _map)
        {
                piece.Value.SetActive(false);
        }

        _humainPlayer = false;
        _legalMoves.Clear();
        _tileManager.updateLegalMoves(_legalMoves);
        _firstClick = true;

        _map.Clear();
        foreach (var piece in _chessBoard.Board)
        {
            if (piece.Type == ChessType.None) continue;
            GameObject gameObjectPiece = createPieceOnPlacement(piece.Type, piece.Color, piece.Position);
            gameObjectPiece.GetComponent<PiecePieces>().ResetMovement();
            _map.Add(piece, gameObjectPiece);
        }
        playing = true;
    }

    public void InitialisePlay(Dictionary<ChessColor, Player.Player> players)
    {
        if (players[ChessColor.White] != null && players[ChessColor.Black] == null)
            whiteCam.SetActive(false);

        _players = players;
        
        menuCam.SetActive(false);
        GetComponent<AudioSource>().Play();
        playing = true;
        paused = true;
    }

    private void MovePiece(GameObject piece, Move move, bool rock = false)
    {
        int position = move.EndPosition;
        if (move.Eat)
        {
            _pieceManager.AttackWithPiece(piece, _tileManager.getCoordinatesByTilePlacement(position), _tileManager.getCoordinatesByTilePlacement(move.EatenPiece.Position), _map[move.EatenPiece]);
        }
        else
        {
            _pieceManager.MovePiece(piece, _tileManager.getCoordinatesByTilePlacement(position), rock);
        }
    }

    public void ClickTile(int placement)
    {
        if (!_humainPlayer || !playing) return;

        if (_firstClick)
        {
            if ((_legalMoves = _chessBoard.GetMoveFromPosition(placement)).Any())
            {
                _firstClick = false;
            }
        }
        else
        {
            _legalMoves = _legalMoves.Where(move => move.EndPosition.Equals(placement)).ToList();
            if (_legalMoves.Count == 1)
            {
                var move = _legalMoves[0];
                _humainPlayer = false;
                _chessBoard.Play(move);
                MovePiece(_map[move.Piece], move);
                _legalMoves.Clear();
                _firstClick = true;
            }
            else
            {
                _legalMoves = _chessBoard.GetMoveFromPosition(placement);
            }
        }
        
        _tileManager.updateLegalMoves(_legalMoves);
    }

    private GameObject createPieceOnPlacement(ChessType pieceType, ChessColor color, int position)
    {
        var pieceObject = _objectPool.getPooledPiece(pieceType, color, _tileManager.getCoordinatesByTilePlacement(position));
        return pieceObject;
    }

    public void NextTurn()
    {
        if (paused)
        {
            return;
        }

        var nextToPlay = _chessBoard.NextToPlay;
        

        var currentPlayer = _players[nextToPlay];
        if (currentPlayer == null){
            whiteCam.SetActive(nextToPlay == ChessColor.White);
            _humainPlayer = true;
        }
        else
        {
            var move = currentPlayer.GetDesiredMove();
            MovePiece(_map[move.Piece], move);
            _chessBoard.Play(move);
        }
    }

    private void Update()
    {
        if (playing && paused)
        {
            paused = false;
            NextTurn();
        }
        
        if (!playing && !paused)
        {
            paused = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwapCam();       
            //Debug.Log(_chessBoard.GetEvaluationScore());
        }
    }

    private void RockDone(object sender, Move move)
    {
        MovePiece(_map[move.Piece], move, true);
    }

    public void Promotion(Piece piece)
    {
        StartCoroutine(PromotionWait(piece));
        playing = false;
    }

    private IEnumerator PromotionWait(Piece piece)
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => _map[piece].GetComponent<PiecePieces>()._arrived);
        if (_players[_chessBoard.NextToPlay.Reverse()] == null)
        {
            _promotionScript.show(piece);
        }
        else GivePromotion(piece, ChessType.Queen);
    }

    public void GivePromotion(Piece piece, ChessType chessType)
    {
        switch (chessType)
        {
            case ChessType.Bishop:
                _map[piece].SetActive(false);
                _map[piece] = _objectPool.getPooledPiece(ChessType.Bishop, piece.Color, _tileManager.getCoordinatesByTilePlacement(piece.Position));
                break;
            case ChessType.Rook:
                _map[piece].SetActive(false);
                _map[piece] = _objectPool.getPooledPiece(ChessType.Rook, piece.Color, _tileManager.getCoordinatesByTilePlacement(piece.Position));
                break;
            case ChessType.Queen:
                _map[piece].SetActive(false);
                _map[piece] = _objectPool.getPooledPiece(ChessType.Queen, piece.Color, _tileManager.getCoordinatesByTilePlacement(piece.Position));
                break;
            case ChessType.Knight:
                _map[piece].SetActive(false);
                _map[piece] = _objectPool.getPooledPiece(ChessType.Knight, piece.Color, _tileManager.getCoordinatesByTilePlacement(piece.Position));
                break;
        }
        _chessBoard.PromotePawn(piece, chessType);
        playing = true;
    }

    public void EndGameWin(ChessColor color, Piece piece)
    {
        StartCoroutine(EndGameWin2(color, piece));
    }
    
    public void EndGameNull(Piece piece)
    {
        StartCoroutine(EndGameNull2(piece));
    }

    private IEnumerator EndGameWin2(ChessColor color, Piece piece)
    {
        playing = false;
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => _map[piece].GetComponent<PiecePieces>()._arrived);
        EndGameUI.EndGameWin(color);
    }

    private IEnumerator EndGameNull2(Piece piece)
    {
        playing = false;
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => _map[piece].GetComponent<PiecePieces>()._arrived);
        EndGameUI.EndGameNull();
    }
}