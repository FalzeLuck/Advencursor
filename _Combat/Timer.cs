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
    public class Timer
    {
        public readonly Texture2D texture;
        private readonly Vector2 position;
        private readonly SpriteFont font;
        private readonly Vector2 textPosition;
        private string text;
        private readonly float timeLength;
        public float timeLeft {  get; private set; }
        private bool active;
        public bool Repeat {  get; set; }

        public Timer(Texture2D texture,SpriteFont font, Vector2 position,  float length)
        {
            this.texture = texture;
            this.position = position;
            textPosition = new(position.X + 32, position.Y + 2);
            this.font = font;
            this.timeLength = length;
            this.timeLeft = length;
            text = string.Empty;
        }

        private void FormatText()
        {
            text = TimeSpan.FromSeconds(timeLeft).ToString(@"mm\:ss\.ff");
        }

        public void StartStop()
        {
            active = !active;
        }

        public void Reset()
        {
            timeLeft = timeLength;
            FormatText();
        }

        public void Update()
        {
            if (!active) return;
            timeLeft -= TimeManager.TotalSeconds;

            if(timeLeft <= 0)
            {
                if (Repeat)
                {
                    Reset();
                }
                else
                {
                    StartStop();
                    timeLeft = 0f;
                }
            }

            FormatText();
        }

        public void Draw()
        {
            Globals.SpriteBatch.Draw(texture,position,Color.White);
            Globals.SpriteBatch.DrawString(font, text, textPosition, Color.Black);
        }
    }
}
