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
        private Texture2D itemTexture;
        private Vector2 itemOrigin;
        private float itemScale;
        private Texture2D cooldownTexture;
        private bool onCooldown = false;
        public UISkill(Texture2D texture,Vector2 position,Skill skill) : base(texture, position)
        {
            font = Globals.Content.Load<SpriteFont>("basicFont");
            this.skill = skill;
            itemTexture = AllSkills.allSkillTextures[skill.name];
            cooldownTexture = itemTexture;
            itemOrigin = new Vector2(itemTexture.Width / 2, itemTexture.Height / 2);
            itemScale = 0.5f;
        }


        public override void Update(GameTime gameTime)
        {
            if (skill.cooldownTimer > 0)
            {
                onCooldown = true;
            }
            else
            {
                onCooldown = false ;
            }

            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
            {
                Globals.SpriteBatch.Draw(texture, position, null, Color.White * opacity, rotation, origin, scale, SpriteEffects.None, 1);

                
                Globals.SpriteBatch.Draw(itemTexture, position, null, Color.White * opacity, rotation, itemOrigin, itemScale, SpriteEffects.None, 1);

                if (onCooldown)
                {
                    float cooldownProgress = 1f - (skill.cooldownTimer / skill.cooldown);
                    float rotationAngle = MathHelper.TwoPi * cooldownProgress;

                    Globals.SpriteBatch.Draw(cooldownTexture, position, null, Color.Black, rotationAngle, itemOrigin, itemScale + 0.1f, SpriteEffects.None, 0f);
                    string cooldown = skill.cooldownTimer.ToString("F1");
                    Vector2 textSize = font.MeasureString(cooldown);
                    Globals.SpriteBatch.DrawString(font, cooldown, new(position.X + (texture.Width - textSize.X) / 2, position.Y + (texture.Height - textSize.Y) / 2), Color.White, rotation, origin, scale, spriteEffects, 1);
                }

                
            }
        }
    }
}
