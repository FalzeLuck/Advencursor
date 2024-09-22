using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advencursor._Managers;
using Advencursor._Models;
using Advencursor._Models.Enemy;
using Microsoft.Xna.Framework;

namespace Advencursor._AI
{
    public class FollowMovementAI : MovementAI
    {

        public override void Move(Sprite bot)
        {
            if (target == null) return;
            if (!stop)
            {

                var direction = target.position - bot.position;

                if (direction.Length() > 4)
                {
                    direction.Normalize();
                    bot.position += direction * bot.velocity * TimeManager.TimeGlobal;
                }
            }
        }

        public override void Stop()
        {
            stop = true;
        }
        public override void Start()
        {
            stop = false;
        }

    }
}
