using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Advencursor._Managers;
using Advencursor._UI;
using Microsoft.Xna.Framework.Audio;

namespace Advencursor._Models
{
    public class UIButton : UIElement
    {
        private Rectangle _rect;
        private Action onClick;
        private Action onHover;
        private bool hoverSoundPlayed = false;
        public UIButton(Texture2D texture, Vector2 position, Action onClick) : base(texture, position)
        {
            _rect = new((int)(position.X - texture.Width / 2), (int)(position.Y - texture.Height / 2), texture.Width, texture.Height);
            this.onClick = onClick;
        }

        public UIButton(Texture2D texture, Vector2 position, Action onHover,bool hover) : base(texture, position)
        {
            _rect = new((int)(position.X - texture.Width / 2), (int)(position.Y - texture.Height / 2), texture.Width, texture.Height);
            this.onHover = onHover;
            
        }

        public override void Update(GameTime gameTime)
        {

            if (InputManager.MouseCursor.Intersects(_rect))
            {
                scale = 1.1f;
                //_color = Color.DarkGray;
                if (!hoverSoundPlayed)
                {
                    Globals.soundManager.PlaySound("Hover");
                    hoverSoundPlayed = true;
                }
                onHover?.Invoke();
            }
            else
            {
                scale = 1f;
                _color = Color.White;
                hoverSoundPlayed = false;
            }

            if(isVisible && IsClicked())
            {
                Globals.soundManager.PlaySound("Click");
                onClick?.Invoke();
            }

            _rect = new((int)(position.X - texture.Width / 2), (int)(position.Y - texture.Height / 2), texture.Width, texture.Height);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            if (isVisible)
            {
                Globals.SpriteBatch.Draw(texture, position, null, _color, rotation, origin, scale, SpriteEffects.None, 1);
            }
        }

        public bool IsClicked()
        {
            if (InputManager.MouseCursor.Intersects(_rect))
                return InputManager.MouseLeftClicked;
            else return false;
        }
    }
}
