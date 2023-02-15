using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Chess1
{
    enum PieceColor { Empty, White, Black }
    internal class SpriteChessBoard: Sprite
    {
        public List<SpriteChessPiece> chessPiece = new List<SpriteChessPiece>();
        public Point _point;
        public PieceColor _boardPieceColor;
        public bool _enemy;
        public Color _tileColor;
        public SpriteChessBoard(Texture2D image,Vector2 position, Point point, Color originalColor) : base(image, position)
        {
            _tileColor = originalColor;
            _point = point;
            this._color = originalColor;
        }

        public override void Update(List<Sprite> sprites)
        {
            
            chessPiece = sprites.OfType<SpriteChessPiece>().Where(piece => this._bounds.Contains(piece._center)).ToList();

            

            if (chessPiece.Count > 0)
            {
                foreach (SpriteChessPiece piece in chessPiece)
                {
                    if (!piece._isDead)
                    {
                        piece._piecePoint = _point;
                    }
                    if (piece._white)
                    {
                        _boardPieceColor = PieceColor.White;
                    }
                    else
                    {
                        _boardPieceColor = PieceColor.Black;
                    }
                }
               
            }

            else _boardPieceColor = PieceColor.Empty;


            base.Update(sprites);
        }

    }
}
