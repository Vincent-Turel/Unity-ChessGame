using System;
using System.Linq;
using ChessModel;
using Random = System.Random;

namespace Player
{
    public class MinmaxPlayer : Player
    {
        private readonly int _depth;
        private readonly ChessBoard _board;
        private Move _bestMove;
        private readonly Random _rand;


        public MinmaxPlayer(ChessColor color, int depth) : base(color)
        {
            _depth = depth;
            _board = ChessBoard.Instance;
            _rand = new Random();
        }

        public override Move GetDesiredMove()
        {
            Minimax(_depth, float.MinValue, float.MaxValue, Color);
            return _bestMove;
        }

        private float Minimax(int depth, float alpha, float beta, ChessColor color)
        {
            if (_board.IsCheckMate)
                return _board.NextToPlay == ChessColor.White ? -100 : 100;

            if (_board.IsDraw())
                return 0;

            if (depth == 0)
                return _board.GetEvaluationScore();


            float value;
            var moves = _board.GetAllLegalMoves(color).OrderBy(item => _rand.Next());
            if (color == ChessColor.White)
            {
                value = float.MinValue;
                foreach (var move in moves)
                {
                    _board.Play(move, true);
                    var newValue = Minimax(depth - 1, alpha, beta, color.Reverse());
                    _board.Unplay();
                    alpha = Math.Max(alpha, newValue);
                    if (newValue > value)
                    {
                        value = newValue;
                        if (depth == _depth) _bestMove = move;
                    }

                    if (alpha >= beta)
                        break;
                }
            }
            else
            {
                value = float.MaxValue;
                foreach (var move in moves)
                {
                    _board.Play(move, true);
                    var newValue = Minimax(depth - 1, alpha, beta, color.Reverse());
                    _board.Unplay();
                    if (newValue < value)
                    {
                        value = newValue;
                        if (depth == _depth) _bestMove = move;
                    }

                    beta = Math.Min(beta, newValue);

                    if (alpha >= beta)
                        break;
                }
            }

            return value;
        }
    }
}