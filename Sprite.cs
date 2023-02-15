using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Chess1
{
    internal class Sprite 
    {
        public Texture2D _image;
        public Vector2 _position;
        public Rectangle _bounds;
        public Color _color = Color.White;

        public Sprite(Texture2D image, Vector2 position)
        {
            _image = image;
            _position = position;
            _bounds = new Rectangle((int)position.X, (int)position.Y, image.Width, image.Height);

        }


        public virtual void Update(List<Sprite> sprites)
        {
            _bounds.X = (int)_position.X;
            _bounds.Y = (int)_position.Y;

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_image,_position,_color);
        }
    }
}
