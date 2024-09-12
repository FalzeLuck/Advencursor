using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Particles
{
    public class Particle
    {
        public readonly ParticleData data;
        protected Vector2 position;
        protected float lifespanLeft;
        protected float lifespanAmount;
        protected Color color;
        public float opacity;
        public bool IsFinished = false;
        public float scale;
        protected Vector2 origin;
        protected Vector2 direction;
        protected float rotation;

        public Particle(Vector2 position, ParticleData data) 
        {
            this.data = data;
            this.position = position;
            lifespanLeft = data.lifespan;
            lifespanAmount = 1f;
            color = data.colorStart;
            opacity = data.opacityStart;
            origin = new(this.data.texture.Width / 2, this.data.texture.Height / 2);
            rotation = data.rotation;

            if(data.speed != 0)
            {
                this.data.angle = MathHelper.ToRadians(this.data.angle);
                direction = new Vector2((float)Math.Sin(this.data.angle), (float)Math.Cos(this.data.angle));
            }
            else
            {
                direction = Vector2.Zero;
            }
        }

        public virtual void Update()
        {
            lifespanLeft -= TimeManager.TotalSeconds;
            if(lifespanLeft <= 0f)
            {
                IsFinished = true;
                return;
            }

            lifespanAmount = MathHelper.Clamp(lifespanLeft / data.lifespan, 0f, 1f);
            color = Color.Lerp(data.colorEnd, data.colorStart, lifespanAmount);
            opacity = MathHelper.Clamp(MathHelper.Lerp(data.opacityEnd, data.opacityStart, lifespanAmount), 0f, 1f);
            scale = MathHelper.Lerp(data.sizeEnd,data.sizeStart, lifespanAmount) / data.texture.Width;
            position += direction * data.speed * TimeManager.TotalSeconds;


        }

        public virtual void Draw()
        {
            Globals.SpriteBatch.Draw(data.texture, position, null, color * opacity, rotation, origin, scale, SpriteEffects.None, 1f);
        }
    }
}
