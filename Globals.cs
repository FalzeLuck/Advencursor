using Advencursor._Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Advencursor
{
    public static class Globals
    {
        public static Game Game { get; set; }
        public static float TotalSeconds { get; set; }
        public static float BulletTime { get; private set; }
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static SpriteFont SpriteFont { get; set; }
        public static Point Bounds { get; set; }
        public static bool Paused = false;

        public static void Update(GameTime gameTime)
        {
            TimeManager.Update(gameTime);
        }
    }
}
