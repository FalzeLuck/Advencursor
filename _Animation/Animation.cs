using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Animation
{
    public class Animation
    {
        public Texture2D Texture { get; }
        public int Row { get; }
        public int Column { get; }
        public int Startrow { get; }
        public int TotalFrame { get; }
        public float FrameTime { get; }
        public bool IsLooping { get; }
        public bool IsComplete { get; set; }
        public bool IsCollide;
        public bool IsPause {  get; set; }

        public int currentFrame;
        public float timer;

        public Vector2 offset { get; set; }


        public Animation(Texture2D texture,int row, int column,  float fps, bool IsLooping)
        {
            this.Texture = texture;
            this.Row = row;
            this.Column = column;
            Startrow = 0;
            TotalFrame = row * column;
            this.FrameTime = 1 / fps;
            this.IsLooping = IsLooping;
            currentFrame = 0;
            timer = 0f;
            IsComplete = false;
            IsPause = false;
            IsCollide = false;
            offset = Vector2.Zero;
        }

        public Animation(Texture2D texture, int row, int column,int startrow, float fps, bool IsLooping)
        {
            this.Texture = texture;
            this.Row = row;
            this.Column = column;
            Startrow = startrow;
            TotalFrame = row * column;
            this.FrameTime = 1 / fps;
            this.IsLooping = IsLooping;
            currentFrame = (column * Startrow) - column;
            timer = 0f;
            IsComplete = false;
            IsPause = false;
            IsCollide = false;
            offset = Vector2.Zero;
        }


        public void Update(GameTime gameTime)
        {
            timer += (float)TimeManager.TotalSeconds;

            if(timer >= FrameTime  && Startrow == 0)
            {
                timer = 0f;
                currentFrame++;

                if(currentFrame >= TotalFrame)
                {
                    if (IsLooping) 
                    { 
                        currentFrame = 0;
                    }
                    else
                    {
                        currentFrame = TotalFrame - 1;
                        IsComplete = true;
                    }
                    
                }
            }else if( timer >= FrameTime && Startrow != 0)
            {
                timer = 0f;
                currentFrame++;

                if (currentFrame >= Column * Startrow)
                {
                    if (IsLooping)
                    {
                        currentFrame = (Column*Startrow) - Column;
                    }
                    else
                    {
                        currentFrame = TotalFrame - 1;
                        IsComplete = true;
                    }

                }
            }
        }

        public void Draw(Vector2 position)
        {
            if (Startrow == 0)
            {
                int frameWidth = Texture.Width / Column;
                int frameHeight = Texture.Height / Row;
                int _row = currentFrame / Column;
                int _column = currentFrame % Column;

                Rectangle cutRectangle = new Rectangle(frameWidth * _column, frameHeight * _row, frameWidth, frameHeight);
                Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);
                Globals.SpriteBatch.Draw(Texture, position, cutRectangle, Color.White, 0, origin - offset, 1, SpriteEffects.None, 1);
            }
            else if (Startrow != 0)
            {
                int frameWidth = Texture.Width / Column;
                int frameHeight = Texture.Height / Row;
                int _row = currentFrame / Column;
                int _column = currentFrame % Column;

                Rectangle cutRectangle = new Rectangle(frameWidth * _column, frameHeight * _row, frameWidth, frameHeight);
                Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);
                Globals.SpriteBatch.Draw(Texture, position, cutRectangle, Color.White, 0, origin - offset, 1, SpriteEffects.None, 1);
            }
        }

        public Rectangle GetCollision(Vector2 position)
        {
                int frameWidth = Texture.Width / Column;
                int frameHeight = Texture.Height / Row;

                int startX = (int)(position.X - frameWidth/2);
                int startY = (int)(position.Y - frameHeight/2);


                return new(startX, startY, frameWidth, frameHeight);

        }

        
    }
}
