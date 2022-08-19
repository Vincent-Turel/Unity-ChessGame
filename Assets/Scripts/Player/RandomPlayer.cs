using ChessModel;
using Random = System.Random;

namespace Player
{
    public class RandomPlayer : Player
    {
        private readonly Random _rand;

        public RandomPlayer(ChessColor color) : base(color)
        {
            _rand = new Random();
        }

        public override Move GetDesiredMove()
        {
            var list = ChessBoard.Instance.GetAllLegalMoves(Color);
            return list[_rand.Next(list.Count)];
        }
    }
}