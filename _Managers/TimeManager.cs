using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advencursor._Managers
{
    public static class TimeManager
    {
        private const float MIN_TIME_DILATION = 0.05f;
        private const float MAX_TIME_DILATION = 1.00f;
        private const float TIME_DILATION_SPEED = 1.00f;
        private const float TIME_DILATION_PER_SECOND = (MAX_TIME_DILATION - MIN_TIME_DILATION) / TIME_DILATION_SPEED;
        private static float TimeDilation { get; set; } = 1.00f;
        public static float TotalSeconds { get; set; }
        public static float BulletTime { get; private set; }

        public static float framerate = 30;

        public static void Update(GameTime gameTime)
        {
            TotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            BulletTime = 0.05f * TotalSeconds;
            
        }
    }
}
