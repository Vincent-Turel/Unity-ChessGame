namespace ChessModel
{ 
    public class Move
    {
        public int StartPosition { get; }
        public int EndPosition { get; }
        
        public Piece Piece { get; }
        
        public Piece EatenPiece { get; }

        public Move(int startPosition, int endPosition, Piece piece, Piece eatenPiece)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            Piece = piece;
            EatenPiece = eatenPiece;
        }

        public bool Eat => EatenPiece.Type != ChessType.None;

        public override string ToString()
        {
            return StartPosition + ", " + EndPosition + ", " + Piece.Type + ", " + EatenPiece.Type;
        }
    }
}