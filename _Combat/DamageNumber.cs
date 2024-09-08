using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        private float elapsedTime;
        private float timeLive;
        private Vector2 velocity;

        public DamageNumber(string text, Vector2 position, Color color, float timeLive = 1.0f, Vector2? velocity = null)
        {
            this.text = text;
            this.position = position;
            this.color = color;
            this.timeLive = timeLive;
            elapsedTime = 0f;
            this.velocity = velocity ?? new Vector2(0, -30);
        }

        public void Update()
        {
            float deltaTime = TimeManager.TotalSeconds;
            elapsedTime += deltaTime;

            //Update pos
            position += velocity * deltaTime;

            float remainTime = timeLive - elapsedTime;

            float alpha = MathHelper.Clamp(remainTime/timeLive, 0f, 1f);

            color = new Color(color.R, color.G, color.B, alpha);
        }

        public bool IsExpired()
        {
            return timeLive <= 0 || color.A <= 0;
        }

        public void Draw(SpriteFont font)
        {
            Globals.SpriteBatch.DrawString(font, text, position, color);
        }
    }
}
