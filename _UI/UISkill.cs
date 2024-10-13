using Advencursor._Skill;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._UI
{
    public class UISkill : UIElement
    {
        private SpriteFont font;
        private Skill skill;
        public UISkill(Texture2D texture,Vector2 position,Skill skill) : base(texture, position)
        {
            font = Globals.Content.Load<SpriteFont>("basicFont");
            this.skill = skill;
        }


        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
            {
                Globals.SpriteBatch.Draw(texture, position, null, Color.White * opacity, rotation, origin, scale, SpriteEffects.None, 1);
                string cooldown = skill.cooldownTimer.ToString("F1");
                Vector2 textSize = font.MeasureString(cooldown);
                Globals.SpriteBatch.DrawString(font, cooldown, new(position.X + (texture.Width - textSize.X) / 2, position.Y + (texture.Height - textSize.Y) / 2), Color.Black, rotation, origin, scale, spriteEffects, 1);
            }
        }
    }
}
