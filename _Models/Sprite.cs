using Advencursor._Animation;
using Advencursor._Combat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Models
{
    public class Sprite
    {
        public Status Status { get; set; }

        protected Texture2D texture;
        public Vector2 origin;
        public Vector2 position {  get; set; }
        public Vector2 velocity {  get; set; }
        public float rotation {  get; set; }

        public SpriteEffects spriteEffects;

        protected Dictionary<string ,Animation> animations;
        public string indicator;
        protected int row;
        protected int column;
        public float recovery_time;

        public Rectangle collision;

        public Sprite(Texture2D texture,Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            origin = new Vector2(texture.Width/2, texture.Height/2);
            spriteEffects = SpriteEffects.None;
            animations = new Dictionary<string, Animation>()
            {
                {"base",new Animation(texture,1,1,0,false)},
            };
            indicator = "base";
        }

        public virtual void Update(GameTime gameTime)
        {
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update();
            }
        }


        public virtual void Draw()
        {
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Draw(position);
            }
        }

        public void CollisionUpdate()
        {
            collision = animations[indicator].GetCollision(position);
        }

    }
}
