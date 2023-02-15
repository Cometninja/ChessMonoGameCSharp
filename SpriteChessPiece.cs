using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Chess1
{
    enum ChessPiece { Pawn,Rook,Knight,Bishop,King,Queen}
    internal class SpriteChessPiece : Sprite
    {
        public bool _white;
        public ChessPiece _piece;
        public PieceColor _pieceColor;
        public Vector2 _center;
        public bool _selected;
        public bool _dragged;
        public bool _moved;
        public bool _isDead;


        public Point _piecePoint;
        public SpriteChessPiece(Texture2D image,Vector2 position,bool white,ChessPiece piece) : base(image,position) 
        { 
            _white = white;
            _piece = piece;
            _center = new Vector2(
                _position.X + image.Width/2,
                _position.Y + image.Height/2);
            if (_white) 
                _pieceColor = PieceColor.White; 
            else
                _pieceColor = PieceColor.Black;
        }

        public override void Update(List<Sprite> sprites)
        {
            _center = new Vector2(
                _position.X + _image.Width / 2,
                _position.Y + _image.Height / 2);

            base.Update(sprites);
        }

    }





}
