﻿using Advencursor._Animation;
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
        protected Texture2D texture;
        protected Vector2 origin;
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
        }

        public virtual void Update(GameTime gameTime)
        {
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Update(gameTime);
            }
        }


        public virtual void Draw()
        {
            if (animations.ContainsKey(indicator))
            {
                animations[indicator].Draw(position);
            }
        }

    }
}
