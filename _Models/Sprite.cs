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

        public Dictionary<string ,Animation> animations;
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

        public void SetOpacity(float value)
        {
            foreach (var animation in animations.Values)
            {
                animation.SetOpacity(value);
            }
        }

        protected Rectangle ChangeRectangleSize(Rectangle rectangle,int value,bool decrease = false)
        {
            if (decrease)
            {
                int decreaseamount = value;
                int newX = rectangle.X + decreaseamount / 2;
                int newY = rectangle.Y + decreaseamount / 2;
                int newWidth = rectangle.Width - decreaseamount;
                int newHeight = rectangle.Height - decreaseamount;
                return new(newX, newY, newWidth, newHeight);
            }
            else
            {
                int increaseamount = value;
                int newX = rectangle.X - increaseamount / 2;
                int newY = rectangle.Y - increaseamount / 2;
                int newWidth = rectangle.Width + increaseamount;
                int newHeight = rectangle.Height + increaseamount;
                return new Rectangle(newX, newY, newWidth, newHeight);
            }
        }
    }
}
