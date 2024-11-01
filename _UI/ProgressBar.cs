﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._UI
{
    public class ProgressBar : UIElement
    {
        protected readonly Texture2D background;
        protected readonly Texture2D foreground;
        protected Vector2 foregroundPosition;
        protected readonly float maxValue;
        protected float currentValue;
        protected Rectangle part;

        public ProgressBar(Texture2D background, Texture2D foreground, float maxValue, Vector2 position) : base(background,position)
        {
            this.background = background;
            this.foreground = foreground;
            this.maxValue = maxValue;
            currentValue = maxValue;
            part = new Rectangle(0,0,foreground.Width,foreground.Height);
            if (foreground.Width < background.Width)
            {
                int temp = (background.Width - foreground.Width) / 2;
                foregroundPosition = new Vector2(position.X + temp, position.Y);
            }
            else
            {
                foregroundPosition = position;
            }
        }

        public virtual void UpdateValue(float value)
        {
            currentValue = value;
        }

        public override void Update(GameTime gameTime)
        {
            part.Width = (int)(currentValue / maxValue * foreground.Width);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
            {
                Globals.SpriteBatch.Draw(background, position, null, _color * opacity, rotation, origin, scale, SpriteEffects.None, 0.5f);
                Globals.SpriteBatch.Draw(foreground, foregroundPosition, part, _color * opacity, rotation, origin, scale, SpriteEffects.None, 0.5f);
            }
        }


    }
}
