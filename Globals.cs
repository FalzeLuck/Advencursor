using Advencursor._Combat;
using Advencursor._Managers;
using Advencursor._Models.Enemy;
using Advencursor._SaveData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace Advencursor
{
    public static class Globals
    {
        public static Game Game { get; set; }
        public static float BulletTime { get; private set; }
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static SpriteFont SpriteFont { get; set; }
        public static GraphicsDevice graphicsDevice { get; set; }
        public static Camera Camera { get; set; }

        public static Effect grayScaleEffect { get; set; }
        public static Viewport Viewport { get; set; }
        public static Point Bounds { get; set; }
        public static Rectangle fullScreenRectangle { get; set; }
        public static bool Paused = false;
        public static Random random { get; set; } = new Random();
        public static List<_Enemy> EnemyManager { get; set; } = new List<_Enemy>();


        public static void Update(GameTime gameTime)
        {
            TimeManager.Update(gameTime);
            Camera.Update(Globals.Bounds.X,Globals.Bounds.Y);
        }

        public static float RandomFloat(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min)) + min;
        }

        public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius)
        {
            Texture2D texture = new Texture2D(graphicsDevice, radius * 2, radius * 2);
            Color[] colorData = new Color[texture.Width * texture.Height];

            float radSquared = radius * radius;

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    int xDistance = x - radius;
                    int yDistance = y - radius;

                    if (xDistance * xDistance + yDistance * yDistance <= radSquared)
                    {
                        colorData[y * texture.Width + x] = Color.White;
                    }
                    else
                    {
                        colorData[y * texture.Width + x] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }
        public static List<Vector2> CreateCircleOutline(Vector2 center, float radius, int pointCount)
        {
            List<Vector2> circlePoints = new List<Vector2>();

            for (int i = 0; i < pointCount; i++)
            {
                float angle = MathHelper.TwoPi * i / pointCount;

                float x = center.X + radius * (float)Math.Cos(angle);
                float y = center.Y + radius * (float)Math.Sin(angle);

                circlePoints.Add(new Vector2(x, y));
            }

            return circlePoints;
        }

        public static Texture2D CreateRectangleTexture(int width, int height,Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            Color[] colorData = new Color[width * height];

            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = color;
            }

            texture.SetData(colorData);
            return texture;
        }

        public static Vector2 RotatePoint(Vector2 point, Vector2 pivot, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            Vector2 translatedPoint = point - pivot;

            float rotatedX = translatedPoint.X * cos - translatedPoint.Y * sin;
            float rotatedY = translatedPoint.X * sin + translatedPoint.Y * cos;

            return new Vector2(rotatedX, rotatedY) + pivot;
        }

        public static Rectangle CreateBoundingRectangle(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
        {
            
            float minX = Math.Min(Math.Min(topLeft.X, topRight.X), Math.Min(bottomLeft.X, bottomRight.X));
            float minY = Math.Min(Math.Min(topLeft.Y, topRight.Y), Math.Min(bottomLeft.Y, bottomRight.Y));

            float maxX = Math.Max(Math.Max(topLeft.X, topRight.X), Math.Max(bottomLeft.X, bottomRight.X));
            float maxY = Math.Max(Math.Max(topLeft.Y, topRight.Y), Math.Max(bottomLeft.Y, bottomRight.Y));


            return new Rectangle(
                (int)minX,  
                (int)minY, 
                (int)(maxX - minX),  
                (int)(maxY - minY) 
            );
        }
        public static void DrawCursor()
        {
            Texture2D cursor = Content.Load<Texture2D>("Cursor");

            SpriteBatch.Draw(cursor, InputManager._mousePosition,Color.White);
        }
        public static void DrawLine(Vector2 start, Vector2 end, Color color, int thickness = 1)
        {
            Texture2D pixel = Globals.CreateRectangleTexture(1, 1, Color.White);

            float distance = Vector2.Distance(start, end);

            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            SpriteBatch.Draw(pixel,
                             start,
                             null,  
                             color, 
                             angle, 
                             Vector2.Zero, 
                             new Vector2(distance, thickness),
                             SpriteEffects.None,
                             0);
        }

        public static Vector2 MoveVector(Vector2 start, float distance, float angleInRadians)
        {
            float newX = start.X + distance * (float)Math.Cos(angleInRadians);
            float newY = start.Y + distance * (float)Math.Sin(angleInRadians);

            return new Vector2(newX, newY);
        }

        public static void SetGreyScale(float amount)
        {
            grayScaleEffect.Parameters["grayscaleAmount"].SetValue(amount);
        }

        //If Begin Pleas End too if not it will Error
        public static void BeginDrawGrayScale()
        {
            SpriteBatch.End();
            SpriteBatch.Begin(effect: grayScaleEffect, transformMatrix: Camera.transform);
        }

        public static void EndDrawGrayScale()
        {
            SpriteBatch.End();
            SpriteBatch.Begin(transformMatrix: Globals.Camera.transform);
        }
    }
}
