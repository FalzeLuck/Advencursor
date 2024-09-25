using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Advencursor._Scene.Transition
{
    public class FadeTransition : ITransition
    {
        private float opacity;
        private float transitionSpeed;
        private bool isInTransition;
        public bool IsComplete => isInTransition ? opacity <= 0f : opacity >= 1f;
        public bool IsInTransition => isInTransition;

        private Texture2D transitionTexture;

        public FadeTransition(float speed = 3f)
        {
            transitionSpeed = speed;
            
            opacity = 1f; 
        }

        public void Start(bool isInTransition)
        {
            transitionTexture = Globals.Content.Load<Texture2D>("blackTexture");
            this.isInTransition = isInTransition;
            opacity = isInTransition? 1f : 0f;
        }

        public void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds * transitionSpeed;
            if (isInTransition)
            {
                opacity -= time;
                if (opacity < 0f)
                {
                    opacity = 0f;
                }
            }
            else
            {
                opacity += time;
                if (opacity > 1f)
                {
                    opacity = 1f;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (transitionTexture != null)
            {
                spriteBatch.Draw(transitionTexture, Globals.fullScreenRectangle, Color.Black * opacity);
            }
        }
    }
}
