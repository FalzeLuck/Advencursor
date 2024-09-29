using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Advencursor._Animation
{
    public class Animation
    {
        public Texture2D Texture { get; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public int maxColumn { get; private set; }
        public int startColumn { get; private set; }
        public int Startrow { get; }
        public int TotalFrame { get; }
        public float FrameTime { get; }
        public bool IsLooping { get; }
        public bool IsComplete { get; set; }
        public bool IsCollide;
        public bool IsPause {  get; set; }
        public bool IsFlip {  get; set; }

        public int currentFrame;
        public float timer;

        public Vector2 offset { get; set; }
        public Vector2 position { get; set; }
        public float scale { get; set; }

        public float opacityValue {  get; set; }
        public float blinkingDuration { get; set; } = 0f;
        public bool IsBlinking { get; private set; } = false;


        public Animation(Texture2D texture,int row, int column,  float fps, bool IsLooping , float scale = 1f)
        {
            this.Texture = texture;
            this.Row = row;
            this.Column = column;
            maxColumn = column;
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
            opacityValue = 1f;
            this.scale = scale;
        }

        public Animation(Texture2D texture, int row, int column,int startrow, float fps, bool IsLooping, float scale = 1f)
        {
            this.Texture = texture;
            this.Row = row;
            this.Column = column;
            maxColumn = column;
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
            opacityValue = 1f;
            this.scale = scale;
        }
        public Animation(Texture2D texture, int row, int column, int maxColumn, int startrow, float fps, bool IsLooping, float scale = 1f)
        {
            this.Texture = texture;
            this.Row = row;
            this.Column = column;
            this.maxColumn = maxColumn;
            this.startColumn = 0;
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
            opacityValue = 1f;
            this.scale = scale;
        }

        public Animation(Texture2D texture, int row, int column,int startColumn,int maxColumn, int startrow, float fps, bool IsLooping, float scale = 1f)
        {
            this.Texture = texture;
            this.Row = row;
            this.Column = column;
            this.maxColumn = maxColumn;
            this.startColumn = startColumn;
            Startrow = startrow;
            TotalFrame = row * column;
            this.FrameTime = 1 / fps;
            this.IsLooping = IsLooping;
            currentFrame = (column * Startrow) - column + startColumn;
            timer = 0f;
            IsComplete = false;
            IsPause = false;
            IsCollide = false;
            offset = Vector2.Zero;
            opacityValue = 1f;
            this.scale = scale;
        }


        public void Update()
        {
            timer += TimeManager.TimeGlobal;
            if (!IsPause)
            {
                if (timer >= FrameTime && Startrow == 0)
                {
                    timer = 0f;
                    currentFrame++;

                    if (currentFrame >= TotalFrame)
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
                }
                else if (timer >= FrameTime && Startrow != 0)
                {
                    timer = 0f;
                    currentFrame++;
                    if (currentFrame >= (Column * Startrow) - (Column-maxColumn))
                    {
                        if (IsLooping)
                        {
                            currentFrame = (Column * Startrow) - Column + startColumn;
                        }
                        else
                        {
                            currentFrame = TotalFrame - 1;
                            IsComplete = true;
                        }

                    }
                }
            }
            if (IsBlinking)
            {
                if (blinkingDuration >= 0f)
                {
                    blinkingDuration -= TimeManager.TimeGlobal;
                    if (opacityValue >= 0f)
                    {
                        opacityValue -= 0.1f;
                    }
                    else if (opacityValue < 0f)
                    {
                        opacityValue = 1f;
                    }
                }
                else
                {
                    opacityValue = 1f;
                    IsBlinking = false;
                }
            }
        }

        public void Draw(Vector2 position)
        {
            SpriteEffects flip = SpriteEffects.None;
            if (IsFlip)
            {
                flip = SpriteEffects.FlipHorizontally;
            }
            if(!IsFlip)
            {
                flip = SpriteEffects.None;
            }
            if (Startrow == 0)
            {
                int frameWidth = Texture.Width / Column;
                int frameHeight = Texture.Height / Row;
                int _row = currentFrame / Column;
                int _column = currentFrame % Column;

                Rectangle cutRectangle = new Rectangle(frameWidth * _column, frameHeight * _row, frameWidth, frameHeight);
                Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);
                Globals.SpriteBatch.Draw(Texture, position, cutRectangle, Color.White * opacityValue, 0, origin - offset, scale, flip, 1);
            }
            else if (Startrow != 0)
            {
                int frameWidth = Texture.Width / Column;
                int frameHeight = Texture.Height / Row;
                int _row = currentFrame / Column;
                int _column = currentFrame % Column;

                Rectangle cutRectangle = new Rectangle(frameWidth * _column, frameHeight * _row, frameWidth, frameHeight);
                Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);
                Globals.SpriteBatch.Draw(Texture, position, cutRectangle, Color.White * opacityValue, 0, origin - offset, scale, flip, 1);
            }
        }

        public Rectangle GetCollision(Vector2 position)
        {
                int frameWidth = (int)(Texture.Width / Column * scale);
                int frameHeight = (int)(Texture.Height / Row * scale);

                int startX = (int)((position.X - frameWidth/2) + offset.X);
                int startY = (int)((position.Y - frameHeight/2) + offset.Y);


                return new(startX, startY, frameWidth, frameHeight);

        }

        public void SetRowColumn(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public void Blink(float duration)
        {
            blinkingDuration = duration;
            IsBlinking = true;
        }

        public void Reset()
        {
            currentFrame = (Column * Startrow) - Column;
            IsComplete = false;
        }

        public void PauseFrame(int frame)
        {
            currentFrame = ((Column * Startrow) - (Column - maxColumn)) - (maxColumn-(frame-1));
            IsComplete = false;
            IsPause = true;
        }

        public void Play()
        {
            IsPause = false;
        }

        public void SetOpacity(float value)
        {
            opacityValue = value;
        }

        public void SetFlip(bool flip)
        {
            IsFlip = flip;
        }
    }
}
