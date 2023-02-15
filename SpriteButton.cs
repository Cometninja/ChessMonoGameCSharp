using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Chess1
{
    internal class SpriteButton : Sprite
    {
        public bool _isButtonPressed;
        public ButtonType _buttonType;
        SpriteFont _buttonFont;
        public SpriteButton(Texture2D image, Vector2 position, ButtonType buttonType,SpriteFont spriteFont) : base(image, position)
        {
            _buttonFont = spriteFont;
            _buttonType = buttonType;
        }

        public override void Update(List<Sprite> sprites)
        {
            base.Update(sprites);

            if (_bounds.Contains(Mouse.GetState().Position) && Mouse.GetState().LeftButton == ButtonState.Pressed)
                _isButtonPressed = true;
            
            else _isButtonPressed = false;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            switch(_buttonType) 
            {
                case ButtonType.Start:

                    Vector2 center = _buttonFont.MeasureString("Start");

                    spriteBatch.DrawString(_buttonFont,"Start", new Vector2 (
                        _position.X + (_image.Width/2) - (center.X / 2),
                        _position.Y + (_image.Height/2) - (center.Y / 2)),
                        Color.Black);

                    break;
            
            
            }
        }




    }
    enum ButtonType { Start,Restart,Quit,Exit}
}
