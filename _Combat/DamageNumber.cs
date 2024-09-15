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
        public float scale { get; private set; }


        private float elapsedTime;
        private float timeLive;
        private Vector2 velocity;

        private bool isCrit;

        Random random;

        public DamageNumber(string text, Vector2 position, Color color, float timeLive = 0.5f, Vector2? velocity = null)
        {
            random = new Random();
            this.text = text;
            this.position = position + new Vector2(random.Next(-20,20));
            this.color = color;
            this.timeLive = timeLive;
            elapsedTime = 0f;
            this.velocity = velocity ?? new Vector2(0, -30);
            scale = 1.5f;
        }

        public void Update()
        {
            float deltaTime = TimeManager.TimeGlobal;
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
            Globals.SpriteBatch.DrawString(font,text, position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0f); 
        }
    }
}
