using Advencursor._Combat;
using Advencursor._Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._UI
{
    public class UIBackground : UIElement
    {
        public UIBackground(Texture2D texture, Vector2 position) : base(texture, position)
        {
            
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
            {
                Globals.SpriteBatch.Draw(texture, position, null, _color * opacity, rotation, origin, 1f, SpriteEffects.None, 1);
            }
        }
    }
}
