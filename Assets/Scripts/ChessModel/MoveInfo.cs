namespace ChessModel
{
    public struct MoveInfo
    {
        public Move Move { get; }
        public bool WhiteLeftCastle { get; }
        public bool WhiteRightCastle { get; }
        public bool BlackLeftCastle { get; }
        public bool BlackRightCastle { get; }
        public bool BlackHasCastle { get; }
        public bool WhiteHasCastle { get; }

        public MoveInfo(Move move, bool whiteLeftCastle, bool whiteRightCastle, bool blackLeftCastle, bool blackRightCastle, bool whiteHasCastle, bool blackHasCastle)
        {
            Move = move;
            WhiteLeftCastle = whiteLeftCastle;
            WhiteRightCastle = whiteRightCastle;
            BlackLeftCastle = blackLeftCastle;
            BlackRightCastle = blackRightCastle;
            WhiteHasCastle = whiteHasCastle;
            BlackHasCastle = blackHasCastle;
        }
    }
}