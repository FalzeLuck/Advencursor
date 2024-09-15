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
        public static float TimeGlobal { get; set; }
        public static float GameSpeedParamiter { get; set; }

        public static float framerate = 30;

        private static bool IsIncrease;

        public static void Update(GameTime gameTime)
        {
            TotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            BulletTime();

            TimeGlobal = TotalSeconds * GameSpeedParamiter;
            
        }

        public static void ChangeGameSpeed(float amount)
        {
            if (amount > GameSpeedParamiter)
            {
                IsIncrease = true;
            }
            else
            {
                IsIncrease = false;
            }

            GameSpeedParamiter = amount;
        }

        private static void BulletTime()
        {
            if (IsIncrease)
            {
                TimeDilation += TIME_DILATION_PER_SECOND * TotalSeconds;
            }
            else
            {
                TimeDilation -= TIME_DILATION_PER_SECOND * TotalSeconds;
            }

            TimeDilation = Math.Clamp(TimeDilation, MIN_TIME_DILATION, MAX_TIME_DILATION);
        }
    }
}
