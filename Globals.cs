using Advencursor._Combat;
using Advencursor._Managers;
using Advencursor._Models.Enemy;
using Advencursor._SaveData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static Viewport Viewport { get; set; }
        public static Point Bounds { get; set; }
        public static Rectangle fullScreenRectangle { get; set; }
        public static bool Paused = false;

        public static Random random { get; set; } = new Random();

        public static List<_Enemy> EnemyManager { get; set; } = new List<_Enemy>();


        public static void Update(GameTime gameTime)
        {
            TimeManager.Update(gameTime);
            Camera.Update();
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


    }
}
