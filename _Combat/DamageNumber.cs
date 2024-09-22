using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Combat
{
    public class DamageNumber
    {
        public string text { get; private set; }
        public Vector2 position { get; private set; }
        public Color color { get; private set; }
        public float scale { get; private set; }

        private float tempScale;

        private float elapsedTime;
        private float timeLive;
        private Vector2 velocity;

        private bool isCrit;

        //Random random;

        public DamageNumber(string text, Vector2 position, Color color,float scale = 1f, float timeLive = 0.5f, Vector2? velocity = null)
        {
            this.text = text;
            this.position = position + new Vector2(Globals.random.Next(-50,50), Globals.random.Next(-50, 50));
            this.color = color;
            this.timeLive = timeLive;
            elapsedTime = 0f;
            this.velocity = velocity ?? new Vector2(0, -15);
            this.scale = scale * 1.25f;
            tempScale = scale;
        }

        public void Update()
        {
            float deltaTime = TimeManager.TimeGlobal;
            elapsedTime += deltaTime;
            timeLive -= deltaTime;
            if (scale >= tempScale)
            {
                scale -= 2 * TimeManager.TotalSeconds;
            }
            //Update pos
            position += velocity * deltaTime;

            float remainTime = timeLive - elapsedTime;
            float alpha = MathHelper.Clamp(remainTime/timeLive, 0f, 1f);

            color = new Color(color.R, color.G, color.B);
            
        }

        public bool IsExpired()
        {
            return timeLive <= 0 || color.A <= 0;
        }

        public void Draw(SpriteFont font)
        {
            Color outlineColor = new Color(108,16,156);
            float outlineThickness = 2f;

            // Up down Left Right
            Globals.SpriteBatch.DrawString(font, text, position + new Vector2(-outlineThickness, 0), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(font, text, position + new Vector2(outlineThickness, 0), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(font, text, position + new Vector2(0, -outlineThickness), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(font, text, position + new Vector2(0, outlineThickness), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Diagonals
            Globals.SpriteBatch.DrawString(font, text, position + new Vector2(-outlineThickness, -outlineThickness), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(font, text, position + new Vector2(outlineThickness, -outlineThickness), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(font, text, position + new Vector2(-outlineThickness, outlineThickness), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Globals.SpriteBatch.DrawString(font, text, position + new Vector2(outlineThickness, outlineThickness), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
            
            Globals.SpriteBatch.DrawString(font,text, position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0f); 

        }
    }
}
