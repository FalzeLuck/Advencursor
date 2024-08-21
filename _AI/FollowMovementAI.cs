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
        public Player target {  get; set; }

        public override void Move(Sprite bot)
        {
            if (target == null) return;

            var direction = target.position - bot.position;

            if (direction.Length() > 4){
                direction.Normalize();
                bot.position += direction * bot.speed * TimeManager.TotalSeconds;
            }
        }



    }
}
