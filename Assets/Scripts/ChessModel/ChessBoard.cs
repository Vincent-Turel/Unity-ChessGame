using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessModel
{
    public class ChessBoard
    {
        public static ChessBoard Instance { get; private set; }

        public bool WhiteLeftCastle { get; private set; }
        public bool WhiteRightCastle { get; private set; }
        public bool BlackLeftCastle { get; private set; }
        public bool BlackRightCastle { get; private set; }

        public bool BlackHasCastle { get; private set; }
        public bool WhiteHasCastle { get; private set; }

        private int BlackCount { get; set; }
        private int WhiteCount { get; set; }

        private int NbMovesWhite { get; set; }
        private int NbMovesBlack { get; set; }

        public ChessColor NextToPlay { get; private set; }

        private readonly Stack<MoveInfo> _moveHistory;
        public Piece[] Board { get; }

        private readonly BoardManager _boardManager;

        public ChessBoard(BoardManager boardManager)
        {
            Board = new Piece[64];
            _boardManager = boardManager;
            _moveHistory = new Stack<MoveInfo>(50);
            Instance = this;
            InitializeBoard();
        }

        public Move LastMove => _moveHistory.Any() ? _moveHistory.Peek().Move : null;

        public void Play(Move move, bool simulation = false)
        {
            _moveHistory.Push(new MoveInfo(move, WhiteLeftCastle, WhiteRightCastle, BlackLeftCastle, BlackRightCastle,
                WhiteHasCastle, BlackHasCastle));
            var startPosition = move.StartPosition;
            var endPosition = move.EndPosition;
            UpdateCastle(move, simulation);
            TestPromotion(move, simulation);
            UpdateScore(endPosition);
            if (move.Eat)
                Board[move.EatenPiece.Position] = new Piece(ChessColor.None, move.EatenPiece.Position, ChessType.None);
            Switch(startPosition, endPosition);
            NextToPlay = NextToPlay.Reverse();
            if (!simulation && IsCheckMate)
            {
                _boardManager.EndGameWin(NextToPlay.Reverse(), move.Piece);
            }
            else if (!simulation && IsDraw())
            {
                _boardManager.EndGameNull(move.Piece);
            }
            // Debug.Log(ToString());
        }


        public void Unplay()
        {
            var moveInfo = _moveHistory.Pop();
            var move = moveInfo.Move;
            var startPosition = move.StartPosition;
            var endPosition = move.EndPosition;
            Switch(startPosition, endPosition);
            if (move.Eat)
                Board[move.EatenPiece.Position] = move.EatenPiece;
            if (NextToPlay == ChessColor.White)
                BlackCount -= Board[endPosition].Value;
            else
                WhiteCount -= Board[endPosition].Value;

            switch (endPosition)
            {
                case 2 when startPosition == 4:
                    Switch(0, 3);
                    break;
                case 6 when startPosition == 4:
                    Switch(5, 7);
                    break;
                case 58 when startPosition == 60:
                    Switch(56, 59);
                    break;
                case 62 when startPosition == 60:
                    Switch(61, 63);
                    break;
            }

            WhiteLeftCastle = moveInfo.WhiteLeftCastle;
            WhiteRightCastle = moveInfo.WhiteRightCastle;
            BlackLeftCastle = moveInfo.BlackLeftCastle;
            BlackRightCastle = moveInfo.BlackRightCastle;
            WhiteHasCastle = moveInfo.WhiteHasCastle;
            BlackHasCastle = moveInfo.BlackHasCastle;
            NextToPlay = NextToPlay.Reverse();
        }

        private void Switch(int position1, int position2)
        {
            (Board[position1], Board[position2]) = (Board[position2], Board[position1]);
            Board[position1].MoveTo(position1);
            Board[position2].MoveTo(position2);
        }

        public Piece GetPiece(int position)
        {
            return Board[position];
        }

        public List<Move> GetMoveFromPosition(int position)
        {
            var piece = Board[position];
            return piece.Color == NextToPlay ? piece.GetLegalMoves() : new List<Move>();
        }

        public float GetEvaluationScore()
        {
            float score = WhiteCount - BlackCount;
            if (IsThreatening(35, ChessColor.White) || IsThreatening(36, ChessColor.White)) score += 0.1f;
            if (IsThreatening(27, ChessColor.Black) || IsThreatening(28, ChessColor.Black)) score -= 0.1f;
            if (!WhiteLeftCastle && !WhiteRightCastle && !WhiteHasCastle) score -= 0.9f;
            if (!BlackLeftCastle && !BlackRightCastle && !BlackHasCastle) score += 0.9f;
            if (WhiteHasCastle) score += 0.9f;
            if (BlackHasCastle) score -= 0.9f;
            return score;
        }

        private void UpdateScore(int endPosition)
        {
            if (NextToPlay == ChessColor.White)
                WhiteCount += Board[endPosition].Value;
            else
                BlackCount += Board[endPosition].Value;
        }

        private void TestPromotion(Move move, bool simulation)
        {
            if (move.Piece.Color == ChessColor.Black && move.Piece.Type == ChessType.Pawn &&
                move.EndPosition / 8 == 0 ||
                move.Piece.Color == ChessColor.White && move.Piece.Type == ChessType.Pawn && move.EndPosition / 8 == 7)
            {
                PromotePawn(move, simulation);
            }
        }

        private void PromotePawn(Move move, bool simulation)
        {
            if (!simulation)
                AskPromotion(move.Piece);
        }

        public void PromotePawn(Piece piece, ChessType type)
        {
            piece.Type = type;
        }

        private void AskPromotion(Piece piece)
        {
            _boardManager.Promotion(piece);
        }

        private void UpdateCastle(Move move, bool simulation)
        {
            if (move.StartPosition == 4 && (WhiteLeftCastle || WhiteRightCastle))
            {
                if (WhiteLeftCastle && move.EndPosition == 2)
                {
                    if (!simulation)
                        Rock?.Invoke(this, new Move(0, 3,
                            Board[0],
                            Board[3]));
                    Switch(0, 3);
                    WhiteHasCastle = true;
                }

                if (WhiteRightCastle && move.EndPosition == 6)
                {
                    if (!simulation)
                        Rock?.Invoke(this, new Move(7, 5,
                            Board[7],
                            Board[5]));
                    Switch(5, 7);
                    WhiteHasCastle = true;
                }

                WhiteLeftCastle = WhiteRightCastle = false;
            }
            else if (move.StartPosition == 60 && (BlackLeftCastle || BlackRightCastle))
            {
                if (BlackLeftCastle && move.EndPosition == 58)
                {
                    if (!simulation)
                        Rock?.Invoke(this, new Move(56,
                            59,
                            Board[56],
                            Board[59]));
                    Switch(56, 59);
                    BlackHasCastle = true;
                }

                if (BlackRightCastle && move.EndPosition == 62)
                {
                    if (!simulation)
                        Rock?.Invoke(this, new Move(63,
                            61,
                            Board[63],
                            Board[61]));
                    Switch(61, 63);
                    BlackHasCastle = true;
                }

                BlackLeftCastle = BlackRightCastle = false;
            }
            else if (WhiteLeftCastle && move.StartPosition == 0)
                WhiteLeftCastle = false;
            else if (WhiteRightCastle && move.StartPosition == 7)
                WhiteRightCastle = false;
            else if (BlackLeftCastle && move.StartPosition == 56)
                BlackLeftCastle = false;
            else if (BlackRightCastle && move.StartPosition == 63)
                BlackRightCastle = false;
        }

        public event EventHandler<Move> Rock;

        public List<Move> GetAllLegalMoves(ChessColor color)
        {
            return Board.Where(piece => piece.Color == color).SelectMany(piece => piece.GetLegalMoves()).ToList();
        }

        public bool IsCheck(ChessColor color)
        {
            var position = Array
                    .Find(Board, piece1 =>
                        piece1.Color == color && piece1.Type == ChessType.King).Position;
            
            return Array.Exists(Board, piece =>
                piece.Color == color.Reverse() && piece.GetPseudoMoves().Exists(move => move.EndPosition.Equals(position)));
        }

        public bool IsThreatening(int position, ChessColor color)
        {
            return Array.Exists(Board, piece =>
                piece.Color == color && piece.GetPseudoMoves()
                    .Exists(move => move.EndPosition == position));
        }

        public bool IsCheckMate => IsCheck(NextToPlay) &&
                                   !Array.Exists(Board,
                                       piece => piece.Color == NextToPlay && piece.GetLegalMoves().Any());

        public bool IsDraw()
        {
            return IsPat || InsufficientMaterial;
        }

        private bool IsPat => !IsCheck(NextToPlay) &&
                              !Array.Exists(Board, piece => piece.Color == NextToPlay && piece.GetLegalMoves().Any());

        private bool InsufficientMaterial
        {
            get
            {
                var x = Board.Where(piece => piece.Type != ChessType.None && piece.Type != ChessType.King).ToList();
                return !x.Any() ||
                       x.Count == 1 && (x[0].Type == ChessType.Bishop || x[0].Type == ChessType.Knight) ||
                       x.Count == 2 && x[0].Type == ChessType.Bishop && x[1].Type == ChessType.Bishop &&
                       x[0].Color == x[1].Color;
            }
        }

        public bool IsEmpty(int position)
        {
            return Board[position].Type == ChessType.None;
        }

        public ChessColor? Color(int position)
        {
            return GetPiece(position).Color;
        }

        public void InitializeBoard()
        {
            NextToPlay = ChessColor.White;
            Board[0] = new Piece(ChessColor.White, 0, ChessType.Rook);
            Board[1] = new Piece(ChessColor.White, 1, ChessType.Knight);
            Board[2] = new Piece(ChessColor.White, 2, ChessType.Bishop);
            Board[3] = new Piece(ChessColor.White, 3, ChessType.Queen);
            Board[4] = new Piece(ChessColor.White, 4, ChessType.King);
            Board[5] = new Piece(ChessColor.White, 5, ChessType.Bishop);
            Board[6] = new Piece(ChessColor.White, 6, ChessType.Knight);
            Board[7] = new Piece(ChessColor.White, 7, ChessType.Rook);
            for (var i = 8; i < 16; i++)
            {
                Board[i] = new Piece(ChessColor.White, i, ChessType.Pawn);
            }

            for (var i = 16; i < 48; i++)
            {
                Board[i] = new Piece(ChessColor.None, i, ChessType.None);
            }

            for (var i = 48; i < 56; i++)
            {
                Board[i] = new Piece(ChessColor.Black, i, ChessType.Pawn);
            }

            Board[56] = new Piece(ChessColor.Black, 56, ChessType.Rook);
            Board[57] = new Piece(ChessColor.Black, 57, ChessType.Knight);
            Board[58] = new Piece(ChessColor.Black, 58, ChessType.Bishop);
            Board[59] = new Piece(ChessColor.Black, 59, ChessType.Queen);
            Board[60] = new Piece(ChessColor.Black, 60, ChessType.King);
            Board[61] = new Piece(ChessColor.Black, 61, ChessType.Bishop);
            Board[62] = new Piece(ChessColor.Black, 62, ChessType.Knight);
            Board[63] = new Piece(ChessColor.Black, 63, ChessType.Rook);

            WhiteLeftCastle = WhiteRightCastle = BlackRightCastle = BlackLeftCastle = true;
            WhiteHasCastle = BlackHasCastle = false;
            WhiteCount = BlackCount = 0;
        }

        public ChessColor TileColor(int position)
        {
            return position / 8 % 2 == 0 ? position % 2 == 0 ? ChessColor.Black :
                ChessColor.White :
                position % 2 == 0 ? ChessColor.White : ChessColor.Black;
        }

        public override string ToString()
        {
            return Board.Aggregate("", (current, piece) => current + (piece + "\n"));
        }
    }
}