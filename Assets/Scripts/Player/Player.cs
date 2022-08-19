using ChessModel;

namespace Player
{
    public abstract class Player
    {
        protected readonly ChessColor Color;

        protected Player(ChessColor color)
        {
            Color = color;
        }
        public abstract Move GetDesiredMove();
    }
}