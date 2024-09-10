using Advencursor._Managers;
using Advencursor._Models.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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
        public static Point Bounds { get; set; }
        public static bool Paused = false;

        public static Random random { get; set; } = new Random();

        public static List<_Enemy> EnemyManager { get; set; } = new List<_Enemy>();

        public static void Update(GameTime gameTime)
        {
            TimeManager.Update(gameTime);
        }

        public static float RandomFloat(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min)) + min;
        }
    }
}
