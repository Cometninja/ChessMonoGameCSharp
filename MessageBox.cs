﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Chess1
{
    internal class MessageBox : Sprite
    {
        List<string> _messages = new List<string>();
        SpriteFont _messageBoxFont;

        public MessageBox(Texture2D image, Vector2 position,SpriteFont font) : base(image, position) 
        { 
            _messageBoxFont = font;
        }

        public override void Update(List<Sprite> sprites)
        {
            if (_messages.Count > 4) 
            {
                _messages.RemoveAt(0);
            }
            base.Update(sprites);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            int count = _messages.Count - 1;
            foreach (string message in _messages)
            { 
                spriteBatch.DrawString(_messageBoxFont, message, new Vector2(_position.X + 10,_position.Y + +5 + (20 * count)), Color.White);
                count --;
            }
        }

        public void AddMessage(string message) 
        {
          
            _messages.Add(message);
        }
        public void ClearMessage() 
        {
            _messages.Clear();
        }


    }
}
