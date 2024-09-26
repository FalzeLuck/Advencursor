using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Scene.Transition
{
    public class CircleTransition : ITransition
    {
        private float radius;
        private float maxRadius;
        private float transitionSpeed;
        private bool isInTransition;
        private Texture2D circleTexture;
        private Vector2 screenCenter;

        public bool IsComplete => isInTransition ?  radius <= 0f : radius >= maxRadius ;
        public bool IsInTransition => isInTransition;

        public CircleTransition(GraphicsDevice graphicsDevice, float speed = 2000f)
        {
            transitionSpeed = speed;

            screenCenter = new Vector2(Globals.Bounds.X / 2f, Globals.Bounds.Y / 2f);
            maxRadius = (float)Math.Sqrt(screenCenter.X * screenCenter.X + screenCenter.Y * screenCenter.Y);
            circleTexture = Globals.CreateCircleTexture(Globals.graphicsDevice,300);
        }

        public void Start(bool isInTransition)
        {
            this.isInTransition = isInTransition;

            radius = isInTransition ?  maxRadius : 0f;
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds * transitionSpeed;

            if (isInTransition)
            {
                radius -= delta;
                if (radius < 0.0f) radius = 0.0f;
                
            }
            else
            {
                radius += delta;
                if (radius > maxRadius) radius = maxRadius;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (radius > 0.0f && radius <= maxRadius)
            {
                float scale = radius / (circleTexture.Width / 2f);
                spriteBatch.Draw(circleTexture, screenCenter, null, Color.Black, 0f, new Vector2(circleTexture.Width / 2f, circleTexture.Height / 2f), scale, SpriteEffects.None, 0f);
            }
        }

    }
}
