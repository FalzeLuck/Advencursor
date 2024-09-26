using Advencursor._Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._Models;
using Advencursor._Combat;

namespace Advencursor._UI
{
    public class UIPlayerCheckPanel : UIElement
    {
        Status status;
        Player player;
        private SpriteFont font;
        public UIPlayerCheckPanel(Texture2D texture, Vector2 position,Player player) : base(texture, position)
        {
            status = player.Status;
            this.player = player;
            font = Globals.Content.Load<SpriteFont>("basicFont");
        }

        public override void Update(GameTime gameTime)
        {

            
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            if (isVisible)
            {
                Globals.SpriteBatch.Draw(texture, position, null, _color, rotation, origin, scale, SpriteEffects.None, 1);
                string HP = status.CurrentHP.ToString();
                string Shield = status.Shield.ToString();
                string Attack = status.Attack.ToString();
                string ParryCooldown = player.cooldownTimer.ToString();
                Globals.SpriteBatch.DrawString(font, $"Player HP : {HP}", new(position.X +10,position.Y), Color.Black, rotation, origin, 1, spriteEffects, 1);
                Globals.SpriteBatch.DrawString(font, $"Player Shield : {Shield}", new(position.X +10, position.Y + 20), Color.Black, rotation, origin, 1, spriteEffects, 1);
                Globals.SpriteBatch.DrawString(font, $"Player Attack : {Attack}", new(position.X + 10, position.Y + 40), Color.Black, rotation, origin, 1, spriteEffects, 1);
                Globals.SpriteBatch.DrawString(font, $"Parry Cooldown : {ParryCooldown}", new(position.X + 10, position.Y + 60), Color.Black, rotation, origin, 1, spriteEffects, 1);
            }
        }

    }
}
