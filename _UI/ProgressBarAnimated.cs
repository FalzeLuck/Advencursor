﻿using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._UI
{
    public class ProgressBarAnimated : ProgressBar
    {
        private float targetValue;
        private float animationSpeed;
        private Rectangle animationPart;
        private Vector2 animationPosition;
        private Color animationShade;

        public ProgressBarAnimated(Texture2D background, Texture2D foreground, float maxValue, Vector2 position) : base(background,foreground,maxValue, position)
        {
            targetValue = maxValue;
            animationPart = new Rectangle(foreground.Width,0,0,foreground.Height);
            animationPosition = position;
            animationShade = Color.DarkGray;
            animationSpeed = (maxValue/5);
        }

        public override void Update(GameTime gameTime)
        {
            int x;

            if (targetValue < currentValue)
            {
                currentValue -= animationSpeed * TimeManager.TimeGlobal;
                if (currentValue < targetValue) currentValue = targetValue;
                x = (int)(targetValue / maxValue * foreground.Width);
                animationShade = Color.Gray;
            }
            else
            {
                currentValue += animationSpeed * TimeManager.TimeGlobal;
                if (currentValue > targetValue) currentValue = targetValue;
                x = (int)(currentValue / maxValue * foreground.Width);
                animationShade = Color.DarkGray * 0.5f;
            }

            part.Width = x;
            animationPart.X = x;
            animationPart.Width = (int)(Math.Abs(currentValue - targetValue) / maxValue*foreground.Width);
            animationPosition.X = foregroundPosition.X + x;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Globals.SpriteBatch.Draw(foreground, animationPosition, animationPart, animationShade, 0, origin, scale, SpriteEffects.None, 1f);
        }

        public override void UpdateValue(float value)
        {
            if (value == currentValue) return;
            targetValue = value;
        }
    }
}
